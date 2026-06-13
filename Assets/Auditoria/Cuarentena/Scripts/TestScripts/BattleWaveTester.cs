using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWaveTester : MonoBehaviour
{
    [Header("Referencias")]
    public BattleManager battleManager;
    public BattleSetup battleSetup;
    public PartyRuntimeState partyRuntimeState;

    [Header("Enemigos disponibles")]
    public GameObject skeletonPrefab;
    public GameObject ratPrefab;
    public GameObject lesserDemonPrefab;
    public GameObject wormPrefab;

    [Header("Timing")]
    public float delayBetweenBattles = 2f;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(RunBattleSequence());
    }

    private IEnumerator RunBattleSequence()
    {
        if (battleManager == null || battleSetup == null)
        {
            Debug.LogError("BattleWaveTester: falta BattleManager o BattleSetup.");
            yield break;
        }

        if (partyRuntimeState == null)
            partyRuntimeState = PartyRuntimeState.Instance;

        if (partyRuntimeState == null)
            partyRuntimeState = FindObjectOfType<PartyRuntimeState>();

        if (partyRuntimeState == null)
        {
            Debug.LogError("BattleWaveTester: falta PartyRuntimeState.");
            yield break;
        }

        List<List<GameObject>> waves = CreateWaves();

        for (int i = 0; i < waves.Count; i++)
        {
            Debug.Log($"\n=== COMBATE {i + 1}/{waves.Count} ===\n");

            ClearSpawnedObjects();

            List<Unit> allies = partyRuntimeState.GetOrCreatePartyFromSetup(battleSetup);
            AttachAllyViews(allies);

            List<Unit> enemies = CreateEnemyUnits(waves[i], battleSetup.enemyPositions);

            battleManager.StartBattle(allies, enemies);
            battleManager.AutomateBattle();

            while (battleManager.battleActive)
                yield return null;

            Debug.Log($"\n--- Estado persistente aliado tras combate {i + 1} ---");
            partyRuntimeState.LogPartyState();

            yield return new WaitForSeconds(delayBetweenBattles);
        }

        Debug.Log("\n=== SECUENCIA DE COMBATES TERMINADA ===\n");
    }

    private List<List<GameObject>> CreateWaves()
    {
        return new List<List<GameObject>>
        {
            new List<GameObject> { ratPrefab, ratPrefab, skeletonPrefab },
            new List<GameObject> { ratPrefab, wormPrefab },
            new List<GameObject> { skeletonPrefab, skeletonPrefab, skeletonPrefab },
            new List<GameObject> { lesserDemonPrefab, skeletonPrefab, ratPrefab },
            new List<GameObject> { lesserDemonPrefab, wormPrefab, skeletonPrefab, ratPrefab }
        };
    }

    private void AttachAllyViews(List<Unit> allies)
    {
        if (allies == null || battleSetup == null)
            return;

        int count = Mathf.Min(
            allies.Count,
            Mathf.Min(battleSetup.allyPrefabs.Count, battleSetup.allyPositions.Count)
        );

        for (int i = 0; i < count; i++)
        {
            Unit unit = allies[i];
            GameObject prefab = battleSetup.allyPrefabs[i];
            Transform pos = battleSetup.allyPositions[i];

            if (unit == null || prefab == null || pos == null)
                continue;

            GameObject instance = Instantiate(prefab, pos.position, Quaternion.identity, pos.parent);
            spawnedObjects.Add(instance);

            UnitView view = instance.GetComponent<UnitView>();

            if (view == null)
                view = instance.AddComponent<UnitView>();

            unit.unitView = view;
            view.SetUnit(unit);
        }
    }

    private List<Unit> CreateEnemyUnits(List<GameObject> prefabs, List<Transform> positions)
    {
        List<Unit> units = new List<Unit>();

        if (prefabs == null || positions == null)
            return units;

        int count = Mathf.Min(prefabs.Count, positions.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabs[i];
            Transform pos = positions[i];

            if (prefab == null || pos == null)
                continue;

            GameObject instance = Instantiate(prefab, pos.position, Quaternion.identity, pos.parent);
            spawnedObjects.Add(instance);

            UnitData data = instance.GetComponent<UnitData>();

            if (data == null)
            {
                Debug.LogError($"{instance.name} no tiene UnitData.");
                continue;
            }

            Unit unit = data.CreateUnit();

            UnitView view = instance.GetComponent<UnitView>();

            if (view == null)
                view = instance.AddComponent<UnitView>();

            unit.unitView = view;
            view.SetUnit(unit);

            unit.ResetAbilities();
            unit.ResetTurnMeter();

            units.Add(unit);
        }

        return units;
    }

    private void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();
    }
}