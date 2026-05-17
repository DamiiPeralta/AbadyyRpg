using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Caravan RPG/Party Setup")]
public class PartySetup : ScriptableObject
{
    public List<UnitData> startingUnits = new List<UnitData>();
}