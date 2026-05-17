using System;
using UnityEngine;

[Serializable]
public class InventoryEntry
{
    public string itemId;
    public int amount;

    public InventoryEntry(string itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }
}