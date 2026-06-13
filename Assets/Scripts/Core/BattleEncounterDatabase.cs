using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Caravan RPG/Battle Encounter Database")]
public class BattleEncounterDatabase : ScriptableObject
{
    public List<BattleEncounterSO> encounters = new List<BattleEncounterSO>();

    public BattleEncounterSO GetEncounterById(string encounterId)
    {
        if (string.IsNullOrWhiteSpace(encounterId) || encounters == null)
            return null;

        foreach (BattleEncounterSO encounter in encounters)
        {
            if (encounter != null && encounter.encounterId == encounterId)
                return encounter;
        }

        return null;
    }
}
