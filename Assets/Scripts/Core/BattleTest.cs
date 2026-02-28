using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// Script de prueba para ejecutar combates automáticos
public class BattleTest : MonoBehaviour
{
    public BattleManager battleManager;


    // Posiciones donde se instanciarán las unidades
    public Transform[] mercenarioPosiciones = new Transform[3];
    public Transform[] enemigoPosiciones = new Transform[8];

    // Prefabs de las unidades
    public GameObject[] mercenarioPrefabs = new GameObject[3];
    public GameObject[] enemigoPrefabs = new GameObject[8];

    

    private void Start()
    {
        

        // Instanciar los mercenarios en sus posiciones y crear las unidades
        List<Unit> playerUnits = CreatePlayerUnits();

        // Instanciar los enemigos en sus posiciones y crear las unidades
        List<Unit> enemyUnits = CreateEnemyUnits();

        // Iniciar el combate
        battleManager.StartBattle(playerUnits, enemyUnits);

        // Ejecutar la batalla automática completa
        battleManager.AutomateBattle();

        // Mostrar estado final
        Debug.Log(battleManager.GetBattleStatus());
    }

   

    // Instancia los prefabs de mercenarios en sus posiciones y crea las unidades
    private List<Unit> CreatePlayerUnits()
    {
        List<Unit> units = new List<Unit>();

        for (int i = 0; i < 3; i++)
        {
            if (mercenarioPrefabs[i] != null && mercenarioPosiciones[i] != null)
            {
                // Instanciar el prefab en la posición
                GameObject instanceGO = Instantiate(mercenarioPrefabs[i], 
                    mercenarioPosiciones[i].position, 
                    Quaternion.identity, 
                    mercenarioPosiciones[i].parent);

                // Obtener el componente UnitData del instanciado
                UnitData unitData = instanceGO.GetComponent<UnitData>();
                if (unitData != null)
                {
                    // Crear la unidad a partir de los datos del prefab
                    Unit unit = unitData.CreateUnit();

                    // Agregar la visualización
                    UnitView unitView = instanceGO.GetComponent<UnitView>();
                    if (unitView == null)
                    {
                        SpriteRenderer spriteRenderer = instanceGO.GetComponent<SpriteRenderer>();
                        if (spriteRenderer == null)
                        {
                            spriteRenderer = instanceGO.AddComponent<SpriteRenderer>();
                        }
                        unitView = instanceGO.AddComponent<UnitView>();
                    }

                    unit.unitView = unitView;
                    unitView.SetUnit(unit);

                    // Asignar daños y armaduras para prueba (si es mercenario)
                    unit.physicalDamage = 10;
                    unit.magicalDamage = 5;
                    unit.maxPhysicalArmor = 15;
                    unit.currentPhysicalArmor = 15;
                    unit.maxMagicalArmor = 8;
                    unit.currentMagicalArmor = 8;

                    units.Add(unit);

                    Debug.Log($"Mercenario instanciado: {unit.GetInfo()}");
                }
                else
                {
                    Debug.LogWarning($"El prefab mercenario {i} no tiene componente UnitData");
                }
            }
            else
            {
                Debug.LogWarning($"Prefab o posición de mercenario {i} no asignado");
            }
        }

        return units;
    }

    // Instancia los prefabs de enemigos en sus posiciones y crea las unidades
    private List<Unit> CreateEnemyUnits()
    {
        List<Unit> units = new List<Unit>();

        for (int i = 0; i < 8; i++)
        {
            if (enemigoPrefabs[i] != null && enemigoPosiciones[i] != null)
            {
                // Instanciar el prefab en la posición
                GameObject instanceGO = Instantiate(enemigoPrefabs[i], 
                    enemigoPosiciones[i].position, 
                    Quaternion.identity, 
                    enemigoPosiciones[i].parent);

                // Obtener el componente UnitData del instanciado
                UnitData unitData = instanceGO.GetComponent<UnitData>();
                if (unitData != null)
                {
                    // Crear la unidad a partir de los datos del prefab
                    Unit unit = unitData.CreateUnit();

                    // Agregar la visualización
                    UnitView unitView = instanceGO.GetComponent<UnitView>();
                    if (unitView == null)
                    {
                        SpriteRenderer spriteRenderer = instanceGO.GetComponent<SpriteRenderer>();
                        if (spriteRenderer == null)
                        {
                            spriteRenderer = instanceGO.AddComponent<SpriteRenderer>();
                        }
                        unitView = instanceGO.AddComponent<UnitView>();
                    }

                    unit.unitView = unitView;
                    unitView.SetUnit(unit);

                    // Asignar daños y armaduras para prueba (si es enemigo)
                    unit.physicalDamage = 8;
                    unit.magicalDamage = 3;
                    unit.maxPhysicalArmor = 10;
                    unit.currentPhysicalArmor = 10;
                    unit.maxMagicalArmor = 5;
                    unit.currentMagicalArmor = 5;

                    units.Add(unit);

                    Debug.Log($"Enemigo instanciado: {unit.GetInfo()}");
                }
                else
                {
                    Debug.LogWarning($"El prefab enemigo {i} no tiene componente UnitData");
                }
            }
            else
            {
                Debug.LogWarning($"Prefab o posición de enemigo {i} no asignado");
            }
        }

        return units;
    }
}


