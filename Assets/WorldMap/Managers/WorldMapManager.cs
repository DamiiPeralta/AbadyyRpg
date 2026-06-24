using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance;

    [Header("Estado del mapa")]
    public WorldMapNode currentNode;

    [Header("Nodos")]
    public List<WorldMapNode> allNodes = new List<WorldMapNode>();

    [Header("Scenes")]
    public string battleSceneName = "BattleScene";
    public string worldMapSceneName = "WorldMapScene";

    [Header("UI")]
    public EventPanelUI eventPanelUI;
    public WorldMapHudUI hudUI;
    public WorldMapTimeOfDayOverlayUI timeOfDayOverlayUI;

    private WorldMapEvent currentEvent;
    private bool caravanDefeatPanelShown;
    private bool demoVictoryPanelShown;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameSfxPlayer.GetOrCreate();
        timeOfDayOverlayUI = timeOfDayOverlayUI != null
            ? timeOfDayOverlayUI
            : WorldMapTimeOfDayOverlayUI.GetOrCreate();

        if (eventPanelUI != null)
            eventPanelUI.Hide();

        AutoFillNodesIfNeeded();

        ApplyPersistentMapState();
        ApplyReturnedCombatResult();

        if (currentNode == null)
        {
            Debug.LogError("WorldMapManager: no hay currentNode asignado.");
            return;
        }

        if (hudUI != null)
            hudUI.BindSearchResources(SearchCurrentNodeResources);

        currentNode.SetVisited(true);
        currentNode.SetCurrent(true);

        if (GameRunState.Instance != null)
        {
            GameRunState.Instance.RegisterCurrentNode(currentNode);
            GameRunState.Instance.RegisterVisitedNode(currentNode.nodeId);
        }

        RefreshHud();

        Debug.Log($"Starting at node: {currentNode.nodeName}");
    }

    public void TryMoveToNode(WorldMapNode targetNode)
    {
        if (targetNode == null)
            return;

        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion ya fracaso. No se puede viajar.");
            RefreshHud();
            return;
        }

        if (currentEvent != null)
        {
            Debug.Log("Cannot move while an event is active.");
            return;
        }

        if (targetNode == currentNode)
        {
            Debug.Log($"Already at node: {targetNode.nodeName}");
            return;
        }

        if (!targetNode.isUnlocked)
        {
            Debug.Log($"Node locked: {targetNode.nodeName}");
            return;
        }

        if (!currentNode.IsConnectedTo(targetNode))
        {
            Debug.Log($"Invalid movement. {currentNode.nodeName} is not connected to {targetNode.nodeName}");
            return;
        }

        if (!CanPayTravelCost(targetNode))
            return;

        PayTravelCost(targetNode);
        MoveToNode(targetNode);
    }

    private bool CanPayTravelCost(WorldMapNode targetNode)
    {
        CaravanState caravan = CaravanState.Instance;

        if (caravan == null)
        {
            Debug.LogWarning("WorldMapManager: no existe CaravanState. El viaje no consumira stamina ni horas.");
            return true;
        }

        int staminaCost = Mathf.Max(0, targetNode.travelStaminaCost);

        if (caravan.caravanStamina < staminaCost)
        {
            Debug.Log($"No hay stamina suficiente para viajar a {targetNode.nodeName}. Requiere {staminaCost}, disponible {caravan.caravanStamina}.");
            RefreshHud();
            return false;
        }

        return true;
    }

    private void PayTravelCost(WorldMapNode targetNode)
    {
        CaravanState caravan = CaravanState.Instance;

        if (caravan == null)
            return;

        int staminaCost = Mathf.Max(0, targetNode.travelStaminaCost);
        int hourCost = Mathf.Max(0, targetNode.travelHourCost);

        caravan.ChangeStamina(-staminaCost);
        caravan.AdvanceHours(hourCost);

        Debug.Log($"Viaje a {targetNode.nodeName}: -{staminaCost} stamina, +{hourCost} horas.");
    }

    private void MoveToNode(WorldMapNode targetNode)
    {
        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion fracaso durante el viaje. Movimiento detenido.");
            RefreshHud();
            return;
        }

        Debug.Log($"Moving from {currentNode.nodeName} to {targetNode.nodeName}");

        currentNode.SetCurrent(false);

        currentNode = targetNode;

        currentNode.SetVisited(true);
        currentNode.SetCurrent(true);

        if (GameSfxPlayer.Instance != null)
            GameSfxPlayer.Instance.PlayNodeTravel();

        if (GameRunState.Instance != null)
        {
            GameRunState.Instance.RegisterCurrentNode(currentNode);
            GameRunState.Instance.RegisterVisitedNode(currentNode.nodeId);
        }

        RefreshHud();

        Debug.Log($"Current node: {currentNode.nodeName}");

        if (currentNode.isCombatNode && (!currentNode.isCompleted || currentNode.isRepeatableCombat))
        {
            StartCombatFromNode(currentNode);
            return;
        }

        TryStartNodeEvent(currentNode);
    }

    private void StartCombatFromNode(WorldMapNode node)
    {
        if (node == null)
            return;

        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion ya fracaso. No se puede iniciar combate.");
            RefreshHud();
            return;
        }

        if (GameRunState.Instance == null)
        {
            Debug.LogError("No existe GameRunState en la escena.");
            return;
        }

        GameRunState.Instance.RequestBattle(
            node.battleGroupId,
            node.nodeId,
            worldMapSceneName
        );

        Debug.Log($"Iniciando combate desde nodo {node.nodeName} con grupo {node.battleGroupId}");

        SceneManager.LoadScene(battleSceneName);
    }

    private void StartCombatFromEventOption(WorldMapEventOption option)
    {
        if (option == null)
            return;

        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion ya fracaso. No se puede iniciar combate de evento.");
            RefreshHud();
            return;
        }

        if (currentNode == null)
        {
            Debug.LogError("No hay currentNode para iniciar combate desde evento.");
            return;
        }

        if (GameRunState.Instance == null)
        {
            Debug.LogError("No existe GameRunState. No se puede iniciar combate desde evento.");
            return;
        }

        currentEvent = null;

        if (eventPanelUI != null)
            eventPanelUI.Hide();

        GameRunState.Instance.RequestBattle(
            option.battleGroupId,
            currentNode.nodeId,
            worldMapSceneName,
            option.reward
        );

        Debug.Log($"Evento inicia combate desde nodo {currentNode.nodeName} con grupo {option.battleGroupId}");

        SceneManager.LoadScene(battleSceneName);
    }

    private void ApplyReturnedCombatResult()
    {
        if (GameRunState.Instance == null)
            return;

        if (!GameRunState.Instance.hasPendingCombatResult)
            return;

        if (!GameRunState.Instance.lastCombatWasVictory)
        {
            Debug.Log("Volvimos de combate, pero no hubo victoria.");
            GameRunState.Instance.ClearPendingBattleReward();
            GameRunState.Instance.ClearPendingCombatResult();
            RefreshAllNodesVisualState();
            RefreshHud();
            return;
        }

        string completedNodeId = GameRunState.Instance.lastCompletedCombatNodeId;

        WorldMapNode completedNode = FindNodeById(completedNodeId);

        if (completedNode == null)
        {
            Debug.LogWarning($"No se encontró el nodo completado: {completedNodeId}");
            GameRunState.Instance.ClearPendingBattleReward();
            GameRunState.Instance.ClearPendingCombatResult();
            return;
        }

        completedNode.isCompleted = true;
        completedNode.SetVisited(true);

        CompleteEventAfterCombatVictory(completedNode);

        SetCurrentNodeAfterSceneReturn(completedNode);

        Debug.Log($"Nodo de combate completado: {completedNode.nodeName}");

        RegisterDemoBossVictoryFlags(completedNode);

        RewardData pendingEventReward = GameRunState.Instance.ConsumePendingBattleReward();

        if (pendingEventReward != null && !pendingEventReward.IsEmpty())
        {
            RewardApplier.ApplyReward(pendingEventReward);
        }
        else if (completedNode.combatVictoryReward != null && !completedNode.combatVictoryReward.IsEmpty())
        {
            RewardApplier.ApplyReward(completedNode.combatVictoryReward);
        }

        RefreshHud();

        foreach (WorldMapNode nodeToUnlock in completedNode.unlockOnCombatVictory)
        {
            if (nodeToUnlock == null)
                continue;

            nodeToUnlock.SetUnlocked(true);
            GameRunState.Instance.RegisterUnlockedNode(nodeToUnlock.nodeId);

            Debug.Log($"Nodo desbloqueado por victoria: {nodeToUnlock.nodeName}");
        }

        GameRunState.Instance.ClearPendingCombatResult();

        RefreshAllNodesVisualState();
    }

    private void CompleteEventAfterCombatVictory(WorldMapNode completedNode)
    {
        if (completedNode == null || !completedNode.hasEvent || completedNode.eventCompleted)
            return;

        completedNode.eventCompleted = true;

        if (GameRunState.Instance != null)
            GameRunState.Instance.RegisterEventCompleted(completedNode.nodeId);
    }

    private void RegisterDemoBossVictoryFlags(WorldMapNode completedNode)
    {
        if (completedNode == null || GameRunState.Instance == null)
            return;

        if (completedNode.nodeId != "node_portal_excavadores")
            return;

        GameRunState.Instance.RegisterFlag(GameRunState.BossDefeatedFlag);
        GameRunState.Instance.RegisterFlag("flag_frontera_abierta");
        Debug.Log("Demo Tier 1: jefe derrotado y frontera abierta.");
    }

    private void ApplyPersistentMapState()
    {
        if (GameRunState.Instance == null)
            return;

        foreach (WorldMapNode node in allNodes)
        {
            if (node == null)
                continue;

            if (GameRunState.Instance.HasCompletedNode(node.nodeId))
            {
                node.isCompleted = true;
                node.SetVisited(true);
            }

            if (GameRunState.Instance.HasUnlockedNode(node.nodeId))
            {
                node.SetUnlocked(true);
            }

            if (GameRunState.Instance.HasVisitedNode(node.nodeId))
            {
                node.SetVisited(true);
            }

            if (GameRunState.Instance.HasCompletedEvent(node.nodeId))
            {
                node.eventCompleted = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(GameRunState.Instance.currentNodeId))
        {
            WorldMapNode savedCurrentNode = FindNodeById(GameRunState.Instance.currentNodeId);

            if (savedCurrentNode != null)
            {
                SetCurrentNodeAfterSceneReturn(savedCurrentNode);
            }
        }

        RefreshAllNodesVisualState();
    }

    private WorldMapNode FindNodeById(string nodeId)
    {
        foreach (WorldMapNode node in allNodes)
        {
            if (node != null && node.nodeId == nodeId)
            {
                return node;
            }
        }

        return null;
    }

    private void AutoFillNodesIfNeeded()
    {
        if (allNodes != null && allNodes.Count > 0)
            return;

        WorldMapNode[] foundNodes = FindObjectsOfType<WorldMapNode>();
        allNodes = new List<WorldMapNode>(foundNodes);

        Debug.Log($"WorldMapManager: nodos encontrados automáticamente: {allNodes.Count}");
    }

    private void RefreshAllNodesVisualState()
    {
        foreach (WorldMapNode node in allNodes)
        {
            if (node != null)
            {
                node.RefreshVisual();
            }
        }
    }

    private void TryStartNodeEvent(WorldMapNode node)
    {
        if (node == null)
            return;

        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion ya fracaso. No se puede iniciar evento.");
            RefreshHud();
            return;
        }

        if (!node.hasEvent)
            return;

        if (node.eventCompleted)
        {
            Debug.Log($"Event already completed at node: {node.nodeName}");
            return;
        }

        currentEvent = node.nodeEvent;

        Debug.Log($"Event started at node: {node.nodeName}");

        if (eventPanelUI != null)
        {
            eventPanelUI.ShowEvent(currentEvent);
        }
        else
        {
            Debug.LogWarning("WorldMapManager: no hay EventPanelUI asignado.");
        }
    }

    public void ResolveOption(WorldMapEventOption option)
    {
        if (option == null)
            return;

        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion ya fracaso. No se puede resolver opcion de evento.");
            RefreshHud();
            return;
        }

        string optionTitle = !string.IsNullOrWhiteSpace(option.title) ? option.title : "Opcion sin titulo";
        Debug.Log($"Option chosen: {optionTitle}");

        if (option.startsCombat)
        {
            StartCombatFromEventOption(option);
            return;
        }

        ApplyOptionResults(option);

        if (IsExpeditionFailed())
        {
            RefreshHud();
            return;
        }

        if (option.reward != null && !option.reward.IsEmpty())
        {
            RewardApplier.ApplyReward(option.reward);
            RefreshHud();
        }

        if (option.returnToStartNode)
        {
            MoveToNode(currentNode); // pendiente: mejorar con startNode real
        }

        currentEvent = null;

        if (eventPanelUI != null)
            eventPanelUI.Hide();
    }

    private void ApplyOptionResults(WorldMapEventOption option)
    {
        foreach (var node in option.nodesToUnlock)
        {
            if (node != null)
            {
                node.SetUnlocked(true);

                if (GameRunState.Instance != null)
                {
                    GameRunState.Instance.RegisterUnlockedNode(node.nodeId);
                }

                Debug.Log($"Unlocked node: {node.nodeName}");
            }
        }

        if (!string.IsNullOrEmpty(option.flagToAdd))
        {
            if (GameRunState.Instance != null)
            {
                GameRunState.Instance.RegisterFlag(option.flagToAdd);
            }

            Debug.Log($"Flag added: {option.flagToAdd}");

            UnlockDemoBridgeBranchesIfNeeded(option.flagToAdd);
        }

        if (option.completesEvent)
        {
            currentNode.eventCompleted = true;

            if (GameRunState.Instance != null)
            {
                GameRunState.Instance.RegisterEventCompleted(currentNode.nodeId);
            }

            Debug.Log($"Event completed at node: {currentNode.nodeName}");
        }
        else
        {
            Debug.Log($"Event NOT completed at node: {currentNode.nodeName}");
        }
    }

    private void UnlockDemoBridgeBranchesIfNeeded(string flag)
    {
        if (flag != "flag_puente_reparado")
            return;

        UnlockNodeById("node_cruce_central");
        UnlockNodeById("node_bosque_aldheron");
        UnlockNodeById("node_torre_vigia");
    }

    private void UnlockNodeById(string nodeId)
    {
        WorldMapNode node = FindNodeById(nodeId);

        if (node == null)
            return;

        node.SetUnlocked(true);

        if (GameRunState.Instance != null)
            GameRunState.Instance.RegisterUnlockedNode(node.nodeId);

        Debug.Log($"Unlocked node: {node.nodeName}");
    }

    private bool IsExpeditionFailed()
    {
        return GameRunState.Instance != null && GameRunState.Instance.expeditionFailed;
    }

    private void SetCurrentNodeAfterSceneReturn(WorldMapNode node)
    {
        if (node == null)
            return;

        foreach (WorldMapNode mapNode in allNodes)
        {
            if (mapNode != null)
                mapNode.SetCurrent(false);
        }

        currentNode = node;
        currentNode.SetVisited(true);
        currentNode.SetCurrent(true);

        if (GameRunState.Instance != null)
        {
            GameRunState.Instance.RegisterCurrentNode(currentNode);
            GameRunState.Instance.RegisterVisitedNode(currentNode.nodeId);
        }

        Debug.Log($"WorldMapManager: currentNode restaurado después del combate: {currentNode.nodeName}");
    }

    private void RefreshHud()
    {
        EvaluateCaravanDefeatConditions();

        if (hudUI != null)
            hudUI.Refresh(currentNode);

        if (timeOfDayOverlayUI != null)
            timeOfDayOverlayUI.Refresh();

        ShowExpeditionFailureIfNeeded();
        ShowDemoVictoryIfNeeded();
    }

    private void EvaluateCaravanDefeatConditions()
    {
        if (GameRunState.Instance == null || GameRunState.Instance.expeditionFailed)
            return;

        PartyRuntimeState party = PartyRuntimeState.Instance;

        if (party == null || !party.HasInitializedParty())
            return;

        if (GetRosterCount(party) == 0)
        {
            GameRunState.Instance.RegisterExpeditionFailure("No tienes mas expedicionarios para continuar la caravana.");
            return;
        }

        if (GetLivingRosterCount(party) <= 0)
        {
            GameRunState.Instance.RegisterExpeditionFailure("No tienes mas expedicionarios vivos para continuar la caravana.");
            return;
        }

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        CaravanState caravan = CaravanState.Instance;

        bool noFood = inventory != null && inventory.food <= 0;
        bool noCaravanEnergy = caravan != null && caravan.caravanStamina <= 0;
        bool noExpeditionaryEnergy = GetLivingRosterStamina(party) <= 0;

        if (noFood && noCaravanEnergy && noExpeditionaryEnergy)
        {
            GameRunState.Instance.RegisterExpeditionFailure(
                "El hambre hizo que todos desertaran: no queda comida, la caravana no tiene energia de viaje y ningun expedicionario conserva energia para seguir."
            );
        }
    }

    private int GetRosterCount(PartyRuntimeState party)
    {
        if (party == null || party.GetRoster() == null)
            return 0;

        int count = 0;

        foreach (Unit unit in party.GetRoster())
        {
            if (unit != null)
                count++;
        }

        return count;
    }

    private int GetLivingRosterCount(PartyRuntimeState party)
    {
        if (party == null || party.GetRoster() == null)
            return 0;

        int count = 0;

        foreach (Unit unit in party.GetRoster())
        {
            if (unit != null && unit.isAlive)
                count++;
        }

        return count;
    }

    private int GetLivingRosterStamina(PartyRuntimeState party)
    {
        if (party == null || party.GetRoster() == null)
            return 0;

        int total = 0;

        foreach (Unit unit in party.GetRoster())
        {
            if (unit != null && unit.isAlive)
                total += Mathf.Max(0, unit.currentStamina);
        }

        return total;
    }

    private void ShowExpeditionFailureIfNeeded()
    {
        if (GameRunState.Instance == null || !GameRunState.Instance.expeditionFailed)
            return;

        ShowCaravanDefeatPanel(GameRunState.Instance.expeditionFailureReason);
    }

    private void ShowCaravanDefeatPanel(string reason)
    {
        if (caravanDefeatPanelShown)
            return;

        caravanDefeatPanelShown = true;
        CaravanDefeatPanelUI.GetOrCreate().Show(reason);
    }

    private void ShowDemoVictoryIfNeeded()
    {
        if (currentNode == null || GameRunState.Instance == null)
            return;

        if (demoVictoryPanelShown || GameRunState.Instance.demoCompleted)
            return;

        if (GameRunState.Instance.expeditionFailed)
            return;

        if (currentNode.nodeId != "node_frontera_abierta")
            return;

        if (!GameRunState.Instance.IsBossDefeated())
            return;

        GameRunState.Instance.RegisterDemoCompleted();
        demoVictoryPanelShown = true;
        DemoVictoryPanelUI.GetOrCreate().Show();
    }

    public void SearchCurrentNodeResources()
    {
        if (IsExpeditionFailed())
        {
            Debug.Log("La expedicion ya fracaso. No se puede buscar recursos.");
            RefreshHud();
            return;
        }

        if (currentEvent != null)
        {
            WorldMapActionToastUI.GetOrCreate().ShowFailure("No puedes buscar recursos mientras hay un evento activo.");
            return;
        }

        bool success = CaravanResourceSearchUtility.TrySearchResources(currentNode, out string message);

        if (success)
        {
            if (GameSfxPlayer.Instance != null)
                GameSfxPlayer.Instance.PlayCraftSuccess();

            WorldMapActionToastUI.GetOrCreate().ShowSuccess(message);
        }
        else
        {
            if (GameSfxPlayer.Instance != null)
                GameSfxPlayer.Instance.PlayCraftFail();

            WorldMapActionToastUI.GetOrCreate().ShowFailure(message);
        }

        RefreshHud();
    }

    public void GoToCaravanScene()
    {
        SceneManager.LoadScene("CaravanScene");
    }
}
