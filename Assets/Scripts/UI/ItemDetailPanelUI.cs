using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanelUI : MonoBehaviour
{
    [Header("Contenido")]
    public Image largeIconImage;
    public Image rarityGemImage;
    public TMP_Text nameText;
    public TMP_Text typeText;
    public TMP_Text dataText;
    public TMP_Text rarityText;
    public TMP_Text rarityValueText;
    public TMP_Text descriptionText;

    [Header("Venta")]
    public Button sellOneButton;
    public Button sellAllButton;
    public TMP_Text sellOneText;
    public TMP_Text sellOnePriceText;
    public TMP_Text sellAllText;
    public TMP_Text sellAllPriceText;

    [Header("Defaults")]
    public string defaultName = "Objeto";
    public string defaultType = "Item";
    [TextArea] public string defaultData = "Datos del objeto.";
    public string defaultRarityLabel = "Rareza:";
    public string defaultRarityValue = "Poco Comun";
    [TextArea] public string defaultDescription = "Descripcion del item";

    private InventoryEntry currentEntry;
    private ItemBase currentItem;
    private System.Action<InventoryEntry, ItemBase> onSellOne;
    private System.Action<InventoryEntry, ItemBase> onSellAll;

    private void Awake()
    {
        AutoBind();
        HookButtons();
    }

    public void BindSellActions(
        System.Action<InventoryEntry, ItemBase> onSellOne,
        System.Action<InventoryEntry, ItemBase> onSellAll)
    {
        this.onSellOne = onSellOne;
        this.onSellAll = onSellAll;
        HookButtons();
    }

    public void Show(InventoryEntry entry, ItemBase item)
    {
        currentEntry = entry;
        currentItem = item;

        if (item == null)
        {
            ShowEmpty();
            return;
        }

        SetText(nameText, string.IsNullOrWhiteSpace(item.itemName) ? defaultName : item.itemName);
        SetText(typeText, BuildTypeText(item));
        SetText(dataText, BuildDataText(item));
        SetText(rarityText, defaultRarityLabel);
        SetText(rarityValueText, defaultRarityValue);
        SetText(descriptionText, string.IsNullOrWhiteSpace(item.description) ? defaultDescription : item.description);

        if (largeIconImage != null && item.icon != null)
        {
            largeIconImage.enabled = true;
            largeIconImage.sprite = item.icon;
        }

        RefreshSellTexts();
    }

    public void ShowEmpty()
    {
        currentEntry = null;
        currentItem = null;

        SetText(nameText, defaultName);
        SetText(typeText, defaultType);
        SetText(dataText, defaultData);
        SetText(rarityText, defaultRarityLabel);
        SetText(rarityValueText, defaultRarityValue);
        SetText(descriptionText, defaultDescription);
        RefreshSellTexts();
    }

    public void RefreshSellTexts()
    {
        int amount = currentEntry != null ? Mathf.Max(0, currentEntry.amount) : 0;
        int sellValue = currentItem != null ? Mathf.Max(0, currentItem.sellValue) : 0;
        int sellAllValue = amount * sellValue;

        SetText(sellOneText, "Vender 1");
        SetText(sellOnePriceText, sellValue.ToString("N0"));
        SetText(sellAllText, "Vender todo");
        SetText(sellAllPriceText, sellAllValue.ToString("N0"));

        if (sellOneButton != null)
            sellOneButton.interactable = currentEntry != null && currentItem != null && amount > 0;

        if (sellAllButton != null)
            sellAllButton.interactable = currentEntry != null && currentItem != null && amount > 0;
    }

    private string BuildTypeText(ItemBase item)
    {
        if (item is Weapon weapon)
            return $"Arma - {FormatWeaponType(weapon.weaponType)}";

        if (item is Armor armor)
            return $"Armadura - {FormatArmorType(armor.armorType)}";

        if (item is ConsumableItem)
            return "Consumible";

        return defaultType;
    }

    private string BuildDataText(ItemBase item)
    {
        StringBuilder builder = new StringBuilder();

        if (item is Weapon weapon)
        {
            builder.AppendLine($"Dano fisico: {weapon.physicalDamageMin}-{weapon.physicalDamageMax}");
            builder.AppendLine($"Dano magico: {weapon.magicalDamageMin}-{weapon.magicalDamageMax}");
            builder.AppendLine($"Slot: {FormatSlot(weapon.slot)}");
        }
        else if (item is Armor armor)
        {
            builder.AppendLine($"Armadura fisica: +{armor.physicalArmor}");
            builder.AppendLine($"Armadura magica: +{armor.magicalArmor}");
            builder.AppendLine($"Slot: {FormatSlot(armor.slot)}");
        }
        else if (item is ConsumableItem consumable)
        {
            builder.AppendLine($"Efecto: {FormatConsumableEffect(consumable.consumableEffectType)}");

            if (consumable.value > 0)
                builder.AppendLine($"Valor: {consumable.value}");

            if (consumable.consumableEffectType == ConsumableEffectType.ApplyStatusToSelf)
                builder.AppendLine($"Estado: {consumable.statusEffectType} ({consumable.statusDuration} turnos)");
        }

        if (item.effects != null && item.effects.Count > 0)
        {
            foreach (ItemEffect effect in item.effects)
            {
                if (effect == null)
                    continue;

                string suffix = effect.isPercent ? "%" : "";
                builder.AppendLine($"{FormatStat(effect.statType)}: +{effect.value}{suffix}");
            }
        }

        string data = builder.ToString().TrimEnd();
        return string.IsNullOrWhiteSpace(data) ? defaultData : data;
    }

    private void HandleSellOne()
    {
        if (currentEntry == null || currentItem == null)
            return;

        onSellOne?.Invoke(currentEntry, currentItem);
    }

    private void HandleSellAll()
    {
        if (currentEntry == null || currentItem == null)
            return;

        onSellAll?.Invoke(currentEntry, currentItem);
    }

    private void HookButtons()
    {
        if (sellOneButton != null)
        {
            sellOneButton.onClick.RemoveListener(HandleSellOne);
            sellOneButton.onClick.AddListener(HandleSellOne);
        }

        if (sellAllButton != null)
        {
            sellAllButton.onClick.RemoveListener(HandleSellAll);
            sellAllButton.onClick.AddListener(HandleSellAll);
        }
    }

    private void AutoBind()
    {
        if (largeIconImage == null)
            largeIconImage = FindChild<Image>("Image_LargeIcon");

        if (rarityGemImage == null)
            rarityGemImage = FindChild<Image>("Image_RarityGem");

        if (nameText == null)
            nameText = FindChild<TMP_Text>("Text_ItemName");

        if (typeText == null)
            typeText = FindChild<TMP_Text>("Text_ItemType");

        if (dataText == null)
            dataText = FindChild<TMP_Text>("Text_ItemData");

        if (rarityText == null)
            rarityText = FindChild<TMP_Text>("Text_Rarity");

        if (rarityValueText == null)
            rarityValueText = FindChild<TMP_Text>("Text_ItemRarityText");

        if (descriptionText == null)
            descriptionText = FindChild<TMP_Text>("Text_ItemDescription");

        if (sellOneButton == null)
            sellOneButton = FindChild<Button>("ButtonSellOne");

        if (sellAllButton == null)
            sellAllButton = FindChild<Button>("ButtonSellAll");

        if (sellOneButton != null)
        {
            sellOneText = sellOneText != null ? sellOneText : FindChildIn<TMP_Text>(sellOneButton.transform, "Text_Sell", "TextSell");
            sellOnePriceText = sellOnePriceText != null ? sellOnePriceText : FindChildIn<TMP_Text>(sellOneButton.transform, "Text_SellPrice", "TextSellPrice");
        }

        if (sellAllButton != null)
        {
            sellAllText = sellAllText != null ? sellAllText : FindChildIn<TMP_Text>(sellAllButton.transform, "Text_Sell", "TextSell");
            sellAllPriceText = sellAllPriceText != null ? sellAllPriceText : FindChildIn<TMP_Text>(sellAllButton.transform, "Text_SellPrice", "TextSellPrice");
        }
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private string FormatWeaponType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:
                return "Espada";
            case WeaponType.Greatsword:
                return "Espada a dos manos";
            case WeaponType.Dagger:
                return "Daga";
            case WeaponType.Rapier:
                return "Estoque";
            case WeaponType.Staff:
                return "Baston";
            case WeaponType.Bow:
                return "Arco";
            default:
                return type.ToString();
        }
    }

    private string FormatArmorType(ArmorType type)
    {
        switch (type)
        {
            case ArmorType.Light:
                return "Ligera";
            case ArmorType.Medium:
                return "Media";
            case ArmorType.Heavy:
                return "Pesada";
            case ArmorType.Robe:
                return "Tunica";
            default:
                return type.ToString();
        }
    }

    private string FormatConsumableEffect(ConsumableEffectType type)
    {
        switch (type)
        {
            case ConsumableEffectType.HealSelf:
                return "Curacion";
            case ConsumableEffectType.ReviveAlly:
                return "Revive aliado";
            case ConsumableEffectType.DamageAllEnemies:
                return "Dano a enemigos";
            case ConsumableEffectType.ApplyStatusToSelf:
                return "Estado propio";
            case ConsumableEffectType.CleanseSelfStatus:
                return "Limpia estado";
            default:
                return "Sin efecto";
        }
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

    private string FormatStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.MaxHP:
                return "Salud maxima";
            case StatType.PhysicalDamage:
                return "Dano fisico";
            case StatType.MagicalDamage:
                return "Dano magico";
            case StatType.Speed:
                return "Velocidad";
            case StatType.PhysicalArmor:
                return "Armadura fisica";
            case StatType.MagicalArmor:
                return "Armadura magica";
            default:
                return stat.ToString();
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

    private T FindChildIn<T>(Transform root, params string[] childNames) where T : Component
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            string trimmedName = child.name.Trim();

            foreach (string childName in childNames)
            {
                if (trimmedName == childName)
                    return child.GetComponent<T>();
            }
        }

        return null;
    }
}
