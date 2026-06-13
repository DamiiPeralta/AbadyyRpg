using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text typeText;
    public TMP_Text descriptionText;
    public Image illustrationImage;
    public List<EventOptionButtonUI> optionButtons = new List<EventOptionButtonUI>();

    private WorldMapEvent currentEvent;

    private void Awake()
    {
        AutoBind();
    }

    public void ShowEvent(WorldMapEvent mapEvent)
    {
        AutoBind();

        currentEvent = mapEvent;

        if (panel != null)
            panel.SetActive(true);

        SetText(titleText, mapEvent != null ? mapEvent.title : string.Empty);
        SetText(typeText, mapEvent != null ? mapEvent.eventType : string.Empty);
        SetText(descriptionText, mapEvent != null ? mapEvent.description : string.Empty);

        if (illustrationImage != null)
        {
            illustrationImage.sprite = mapEvent != null ? mapEvent.illustration : null;
            illustrationImage.enabled = mapEvent != null && mapEvent.illustration != null;
        }

        RefreshOptions();
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void RefreshOptions()
    {
        int optionCount = currentEvent != null && currentEvent.options != null
            ? currentEvent.options.Count
            : 0;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            EventOptionButtonUI optionButton = optionButtons[i];

            if (optionButton == null)
                continue;

            bool hasOption = i < optionCount;
            optionButton.gameObject.SetActive(hasOption);

            if (hasOption)
                optionButton.Bind(currentEvent.options[i], HandleOptionClicked);
        }
    }

    private void HandleOptionClicked(WorldMapEventOption option)
    {
        if (!EventOptionCostUtility.Pay(option))
        {
            RefreshOptions();
            return;
        }

        if (WorldMapManager.Instance != null)
            WorldMapManager.Instance.ResolveOption(option);

        Hide();
    }

    private void AutoBind()
    {
        if (panel == null)
        {
            Transform panelTransform = transform.Find("PanelEvent");
            panel = panelTransform != null ? panelTransform.gameObject : gameObject;
        }

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_EventTitle");

        if (typeText == null)
            typeText = FindChild<TMP_Text>("Text_EventType");

        if (descriptionText == null)
            descriptionText = FindChild<TMP_Text>("Text_EventDescription");

        if (illustrationImage == null)
            illustrationImage = FindChild<Image>("Image_Illustration");

        if (illustrationImage == null)
            illustrationImage = FindChild<Image>("Image_Iustration");

        BindOptionButtons();
        ConfigureRaycastTargets();
    }

    private void BindOptionButtons()
    {
        optionButtons.RemoveAll(button => button == null);

        if (optionButtons.Count > 0)
            return;

        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button == null || !button.name.StartsWith("Button_Option"))
                continue;

            EventOptionButtonUI optionButton = button.GetComponent<EventOptionButtonUI>();

            if (optionButton == null)
                optionButton = button.gameObject.AddComponent<EventOptionButtonUI>();

            optionButtons.Add(optionButton);
        }
    }

    private void ConfigureRaycastTargets()
    {
        Transform raycastRoot = panel != null ? panel.transform : transform;
        Graphic[] graphics = raycastRoot.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null)
                continue;

            Button ownerButton = graphic.GetComponentInParent<Button>();
            graphic.raycastTarget = ownerButton != null && ownerButton.name.StartsWith("Button_Option");
        }

        foreach (EventOptionButtonUI optionButton in optionButtons)
        {
            if (optionButton == null || optionButton.button == null)
                continue;

            Graphic targetGraphic = optionButton.button.targetGraphic;

            if (targetGraphic != null)
                targetGraphic.raycastTarget = true;
        }
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
