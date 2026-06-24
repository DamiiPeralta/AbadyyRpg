using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SuppliesItemFilter
{
    Consumables,
    Weapons,
    Armors,
    Resources,
    All
}

public class SuppliesPanelUI : MonoBehaviour
{
    public Transform rowsRoot;
    public SuppliesItemRowUI rowPrefab;

    [Header("Filtros")]
    public Button consumablesButton;
    public Button weaponsButton;
    public Button armorsButton;
    public Button resourcesButton;

    [Header("Detalle opcional")]
    public TMP_Text detailTitleText;
    public TMP_Text detailBodyText;
    public ItemDetailPanelUI detailPanel;

    [Header("Venta por nodo")]
    public TMP_Text cannotSellHereText;
    [TextArea] public string defaultCannotSellHereMessage = "No puedes vender aqui, no hay mercado.";

    [Header("Refs")]
    public SuppliesResourcesPanelUI resourcesPanel;

    private SuppliesItemFilter currentFilter = SuppliesItemFilter.Consumables;
    private readonly List<SuppliesItemRowUI> rows = new List<SuppliesItemRowUI>();
    private InventoryEntry selectedEntry;
    private ItemBase selectedItem;

    private void Awake()
    {
        AutoBind();
        HookButtons();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        ClearRows();

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null || ItemDatabase.Instance == null)
            return;

        if (selectedEntry != null && selectedEntry.amount <= 0)
            ClearSelection();

        foreach (InventoryEntry entry in inventory.items)
        {
            if (entry == null || entry.amount <= 0 || string.IsNullOrWhiteSpace(entry.itemId))
                continue;

            ItemBase item = ItemDatabase.Instance.GetItemById(entry.itemId);

            if (item == null || !MatchesFilter(item))
                continue;

            SuppliesItemRowUI row = Instantiate(rowPrefab, rowsRoot);
            row.gameObject.SetActive(true);
            row.Bind(entry, item, SelectItem, SellOne);
            row.SetCanSell(CanSellHere());
            rows.Add(row);
        }

