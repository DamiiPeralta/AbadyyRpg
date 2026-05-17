using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject rootPanel;

    [Header("Unit")]
    public MercenaryDetailPanelUI mercenaryDetailPanel;
    public TMP_Text titleText;
    public TMP_Text selectedSlotText;

    [Header("Slots")]
    public Button helmetSlotButton;
    public Button chestSlotButton;
    public Button feetSlotButton;
    public Button handsSlotButton;
    public Button rightHandSlotButton;
    public Button leftHandSlotButton;
    public Button ringSlotButton;
    public Button amuletSlotButton;

    [Header("Inventory")]
    public Transform inventoryRoot;
    public SuppliesItemRowUI itemRowPrefab;
    public ItemDetailPanelUI itemDetailPanel;
    public TMP_Text comparisonText;

    [Header("Actions")]
    public Button equipButton;
    public Button unequipButton;
    public Button closeButton;

    private readonly List<SuppliesItemRowUI> rows = new List<SuppliesItemRowUI>();
    private Unit selectedUnit;
    private EquipmentSlot selectedSlot = EquipmentSlot.RightHand;
    private InventoryEntry selectedEntry;
    private EquipmentItem selectedItem;

    private void Awake()
    {
        AutoBind();
        HookButtons();
        Hide();
    }

    public void Show(Unit unit)
    {
        selectedUnit = unit;

        if (rootPanel != null)
            rootPanel.SetActive(true);
        else
            gameObject.SetActive(true);

        RefreshAll();
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void RefreshAll()
    {
        if (mercenaryDetailPanel != null)
            mercenaryDetailPanel.Show(selectedUnit);

        SetText(titleText, selectedUnit != null ? $"Equipo - {selectedUnit.unitName}" : "Equipo");
        SetText(selectedSlotText, $"Slot: {FormatSlot(selectedSlot)}");

        RebuildInventoryRows();
        RefreshDetail();
        RefreshActionButtons();
    }

    public void SelectSlot(int slotValue)
    {
        selectedSlot = (EquipmentSlot)slotValue;
        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    private void SelectSlot(EquipmentSlot slot)
    {
        selectedSlot = slot;
        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    private void SelectInventoryItem(InventoryEntry entry, ItemBase item)
    {
        selectedEntry = entry;
        selectedItem = item as EquipmentItem;
        RefreshDetail();
        RefreshActionButtons();
    }

    private void EquipSelected()
    {
        if (selectedUnit == null || selectedEntry == null || selectedItem == null)
            return;

        if (!CanEquipSelected())
            return;

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        if (inventory == null || !inventory.RemoveItem(selectedEntry.itemId, 1))
            return;

        EquipmentItem previous = selectedUnit.GetEquippedItem(selectedItem.slot);
        selectedUnit.EquipItem(selectedItem);

        if (previous != null && !string.IsNullOrWhiteSpace(previous.itemId))
            inventory.AddItem(previous.itemId, 1);

        selectedSlot = selectedItem.slot;
        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    private void UnequipSelectedSlot()
    {
        if (selectedUnit == null)
            return;

        EquipmentItem previous = selectedUnit.UnequipItem(selectedSlot);
        if (previous == null)
            return;

        if (InventoryRuntimeState.Instance != null && !string.IsNullOrWhiteSpace(previous.itemId))
            InventoryRuntimeState.Instance.AddItem(previous.itemId, 1);

        RefreshAll();
    }

    private bool CanEquipSelected()
    {
        if (selectedUnit == null || selectedItem == null)
            return false;

        if (selectedItem.levelRequirement > selectedUnit.level)
            return false;

        return selectedItem.slot == selectedSlot;
    }

    private void RebuildInventoryRows()
    {
        ClearRows();

        if (inventoryRoot == null || itemRowPrefab == null || InventoryRuntimeState.Instance == null)
            return;

        foreach (InventoryEntry entry in InventoryRuntimeState.Instance.items)
        {
            if (entry == null || entry.amount <= 0)
                continue;

            ItemBase item = ItemDatabase.Instance != null ? ItemDatabase.Instance.GetItemById(entry.itemId) : null;
            EquipmentItem equipment = item as EquipmentItem;

            if (equipment == null || equipment.slot != selectedSlot)
                continue;

            SuppliesItemRowUI row = Instantiate(itemRowPrefab, inventoryRoot);
            row.gameObject.SetActive(true);
            row.Bind(entry, item, SelectInventoryItem, null);
            rows.Add(row);
        }
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

    private void RefreshDetail()
    {
        if (itemDetailPanel != null)
            itemDetailPanel.Show(selectedEntry, selectedItem);

        SetText(comparisonText, BuildComparisonText());
    }

    private void RefreshActionButtons()
    {
        if (equipButton != null)
            equipButton.interactable = CanEquipSelected();

        if (unequipButton != null)
            unequipButton.interactable = selectedUnit != null && selectedUnit.GetEquippedItem(selectedSlot) != null;
    }

    private string BuildComparisonText()
    {
        if (selectedUnit == null)
            return "Selecciona un mercenario.";

        EquipmentItem current = selectedUnit.GetEquippedItem(selectedSlot);
        StringBuilder builder = new StringBuilder();

        builder.AppendLine($"Equipado: {(current != null ? current.itemName : "Nada")}");

        if (selectedItem == null)
        {
            builder.AppendLine("Selecciona un item compatible para comparar.");
            return builder.ToString().TrimEnd();
        }

        builder.AppendLine($"Nuevo: {selectedItem.itemName}");

        if (selectedItem.levelRequirement > selectedUnit.level)
            builder.AppendLine($"Requiere nivel {selectedItem.levelRequirement}.");

        AppendEquipmentComparison(builder, current, selectedItem);
        return builder.ToString().TrimEnd();
    }

    private void AppendEquipmentComparison(StringBuilder builder, EquipmentItem current, EquipmentItem next)
    {
        if (current is Weapon || next is Weapon)
        {
            Weapon oldWeapon = current as Weapon;
            Weapon newWeapon = next as Weapon;
            AppendDelta(builder, "Dano fisico min", oldWeapon != null ? oldWeapon.physicalDamageMin : 0, newWeapon != null ? newWeapon.physicalDamageMin : 0);
            AppendDelta(builder, "Dano fisico max", oldWeapon != null ? oldWeapon.physicalDamageMax : 0, newWeapon != null ? newWeapon.physicalDamageMax : 0);
            AppendDelta(builder, "Dano magico min", oldWeapon != null ? oldWeapon.magicalDamageMin : 0, newWeapon != null ? newWeapon.magicalDamageMin : 0);
            AppendDelta(builder, "Dano magico max", oldWeapon != null ? oldWeapon.magicalDamageMax : 0, newWeapon != null ? newWeapon.magicalDamageMax : 0);
        }

        if (current is Armor || next is Armor)
        {
            Armor oldArmor = current as Armor;
            Armor newArmor = next as Armor;
            AppendDelta(builder, "Armadura fisica", oldArmor != null ? oldArmor.physicalArmor : 0, newArmor != null ? newArmor.physicalArmor : 0);
            AppendDelta(builder, "Armadura magica", oldArmor != null ? oldArmor.magicalArmor : 0, newArmor != null ? newArmor.magicalArmor : 0);
        }
    }

    private void AppendDelta(StringBuilder builder, string label, int current, int next)
    {
        int delta = next - current;
        string sign = delta >= 0 ? "+" : "";
        builder.AppendLine($"{label}: {current} -> {next} ({sign}{delta})");
    }

    private void HookButtons()
    {
        HookSlot(helmetSlotButton, EquipmentSlot.Helmet);
        HookSlot(chestSlotButton, EquipmentSlot.Chest);
        HookSlot(feetSlotButton, EquipmentSlot.Feet);
        HookSlot(handsSlotButton, EquipmentSlot.Hands);
        HookSlot(rightHandSlotButton, EquipmentSlot.RightHand);
        HookSlot(leftHandSlotButton, EquipmentSlot.LeftHand);
        HookSlot(ringSlotButton, EquipmentSlot.Ring);
        HookSlot(amuletSlotButton, EquipmentSlot.Amulet);

        if (equipButton != null)
        {
            equipButton.onClick.RemoveListener(EquipSelected);
            equipButton.onClick.AddListener(EquipSelected);
        }

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveListener(UnequipSelectedSlot);
            unequipButton.onClick.AddListener(UnequipSelectedSlot);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
            closeButton.onClick.AddListener(Hide);
        }
    }

    private void HookSlot(Button button, EquipmentSlot slot)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectSlot(slot));
    }

    private void AutoBind()
    {
        if (rootPanel == null)
            rootPanel = gameObject;

        if (mercenaryDetailPanel == null)
            mercenaryDetailPanel = GetComponentInChildren<MercenaryDetailPanelUI>(true);

        if (itemDetailPanel == null)
            itemDetailPanel = GetComponentInChildren<ItemDetailPanelUI>(true);

        if (inventoryRoot == null)
            inventoryRoot = FindChildTransform("EquipmentInventoryContent");

        if (itemRowPrefab == null)
            itemRowPrefab = GetComponentInChildren<SuppliesItemRowUI>(true);

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_EquipmentTitle");

        if (selectedSlotText == null)
            selectedSlotText = FindChild<TMP_Text>("Text_SelectedSlot");

        if (comparisonText == null)
            comparisonText = FindChild<TMP_Text>("Text_Comparison");

        helmetSlotButton = helmetSlotButton != null ? helmetSlotButton : FindChild<Button>("Button_SlotHelmet");
        chestSlotButton = chestSlotButton != null ? chestSlotButton : FindChild<Button>("Button_SlotChest");
        feetSlotButton = feetSlotButton != null ? feetSlotButton : FindChild<Button>("Button_SlotFeet");
        handsSlotButton = handsSlotButton != null ? handsSlotButton : FindChild<Button>("Button_SlotHands");
        rightHandSlotButton = rightHandSlotButton != null ? rightHandSlotButton : FindChild<Button>("Button_SlotRightHand");
        leftHandSlotButton = leftHandSlotButton != null ? leftHandSlotButton : FindChild<Button>("Button_SlotLeftHand");
        ringSlotButton = ringSlotButton != null ? ringSlotButton : FindChild<Button>("Button_SlotRing");
        amuletSlotButton = amuletSlotButton != null ? amuletSlotButton : FindChild<Button>("Button_SlotAmulet");

        equipButton = equipButton != null ? equipButton : FindChild<Button>("Button_Equip");
        unequipButton = unequipButton != null ? unequipButton : FindChild<Button>("Button_Unequip");
        closeButton = closeButton != null ? closeButton : FindChild<Button>("Button_CloseEquipment");
    }

    private string FormatSlot(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.RightHand:
                return "Mano derecha";
            case EquipmentSlot.LeftHand:
                return "Mano izquierda";
            case EquipmentSlot.Chest:
                return "Pecho";
            case EquipmentSlot.Helmet:
                return "Casco";
            case EquipmentSlot.Feet:
                return "Pies";
            case EquipmentSlot.Hands:
                return "Manos";
            case EquipmentSlot.Ring:
                return "Anillo";
            case EquipmentSlot.Amulet:
                return "Amuleto";
            default:
                return slot.ToString();
        }
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
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
