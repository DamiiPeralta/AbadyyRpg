using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RosterSelectionPanelUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject rootPanel;
    public Transform cardsRoot;
    public MercenaryCardUI cardPrefab;
    public Button resetButton;
    public Button applyButton;
    public Button closeButton;
    public TMP_Text activeCountText;
    public TMP_Text messageText;

    private readonly List<MercenaryCardUI> cards = new List<MercenaryCardUI>();
    private readonly List<Unit> selectedUnits = new List<Unit>();
    private MercenaryManagementUI owner;

    private void Awake()
    {
        AutoBind();
        HookButtons();
        Hide();
    }

    private void OnEnable()
    {
        PartyRuntimeState.PartyChanged -= HandlePartyChanged;
        PartyRuntimeState.PartyChanged += HandlePartyChanged;

        if (IsVisible())
            RefreshFromRuntimeParty();
    }

    private void OnDisable()
    {
        PartyRuntimeState.PartyChanged -= HandlePartyChanged;
    }

    public void Open(MercenaryManagementUI owner)
    {
        this.owner = owner;

        if (rootPanel != null)
            rootPanel.SetActive(true);
        else
            gameObject.SetActive(true);

        RefreshFromRuntimeParty();
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void LoadCurrentActiveParty()
    {
        selectedUnits.Clear();

        if (PartyRuntimeState.Instance == null || PartyRuntimeState.Instance.GetCurrentParty() == null)
            return;

        foreach (Unit unit in PartyRuntimeState.Instance.GetCurrentParty())
        {
            if (unit != null && !selectedUnits.Contains(unit))
                selectedUnits.Add(unit);
        }
    }

    private void RefreshFromRuntimeParty()
    {
        LoadCurrentActiveParty();
        RebuildCards();
        RefreshState();
    }

    private void HandlePartyChanged()
    {
        if (!IsVisible())
            return;

        RefreshFromRuntimeParty();
    }

    private bool IsVisible()
    {
        if (rootPanel != null)
            return rootPanel.activeInHierarchy;

        return gameObject.activeInHierarchy;
    }

    private void RebuildCards()
    {
        List<Unit> roster = PartyRuntimeState.Instance != null ? PartyRuntimeState.Instance.GetRoster() : null;

        if (roster == null)
            roster = new List<Unit>();

        EnsureCardInstances(roster.Count);

        for (int i = 0; i < cards.Count; i++)
        {
            MercenaryCardUI card = cards[i];

            if (card == null)
                continue;

            bool hasUnit = i < roster.Count && roster[i] != null;
            card.gameObject.SetActive(hasUnit);

            if (hasUnit)
                card.Bind(roster[i], ToggleUnitSelection);
        }
    }

    private void EnsureCardInstances(int neededCount)
    {
        if (cardsRoot == null)
            cardsRoot = transform;

        if (cards.Count == 0)
        {
            foreach (MercenaryCardUI card in cardsRoot.GetComponentsInChildren<MercenaryCardUI>(true))
            {
                if (card != null && !cards.Contains(card))
                    cards.Add(card);
            }
        }

        if (cardPrefab == null && cards.Count > 0)
            cardPrefab = cards[0];

        while (cards.Count < neededCount && cardPrefab != null)
        {
            MercenaryCardUI card = Instantiate(cardPrefab, cardsRoot);
            cards.Add(card);
        }
    }

    private void ToggleUnitSelection(Unit unit)
    {
        if (unit == null || PartyRuntimeState.Instance == null)
            return;

        if (selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            RefreshState();
            return;
        }

        int maxActive = Mathf.Max(1, PartyRuntimeState.Instance.maxActiveMembers);

        if (selectedUnits.Count >= maxActive)
        {
            SetMessage($"Maximo {maxActive} mercenarios activos.");
            return;
        }

        selectedUnits.Add(unit);
        RefreshState();
    }

    private void ResetSelection()
    {
        selectedUnits.Clear();
        RefreshState();
    }

    private void ApplySelection()
    {
        if (PartyRuntimeState.Instance == null)
            return;

        if (selectedUnits.Count == 0)
        {
            SetMessage("Elegí al menos 1 mercenario para salir.");
            return;
        }

        if (!PartyRuntimeState.Instance.SetActivePartyOrdered(selectedUnits))
        {
            SetMessage("No se pudo aplicar el roster.");
            return;
        }

        owner?.RefreshFromRuntime();
        Hide();
    }

    private void RefreshState()
    {
        int maxActive = PartyRuntimeState.Instance != null ? Mathf.Max(1, PartyRuntimeState.Instance.maxActiveMembers) : 4;

        foreach (MercenaryCardUI card in cards)
        {
            if (card == null || card.BoundUnit == null)
                continue;

            card.Refresh();

            int selectedIndex = selectedUnits.IndexOf(card.BoundUnit);
            bool isSelected = selectedIndex >= 0;

            card.SetSelected(isSelected);
            card.SetRosterStateLabel(isSelected ? $"Orden {selectedIndex + 1}" : "Reserva");
        }

        SetText(activeCountText, $"{selectedUnits.Count}/{maxActive}");

        if (applyButton != null)
            applyButton.interactable = selectedUnits.Count > 0;

        if (closeButton != null)
            closeButton.interactable = selectedUnits.Count > 0;

        SetMessage(selectedUnits.Count == 0 ? "Elegí al menos 1 mercenario." : BuildOrderMessage());
    }

    private string BuildOrderMessage()
    {
        if (selectedUnits.Count == 0)
            return string.Empty;

        string message = "Orden: ";

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (i > 0)
                message += " > ";

            message += selectedUnits[i] != null ? selectedUnits[i].unitName : "-";
        }

        return message;
    }

    private void HookButtons()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSelection);

        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySelection);

        if (closeButton != null)
            closeButton.onClick.AddListener(ApplySelection);
    }

    private void AutoBind()
    {
        if (rootPanel == null)
            rootPanel = gameObject;

        if (cardsRoot == null)
            cardsRoot = FindChildTransform("Panel_CombatRosterSelection");

        if (cardsRoot == null)
            cardsRoot = FindChildTransform("Panel_CombatRoster");

        if (cardsRoot == null)
            cardsRoot = transform;

        if (cardPrefab == null)
            cardPrefab = GetComponentInChildren<MercenaryCardUI>(true);

        resetButton = resetButton != null ? resetButton : FindChild<Button>("Button_Reset");
        applyButton = applyButton != null ? applyButton : FindChild<Button>("Button_Apply");
        applyButton = applyButton != null ? applyButton : FindChild<Button>("Button_Confirm");
        applyButton = applyButton != null ? applyButton : FindChild<Button>("Button_Accept");
        applyButton = applyButton != null ? applyButton : FindChild<Button>("Button_SaveRoster");
        closeButton = closeButton != null ? closeButton : FindChild<Button>("Button_CloseRoster");

        if (activeCountText == null)
            activeCountText = FindChild<TMP_Text>("Text_ActiveSelection");

        if (activeCountText == null)
            activeCountText = FindChild<TMP_Text>("Text_Actives");

        if (messageText == null)
            messageText = FindChild<TMP_Text>("Text_RosterMessage");
    }

    private void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;

        if (!string.IsNullOrWhiteSpace(message))
            Debug.Log(message);
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
