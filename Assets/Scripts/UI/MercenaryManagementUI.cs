using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryManagementUI : MonoBehaviour
{
    [Header("Roster")]
    public Transform rosterRoot;
    public MercenaryCardUI cardPrefab;
    public TMP_Text companyCountText;
    public TMP_Text reserveCountText;
    public TMP_Text activeCountText;

    [Header("Panels")]
    public MercenaryDetailPanelUI detailPanel;
    public EquipmentPanelUI equipmentPanel;
    public TacticsPanelUI tacticsPanel;
    public AbilityPickerModalUI abilityPickerModal;
    public ConditionPickerModalUI conditionPickerModal;
    public ConfirmActionModalUI dismissConfirmModal;

    [Header("Actions")]
    public Button viewEquipmentButton;
    public Button dismissButton;
    public Button changeRosterButton;
    public TMP_Text changeRosterButtonText;
    public TMP_Text dismissButtonText;

    private readonly List<MercenaryCardUI> cards = new List<MercenaryCardUI>();
    private List<Unit> roster = new List<Unit>();
    private Unit selectedUnit;

    private void Awake()
    {
        AutoBind();
        HookButtons();
    }

    private void Start()
    {
        RefreshFromRuntime();
    }

    public void RefreshFromRuntime()
    {
        roster = PartyRuntimeState.Instance != null && PartyRuntimeState.Instance.GetRoster() != null
            ? PartyRuntimeState.Instance.GetRoster()
            : new List<Unit>();

        RebuildRoster();
        RefreshCounters();

        if (selectedUnit == null || !roster.Contains(selectedUnit))
            selectedUnit = roster.Count > 0 ? roster[0] : null;

        SelectUnit(selectedUnit);
    }

    public void SelectUnit(Unit unit)
    {
        selectedUnit = unit;

        foreach (MercenaryCardUI card in cards)
        {
            if (card != null)
                card.SetSelected(card.BoundUnit == selectedUnit);
        }

        if (detailPanel != null)
            detailPanel.Show(selectedUnit);

        if (tacticsPanel != null)
            tacticsPanel.Bind(selectedUnit, abilityPickerModal, conditionPickerModal);

        RefreshActionButtons();
    }

    private void RebuildRoster()
    {
        ClearCards();

        if (rosterRoot == null || cardPrefab == null)
            return;

        foreach (Unit unit in roster)
        {
            if (unit == null)
                continue;

            MercenaryCardUI card = Instantiate(cardPrefab, rosterRoot);
            card.gameObject.SetActive(true);
            card.Bind(unit, SelectUnit);
            cards.Add(card);
        }
    }

    private void ClearCards()
    {
        foreach (MercenaryCardUI card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
    }

    private void RefreshCounters()
    {
        int total = roster != null ? roster.Count : 0;
        int active = PartyRuntimeState.Instance != null && PartyRuntimeState.Instance.GetCurrentParty() != null
            ? PartyRuntimeState.Instance.GetCurrentParty().Count
            : 0;
        int reserve = PartyRuntimeState.Instance != null
            ? PartyRuntimeState.Instance.GetReserveParty().Count
            : 0;

        int maxRoster = PartyRuntimeState.Instance != null ? PartyRuntimeState.Instance.maxRosterMembers : 6;
        int maxActive = PartyRuntimeState.Instance != null ? PartyRuntimeState.Instance.maxActiveMembers : 4;

        SetText(companyCountText, $"{total}/{maxRoster}");
        SetText(reserveCountText, $"{reserve}/{Mathf.Max(0, maxRoster - maxActive)}");
        SetText(activeCountText, $"{active}/{maxActive}");
    }

    private void HookButtons()
    {
        if (dismissButton != null)
            dismissButton.onClick.AddListener(DismissSelected);

        if (viewEquipmentButton != null)
            viewEquipmentButton.onClick.AddListener(OpenEquipment);

        if (changeRosterButton != null)
            changeRosterButton.onClick.AddListener(ToggleSelectedRosterState);
    }

    private void DismissSelected()
    {
        if (selectedUnit == null || roster == null)
            return;

        if (dismissConfirmModal != null)
        {
            string body = $"Vas a despedir a {selectedUnit.unitName}. Esta accion lo quita del roster.";
            dismissConfirmModal.Show("Despedir mercenario", body, ConfirmDismissSelected, "Despedir", "Cancelar");
            return;
        }

        Debug.LogWarning("MercenaryManagementUI: falta ConfirmActionModalUI para doble confirmacion de despido.");
    }

    private void ConfirmDismissSelected()
    {
        if (selectedUnit == null || PartyRuntimeState.Instance == null)
            return;

        Unit dismissed = selectedUnit;

        if (!PartyRuntimeState.Instance.DismissUnit(dismissed))
        {
            Debug.LogWarning($"No se pudo despedir a {dismissed.unitName}. Debe quedar al menos 1 mercenario activo.");
            return;
        }

        selectedUnit = null;
        RefreshFromRuntime();
    }

    private void ToggleSelectedRosterState()
    {
        if (selectedUnit == null || PartyRuntimeState.Instance == null)
            return;

        if (!PartyRuntimeState.Instance.ToggleActive(selectedUnit))
        {
            Debug.LogWarning("No se pudo cambiar el roster. Revisa limite de activos o minimo de party.");
            return;
        }

        RefreshFromRuntime();
    }

    private void OpenEquipment()
    {
        if (selectedUnit == null)
            return;

        if (equipmentPanel != null)
            equipmentPanel.Show(selectedUnit);
        else
            Debug.LogWarning("MercenaryManagementUI: falta EquipmentPanelUI.");
    }

    private void RefreshActionButtons()
    {
        bool hasSelection = selectedUnit != null;
        bool isActive = PartyRuntimeState.Instance == null || PartyRuntimeState.Instance.IsActiveMember(selectedUnit);

        if (viewEquipmentButton != null)
            viewEquipmentButton.interactable = hasSelection;

        if (dismissButton != null)
            dismissButton.interactable = hasSelection;

        if (changeRosterButton != null)
            changeRosterButton.interactable = hasSelection;

        SetText(changeRosterButtonText, isActive ? "Enviar reserva" : "Activar");
        SetText(dismissButtonText, "Despedir");
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private void AutoBind()
    {
        if (detailPanel == null)
            detailPanel = GetComponentInChildren<MercenaryDetailPanelUI>(true);

        if (tacticsPanel == null)
            tacticsPanel = GetComponentInChildren<TacticsPanelUI>(true);

        if (equipmentPanel == null)
            equipmentPanel = GetComponentInChildren<EquipmentPanelUI>(true);

        if (abilityPickerModal == null)
            abilityPickerModal = GetComponentInChildren<AbilityPickerModalUI>(true);

        if (conditionPickerModal == null)
            conditionPickerModal = GetComponentInChildren<ConditionPickerModalUI>(true);

        if (dismissConfirmModal == null)
            dismissConfirmModal = GetComponentInChildren<ConfirmActionModalUI>(true);

        if (rosterRoot == null)
            rosterRoot = FindChildTransform("RosterRoot");

        if (cardPrefab == null)
            cardPrefab = GetComponentInChildren<MercenaryCardUI>(true);

        if (viewEquipmentButton == null)
            viewEquipmentButton = FindChild<Button>("Button_ViewEquipment");

        if (dismissButton == null)
            dismissButton = FindChild<Button>("Button_Dismiss");

        if (changeRosterButton == null)
            changeRosterButton = FindChild<Button>("Button_ChangeRoster");

        if (changeRosterButton != null && changeRosterButtonText == null)
            changeRosterButtonText = changeRosterButton.GetComponentInChildren<TMP_Text>(true);

        if (dismissButton != null && dismissButtonText == null)
            dismissButtonText = dismissButton.GetComponentInChildren<TMP_Text>(true);
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
