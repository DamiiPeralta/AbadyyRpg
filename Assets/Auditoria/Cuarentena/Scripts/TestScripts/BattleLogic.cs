using UnityEngine;
using System.Collections.Generic;

// Reemplazo de BattleTest: maneja la lógica de inicialización de una batalla
public class BattleLogic : MonoBehaviour
{
    public BattleManager battleManager;
    public BattleSetup battleSetup;

    private void Start()
    {
        if (battleManager == null)
        {
            Debug.LogError("BattleLogic: battleManager no asignado");
            return;
        }

        if (battleSetup == null)
        {
            Debug.LogError("BattleLogic: battleSetup no asignado");
            return;
        }

        List<Unit> playerUnits = CreateUnitsFromSetup(battleSetup.allyPrefabs, battleSetup.allyPositions, true);
        List<Unit> enemyUnits = CreateUnitsFromSetup(battleSetup.enemyPrefabs, battleSetup.enemyPositions, false);

        battleManager.StartBattle(playerUnits, enemyUnits);
        battleManager.AutomateBattle();

        Debug.Log(battleManager.GetBattleStatus());
    }

    private List<Unit> CreateUnitsFromSetup(List<GameObject> prefabs, List<Transform> positions, bool isPlayer)
    {
        List<Unit> units = new List<Unit>();

        int count = Mathf.Min(prefabs != null ? prefabs.Count : 0, positions != null ? positions.Count : 0);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabs[i];
            Transform pos = positions[i];

            if (prefab == null || pos == null)
            {
                Debug.LogWarning($"BattleLogic: prefab o posición nulo en índice {i}");
                continue;
            }

            GameObject instanceGO = Instantiate(prefab, pos.position, Quaternion.identity, pos.parent);

            UnitData unitData = instanceGO.GetComponent<UnitData>();
            if (unitData == null)
            {
                Debug.LogWarning($"BattleLogic: prefab en índice {i} no tiene UnitData");
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

            // Reiniciar índice de habilidades por si el objeto se reutiliza en otra batalla
            unit.ResetAbilities();

            // Los valores de daño y armadura ahora se configuran en el prefab (UnitData).
            // Si necesita ajustes dinámicos, se pueden aplicar aquí después de la creación.

            units.Add(unit);
            Debug.Log($"Unit instanciada: {unit.GetInfo()}");
        }

        return units;
    }
}
