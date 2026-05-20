using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentItemRowUI : MonoBehaviour
{
    public Button selectButton;
    public Image iconImage;
    public TMP_Text itemNameText;
    public TMP_Text primaryStatText;
    public TMP_Text secondaryStatText;
    public Image primaryStatIconImage;
    public Image secondaryStatIconImage;

    [Header("Effect Icons")]
    public Sprite physicalDamageIcon;
    public Sprite magicalDamageIcon;
    public Sprite physicalArmorIcon;
    public Sprite magicalArmorIcon;
    public Sprite healIcon;
    public Sprite reviveIcon;
    public Sprite damageAllIcon;
    public Sprite statusIcon;
    public Sprite cleanseIcon;

    private InventoryEntry entry;
    private ItemBase item;
    private Action<InventoryEntry, ItemBase> onSelected;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(InventoryEntry entry, ItemBase item, Action<InventoryEntry, ItemBase> onSelected)
    {
        this.entry = entry;
        this.item = item;
        this.onSelected = onSelected;

        Refresh();

        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(HandleSelected);
            selectButton.onClick.AddListener(HandleSelected);
        }
    }

    public void Refresh()
    {
        SetText(itemNameText, item != null ? item.itemName : entry != null ? entry.itemId : "-");

        SetStats();
        SetMainIcon();
    }

    private void SetStats()
    {
        if (item is Weapon weapon)
        {
            SetText(primaryStatText, $"{weapon.physicalDamageMin} - {weapon.physicalDamageMax}");
            SetText(secondaryStatText, $"{weapon.magicalDamageMin} - {weapon.magicalDamageMax}");
            SetIcon(primaryStatIconImage, physicalDamageIcon);
            SetIcon(secondaryStatIconImage, magicalDamageIcon);
            return;
        }

        if (item is Armor armor)
        {
            SetText(primaryStatText, armor.physicalArmor.ToString("00"));
            SetText(secondaryStatText, armor.magicalArmor.ToString("00"));
            SetIcon(primaryStatIconImage, physicalArmorIcon);
            SetIcon(secondaryStatIconImage, magicalArmorIcon);
            return;
        }

        if (item is ConsumableItem consumable)
        {
            SetConsumableStats(consumable);
            return;
        }

        SetText(primaryStatText, string.Empty);
        SetText(secondaryStatText, string.Empty);
        SetIcon(primaryStatIconImage, null);
        SetIcon(secondaryStatIconImage, null);
    }

    private void SetMainIcon()
    {
        if (iconImage == null)
            return;

        Sprite sprite = item != null ? item.icon : null;

        if (sprite == null)
            sprite = GetFallbackIcon();

        iconImage.enabled = sprite != null;

        if (sprite != null)
            iconImage.sprite = sprite;
    }

    private Sprite GetFallbackIcon()
    {
        if (item is Weapon)
            return physicalDamageIcon != null ? physicalDamageIcon : magicalDamageIcon;

        if (item is Armor)
            return physicalArmorIcon != null ? physicalArmorIcon : magicalArmorIcon;

        if (item is ConsumableItem consumable)
        {
            switch (consumable.consumableEffectType)
            {
                case ConsumableEffectType.HealSelf:
                    return healIcon;
                case ConsumableEffectType.ReviveAlly:
                    return reviveIcon;
                case ConsumableEffectType.DamageAllEnemies:
                    return damageAllIcon;
                case ConsumableEffectType.ApplyStatusToSelf:
                    return statusIcon;
                case ConsumableEffectType.CleanseSelfStatus:
                    return cleanseIcon;
            }
        }

        return null;
    }

    private void SetConsumableStats(ConsumableItem consumable)
    {
        switch (consumable.consumableEffectType)
        {
            case ConsumableEffectType.HealSelf:
                SetText(primaryStatText, $"+{consumable.value}%");
                SetText(secondaryStatText, string.Empty);
                SetIcon(primaryStatIconImage, healIcon);
                SetIcon(secondaryStatIconImage, null);
                break;

            case ConsumableEffectType.ReviveAlly:
                SetText(primaryStatText, "Revive");
                SetText(secondaryStatText, consumable.value > 0 ? $"+{consumable.value}%" : string.Empty);
                SetIcon(primaryStatIconImage, reviveIcon);
                SetIcon(secondaryStatIconImage, consumable.value > 0 ? healIcon : null);
                break;

            case ConsumableEffectType.DamageAllEnemies:
                SetText(primaryStatText, consumable.value.ToString("00"));
                SetText(secondaryStatText, "Todos");
                SetIcon(primaryStatIconImage, damageAllIcon);
                SetIcon(secondaryStatIconImage, statusIcon);
                break;

            case ConsumableEffectType.ApplyStatusToSelf:
                SetText(primaryStatText, consumable.statusEffectType.ToString());
                SetText(secondaryStatText, $"{consumable.statusDuration}t");
                SetIcon(primaryStatIconImage, statusIcon);
                SetIcon(secondaryStatIconImage, null);
                break;

            case ConsumableEffectType.CleanseSelfStatus:
                SetText(primaryStatText, "Limpia");
                SetText(secondaryStatText, string.Empty);
                SetIcon(primaryStatIconImage, cleanseIcon);
                SetIcon(secondaryStatIconImage, null);
                break;

            default:
                SetText(primaryStatText, string.Empty);
                SetText(secondaryStatText, string.Empty);
                SetIcon(primaryStatIconImage, null);
                SetIcon(secondaryStatIconImage, null);
                break;
        }
    }

    private void HandleSelected()
    {
        onSelected?.Invoke(entry, item);
    }

    private void AutoBind()
    {
        if (selectButton == null)
            selectButton = GetComponent<Button>();

        if (iconImage == null)
            iconImage = FindChild<Image>("Image_ItemIcon");

        if (itemNameText == null)
            itemNameText = FindChild<TMP_Text>("Text_ItemName");

        if (primaryStatText == null)
            primaryStatText = FindChild<TMP_Text>("Text_PrincipalIcon1");

        if (primaryStatText == null)
            primaryStatText = FindChild<TMP_Text>("Text_PrimaryStat");

        if (secondaryStatText == null)
            secondaryStatText = FindChild<TMP_Text>("Text_PrincipalStatistic2");

        if (secondaryStatText == null)
            secondaryStatText = FindChild<TMP_Text>("Text_SecondaryStat");

        if (primaryStatIconImage == null)
            primaryStatIconImage = FindChild<Image>("Image_PrincipalIcon1");

        if (secondaryStatIconImage == null)
            secondaryStatIconImage = FindChild<Image>("Image_PrincipalIcon2");
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private void SetIcon(Image image, Sprite sprite)
    {
        if (image == null)
            return;

        image.enabled = sprite != null;

        if (sprite != null)
            image.sprite = sprite;
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
