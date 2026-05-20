using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotButtonUI : MonoBehaviour
{
    public enum SlotKind
    {
        Equipment,
        Consumable
    }

    [Header("Slot")]
    public SlotKind slotKind = SlotKind.Equipment;
    public EquipmentSlot equipmentSlot = EquipmentSlot.RightHand;
    public ConsumableSlot consumableSlot = ConsumableSlot.Consumable1;

    [Header("Refs")]
    public Button button;
    public Image iconImage;
    public Image backgroundImage;
    public TMP_Text labelText;

    [Header("Visual")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.22f);
    public Color selectedColor = new Color(1f, 0.78f, 0.28f, 0.45f);

    private EquipmentPanelUI owner;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(EquipmentPanelUI owner)
    {
        this.owner = owner;

        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
    }

    public void Refresh(Unit unit, bool selected)
    {
        ItemBase item = GetItem(unit);

        if (iconImage != null)
        {
            iconImage.enabled = item != null && item.icon != null;

            if (item != null && item.icon != null)
                iconImage.sprite = item.icon;
        }

        if (backgroundImage != null)
            backgroundImage.color = selected ? selectedColor : normalColor;

        if (labelText != null && string.IsNullOrWhiteSpace(labelText.text))
            labelText.text = GetDefaultLabel();
    }

    public bool MatchesEquipment(EquipmentSlot slot)
    {
        return slotKind == SlotKind.Equipment && equipmentSlot == slot;
    }

    public bool MatchesConsumable(ConsumableSlot slot)
    {
        return slotKind == SlotKind.Consumable && consumableSlot == slot;
    }

    private ItemBase GetItem(Unit unit)
    {
        if (unit == null)
            return null;

        if (slotKind == SlotKind.Consumable)
            return consumableSlot == ConsumableSlot.Consumable2 ? unit.consumable2 : unit.consumable1;

        return unit.GetEquippedItem(equipmentSlot);
    }

    private void HandleClick()
    {
        if (owner == null)
            return;

        if (slotKind == SlotKind.Consumable)
            owner.SelectConsumableSlot(consumableSlot);
        else
            owner.SelectEquipmentSlot(equipmentSlot);
    }

    private void AutoBind()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (iconImage == null)
            iconImage = FindChild<Image>("Image");

        if (labelText == null)
            labelText = GetComponentInChildren<TMP_Text>(true);
    }

    private string GetDefaultLabel()
    {
        if (slotKind == SlotKind.Consumable)
            return consumableSlot == ConsumableSlot.Consumable2 ? "Objeto 2" : "Objeto 1";

        switch (equipmentSlot)
        {
            case EquipmentSlot.Helmet:
                return "Casco";
            case EquipmentSlot.Chest:
                return "Pecho";
            case EquipmentSlot.Feet:
                return "Botas";
            case EquipmentSlot.Hands:
                return "Manos";
            case EquipmentSlot.RightHand:
                return "Mano Derecha";
            case EquipmentSlot.LeftHand:
                return "Mano Izquierda";
            case EquipmentSlot.Ring:
                return "Anillo";
            case EquipmentSlot.Amulet:
                return "Amuleto";
            default:
                return equipmentSlot.ToString();
        }
    }

    private T FindChild<T>(string childName) where T : Component
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child.GetComponent<T>();
        }

        return null;
    }
}
