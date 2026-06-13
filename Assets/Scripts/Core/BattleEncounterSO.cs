using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleEnemyEntry
{
    public EnemyDefinitionSO enemy;
    public int amount = 1;
}

[CreateAssetMenu(menuName = "Caravan RPG/Battle Encounter")]
public class BattleEncounterSO : ScriptableObject
{
    [Header("Identity")]
    public string encounterId;
    public string encounterName;

    [TextArea(3, 8)]
    public string description;

    [Header("Enemies")]
    public List<BattleEnemyEntry> enemies = new List<BattleEnemyEntry>();

    [Header("Rewards")]
    public RewardData reward = new RewardData();

    [Header("Flags")]
    public List<string> requiredFlags = new List<string>();
    public List<string> blockedByFlags = new List<string>();

    public bool CanStart(GameRunState runState)
    {
        if (runState == null)
            return true;

        if (requiredFlags != null)
        {
            foreach (string flag in requiredFlags)
            {
                if (!string.IsNullOrWhiteSpace(flag) && !runState.HasFlag(flag))
                    return false;
            }
        }

        if (blockedByFlags != null)
        {
            foreach (string flag in blockedByFlags)
            {
                if (!string.IsNullOrWhiteSpace(flag) && runState.HasFlag(flag))
                    return false;
            }
        }

        return true;
    }

    public List<EnemyDefinitionSO> ExpandEnemies()
    {
        List<EnemyDefinitionSO> result = new List<EnemyDefinitionSO>();

        if (enemies == null)
            return result;

        foreach (BattleEnemyEntry entry in enemies)
        {
            if (entry == null || entry.enemy == null)
                continue;

            int amount = Mathf.Max(1, entry.amount);

            for (int i = 0; i < amount; i++)
                result.Add(entry.enemy);
        }

        return result;
    }
}
