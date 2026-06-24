using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuppliesItemRowUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text itemNameText;
    public TMP_Text quantityText;
    public TMP_Text valueText;
    public Image coinIconImage;
    public Button sellButton;
    public Button selectButton;

    [Header("Layout sugerido")]
    public float rowWidth = 800f;
    public float rowHeight = 56f;
    public float iconWidth = 40f;
    public float nameWidth = 360f;
    public float quantityWidth = 64f;
    public float coinWidth = 28f;
    public float valueWidth = 64f;
    public float sellWidth = 90f;

    private InventoryEntry entry;
    private ItemBase item;
    private Action<InventoryEntry, ItemBase> onSelected;
    private Action<InventoryEntry, ItemBase> onSell;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(
        InventoryEntry entry,
        ItemBase item,
        Action<InventoryEntry, ItemBase> onSelected,
        Action<InventoryEntry, ItemBase> onSell)
    {
        this.entry = entry;
        this.item = item;
        this.onSelected = onSelected;
        this.onSell = onSell;

        Refresh();
        HookButtons();
    }

    public void Refresh()
    {
        if (itemNameText != null)
            itemNameText.text = item != null ? item.itemName : entry.itemId;

        if (quantityText != null)
            quantityText.text = entry != null ? entry.amount.ToString("00") : "00";

        if (valueText != null)
            valueText.text = item != null ? item.sellValue.ToString("00") : "00";

        if (iconImage != null)
        {
            iconImage.enabled = item != null && item.icon != null;

            if (item != null && item.icon != null)
                iconImage.sprite = item.icon;
        }
    }

    public void SetCanSell(bool canSell)
    {
        if (sellButton != null)
            sellButton.interactable = canSell;
    }

    private void HookButtons()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(HandleSelected);
            selectButton.onClick.AddListener(HandleSelected);
        }

        if (sellButton != null)
        {
            sellButton.onClick.RemoveListener(HandleSell);
            sellButton.onClick.AddListener(HandleSell);
        }
    }

    private void HandleSelected()
    {
        onSelected?.Invoke(entry, item);
    }

    private void HandleSell()
    {
        onSell?.Invoke(entry, item);
    }

    private void AutoBind()
    {
        if (iconImage == null)
            iconImage = FindChild<Image>("Image_ItemIcon");

        if (itemNameText == null)
            itemNameText = FindChild<TMP_Text>("Text_ItemName");

        if (quantityText == null)
            quantityText = FindChild<TMP_Text>("Text_ItemQuantity");

        if (valueText == null)
            valueText = FindChild<TMP_Text>("Text_ItemValue");

        if (coinIconImage == null)
            coinIconImage = FindChild<Image>("Image_CoinIcon");

        if (sellButton == null)
            sellButton = FindChild<Button>("ButtonSellItem");

        if (selectButton == null)
            selectButton = GetComponent<Button>();
    }

    [ContextMenu("Setup Supplies Row Layout")]
    public void SetupSuggestedLayout()
    {
        AutoBind();

        RectTransform rowRect = GetComponent<RectTransform>();
        if (rowRect != null)
        {
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.sizeDelta = new Vector2(0f, rowHeight);
        }

        LayoutElement rowLayout = GetOrAdd<LayoutElement>(gameObject);
        rowLayout.preferredWidth = rowWidth;
        rowLayout.preferredHeight = rowHeight;
        rowLayout.layoutPriority = 1;

        HorizontalLayoutGroup rowGroup = GetOrAdd<HorizontalLayoutGroup>(gameObject);
        rowGroup.padding = new RectOffset(12, 10, 6, 6);
        rowGroup.spacing = 8f;
        rowGroup.childAlignment = TextAnchor.MiddleLeft;
        rowGroup.childControlWidth = false;
        rowGroup.childControlHeight = true;
        rowGroup.childForceExpandWidth = false;
        rowGroup.childForceExpandHeight = false;

        ConfigureGraphic(iconImage, iconWidth, rowHeight - 12f);
        ConfigureText(itemNameText, nameWidth, rowHeight - 12f, TextAlignmentOptions.MidlineLeft);
        ConfigureText(quantityText, quantityWidth, rowHeight - 12f, TextAlignmentOptions.MidlineGeoAligned);
        ConfigureGraphic(coinIconImage, coinWidth, rowHeight - 18f);
        ConfigureText(valueText, valueWidth, rowHeight - 12f, TextAlignmentOptions.MidlineGeoAligned);

        if (sellButton != null)
        {
            ConfigureRect(sellButton.GetComponent<RectTransform>(), sellWidth, rowHeight - 16f);

            TMP_Text sellText = sellButton.GetComponentInChildren<TMP_Text>(true);
            if (sellText != null)
                sellText.alignment = TextAlignmentOptions.Center;
        }
    }

    private void ConfigureGraphic(Image image, float width, float height)
    {
        if (image == null)
            return;

        image.preserveAspect = true;
        ConfigureRect(image.rectTransform, width, height);
    }

    private void ConfigureText(TMP_Text text, float width, float height, TextAlignmentOptions alignment)
    {
        if (text == null)
            return;

        text.alignment = alignment;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        ConfigureRect(text.rectTransform, width, height);
    }

    private void ConfigureRect(RectTransform rect, float width, float height)
    {
        if (rect == null)
            return;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);

        LayoutElement layout = GetOrAdd<LayoutElement>(rect.gameObject);
        layout.preferredWidth = width;
        layout.preferredHeight = height;
    }

    private T GetOrAdd<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();

        if (component == null)
            component = target.AddComponent<T>();

        return component;
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
