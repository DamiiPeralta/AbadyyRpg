using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryTestSeeder : MonoBehaviour
{
    [Header("Modo")]
    public bool seedOnStart = true;
    public bool clearInventoryBeforeSeed = true;
    public bool setResources = true;
    public int defaultItemAmount = 3;

    [Header("Recursos iniciales")]
    public int gold = 2450;
    public int food = 2450;
    public int wood = 2450;
    public int stone = 2450;
    public int iron = 2450;
    public int leather = 2450;
    public int crystals = 2450;

    [Header("Items por asset")]
    public List<SeedItemAssetEntry> itemAssets = new List<SeedItemAssetEntry>();

    [Header("Items por ID")]
    public List<SeedItemIdEntry> itemIds = new List<SeedItemIdEntry>();

    private void Start()
    {
        if (seedOnStart)
            Seed();
    }

    [ContextMenu("Seed Inventory Now")]
    public void Seed()
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null)
        {
            Debug.LogWarning("InventoryTestSeeder: no hay InventoryRuntimeState en escena.");
            return;
        }

        if (clearInventoryBeforeSeed)
            inventory.items.Clear();

        if (setResources)
            ApplyResources(inventory);

        AddAssetEntries(inventory);
        AddIdEntries(inventory);

        RefreshSuppliesPanels();
    }

    [ContextMenu("Seed And Register Items Now")]
    public void SeedAndRegisterItems()
    {
        RegisterSeedItemsInDatabase();
        Seed();
    }

    [ContextMenu("Clear Inventory Items")]
    public void ClearInventoryItems()
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null)
            return;

        inventory.items.Clear();
        RefreshSuppliesPanels();
    }

    private void ApplyResources(InventoryRuntimeState inventory)
    {
        inventory.gold = Mathf.Max(0, gold);
        inventory.food = Mathf.Max(0, food);
        inventory.wood = Mathf.Max(0, wood);
        inventory.stone = Mathf.Max(0, stone);
        inventory.iron = Mathf.Max(0, iron);
        inventory.leather = Mathf.Max(0, leather);
        inventory.crystals = Mathf.Max(0, crystals);
    }

    private void AddAssetEntries(InventoryRuntimeState inventory)
    {
        foreach (SeedItemAssetEntry entry in itemAssets)
        {
            if (entry == null || entry.item == null || entry.amount <= 0)
                continue;

            inventory.AddItem(entry.item.itemId, entry.amount);
        }
    }

    private void AddIdEntries(InventoryRuntimeState inventory)
    {
        foreach (SeedItemIdEntry entry in itemIds)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.itemId) || entry.amount <= 0)
                continue;

            inventory.AddItem(entry.itemId, entry.amount);
        }
    }

    private void RefreshSuppliesPanels()
    {
        SuppliesResourcesPanelUI[] resourcesPanels = FindObjectsOfType<SuppliesResourcesPanelUI>(true);
        foreach (SuppliesResourcesPanelUI panel in resourcesPanels)
            panel.Refresh();

        SuppliesPanelUI[] suppliesPanels = FindObjectsOfType<SuppliesPanelUI>(true);
        foreach (SuppliesPanelUI panel in suppliesPanels)
            panel.Refresh();
    }

    [ContextMenu("Register Seed Items In ItemDatabase")]
    public void RegisterSeedItemsInDatabase()
    {
        ItemDatabase database = ItemDatabase.Instance != null ? ItemDatabase.Instance : FindObjectOfType<ItemDatabase>(true);

        if (database == null)
        {
            Debug.LogWarning("InventoryTestSeeder: no hay ItemDatabase en escena.");
            return;
        }

        bool changed = false;

        foreach (SeedItemAssetEntry entry in itemAssets)
        {
            if (entry == null || entry.item == null)
                continue;

            if (!database.items.Contains(entry.item))
            {
                database.items.Add(entry.item);
                changed = true;
            }
        }

        if (changed)
        {
            database.BuildDatabase();
#if UNITY_EDITOR
            EditorUtility.SetDirty(database);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(database.gameObject.scene);
#endif
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Load All Project Items Into Seed List")]
    public void LoadAllProjectItemsIntoSeedList()
    {
        itemAssets.Clear();

        string[] guids = AssetDatabase.FindAssets("t:ItemBase", new[] { "Assets/Items" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemBase item = AssetDatabase.LoadAssetAtPath<ItemBase>(path);

            if (item == null)
                continue;

            itemAssets.Add(new SeedItemAssetEntry
            {
                item = item,
                amount = Mathf.Max(1, defaultItemAmount)
            });
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"InventoryTestSeeder: cargados {itemAssets.Count} items del proyecto.");
    }

    [ContextMenu("Load Weapons Into Seed List")]
    public void LoadWeaponsIntoSeedList()
    {
        LoadProjectItemsByType(ItemType.Weapon);
    }

    [ContextMenu("Load Consumables Into Seed List")]
    public void LoadConsumablesIntoSeedList()
    {
        LoadProjectItemsByType(ItemType.Consumable);
    }

    [ContextMenu("Load Armors Into Seed List")]
    public void LoadArmorsIntoSeedList()
    {
        LoadProjectItemsByType(ItemType.Armor);
    }

    private void LoadProjectItemsByType(ItemType itemType)
    {
        itemAssets.Clear();

        string[] guids = AssetDatabase.FindAssets("t:ItemBase", new[] { "Assets/Items" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemBase item = AssetDatabase.LoadAssetAtPath<ItemBase>(path);

            if (item == null || item.itemType != itemType)
                continue;

            itemAssets.Add(new SeedItemAssetEntry
            {
                item = item,
                amount = Mathf.Max(1, defaultItemAmount)
            });
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"InventoryTestSeeder: cargados {itemAssets.Count} items de tipo {itemType}.");
    }
#endif
}

[Serializable]
public class SeedItemAssetEntry
{
    public ItemBase item;
    public int amount = 1;
}

[Serializable]
public class SeedItemIdEntry
{
    public string itemId;
    public int amount = 1;
}
