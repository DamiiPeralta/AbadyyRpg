using System.Collections.Generic;
using UnityEngine;

public class InventoryRuntimeState : MonoBehaviour
{
    public static InventoryRuntimeState Instance { get; private set; }

    [Header("Currency")]
    public int gold = 0;

    [Header("Basic Resources")]
    public int food = 0;
    public int wood = 0;
    public int stone = 0;
    public int iron = 0;
    public int leather = 0;
    public int crystals = 0;

    [Header("Items")]
    public List<InventoryEntry> items = new List<InventoryEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string itemId, int amount)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("No se puede agregar un item con itemId vacío.");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("No se puede agregar una cantidad menor o igual a 0.");
            return;
        }

        if (!IsItemStackable(itemId))
        {
            for (int i = 0; i < amount; i++)
                items.Add(new InventoryEntry(itemId, 1));

            return;
        }

        InventoryEntry entry = items.Find(x => x.itemId == itemId);

        if (entry == null)
        {
            items.Add(new InventoryEntry(itemId, amount));
        }
        else
        {
            entry.amount += amount;
        }
    }

    public bool RemoveItem(string itemId, int amount)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        if (amount <= 0)
            return false;

        if (GetAmount(itemId) < amount)
            return false;

        int remaining = amount;

        for (int i = items.Count - 1; i >= 0 && remaining > 0; i--)
        {
            InventoryEntry entry = items[i];

            if (entry == null || entry.itemId != itemId)
                continue;

            int removed = Mathf.Min(entry.amount, remaining);
            entry.amount -= removed;
            remaining -= removed;

            if (entry.amount <= 0)
                items.RemoveAt(i);
        }

        return true;
    }

    public bool RemoveItem(InventoryEntry entry, int amount)
    {
        if (entry == null || amount <= 0)
            return false;

        if (!items.Contains(entry))
            return false;

        if (entry.amount < amount)
            return false;

        entry.amount -= amount;

        if (entry.amount <= 0)
            items.Remove(entry);

        return true;
    }

    public bool HasItem(string itemId, int amount)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        if (amount <= 0)
            return true;

        return GetAmount(itemId) >= amount;
    }

    public int GetAmount(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return 0;

        int total = 0;

        foreach (InventoryEntry entry in items)
        {
            if (entry != null && entry.itemId == itemId)
                total += Mathf.Max(0, entry.amount);
        }

        return total;
    }

    private bool IsItemStackable(string itemId)
    {
        ItemBase item = ItemDatabase.Instance != null ? ItemDatabase.Instance.GetItemById(itemId) : null;
        return item == null || item.IsStackable;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
            return true;

        if (gold < amount)
            return false;

        gold -= amount;
        return true;
    }

    public bool SpendFood(int amount)
    {
        if (amount <= 0)
            return true;

        if (food < amount)
            return false;

        food -= amount;
        return true;
    }

    public void AddFood(int amount)
    {
        if (amount <= 0)
            return;

        food += amount;
    }

    public void AddWood(int amount)
    {
        if (amount <= 0)
            return;

        wood += amount;
    }

    public bool SpendWood(int amount)
    {
        if (amount <= 0)
            return true;

        if (wood < amount)
            return false;

        wood -= amount;
        return true;
    }

    public void AddStone(int amount)
    {
        if (amount <= 0)
            return;

        stone += amount;
    }

    public bool SpendStone(int amount)
    {
        if (amount <= 0)
            return true;

        if (stone < amount)
            return false;

        stone -= amount;
        return true;
    }

    public void AddIron(int amount)
    {
        if (amount <= 0)
            return;

        iron += amount;
    }

    public bool SpendIron(int amount)
    {
        if (amount <= 0)
            return true;

        if (iron < amount)
            return false;

        iron -= amount;
        return true;
    }

    public void AddLeather(int amount)
    {
        if (amount <= 0)
            return;

        leather += amount;
    }

    public bool SpendLeather(int amount)
    {
        if (amount <= 0)
            return true;

        if (leather < amount)
            return false;

        leather -= amount;
        return true;
    }

    public void AddCrystals(int amount)
    {
        if (amount <= 0)
            return;

        crystals += amount;
    }

    public bool SpendCrystals(int amount)
    {
        if (amount <= 0)
            return true;

        if (crystals < amount)
            return false;

        crystals -= amount;
        return true;
    }
}
