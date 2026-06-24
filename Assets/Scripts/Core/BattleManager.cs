using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BattleManager : MonoBehaviour
{
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;

    [Header("Setup de batalla")]
    public BattleSetup battleSetup;
    public PartyRuntimeState partyRuntimeState;

    [Header("Turnos acumulativos")]
    public int turnThreshold = 100;
    public int maxTurnsPerRound = 20;

    [Header("Taunt")]
    public int frontMaxTaunt = 100;
    public int backMaxTaunt = 80;
    public int tauntDecayPerRound = 30;

    private List<Unit> activeUnits = new List<Unit>();
    private Queue<Unit> turnQueue = new Queue<Unit>();

    [Header("UI Turnos")]
    private List<Unit> currentRoundOrder = new List<Unit>();
    private int currentTurnIndex = 0;
    private bool cleanedRuntimeViews = false;
    private BattleResultPanelUI battleResultPanelUI;

    public GameManager GameManager;
    public bool battleActive;

    public void StartBattleFromSetup()
    {
        if (battleSetup == null)
        {
            Debug.LogError("BattleManager: battleSetup no asignado.");
            return;
        }

        if (GameRunState.Instance != null)
        {
            Debug.Log("BattleManager: GameRunState detectado.");
            Debug.Log($"BattleManager: currentBattleGroupId = {GameRunState.Instance.currentBattleGroupId}");
            Debug.Log($"BattleManager: returnNodeId = {GameRunState.Instance.returnNodeId}");
        }
        else
        {
            Debug.LogWarning("BattleManager: no hay GameRunState.Instance. ¿Entraste directo a BattleScene?");
        }

        partyRuntimeState = PartyRuntimeState.Instance;

        if (partyRuntimeState == null)
        {
            Debug.LogError("BattleManager: no existe PartyRuntimeState.Instance. Entrá al combate desde WorldMapScene, no directo desde BattleScene.");
            return;
        }

        if (!partyRuntimeState.HasParty())
        {
            Debug.LogError("BattleManager: PartyRuntimeState existe, pero no tiene party. Inicializala desde WorldMapScene antes de entrar a combate.");
            return;
        }

        List<Unit> players = partyRuntimeState.GetCurrentParty();

        Debug.Log($"BattleManager: party cargada desde runtime. Cantidad aliados = {players.Count}");

        AttachPlayerViews(players);

        List<Unit> enemies = CreateEnemyUnitsFromSetup();

        Debug.Log($"BattleManager: enemigos creados. Cantidad enemigos = {enemies.Count}");

        StartBattle(players, enemies);
    }

    private void AttachPlayerViews(List<Unit> players)
    {
        if (players == null || battleSetup == null)
            return;

        int count = Mathf.Min(
            players.Count,
            battleSetup.allyPositions != null ? battleSetup.allyPositions.Count : 0
        );

        for (int i = 0; i < count; i++)
        {
            Unit unit = players[i];
            Transform pos = battleSetup.allyPositions[i];
            GameObject prefab = GetPlayerViewPrefab(unit, i);

            if (unit == null || prefab == null || pos == null)
                continue;

            unit.unitView = null;

            GameObject instanceGO = Instantiate(prefab, pos.position, Quaternion.identity, pos.parent);

            UnitView unitView = instanceGO.GetComponent<UnitView>();
            if (unitView == null)
            {
                SpriteRenderer sr = instanceGO.GetComponent<SpriteRenderer>();
                if (sr == null)
                    sr = instanceGO.AddComponent<SpriteRenderer>();

                unitView = instanceGO.AddComponent<UnitView>();
            }

            unit.unitView = unitView;
            unitView.SetUnit(unit);
        }
    }

    private GameObject GetPlayerViewPrefab(Unit unit, int index)
    {
        if (unit != null && unit.unitData != null)
            return unit.unitData.gameObject;

        if (battleSetup != null &&
            battleSetup.allyPrefabs != null &&
            index >= 0 &&
            index < battleSetup.allyPrefabs.Count)
        {
            return battleSetup.allyPrefabs[index];
        }

        return null;
    }

    private List<Unit> CreateEnemyUnitsFromSetup()
    {
        List<Unit> enemies = new List<Unit>();

        if (battleSetup == null)
        {
            Debug.LogError("BattleManager: battleSetup es null al crear enemigos.");
            return enemies;
        }

        if (GameRunState.Instance != null && !string.IsNullOrWhiteSpace(GameRunState.Instance.currentBattleGroupId))
        {
            BattleEncounterSO encounter = battleSetup.GetEncounterById(GameRunState.Instance.currentBattleGroupId);

            if (encounter != null)
            {
                Debug.Log($"BattleManager: encounter {encounter.encounterId} encontrado.");

                if (GameRunState.Instance != null)
                    GameRunState.Instance.SetPendingBattleRewardIfEmpty(encounter.reward);

                return CreateEnemyUnitsFromEncounter(encounter);
            }
        }

        List<GameObject> enemiesToSpawn = battleSetup.enemyPrefabs;

        if (GameRunState.Instance != null && !string.IsNullOrWhiteSpace(GameRunState.Instance.currentBattleGroupId))
        {
            string requestedGroupId = GameRunState.Instance.currentBattleGroupId;

            Debug.Log($"BattleManager: intentando cargar grupo enemigo: {requestedGroupId}");

            List<GameObject> groupPrefabs = battleSetup.GetEnemyPrefabsForGroup(requestedGroupId);

            if (groupPrefabs != null && groupPrefabs.Count > 0)
            {
                enemiesToSpawn = groupPrefabs;
                Debug.Log($"BattleManager: grupo {requestedGroupId} encontrado con {groupPrefabs.Count} enemigos.");
            }
            else
            {
                Debug.LogWarning($"BattleManager: grupo {requestedGroupId} no encontrado o vacío. Usando enemigos por defecto.");
            }
        }
        else
        {
            Debug.LogWarning("BattleManager: no hay GameRunState o currentBattleGroupId. Usando enemigos por defecto.");
        }

        int count = Mathf.Min(
            enemiesToSpawn != null ? enemiesToSpawn.Count : 0,
            battleSetup.enemyPositions != null ? battleSetup.enemyPositions.Count : 0
        );

        Debug.Log($"BattleManager: enemiesToSpawn = {(enemiesToSpawn != null ? enemiesToSpawn.Count : 0)}, enemyPositions = {(battleSetup.enemyPositions != null ? battleSetup.enemyPositions.Count : 0)}, count final = {count}");

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = enemiesToSpawn[i];
            Transform pos = battleSetup.enemyPositions[i];

            if (prefab == null)
            {
                Debug.LogWarning($"BattleManager: prefab enemigo en índice {i} es null.");
                continue;
            }

            if (pos == null)
            {
                Debug.LogWarning($"BattleManager: posición enemiga en índice {i} es null.");
                continue;
            }

            GameObject instanceGO = Instantiate(prefab, pos.position, Quaternion.identity, pos.parent);

            UnitData unitData = instanceGO.GetComponent<UnitData>();
            if (unitData == null)
            {
                Debug.LogWarning($"BattleManager: enemigo {prefab.name} no tiene UnitData.");
                continue;
            }

            Unit unit = unitData.CreateUnit();

            UnitView unitView = instanceGO.GetComponent<UnitView>();
            if (unitView == null)
            {
                SpriteRenderer sr = instanceGO.GetComponent<SpriteRenderer>();
                if (sr == null)
                    sr = instanceGO.AddComponent<SpriteRenderer>();

                unitView = instanceGO.AddComponent<UnitView>();
            }

            unit.unitView = unitView;
            unitView.SetUnit(unit);

            enemies.Add(unit);

            Debug.Log($"BattleManager: enemigo creado: {unit.unitName}");
        }

        return enemies;
    }

    private List<Unit> CreateEnemyUnitsFromEncounter(BattleEncounterSO encounter)
    {
        List<Unit> enemies = new List<Unit>();

        if (encounter == null)
            return enemies;

        List<EnemyDefinitionSO> expandedEnemies = encounter.ExpandEnemies();

        int count = Mathf.Min(
            expandedEnemies != null ? expandedEnemies.Count : 0,
            battleSetup.enemyPositions != null ? battleSetup.enemyPositions.Count : 0
        );

        for (int i = 0; i < count; i++)
        {
            EnemyDefinitionSO enemyDefinition = expandedEnemies[i];
            Transform pos = battleSetup.enemyPositions[i];

            if (enemyDefinition == null || pos == null)
                continue;

            Unit unit = enemyDefinition.CreateUnit();
            GameObject instanceGO = CreateEnemyViewObject(enemyDefinition, pos);

            UnitData unitData = instanceGO.GetComponent<UnitData>();
            if (unitData == null)
                unitData = instanceGO.AddComponent<UnitData>();

            unitData.unitName = enemyDefinition.enemyName;
            unitData.description = enemyDefinition.description;
            unitData.icon = enemyDefinition.icon != null ? enemyDefinition.icon : enemyDefinition.battleSprite;
            unitData.battleSprite = enemyDefinition.battleSprite != null ? enemyDefinition.battleSprite : unitData.icon;
            unit.unitData = unitData;

            UnitView unitView = instanceGO.GetComponent<UnitView>();
            if (unitView == null)
                unitView = instanceGO.AddComponent<UnitView>();

            UnitHealthBar healthBar = instanceGO.GetComponent<UnitHealthBar>();
            if (healthBar == null)
                healthBar = instanceGO.AddComponent<UnitHealthBar>();

            unit.unitView = unitView;
            unitView.SetUnit(unit);

            enemies.Add(unit);

            Debug.Log($"BattleManager: enemigo de encounter creado: {unit.unitName}");
        }

        if (expandedEnemies != null && expandedEnemies.Count > count)
            Debug.LogWarning($"BattleManager: encounter {encounter.encounterId} tiene {expandedEnemies.Count} enemigos, pero solo {count} posiciones disponibles.");

        return enemies;
    }

    private GameObject CreateEnemyViewObject(EnemyDefinitionSO enemyDefinition, Transform pos)
    {
        GameObject instanceGO;

        if (enemyDefinition.visualPrefab != null)
        {
            instanceGO = Instantiate(enemyDefinition.visualPrefab, pos.position, Quaternion.identity, pos.parent);
            instanceGO.name = enemyDefinition.enemyName;
        }
        else
        {
            instanceGO = new GameObject(enemyDefinition.enemyName);
            instanceGO.transform.SetParent(pos.parent);
            instanceGO.transform.position = pos.position;
        }

        SpriteRenderer spriteRenderer = instanceGO.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = instanceGO.AddComponent<SpriteRenderer>();

        Sprite sprite = enemyDefinition.battleSprite != null ? enemyDefinition.battleSprite : enemyDefinition.icon;
        if (sprite != null)
            spriteRenderer.sprite = sprite;

        spriteRenderer.color = sprite != null ? Color.white : GetEnemyColor(enemyDefinition.kind);

        return instanceGO;
    }

    private Color GetEnemyColor(EnemyKind kind)
    {
        switch (kind)
        {
            case EnemyKind.Beast:
                return new Color(0.8f, 0.55f, 0.35f, 1f);
            case EnemyKind.Cult:
                return new Color(0.55f, 0.35f, 0.9f, 1f);
            case EnemyKind.Aberration:
                return new Color(0.25f, 0.9f, 0.65f, 1f);
            default:
                return new Color(0.85f, 0.25f, 0.25f, 1f);
        }
    }

    public void StartBattle(List<Unit> players, List<Unit> enemies)
    {
        cleanedRuntimeViews = false;
        GameSfxPlayer.GetOrCreate();
        battleResultPanelUI = BattleResultPanelUI.GetOrCreate();
        battleResultPanelUI.Hide();

        playerUnits = new List<Unit>(players);
        enemyUnits = new List<Unit>(enemies);

        foreach (var u in playerUnits)
        {
            u.ResetAbilities();
            u.ResetTurnMeter();
            u.ResetTaunt();
            u.frontMaxTaunt = frontMaxTaunt;
            u.backMaxTaunt = backMaxTaunt;
            u.RecalculateStats();
        }

        foreach (var u in enemyUnits)
        {
            u.ResetAbilities();
            u.ResetTurnMeter();
            u.ResetTaunt();
            u.frontMaxTaunt = frontMaxTaunt;
            u.backMaxTaunt = backMaxTaunt;
            u.RecalculateStats();
        }

        battleActive = true;

        if (GameManager != null)
            GameManager.SetHealthBars(playerUnits, enemyUnits);

        BuildTurnQueue();

        Debug.Log("=== BATALLA INICIADA ===");
        Debug.Log(GetBattleStatus());
    }

    private void BuildTurnQueue()
    {
        ApplyRoundTauntDecay();

        activeUnits.Clear();
        turnQueue.Clear();

        activeUnits.AddRange(playerUnits.Where(u => u != null && u.isAlive));
        activeUnits.AddRange(enemyUnits.Where(u => u != null && u.isAlive));

        int safety = 0;

        while (turnQueue.Count < maxTurnsPerRound && activeUnits.Count > 0 && safety < 10000)
        {
            foreach (Unit unit in activeUnits)
            {
                if (unit == null || !unit.isAlive)
                    continue;

                unit.RecalculateStats();
                unit.turnMeter += unit.speed;
            }

            List<Unit> readyUnits = activeUnits
                .Where(u => u != null && u.isAlive && u.turnMeter >= turnThreshold)
                .OrderByDescending(u => u.turnMeter)
                .ThenByDescending(u => u.speed)
                .ToList();

            foreach (Unit unit in readyUnits)
            {
                if (turnQueue.Count >= maxTurnsPerRound)
                    break;

                unit.turnMeter -= turnThreshold;
                turnQueue.Enqueue(unit);
            }

            safety++;
        }

        if (safety >= 10000)
            Debug.LogWarning("BuildTurnQueue llegó al límite de seguridad.");

        currentRoundOrder = turnQueue.ToList();
        currentTurnIndex = 0;

        Debug.Log("=== NUEVA RONDA DE TURNOS ===");

        int index = 1;
        foreach (Unit unit in currentRoundOrder)
        {
            Debug.Log($"{index}. {unit.unitName} | SPD: {unit.speed} | Meter restante: {unit.turnMeter} | Taunt {unit.currentTaunt}/{unit.CurrentMaxTaunt}");
            index++;
        }
    }

    private void ApplyRoundTauntDecay()
    {
        if (playerUnits != null)
        {
            foreach (Unit unit in playerUnits)
            {
                if (unit != null && unit.isAlive)
                    unit.DecayTaunt(tauntDecayPerRound);
            }
        }

        if (enemyUnits != null)
        {
            foreach (Unit unit in enemyUnits)
            {
                if (unit != null && unit.isAlive)
                    unit.DecayTaunt(tauntDecayPerRound);
            }
        }
    }

    public void ExecuteTurn()
    {
        if (!battleActive)
            return;

        CheckBattleEnd();

        if (!battleActive)
            return;

        if (turnQueue == null || turnQueue.Count == 0)
            BuildTurnQueue();

        if (turnQueue.Count == 0)
        {
            CheckBattleEnd();
            return;
        }

        Unit currentUnit = turnQueue.Dequeue();

        currentTurnIndex++;

        if (currentUnit == null || !currentUnit.isAlive)
        {
            CheckBattleEnd();
            return;
        }

        ExecuteUnitTurn(currentUnit);

        CheckBattleEnd();

        if (!battleActive)
            return;

        if (turnQueue.Count == 0)
            BuildTurnQueue();
    }

    private void ExecuteUnitTurn(Unit currentUnit)
    {
        currentUnit.RecalculateStats();

        currentUnit.ProcessStatusEffects();

        if (!currentUnit.isAlive)
        {
            UpdateUnitVisualsIfExists(currentUnit);
            return;
        }

        if (currentUnit.unitView != null)
            currentUnit.unitView.FlashTurn();

        if (currentUnit.skipNextTurn)
        {
            Debug.Log($"[SALTO] {currentUnit.unitName} está incapacitado y pierde este turno.");
            currentUnit.skipNextTurn = false;
            return;
        }

        bool isPlayer = playerUnits.Contains(currentUnit);

        List<Unit> allies = isPlayer ? playerUnits : enemyUnits;
        List<Unit> enemies = isPlayer ? enemyUnits : playerUnits;

        bool usedConsumable = ConsumableResolver.TryUseFirstValidConsumable(
            currentUnit,
            allies,
            enemies,
            this
        );

        if (usedConsumable)
            return;

        AbilitySO ability = currentUnit.GetAbilityForTurn(allies, enemies, this);

        if (ability != null)
        {
            if (currentUnit.unitView != null)
                currentUnit.unitView.AttackMotion();

            ability.Execute(currentUnit, allies, enemies, this);
        }
        else
        {
            Unit target = isPlayer ? GetFirstAliveTarget(enemies) : GetHighestTauntTarget(enemies);

            if (target != null)
                PerformAttack(currentUnit, target);
            else
                Debug.Log("No hay objetivos vivos para atacar.");
        }
    }

    private Unit GetFirstAliveTarget(List<Unit> targets)
    {
        foreach (Unit unit in targets)
        {
            if (unit != null && unit.isAlive)
                return unit;
        }

        return null;
    }

    private Unit GetHighestTauntTarget(List<Unit> targets)
    {
        return targets
            .Where(u => u != null && u.isAlive)
            .OrderByDescending(u => u.TotalTaunt)
            .ThenByDescending(u => u.currentHP)
            .FirstOrDefault();
    }

    private void PerformAttack(Unit attacker, Unit target)
    {
        PerformAttack(attacker, target, 1f);
    }

    private void PerformAttack(Unit attacker, Unit target, float multiplier)
    {
        if (attacker.unitView != null)
            attacker.unitView.AttackMotion();

        int physAtk = Mathf.RoundToInt(attacker.RollPhysicalDamage() * multiplier);
        int magAtk = Mathf.RoundToInt(attacker.RollMagicalDamage() * multiplier);

        var (physArmorAbsorbed, magArmorAbsorbed, hpDamage) = target.TakeDamage(physAtk, magAtk);

        if (target.unitView != null)
        {
            target.unitView.ShowDamageBreakdown(physArmorAbsorbed, magArmorAbsorbed, hpDamage);
            target.unitView.FlashHit(Color.white);
            target.unitView.ShakeOnHit();
        }

        bool attackDealtDamage = physArmorAbsorbed > 0 || magArmorAbsorbed > 0 || hpDamage > 0;

        if (GameSfxPlayer.Instance != null && attackDealtDamage)
            GameSfxPlayer.Instance.PlayHitDelayed();

        bool attackerIsPlayer = playerUnits.Contains(attacker);

        if (attackerIsPlayer)
            attacker.AddTaunt(30);

        Debug.Log($"⚔️ {attacker.unitName} ataca a {target.unitName}");
        Debug.Log($"   Daño Físico: {physAtk} | Daño Mágico: {magAtk}");
        Debug.Log($"   Armadura Física absorbió: {physArmorAbsorbed} | Armadura Mágica absorbió: {magArmorAbsorbed} | Daño a HP: {hpDamage}");
        Debug.Log($"   {target.GetInfo()}");

        if (!target.isAlive)
        {
            if (GameSfxPlayer.Instance != null)
                GameSfxPlayer.Instance.PlayDeathDelayed();

            Debug.Log($"💀 {target.unitName} ha sido derrotado.");
        }

        UpdateUnitVisualsIfExists(target);
    }

    public void UpdateUnitVisualsIfExists(Unit unit)
    {
        if (unit != null && unit.unitView != null)
            unit.unitView.UpdateVisuals();
    }

    public void CheckBattleEnd()
    {
        bool playerAlive = playerUnits.Any(u => u != null && u.isAlive);
        bool enemiesAlive = enemyUnits.Any(u => u != null && u.isAlive);

        if (!playerAlive)
            EndBattle(false);
        else if (!enemiesAlive)
            EndBattle(true);
    }

    private void EndBattle(bool playerWon)
    {
        battleActive = false;
        turnQueue.Clear();

        currentRoundOrder.Clear();
        currentTurnIndex = 0;

        CleanupRuntimePartyViews();

        if (playerWon)
        {
            if (GameSfxPlayer.Instance != null)
                GameSfxPlayer.Instance.PlayVictory();

            Debug.Log("\n🎉 ¡EL JUGADOR HA GANADO!\n");
        }
        else
        {
            if (GameSfxPlayer.Instance != null)
                GameSfxPlayer.Instance.PlayDefeat();

            Debug.Log("\n💀 ¡LOS ENEMIGOS HAN GANADO!\n");
        }

        if (GameRunState.Instance != null)
        {
            GameRunState.Instance.RegisterCombatResult(playerWon);

            string returnSceneName = string.IsNullOrWhiteSpace(GameRunState.Instance.returnSceneName)
                ? "WorldMapScene"
                : GameRunState.Instance.returnSceneName;

            ShowBattleResultPanel(playerWon, returnSceneName);
        }
        else
        {
            Debug.Log("BattleManager: combate terminado sin GameRunState. Usando WorldMapScene como retorno por defecto.");
            ShowBattleResultPanel(playerWon, "WorldMapScene");
        }
    }

    private void ShowBattleResultPanel(bool playerWon, string returnSceneName)
    {
        if (battleResultPanelUI == null)
            battleResultPanelUI = BattleResultPanelUI.GetOrCreate();

        if (playerWon)
        {
            battleResultPanelUI.ShowVictory(BuildVictoryRewardText(), returnSceneName);
        }
        else
        {
            battleResultPanelUI.ShowDefeat(returnSceneName);
        }
    }

    private string BuildVictoryRewardText()
    {
        RewardData reward = null;

        if (GameRunState.Instance != null && GameRunState.Instance.hasPendingBattleReward)
            reward = GameRunState.Instance.pendingBattleReward;

        if (reward == null || reward.IsEmpty())
            return "Victoria. Sin recompensas registradas.";

        StringBuilder builder = new StringBuilder();
        builder.Append(RewardApplier.BuildRewardSummary(reward));

        AppendLevelUpPreview(builder, reward);

        return builder.ToString();
    }

    private void AppendLevelUpPreview(StringBuilder builder, RewardData reward)
    {
        if (builder == null || reward == null || reward.experience <= 0)
            return;

        List<Unit> party = PartyRuntimeState.Instance != null
            ? PartyRuntimeState.Instance.GetCurrentParty()
            : playerUnits;

        if (party == null || party.Count == 0)
            return;

        foreach (Unit unit in party)
        {
            if (unit == null)
                continue;

            int projectedLevel = GetProjectedLevelAfterExperience(unit, reward.experience);

            if (projectedLevel <= unit.level)
                continue;

            builder.AppendLine();
            builder.Append(unit.unitName);
            builder.Append(" ha subido al nivel ");
            builder.Append(projectedLevel);
            builder.Append(".");

            AppendLevelUpRewards(builder, unit, projectedLevel);
        }
    }

    private void AppendLevelUpRewards(StringBuilder builder, Unit unit, int projectedLevel)
    {
        if (builder == null || unit == null || projectedLevel <= unit.level)
            return;

        UnitLevelGrowth totalGrowth = BuildTotalGrowth(unit, unit.level + 1, projectedLevel);
        List<AbilitySO> learnedAbilities = GetProjectedLearnedAbilities(unit, projectedLevel);

        string growthText = BuildGrowthText(totalGrowth);
        if (!string.IsNullOrWhiteSpace(growthText))
        {
            builder.AppendLine();
            builder.Append(growthText);
        }

        if (learnedAbilities.Count == 0)
            return;

        foreach (AbilitySO ability in learnedAbilities)
        {
            builder.AppendLine();
            builder.Append("Aprende ");
            builder.Append(!string.IsNullOrWhiteSpace(ability.abilityName) ? ability.abilityName : ability.name);
            builder.Append(".");
        }
    }

    private UnitLevelGrowth BuildTotalGrowth(Unit unit, int firstLevel, int lastLevel)
    {
        UnitLevelGrowth total = new UnitLevelGrowth();

        if (unit == null)
            return total;

        for (int level = firstLevel; level <= lastLevel; level++)
        {
            UnitLevelGrowth growth = unit.GetGrowthForLevel(level);

            if (growth == null)
                continue;

            total.strength += growth.strength;
            total.dexterity += growth.dexterity;
            total.intelligence += growth.intelligence;
            total.constitution += growth.constitution;
            total.stamina += growth.stamina;
            total.mana += growth.mana;
            total.physicalArmor += growth.physicalArmor;
            total.magicalArmor += growth.magicalArmor;
        }

        return total;
    }

    private string BuildGrowthText(UnitLevelGrowth growth)
    {
        if (growth == null)
            return "";

        List<string> parts = new List<string>();

        AddGrowthPart(parts, growth.strength, "Fuerza");
        AddGrowthPart(parts, growth.dexterity, "Destreza");
        AddGrowthPart(parts, growth.intelligence, "Inteligencia");
        AddGrowthPart(parts, growth.constitution, "Constitucion");
        AddGrowthPart(parts, growth.stamina, "Stamina");
        AddGrowthPart(parts, growth.mana, "Mana");
        AddGrowthPart(parts, growth.physicalArmor, "Armadura fisica");
        AddGrowthPart(parts, growth.magicalArmor, "Armadura magica");

        return parts.Count > 0 ? string.Join(", ", parts) + "." : "";
    }

    private void AddGrowthPart(List<string> parts, int amount, string label)
    {
        if (parts == null || amount == 0)
            return;

        string sign = amount > 0 ? "+" : "";
        parts.Add(label + " " + sign + amount);
    }

    private List<AbilitySO> GetProjectedLearnedAbilities(Unit unit, int projectedLevel)
    {
        List<AbilitySO> learned = new List<AbilitySO>();

        if (unit == null ||
            unit.unitData == null ||
            !unit.unitData.includeClassAbilities ||
            unit.unitData.characterClass == null)
        {
            return learned;
        }

        List<AbilitySO> previousAbilities = unit.unitData.characterClass.GetAbilitiesForLevel(unit.level);
        List<AbilitySO> projectedAbilities = unit.unitData.characterClass.GetAbilitiesForLevel(projectedLevel);

        if (projectedAbilities == null)
            return learned;

        foreach (AbilitySO ability in projectedAbilities)
        {
            if (ability == null)
                continue;

            bool alreadyAvailableByLevel = previousAbilities != null && previousAbilities.Contains(ability);
            bool alreadyKnown = unit.abilities != null && unit.abilities.Contains(ability);

            if (!alreadyAvailableByLevel && !alreadyKnown && !learned.Contains(ability))
                learned.Add(ability);
        }

        return learned;
    }

    private int GetProjectedLevelAfterExperience(Unit unit, int experienceReward)
    {
        if (unit == null || experienceReward <= 0)
            return unit != null ? unit.level : 0;

        int projectedLevel = unit.level;
        int projectedExperience = Mathf.Max(0, unit.experience + experienceReward);

        while (projectedLevel < unit.maxLevel &&
               projectedExperience >= unit.GetExperienceRequiredForLevel(projectedLevel + 1))
        {
            projectedLevel++;
        }

        return projectedLevel;
    }

    private void CleanupRuntimePartyViews()
    {
        if (cleanedRuntimeViews)
            return;

        cleanedRuntimeViews = true;

        if (partyRuntimeState != null)
            partyRuntimeState.ClearSceneViews();
    }

    private void OnDestroy()
    {
        CleanupRuntimePartyViews();
    }

    public List<Unit> GetCurrentRoundOrder()
    {
        return currentRoundOrder;
    }

    public int GetCurrentTurnIndex()
    {
        return currentTurnIndex;
    }

    public string GetBattleStatus()
    {
        string status = "=== ESTADO DE LA BATALLA ===\n";

        status += "Aliados:\n";
        foreach (Unit unit in playerUnits)
        {
            status += $"  {unit.GetInfo()}";

            if (unit.skipNextTurn)
                status += " (saltará turno)";

            status += "\n";
        }

        status += "\nEnemigos:\n";
        foreach (Unit unit in enemyUnits)
        {
            status += $"  {unit.GetInfo()}";

            if (unit.skipNextTurn)
                status += " (saltará turno)";

            status += "\n";
        }

        return status;
    }

    public void AutomateBattle()
    {
        StartCoroutine(AutomateBattleCoroutine());
    }

    private System.Collections.IEnumerator AutomateBattleCoroutine()
    {
        while (battleActive)
        {
            ExecuteTurn();
            yield return new WaitForSeconds(1f);
        }
    }
}
