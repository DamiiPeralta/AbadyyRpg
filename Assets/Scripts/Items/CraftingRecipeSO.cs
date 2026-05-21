using System.Collections.Generic;
using UnityEngine;

public enum CraftingStationType
{
    Blacksmith,
    Cook,
    Alchemist,
    RunicMage
}

public enum RecipeStackType
{
    Item,
    Gold,
    Food,
    Wood,
    Stone,
    Iron,
    Leather,
    Crystals
}

[System.Serializable]
public class RecipeStack
{
    public RecipeStackType stackType = RecipeStackType.Item;
    public ItemBase item;
    public string itemId;
    public int amount = 1;

    public string GetItemId()
    {
        if (item != null && !string.IsNullOrWhiteSpace(item.itemId))
            return item.itemId;

        return itemId;
    }

    public ItemBase GetItem()
    {
        if (item != null)
            return item;

        return ItemDatabase.Instance != null ? ItemDatabase.Instance.GetItemById(itemId) : null;
    }
}

[CreateAssetMenu(menuName = "Caravan/Crafting Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [Header("Info")]
    public string recipeId;
    public string recipeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Station")]
    public CraftingStationType stationType = CraftingStationType.Cook;

    [Header("Preview")]
    public ItemBase previewItem;

    [Header("Transformation")]
    public List<RecipeStack> inputs = new List<RecipeStack>();
    public List<RecipeStack> outputs = new List<RecipeStack>();

    public Sprite GetIcon()
    {
        if (icon != null)
            return icon;

        if (previewItem != null)
            return previewItem.icon;

        ItemBase firstOutput = GetFirstOutputItem();
        return firstOutput != null ? firstOutput.icon : null;
    }

    public string GetDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(recipeName))
            return recipeName;

        if (previewItem != null && !string.IsNullOrWhiteSpace(previewItem.itemName))
            return previewItem.itemName;

        ItemBase firstOutput = GetFirstOutputItem();
        return firstOutput != null ? firstOutput.itemName : name;
    }

    public ItemBase GetFirstOutputItem()
    {
        if (outputs == null || ItemDatabase.Instance == null)
            return null;

        foreach (RecipeStack output in outputs)
        {
            if (output == null || output.stackType != RecipeStackType.Item)
                continue;

            ItemBase item = output.GetItem();

            if (item != null)
                return item;
        }

        return null;
    }
}