        resourcesPanel?.Refresh();
        RefreshTradingStateText();
        detailPanel?.RefreshSellTexts();
    }

    public void SetFilterConsumables()
    {
        SetFilter(SuppliesItemFilter.Consumables);
    }

    public void SetFilterWeapons()
    {
        SetFilter(SuppliesItemFilter.Weapons);
    }

    public void SetFilterArmors()
    {
        SetFilter(SuppliesItemFilter.Armors);
    }

    public void SetFilterResources()
    {
        SetFilter(SuppliesItemFilter.Resources);
    }

    private void SetFilter(SuppliesItemFilter filter)
    {
        currentFilter = filter;
        Refresh();
    }

    private void SelectItem(InventoryEntry entry, ItemBase item)
    {
        if (item == null)
            return;

        selectedEntry = entry;
        selectedItem = item;

        if (detailPanel != null)
            detailPanel.Show(entry, item);

        if (detailTitleText != null)
            detailTitleText.text = item.itemName;

        if (detailBodyText != null)
            detailBodyText.text = BuildDetailText(item);
    }

    private void SellOne(InventoryEntry entry, ItemBase item)
    {
        if (entry == null || item == null || InventoryRuntimeState.Instance == null)
            return;

        if (!CanSellHere())
        {
            RefreshTradingStateText();
            return;
        }

        if (!InventoryRuntimeState.Instance.RemoveItem(entry.itemId, 1))
            return;

        InventoryRuntimeState.Instance.AddGold(Mathf.Max(0, item.sellValue));
        UpdateSelectionAfterSell(entry, item);
        Refresh();
    }

    private void SellAll(InventoryEntry entry, ItemBase item)
    {
        if (entry == null || item == null || InventoryRuntimeState.Instance == null)
            return;

        if (!CanSellHere())
        {
            RefreshTradingStateText();
            return;
        }

        int amount = Mathf.Max(0, entry.amount);

        if (amount <= 0)
            return;

        if (!InventoryRuntimeState.Instance.RemoveItem(entry.itemId, amount))
            return;

        InventoryRuntimeState.Instance.AddGold(Mathf.Max(0, item.sellValue) * amount);
        ClearSelection();
        Refresh();
    }

    private void UpdateSelectionAfterSell(InventoryEntry entry, ItemBase item)
    {
        if (entry != selectedEntry)
            return;

        if (entry.amount > 0)
        {
            selectedEntry = entry;
            selectedItem = item;
            detailPanel?.Show(entry, item);
        }
        else
        {
            ClearSelection();
        }
    }

    private void ClearSelection()
    {
        selectedEntry = null;
        selectedItem = null;
        detailPanel?.ShowEmpty();
    }

    private string BuildDetailText(ItemBase item)
    {
        string detail = item.description;

        if (item is Weapon weapon)
        {
            detail += $"\nFisico {weapon.physicalDamageMin}-{weapon.physicalDamageMax}";
            detail += $"\nMagico {weapon.magicalDamageMin}-{weapon.magicalDamageMax}";
        }
        else if (item is Armor armor)
        {
            detail += $"\nArmadura fisica +{armor.physicalArmor}";
            detail += $"\nArmadura magica +{armor.magicalArmor}";
        }
        else if (item is ConsumableItem consumable)
        {
            detail += $"\nEfecto: {consumable.consumableEffectType}";
            detail += $"\nValor: {consumable.value}";
        }

        detail += $"\nVenta: {item.sellValue} oro";
        return detail;
    }

    private bool MatchesFilter(ItemBase item)
    {
        switch (currentFilter)
        {
            case SuppliesItemFilter.Consumables:
                return item.itemType == ItemType.Consumable;
            case SuppliesItemFilter.Weapons:
                return item.itemType == ItemType.Weapon;
            case SuppliesItemFilter.Armors:
                return item.itemType == ItemType.Armor;
            case SuppliesItemFilter.Resources:
                return false;
            case SuppliesItemFilter.All:
                return true;
        }

        return true;
    }

    private bool CanSellHere()
    {
        return GameRunState.Instance == null || GameRunState.Instance.currentNodeAllowsTrading;
    }

    private string GetCannotSellHereMessage()
    {
        if (GameRunState.Instance != null && !string.IsNullOrWhiteSpace(GameRunState.Instance.currentNodeNoTradingMessage))
            return GameRunState.Instance.currentNodeNoTradingMessage;

        return defaultCannotSellHereMessage;
    }

    private void RefreshTradingStateText()
    {
        bool canSell = CanSellHere();

        if (cannotSellHereText != null)
        {
            cannotSellHereText.gameObject.SetActive(!canSell);
            cannotSellHereText.text = GetCannotSellHereMessage();
        }

        if (detailPanel != null)
            detailPanel.SetCanSellHere(canSell);
    }

    private void HookButtons()
    {
        if (consumablesButton != null)
            consumablesButton.onClick.AddListener(SetFilterConsumables);

        if (weaponsButton != null)
            weaponsButton.onClick.AddListener(SetFilterWeapons);

        if (armorsButton != null)
            armorsButton.onClick.AddListener(SetFilterArmors);

        if (resourcesButton != null)
            resourcesButton.onClick.AddListener(SetFilterResources);
    }

    private void ClearRows()
    {
        foreach (SuppliesItemRowUI row in rows)
        {
            if (row != null)
                Destroy(row.gameObject);
        }

        rows.Clear();
    }

    private void AutoBind()
    {
        if (rowsRoot == null)
            rowsRoot = FindChildTransform("PanelInventory");

        if (rowsRoot == null)
            rowsRoot = transform;

        if (rowPrefab == null)
            rowPrefab = GetComponentInChildren<SuppliesItemRowUI>(true);

        if (consumablesButton == null)
            consumablesButton = FindChild<Button>("Button_Consumable");

        if (weaponsButton == null)
            weaponsButton = FindChild<Button>("Button_Weapons");

        if (armorsButton == null)
            armorsButton = FindChild<Button>("Button_Armors");

        if (resourcesButton == null)
            resourcesButton = FindChild<Button>("Button_Resources");

        if (resourcesPanel == null)
            resourcesPanel = GetComponentInChildren<SuppliesResourcesPanelUI>(true);

        if (detailPanel == null)
            detailPanel = GetComponentInChildren<ItemDetailPanelUI>(true);

        if (detailPanel != null)
            detailPanel.BindSellActions(SellOne, SellAll);
    }

    private T FindChild<T>(string childName) where T : Component
    {
        Transform child = FindChildTransform(childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindChildTransform(string childName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child;
        }

        return null;
    }
}
