using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryCardUI : MonoBehaviour
{
    [Header("Refs")]
    public Button interactButton;
    public GameObject selectionFrame;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text classLevelText;
    public TMP_Text rosterStateText;
    public Slider hpSlider;
    public TMP_Text hpText;
    public Slider staminaSlider;
    public TMP_Text staminaText;

    private Unit unit;
    private Action<Unit> onClicked;

    public Unit BoundUnit => unit;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(Unit unit, Action<Unit> onClicked)
    {
        this.unit = unit;
        this.onClicked = onClicked;

        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(HandleClick);
            interactButton.onClick.AddListener(HandleClick);
        }

        Refresh();
    }

    public void SetSelected(bool selected)
    {
        if (selectionFrame != null)
            selectionFrame.SetActive(selected);
    }

    public void Refresh()
    {
        if (unit == null)
            return;

        if (nameText != null)
            nameText.text = unit.unitName;

        if (classLevelText != null)
            classLevelText.text = $"Nivel {unit.level}";

        if (rosterStateText != null)
        {
            bool isActive = PartyRuntimeState.Instance == null || PartyRuntimeState.Instance.IsActiveMember(unit);
            rosterStateText.text = isActive ? "Activo" : "Reserva";
        }

        if (portraitImage != null && unit.unitData != null && unit.unitData.icon != null)
            portraitImage.sprite = unit.unitData.icon;

        SetSlider(hpSlider, unit.currentHP, unit.maxHP);
        SetSlider(staminaSlider, unit.currentStamina, unit.maxStamina);

        if (hpText != null)
            hpText.text = $"{unit.currentHP}/{unit.maxHP}";

        if (staminaText != null)
            staminaText.text = $"{unit.currentStamina}/{unit.maxStamina}";
    }

    private void HandleClick()
    {
        onClicked?.Invoke(unit);
    }

    private void SetSlider(Slider slider, int current, int maximum)
    {
        if (slider == null)
            return;

        slider.minValue = 0;
        slider.maxValue = Mathf.Max(1, maximum);
        slider.value = Mathf.Clamp(current, 0, slider.maxValue);
    }

    private void AutoBind()
    {
        if (interactButton == null)
            interactButton = FindChild<Button>("Button_Interact");

        if (selectionFrame == null)
            selectionFrame = FindChildTransform("SelectionFrame")?.gameObject;

        if (portraitImage == null)
            portraitImage = FindChild<Image>("PortraitImage");

        if (nameText == null)
            nameText = FindChild<TMP_Text>("Text_Name");

        if (classLevelText == null)
            classLevelText = FindChild<TMP_Text>("Text_ClassLevel");

        if (rosterStateText == null)
            rosterStateText = FindChild<TMP_Text>("Text_RosterState");

        if (hpSlider == null)
            hpSlider = FindChild<Slider>("Slider_HP");

        if (hpText == null)
            hpText = FindChild<TMP_Text>("Text_HP");

        if (staminaSlider == null)
            staminaSlider = FindChild<Slider>("Slider_Stamina");

        if (staminaText == null)
            staminaText = FindChild<TMP_Text>("Text_Stamina");
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
