using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventOptionButtonUI : MonoBehaviour
{
    [Header("Refs")]
    public Button button;
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text costText;

    [Header("Colors")]
    public Color availableCostColor = Color.white;
    public Color missingCostColor = new Color(1f, 0.45f, 0.35f, 1f);

    private WorldMapEventOption option;
    private Action<WorldMapEventOption> onClicked;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(WorldMapEventOption option, Action<WorldMapEventOption> onClicked)
    {
        AutoBind();

        this.option = option;
        this.onClicked = onClicked;

        string title = option != null ? option.title : string.Empty;
        string description = option != null ? option.description : string.Empty;

        if (titleText != null)
        {
            SetText(titleText, title);
            SetText(descriptionText, description);
        }
        else
        {
            SetText(descriptionText, string.IsNullOrWhiteSpace(description) ? title : $"{title}\n{description}");
        }

        if (iconImage != null)
        {
            iconImage.sprite = option != null ? option.icon : null;
            iconImage.enabled = option != null && option.icon != null;
        }

        bool canPay = EventOptionCostUtility.CanPay(option);
        SetText(costText, EventOptionCostUtility.BuildCostText(option));

        if (costText != null)
            costText.color = canPay ? availableCostColor : missingCostColor;

        if (button != null)
        {
            button.interactable = canPay;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        Debug.Log($"Event option clicked: {(option != null ? option.title : "null")}");
        onClicked?.Invoke(option);
    }

    private void AutoBind()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (iconImage == null)
            iconImage = FindChild<Image>("Image_Icon");

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_ChoiceTitle");

        if (descriptionText == null)
            descriptionText = FindChild<TMP_Text>("Text_ChoiceDescription");

        if (costText == null)
            costText = FindChild<TMP_Text>("Text_ChoiceCost");
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
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
