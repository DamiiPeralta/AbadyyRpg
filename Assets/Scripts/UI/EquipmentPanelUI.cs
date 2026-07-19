using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelUI : MonoBehaviour
{
    private enum EquipmentInventoryFilter
    {
        Weapons,
        Armors,
        Consumables
    }

    [Header("Root")]
    public GameObject rootPanel;

    [Header("Unit")]
    public MercenaryDetailPanelUI mercenaryDetailPanel;
    [HideInInspector] public TMP_Text titleText;
    [HideInInspector] public TMP_Text selectedSlotText;

    [Header("Slots")]
    public List<EquipmentSlotButtonUI> slotButtons = new List<EquipmentSlotButtonUI>();

    [Header("Inventory")]
    public GameObject inventoryPanelRoot;
    public Transform inventoryRoot;
    public EquipmentItemRowUI equipmentItemRowPrefab;
    [HideInInspector] public SuppliesItemRowUI itemRowPrefab;
    [HideInInspector] public ItemDetailPanelUI itemDetailPanel;
    [HideInInspector] public TMP_Text comparisonText;
    [HideInInspector] public Image unitPortraitImage;

    [Header("Actions")]
    public Button equipButton;
    public Button unequipButton;
    public Button closeButton;

    private readonly List<GameObject> rows = new List<GameObject>();
    private Unit selectedUnit;
    private EquipmentSlot selectedSlot = EquipmentSlot.RightHand;
    private ConsumableSlot selectedConsumableSlot = ConsumableSlot.Consumable1;
    private bool selectingConsumable;
    private EquipmentInventoryFilter currentFilter = EquipmentInventoryFilter.Weapons;
    private InventoryEntry selectedEntry;
    private ItemBase selectedItem;

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

        if (inventoryPanelRoot != null)
            inventoryPanelRoot.SetActive(true);

        RefreshAll();
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
        else
            gameObject.SetActive(false);

        if (inventoryPanelRoot != null)
            inventoryPanelRoot.SetActive(false);

        selectedEntry = null;
        selectedItem = null;

        if (mercenaryDetailPanel != null)
            mercenaryDetailPanel.Show(selectedUnit);
    }

    public void RefreshAll()
    {
        if (mercenaryDetailPanel != null)
            mercenaryDetailPanel.Show(selectedUnit);

        SetText(titleText, selectedUnit != null ? $"Equipo - {selectedUnit.unitName}" : "Equipo");
        SetText(selectedSlotText, selectingConsumable ? $"Slot: {FormatConsumableSlot(selectedConsumableSlot)}" : $"Slot: {FormatSlot(selectedSlot)}");

        if (unitPortraitImage != null && selectedUnit != null && selectedUnit.unitData != null && selectedUnit.unitData.icon != null)
            unitPortraitImage.sprite = selectedUnit.unitData.icon;

        RebuildInventoryRows();
        RefreshDetail();
        RefreshMercenaryPreview();
        RefreshActionButtons();
    }

    public void SelectSlot(int slotValue)
    {
        selectedSlot = (EquipmentSlot)slotValue;
        selectingConsumable = false;
        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    public void SelectEquipmentSlot(EquipmentSlot slot)
    {
        selectedSlot = slot;
        selectingConsumable = false;
        currentFilter = IsArmorSlot(slot) ? EquipmentInventoryFilter.Armors : EquipmentInventoryFilter.Weapons;
        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    public void SelectConsumableSlot(ConsumableSlot slot)
    {
        selectedConsumableSlot = slot;
        selectingConsumable = true;
        currentFilter = EquipmentInventoryFilter.Consumables;
        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    private void SelectInventoryItem(InventoryEntry entry, ItemBase item)
    {
        selectedEntry = entry;
        selectedItem = item;
        RefreshDetail();
        RefreshMercenaryPreview();
        RefreshActionButtons();
    }

    private void EquipSelected()
    {
        if (selectedUnit == null || selectedEntry == null || selectedItem == null)
            return;

        if (!CanEquipSelected())
            return;

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        if (inventory == null || !inventory.RemoveItem(selectedEntry, 1))
            return;

        if (selectedItem is ConsumableItem consumable)
        {
            ConsumableItem previousConsumable = selectingConsumable && selectedConsumableSlot == ConsumableSlot.Consumable2
                ? selectedUnit.RemoveConsumable2()
                : selectedUnit.RemoveConsumable1();

            if (selectingConsumable && selectedConsumableSlot == ConsumableSlot.Consumable2)
                selectedUnit.SetConsumable2(consumable);
            else
                selectedUnit.SetConsumable1(consumable);

            if (previousConsumable != null && !string.IsNullOrWhiteSpace(previousConsumable.itemId))
                inventory.AddItem(previousConsumable.itemId, 1);
        }
        else if (selectedItem is EquipmentItem equipment)
        {
            EquipmentItem previous = selectedUnit.GetEquippedItem(equipment.slot);
            selectedUnit.EquipItem(equipment);

            if (previous != null && !string.IsNullOrWhiteSpace(previous.itemId))
                inventory.AddItem(previous.itemId, 1);

            selectedSlot = equipment.slot;
        }

        selectedEntry = null;
        selectedItem = null;
        RefreshAll();
    }

    private void UnequipSelectedSlot()
    {
        if (selectedUnit == null)
            return;

        ItemBase previous;
        bool shouldReturnToInventory = true;

        if (selectingConsumable)
        {
            previous = selectedConsumableSlot == ConsumableSlot.Consumable2 ? selectedUnit.RemoveConsumable2() : selectedUnit.RemoveConsumable1();
        }
        else
        {
            EquipmentItem equippedItem = selectedUnit.GetEquippedItem(selectedSlot);
            shouldReturnToInventory = ShouldReturnUnequippedItem(equippedItem);
            previous = selectedUnit.UnequipItem(selectedSlot);
        }

        if (previous == null)
            return;

        if (shouldReturnToInventory && InventoryRuntimeState.Instance != null && !string.IsNullOrWhiteSpace(previous.itemId))
            InventoryRuntimeState.Instance.AddItem(previous.itemId, 1);

        RefreshAll();
    }

    private bool ShouldReturnUnequippedItem(EquipmentItem item)
    {
        if (item is Armor)
        {
            bool physicalArmorFull = selectedUnit.currentPhysicalArmor >= selectedUnit.maxPhysicalArmor;
            bool magicalArmorFull = selectedUnit.currentMagicalArmor >= selectedUnit.maxMagicalArmor;
            return physicalArmorFull && magicalArmorFull;
        }

        return true;
    }

    private bool CanEquipSelected()
    {
        if (selectedUnit == null || selectedItem == null)
            return false;

        if (selectedItem is ConsumableItem)
            return selectingConsumable;

        if (selectedItem is EquipmentItem equipment)
        {
            if (equipment.levelRequirement > selectedUnit.level)
                return false;

            return !selectingConsumable && equipment.slot == selectedSlot;
        }

        return false;
    }

    private void RebuildInventoryRows()
    {
        ClearRows();

        if (inventoryRoot == null || (equipmentItemRowPrefab == null && itemRowPrefab == null) || InventoryRuntimeState.Instance == null)
            return;

        foreach (InventoryEntry entry in InventoryRuntimeState.Instance.items)
        {
            if (entry == null || entry.amount <= 0)
                continue;

            ItemBase item = ItemDatabase.Instance != null ? ItemDatabase.Instance.GetItemById(entry.itemId) : null;
            if (!ShouldShowItem(item))
                continue;

            if (equipmentItemRowPrefab != null)
            {
                EquipmentItemRowUI row = Instantiate(equipmentItemRowPrefab, inventoryRoot);
                row.gameObject.SetActive(true);
                row.Bind(entry, item, SelectInventoryItem);
                rows.Add(row.gameObject);
            }
            else
            {
                SuppliesItemRowUI row = Instantiate(itemRowPrefab, inventoryRoot);
                row.gameObject.SetActive(true);
                row.Bind(entry, item, SelectInventoryItem, null);
                rows.Add(row.gameObject);
            }
        }
    }

    private void ClearRows()
    {
        foreach (GameObject row in rows)
        {
            if (row != null)
                Destroy(row);
        }

        rows.Clear();
    }

    private bool ShouldShowItem(ItemBase item)
    {
        if (item == null)
            return false;

        switch (currentFilter)
        {
            case EquipmentInventoryFilter.Weapons:
                return item is Weapon weapon && !selectingConsumable && weapon.slot == selectedSlot;

            case EquipmentInventoryFilter.Armors:
                return item is Armor armor && !selectingConsumable && armor.slot == selectedSlot;

            case EquipmentInventoryFilter.Consumables:
                return item is ConsumableItem;
        }

        return false;
    }

    private void RefreshDetail()
    {
        if (itemDetailPanel != null)
            itemDetailPanel.Show(selectedEntry, selectedItem);

        SetText(comparisonText, BuildComparisonText());
        RefreshSlotButtons();
    }

    private void RefreshMercenaryPreview()
    {
        if (mercenaryDetailPanel == null)
            return;

        if (selectedItem is EquipmentItem equipment && !selectingConsumable)
            mercenaryDetailPanel.ShowEquipmentPreview(selectedUnit, selectedSlot, equipment);
        else
            mercenaryDetailPanel.Show(selectedUnit);
    }

    private void RefreshActionButtons()
    {
        if (equipButton != null)
            equipButton.interactable = CanEquipSelected();

        if (unequipButton != null)
            unequipButton.interactable = selectedUnit != null && GetCurrentSelectedSlotItem() != null;
    }

    private string BuildComparisonText()
    {
        if (selectedUnit == null)
            return "Selecciona un mercenario.";

        ItemBase current = GetCurrentSelectedSlotItem();
        StringBuilder builder = new StringBuilder();

        builder.AppendLine($"Equipado: {(current != null ? current.itemName : "Nada")}");

        if (selectedItem == null)
        {
            builder.AppendLine("Selecciona un item compatible para comparar.");
            return builder.ToString().TrimEnd();
        }

        builder.AppendLine($"Nuevo: {selectedItem.itemName}");

        if (selectedItem is EquipmentItem selectedEquipment && selectedEquipment.levelRequirement > selectedUnit.level)
            builder.AppendLine(ColorText($"Requiere nivel {selectedEquipment.levelRequirement}.", false));

        AppendEquipmentComparison(builder, current, selectedItem);
        return builder.ToString().TrimEnd();
    }

    private void AppendEquipmentComparison(StringBuilder builder, ItemBase current, ItemBase next)
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

        if (next is ConsumableItem consumable)
        {
            builder.AppendLine($"Consumible: {FormatConsumableEffect(consumable)}");

            if (consumable.value > 0)
                builder.AppendLine($"Valor: {consumable.value}");
        }
    }

    private ItemBase GetCurrentSelectedSlotItem()
    {
        if (selectedUnit == null)
            return null;

        if (selectingConsumable)
            return selectedConsumableSlot == ConsumableSlot.Consumable2 ? selectedUnit.consumable2 : selectedUnit.consumable1;

        return selectedUnit.GetEquippedItem(selectedSlot);
    }

    private void AppendDelta(StringBuilder builder, string label, int current, int next)
    {
        int delta = next - current;
        string sign = delta >= 0 ? "+" : "";
        string deltaText = $"{label}: {current} -> {next} ({sign}{delta})";
        builder.AppendLine(delta == 0 ? deltaText : ColorText(deltaText, delta > 0));
    }

    private void HookButtons()
    {
        BindSlotButtons();

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

    private void AutoBind()
    {
        if (rootPanel == null)
            rootPanel = gameObject;

        rootPanel.SetActive(true);

        if (mercenaryDetailPanel == null)
            mercenaryDetailPanel = GetComponentInChildren<MercenaryDetailPanelUI>(true);

        if (itemDetailPanel == null)
            itemDetailPanel = GetComponentInChildren<ItemDetailPanelUI>(true);

        if (inventoryPanelRoot == null)
            inventoryPanelRoot = FindSceneTransform("PanelSuppliesEquipment")?.gameObject;

        if (inventoryRoot == null)
            inventoryRoot = FindChildTransform("EquipmentInventoryContent");

        if (inventoryRoot == null)
            inventoryRoot = FindChildTransform("Content");

        if (inventoryRoot == null && inventoryPanelRoot != null)
            inventoryRoot = FindChildTransformIn(inventoryPanelRoot.transform, "Content");

        if (equipmentItemRowPrefab == null)
            equipmentItemRowPrefab = GetComponentInChildren<EquipmentItemRowUI>(true);

        if (itemRowPrefab == null)
            itemRowPrefab = GetComponentInChildren<SuppliesItemRowUI>(true);

        if (equipmentItemRowPrefab == null && itemRowPrefab == null)
            equipmentItemRowPrefab = FindEquipmentRowTemplate();

        if (equipmentItemRowPrefab == null && itemRowPrefab == null)
            itemRowPrefab = FindRowTemplate();
        else
            HideRowTemplates();

        FindSlotButtons();

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_EquipmentTitle");

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_Name");

        if (selectedSlotText == null)
            selectedSlotText = FindChild<TMP_Text>("Text_SelectedSlot");

        if (comparisonText == null)
            comparisonText = FindChild<TMP_Text>("Text_Comparison");

        if (comparisonText == null)
            comparisonText = FindChild<TMP_Text>("Text_PrincipalStatistic1");

        if (comparisonText == null)
            comparisonText = FindChild<TMP_Text>("Text_PrincipalStatistic2");

        if (unitPortraitImage == null)
            unitPortraitImage = FindChild<Image>("Image_UnitIcon");

        if (unitPortraitImage == null)
            unitPortraitImage = FindChild<Image>("Image_MercSpriteBackground");

        equipButton = equipButton != null ? equipButton : FindChild<Button>("Button_Equip");
        equipButton = equipButton != null ? equipButton : FindChild<Button>("ButtonEquip");
        unequipButton = unequipButton != null ? unequipButton : FindChild<Button>("Button_Unequip");
        unequipButton = unequipButton != null ? unequipButton : FindChild<Button>("ButtonUnequip");
        closeButton = closeButton != null ? closeButton : FindChild<Button>("Button_CloseEquipment");
        closeButton = closeButton != null ? closeButton : FindChild<Button>("Button_Close");
    }

    private void FindSlotButtons()
    {
        slotButtons.Clear();

        EquipmentSlotButtonUI[] existingSlots = GetComponentsInChildren<EquipmentSlotButtonUI>(true);
        foreach (EquipmentSlotButtonUI slot in existingSlots)
        {
            if (slot != null && !slotButtons.Contains(slot))
                slotButtons.Add(slot);
        }

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (button == null || !button.name.StartsWith("ButtonEquipmentUISlot"))
                continue;

            EquipmentSlotButtonUI slot = button.GetComponent<EquipmentSlotButtonUI>();
            if (slot == null)
                slot = button.gameObject.AddComponent<EquipmentSlotButtonUI>();

            ConfigureSlotFromLabel(slot);

            if (!slotButtons.Contains(slot))
                slotButtons.Add(slot);
        }
    }

    private void BindSlotButtons()
    {
        foreach (EquipmentSlotButtonUI slot in slotButtons)
        {
            if (slot == null)
                continue;

            slot.Bind(this);
        }
    }

    private void RefreshSlotButtons()
    {
        foreach (EquipmentSlotButtonUI slot in slotButtons)
        {
            if (slot == null)
                continue;

            bool selected = selectingConsumable
                ? slot.MatchesConsumable(selectedConsumableSlot)
                : slot.MatchesEquipment(selectedSlot);

            slot.Refresh(selectedUnit, selected);
        }
    }

    private void ConfigureSlotFromLabel(EquipmentSlotButtonUI slot)
    {
        TMP_Text label = slot.GetComponentInChildren<TMP_Text>(true);
        string text = Normalize(label != null ? label.text : slot.name);

        if (text.Contains("objeto 2"))
        {
            slot.slotKind = EquipmentSlotButtonUI.SlotKind.Consumable;
            slot.consumableSlot = ConsumableSlot.Consumable2;
        }
        else if (text.Contains("objeto 1"))
        {
            slot.slotKind = EquipmentSlotButtonUI.SlotKind.Consumable;
            slot.consumableSlot = ConsumableSlot.Consumable1;
        }
        else
        {
            slot.slotKind = EquipmentSlotButtonUI.SlotKind.Equipment;

            if (text.Contains("casco"))
                slot.equipmentSlot = EquipmentSlot.Helmet;
            else if (text.Contains("pecho"))
                slot.equipmentSlot = EquipmentSlot.Chest;
            else if (text.Contains("botas") || text.Contains("pies"))
                slot.equipmentSlot = EquipmentSlot.Feet;
            else if (text.Contains("mano izquierda"))
                slot.equipmentSlot = EquipmentSlot.LeftHand;
            else if (text.Contains("mano derecha"))
                slot.equipmentSlot = EquipmentSlot.RightHand;
            else if (text.Contains("anillo"))
                slot.equipmentSlot = EquipmentSlot.Ring;
            else if (text.Contains("amuleto"))
                slot.equipmentSlot = EquipmentSlot.Amulet;
            else if (text.Contains("manos") || text.Contains("guantes"))
                slot.equipmentSlot = EquipmentSlot.Hands;
        }
    }

    private SuppliesItemRowUI FindRowTemplate()
    {
        Transform row = FindChildTransform("ButtonEquipItemDetail");

        if (row == null)
            row = FindChildTransform("ButtonItemDetail");

        if (row == null)
            return null;

        SuppliesItemRowUI rowUI = row.GetComponent<SuppliesItemRowUI>();
        if (rowUI == null)
            rowUI = row.gameObject.AddComponent<SuppliesItemRowUI>();

        row.gameObject.SetActive(false);
        HideRowTemplates();
        return rowUI;
    }

    private EquipmentItemRowUI FindEquipmentRowTemplate()
    {
        Transform row = FindChildTransform("ButtonEquipItemDetail");

        if (row == null)
            row = FindChildTransform("ButtonEquipmentDetail");

        if (row == null)
            return null;

        EquipmentItemRowUI rowUI = row.GetComponent<EquipmentItemRowUI>();
        if (rowUI == null)
            rowUI = row.gameObject.AddComponent<EquipmentItemRowUI>();

        row.gameObject.SetActive(false);
        HideRowTemplates();
        return rowUI;
    }

    private void HideRowTemplates()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child == null)
                continue;

            string childName = child.name.Trim();
            if (childName == "ButtonEquipItemDetail" ||
                childName.StartsWith("ButtonEquipItemDetail (") ||
                childName == "ButtonItemDetail" ||
                childName.StartsWith("ButtonItemDetail ("))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private Button FindButtonByNamesOrText(params string[] namesOrTexts)
    {
        foreach (string value in namesOrTexts)
        {
            Button byName = FindChild<Button>(value);
            if (byName != null)
                return byName;
        }

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
            if (text == null)
                continue;

            string normalizedText = Normalize(text.text);
            foreach (string value in namesOrTexts)
            {
                if (normalizedText == Normalize(value))
                    return button;
            }
        }

        return null;
    }

    private bool IsArmorSlot(EquipmentSlot slot)
    {
        return slot == EquipmentSlot.Helmet ||
               slot == EquipmentSlot.Chest ||
               slot == EquipmentSlot.Feet ||
               slot == EquipmentSlot.Hands;
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

    private string FormatConsumableSlot(ConsumableSlot slot)
    {
        return slot == ConsumableSlot.Consumable2 ? "Objeto 2" : "Objeto 1";
    }

    private string FormatConsumableEffect(ConsumableItem item)
    {
        switch (item.consumableEffectType)
        {
            case ConsumableEffectType.HealSelf:
                return "Curacion";
            case ConsumableEffectType.ReviveAlly:
                return "Revive aliado";
            case ConsumableEffectType.DamageAllEnemies:
                return "Dano a enemigos";
            case ConsumableEffectType.ApplyStatusToSelf:
                return $"Estado {item.statusEffectType}";
            case ConsumableEffectType.CleanseSelfStatus:
                return "Limpia estado";
            default:
                return "Sin efecto";
        }
    }

    private string Normalize(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToLowerInvariant();
    }

    private string ColorText(string text, bool positive)
    {
        return positive ? $"<color=#6DFF7A>{text}</color>" : $"<color=#FF6B6B>{text}</color>";
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

    private Transform FindChildTransformIn(Transform root, string childName)
    {
        if (root == null)
            return null;

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child;
        }

        return null;
    }

    private Transform FindSceneTransform(string childName)
    {
        foreach (Transform child in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (child == null || child.hideFlags != HideFlags.None)
                continue;

            if (child.name.Trim() == childName)
                return child;
        }

        return null;
    }
}
