using System;
using UnityEngine;

public enum CaravanFireActionType
{
    PartialRest,
    RepairPhysicalArmor,
    RepairMagicalArmor,
    Sleep
}

public enum CaravanResourceType
{
    Food,
    Wood,
    Iron,
    Leather,
    Crystals
}

[Serializable]
public class CaravanFireResourceCost
{
    public CaravanResourceType resourceType;
    public int amount = 1;
    public bool perLivingRosterMember = false;

    public int GetTotalCost(int livingRosterMembers)
    {
        int baseAmount = Mathf.Max(0, amount);
        return perLivingRosterMember ? baseAmount * Mathf.Max(0, livingRosterMembers) : baseAmount;
    }
}
