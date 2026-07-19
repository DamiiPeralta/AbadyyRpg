using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    [Header("Info")]
    public string itemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Tipo")]
    public ItemType itemType;
    public ItemTier itemTier = ItemTier.Tier1;

    [Header("Economia")]
    public int sellValue = 0;

    [Header("Inventario")]
    public bool isStackable = true;

    [Header("Efectos")]
    public List<ItemEffect> effects = new List<ItemEffect>();

    public virtual bool IsStackable => isStackable;
}
