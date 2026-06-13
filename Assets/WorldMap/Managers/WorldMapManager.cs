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

    private WorldMapEvent currentEvent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

        currentNode.SetVisited(true);
        currentNode.SetCurrent(true);

        if (GameRunState.Instance != null)
        {
            GameRunState.Instance.RegisterCurrentNode(currentNode.nodeId);
            GameRunState.Instance.RegisterVisitedNode(currentNode.nodeId);
        }

        RefreshHud();

        Debug.Log($"Starting at node: {currentNode.nodeName}");
    }

    public void TryMoveToNode(WorldMapNode targetNode)
    {
        if (targetNode == null)
            return;

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
        Debug.Log($"Moving from {currentNode.nodeName} to {targetNode.nodeName}");

        currentNode.SetCurrent(false);

        currentNode = targetNode;

        currentNode.SetVisited(true);
        currentNode.SetCurrent(true);

        if (GameRunState.Instance != null)
        {
            GameRunState.Instance.RegisterCurrentNode(currentNode.nodeId);
            GameRunState.Instance.RegisterVisitedNode(currentNode.nodeId);
        }

        RefreshHud();

        Debug.Log($"Current node: {currentNode.nodeName}");

        if (currentNode.isCombatNode && !currentNode.isCompleted)
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

        SetCurrentNodeAfterSceneReturn(completedNode);

        Debug.Log($"Nodo de combate completado: {completedNode.nodeName}");

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

        string optionTitle = !string.IsNullOrWhiteSpace(option.title) ? option.title : "Opcion sin titulo";
        Debug.Log($"Option chosen: {optionTitle}");

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

        if (option.startsCombat)
        {
            StartCombatFromEventOption(option);
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
            GameRunState.Instance.RegisterCurrentNode(currentNode.nodeId);
            GameRunState.Instance.RegisterVisitedNode(currentNode.nodeId);
        }

        Debug.Log($"WorldMapManager: currentNode restaurado después del combate: {currentNode.nodeName}");
    }

    private void RefreshHud()
    {
        if (hudUI != null)
            hudUI.Refresh(currentNode);
    }

    public void GoToCaravanScene()
    {
        SceneManager.LoadScene("CaravanScene");
    }
}
