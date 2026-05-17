using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticsPanelUI : MonoBehaviour
{
    public Transform rowsRoot;
    public TacticRuleRowUI rowPrefab;
    public Button addRuleButton;
    public Button resetButton;
    public AbilityPickerModalUI abilityPickerModal;
    public ConditionPickerModalUI conditionPickerModal;

    private Unit selectedUnit;
    private readonly List<TacticRuleRowUI> rowInstances = new List<TacticRuleRowUI>();

    private void Awake()
    {
        AutoBind();

        if (addRuleButton != null)
            addRuleButton.onClick.AddListener(AddRule);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetRules);
    }

    public void Bind(Unit unit, AbilityPickerModalUI abilityPickerModal, ConditionPickerModalUI conditionPickerModal = null)
    {
        selectedUnit = unit;

        if (abilityPickerModal != null)
            this.abilityPickerModal = abilityPickerModal;

        if (conditionPickerModal != null)
            this.conditionPickerModal = conditionPickerModal;

        RefreshRows();
    }

    public void RefreshRows()
    {
        ClearRows();

        if (selectedUnit == null)
            return;

        EnsureTacticsList();
        SortAndNormalizePriorities();

        int count = Mathf.Min(selectedUnit.tactics.Count, TacticRules.MaxRules);

        for (int i = 0; i < count; i++)
        {
            TacticRule rule = selectedUnit.tactics[i];
            TacticRuleRowUI row = Instantiate(rowPrefab, rowsRoot);
            row.gameObject.SetActive(true);
            row.Bind(rule, this);
            rowInstances.Add(row);
        }
    }

    public void OpenAbilityPicker(TacticRule rule)
    {
        if (selectedUnit == null || rule == null || abilityPickerModal == null)
            return;

        abilityPickerModal.Open(selectedUnit, ability =>
        {
            rule.ability = ability;
            RefreshRows();
        });
    }

    public void OpenConditionPicker(TacticRule rule)
    {
        if (rule == null || conditionPickerModal == null)
            return;

        conditionPickerModal.Open(conditionType =>
        {
            rule.conditionType = conditionType;
            RefreshRows();
        });
    }

    public void MoveRule(TacticRule rule, int direction)
    {
        if (selectedUnit == null || selectedUnit.tactics == null || rule == null)
            return;

        SortAndNormalizePriorities();

        int index = selectedUnit.tactics.IndexOf(rule);
        int targetIndex = Mathf.Clamp(index + direction, 0, selectedUnit.tactics.Count - 1);

        if (index == targetIndex)
            return;

        selectedUnit.tactics.RemoveAt(index);
        selectedUnit.tactics.Insert(targetIndex, rule);
        SortAndNormalizePriorities();
        RefreshRows();
    }

    private void AddRule()
    {
        if (selectedUnit == null)
            return;

        EnsureTacticsList();

        if (selectedUnit.tactics.Count >= TacticRules.MaxRules)
            return;

        selectedUnit.tactics.Add(new TacticRule
        {
            priority = selectedUnit.tactics.Count + 1,
            isActive = true,
            conditionType = TacticConditionType.Always,
            thresholdPercent = 30,
            ability = GetFirstAvailableAbility()
        });

        RefreshRows();
    }

    private void ResetRules()
    {
        if (selectedUnit == null)
            return;

        selectedUnit.tactics = new List<TacticRule>();
        AddRule();
    }

    private AbilitySO GetFirstAvailableAbility()
    {
        if (selectedUnit == null || selectedUnit.abilities == null)
            return null;

        foreach (AbilitySO ability in selectedUnit.abilities)
        {
            if (ability != null)
                return ability;
        }

        return null;
    }

    private void EnsureTacticsList()
    {
        if (selectedUnit.tactics == null)
            selectedUnit.tactics = new List<TacticRule>();

        if (selectedUnit.tactics.Count > TacticRules.MaxRules)
            selectedUnit.tactics.RemoveRange(TacticRules.MaxRules, selectedUnit.tactics.Count - TacticRules.MaxRules);
    }

    private void SortAndNormalizePriorities()
    {
        selectedUnit.tactics.Sort((left, right) => left.priority.CompareTo(right.priority));

        for (int i = 0; i < selectedUnit.tactics.Count; i++)
            selectedUnit.tactics[i].priority = i + 1;
    }

    private void ClearRows()
    {
        foreach (TacticRuleRowUI row in rowInstances)
        {
            if (row != null)
                Destroy(row.gameObject);
        }

        rowInstances.Clear();
    }

    private void AutoBind()
    {
        if (rowsRoot == null)
            rowsRoot = FindChildTransform("RowsRoot");

        if (rowsRoot == null)
            rowsRoot = FindChildTransform("Content");

        if (rowsRoot == null)
            rowsRoot = transform;

        if (rowPrefab == null)
            rowPrefab = GetComponentInChildren<TacticRuleRowUI>(true);

        if (addRuleButton == null)
            addRuleButton = FindChild<Button>("Button_AddRule");

        if (resetButton == null)
            resetButton = FindChild<Button>("Button_Reset");

        if (conditionPickerModal == null)
            conditionPickerModal = GetComponentInChildren<ConditionPickerModalUI>(true);
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
