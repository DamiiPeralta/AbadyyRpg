using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardData
{
    [Header("Resources")]
    public int gold;
    public int food;
    public int wood;
    public int iron;
    public int leather;

    [Header("Items")]
    public List<RewardItemEntry> items = new List<RewardItemEntry>();

    public bool IsEmpty()
    {
        bool hasResources =
            gold > 0 ||
            food > 0 ||
            wood > 0 ||
            iron > 0 ||
            leather > 0;

        bool hasItems = items != null && items.Count > 0;

        return !hasResources && !hasItems;
    }
}

[Serializable]
public class RewardItemEntry
{
    public string itemId;
    public int amount = 1;
}