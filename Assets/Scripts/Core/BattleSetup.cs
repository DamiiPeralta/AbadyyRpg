using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public string groupId = "TestBattle";
    public List<GameObject> enemyPrefabs = new List<GameObject>();
}

public class BattleSetup : MonoBehaviour
{
    [Header("Aliados")]
    public List<GameObject> allyPrefabs = new List<GameObject>();
    public List<Transform> allyPositions = new List<Transform>();

    [Header("Enemigos por defecto")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<Transform> enemyPositions = new List<Transform>();

    [Header("Enemy Groups")]
    public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

    [Header("Battle Encounters")]
    public BattleEncounterDatabase encounterDatabase;

    public List<GameObject> GetEnemyPrefabsForGroup(string groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            Debug.LogWarning("BattleSetup: groupId vacio. Usando enemyPrefabs por defecto.");
            return enemyPrefabs;
        }

        foreach (EnemyGroup group in enemyGroups)
        {
            if (group != null && group.groupId == groupId)
            {
                if (group.enemyPrefabs == null || group.enemyPrefabs.Count == 0)
                {
                    Debug.LogWarning($"BattleSetup: el grupo {groupId} no tiene enemigos. Usando enemyPrefabs por defecto.");
                    return enemyPrefabs;
                }

                return group.enemyPrefabs;
            }
        }

        Debug.LogWarning($"BattleSetup: no se encontro EnemyGroup con id {groupId}. Usando enemyPrefabs por defecto.");
        return enemyPrefabs;
    }

    public BattleEncounterSO GetEncounterById(string encounterId)
    {
        if (encounterDatabase == null)
            return null;

        BattleEncounterSO encounter = encounterDatabase.GetEncounterById(encounterId);

        if (encounter != null && encounter.CanStart(GameRunState.Instance))
            return encounter;

        return null;
    }
}
