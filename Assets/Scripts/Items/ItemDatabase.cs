using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Items registrados")]
    public List<ItemBase> items = new List<ItemBase>();

    private Dictionary<string, ItemBase> itemsById = new Dictionary<string, ItemBase>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("ItemDatabase duplicado destruido.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildDatabase();
    }

    public void BuildDatabase()
    {
        itemsById.Clear();

        foreach (ItemBase item in items)
        {
            if (item == null)
                continue;

            if (string.IsNullOrWhiteSpace(item.itemId))
            {
                Debug.LogWarning($"ItemDatabase: item sin itemId: {item.name}");
                continue;
            }

            if (itemsById.ContainsKey(item.itemId))
            {
                Debug.LogWarning($"ItemDatabase: itemId duplicado detectado: {item.itemId}");
                continue;
            }

            itemsById.Add(item.itemId, item);
        }

        Debug.Log($"ItemDatabase: cargados {itemsById.Count} items.");
    }

    public ItemBase GetItemById(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return null;

        if (itemsById.TryGetValue(itemId, out ItemBase item))
            return item;

        Debug.LogWarning($"ItemDatabase: no existe item con ID: {itemId}");
        return null;
    }

    public Weapon GetWeaponById(string itemId)
    {
        return GetItemById(itemId) as Weapon;
    }

    public Armor GetArmorById(string itemId)
    {
        return GetItemById(itemId) as Armor;
    }

    public ConsumableItem GetConsumableById(string itemId)
    {
        return GetItemById(itemId) as ConsumableItem;
    }
}