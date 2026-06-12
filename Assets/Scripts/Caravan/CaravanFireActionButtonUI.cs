using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaravanFireActionButtonUI : MonoBehaviour
{
    [Header("Refs")]
    public Button button;
    public Image actionIcon;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public GameObject disabledOverlay;
    public CaravanFireCostSlotUI costSlot1;
    public CaravanFireCostSlotUI costSlot2;

    private CaravanFireActionType actionType;
    private Action<CaravanFireActionType> onClicked;

    private void Awake()
    {
        AutoBind();
        HookButton();
    }

    public void Bind(CaravanFireActionType actionType, string title, string description, Action<CaravanFireActionType> onClicked)
    {
        this.actionType = actionType;
        this.onClicked = onClicked;

        SetText(titleText, title);
        SetText(descriptionText, description);
        HookButton();
    }

    public void SetAvailable(bool available)
    {
        if (button != null)
            button.interactable = available;

        if (disabledOverlay != null)
            disabledOverlay.SetActive(!available);
    }

    public void SetCosts(CaravanFireResourceCost[] costs, Sprite[] icons, bool[] canPay)
    {
        SetSlot(costSlot1, costs, icons, canPay, 0);
        SetSlot(costSlot2, costs, icons, canPay, 1);
    }

    private void SetSlot(CaravanFireCostSlotUI slot, CaravanFireResourceCost[] costs, Sprite[] icons, bool[] canPay, int index)
    {
        if (slot == null)
            return;

        if (costs == null || index >= costs.Length || costs[index] == null)
        {
            slot.SetUnused();
            return;
        }

        Sprite icon = icons != null && index < icons.Length ? icons[index] : null;
        bool hasEnough = canPay == null || index >= canPay.Length || canPay[index];
        slot.ShowCost(icon, Mathf.Max(0, costs[index].amount), hasEnough);
    }

    private void HookButton()
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.RemoveListener(HandleClick);
        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        onClicked?.Invoke(actionType);
    }

    private void AutoBind()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (actionIcon == null)
            actionIcon = FindChild<Image>("Image_ActionIcon");

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_Title");

        if (descriptionText == null)
            descriptionText = FindChild<TMP_Text>("Text_Description");

        if (disabledOverlay == null)
            disabledOverlay = FindChildTransform("DisabledOverlay")?.gameObject;

        if (costSlot1 == null)
            costSlot1 = FindChild<CaravanFireCostSlotUI>("CostSlot_01");

        if (costSlot2 == null)
            costSlot2 = FindChild<CaravanFireCostSlotUI>("CostSlot_02");
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
