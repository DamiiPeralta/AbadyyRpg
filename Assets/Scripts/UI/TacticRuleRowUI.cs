using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TacticRuleRowUI : MonoBehaviour
{
    [Header("Prioridad")]
    public Image priorityImage;
    public Sprite[] prioritySprites;

    [Header("Activo")]
    public Button activeButton;
    public Image activeImage;
    public Sprite activeOnSprite;
    public Sprite activeOffSprite;

    [Header("Condicion")]
    public TMP_Text conditionText;
    public Button conditionButton;

    [Header("Habilidad")]
    public Button editAbilityButton;
    public Image abilityIcon;
    public TMP_Text abilityText;

    [Header("Coste")]
    public TMP_Text costText;

    [Header("Orden")]
    public Button moveUpButton;
    public Button moveDownButton;

    private TacticRule rule;
    private TacticsPanelUI owner;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(TacticRule rule, TacticsPanelUI owner)
    {
        this.rule = rule;
        this.owner = owner;

        HookEvents();
        Refresh();
    }

    public void Refresh()
    {
        if (rule == null)
            return;

        if (priorityImage != null && prioritySprites != null && prioritySprites.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(rule.priority - 1, 0, prioritySprites.Length - 1);
            priorityImage.sprite = prioritySprites[spriteIndex];
        }

        if (activeImage != null)
        {
            Sprite nextSprite = rule.isActive ? activeOnSprite : activeOffSprite;

            if (nextSprite != null)
                activeImage.sprite = nextSprite;
        }

        if (conditionText != null)
            conditionText.text = FormatCondition(rule);

        if (abilityText != null)
            abilityText.text = rule.ability != null ? rule.ability.abilityName : "Sin habilidad";

        if (abilityIcon != null)
        {
            abilityIcon.enabled = rule.ability != null && rule.ability.icon != null;

            if (rule.ability != null && rule.ability.icon != null)
                abilityIcon.sprite = rule.ability.icon;
        }

        if (costText != null)
            costText.text = FormatCost(rule.ability);
    }

    private void HookEvents()
    {
        if (activeButton != null)
        {
            activeButton.onClick.RemoveListener(ToggleActive);
            activeButton.onClick.AddListener(ToggleActive);
        }

        if (conditionButton != null)
        {
            conditionButton.onClick.RemoveListener(EditCondition);
            conditionButton.onClick.AddListener(EditCondition);
        }

        if (editAbilityButton != null)
        {
            editAbilityButton.onClick.RemoveListener(EditAbility);
            editAbilityButton.onClick.AddListener(EditAbility);
        }

        if (moveUpButton != null)
        {
            moveUpButton.onClick.RemoveListener(MoveUp);
            moveUpButton.onClick.AddListener(MoveUp);
        }

        if (moveDownButton != null)
        {
            moveDownButton.onClick.RemoveListener(MoveDown);
            moveDownButton.onClick.AddListener(MoveDown);
        }
    }

    private void ToggleActive()
    {
        rule.isActive = !rule.isActive;
        owner.RefreshRows();
    }

    private void EditCondition()
    {
        owner.OpenConditionPicker(rule);
    }

    private void EditAbility()
    {
        owner.OpenAbilityPicker(rule);
    }

    private void MoveUp()
    {
        owner.MoveRule(rule, -1);
    }

    private void MoveDown()
    {
        owner.MoveRule(rule, 1);
    }

    private string FormatCondition(TacticRule tacticRule)
    {
        switch (tacticRule.conditionType)
        {
            case TacticConditionType.Always:
                return "Sin condicion";
            case TacticConditionType.SelfHpBelowPercent:
                return $"Yo HP < {tacticRule.thresholdPercent}%";
            case TacticConditionType.AllyHpBelowPercent:
                return $"Aliado HP < {tacticRule.thresholdPercent}%";
            case TacticConditionType.EnemyHpBelowPercent:
                return $"Enemigo HP < {tacticRule.thresholdPercent}%";
            case TacticConditionType.SelfPhysicalArmorBelowPercent:
                return $"Yo arm. fisica < {tacticRule.thresholdPercent}%";
            case TacticConditionType.AllyPhysicalArmorBelowPercent:
                return $"Aliado arm. fisica < {tacticRule.thresholdPercent}%";
            case TacticConditionType.SelfStaminaAbovePercent:
                return $"Yo energia >= {tacticRule.thresholdPercent}%";
        }

        return tacticRule.conditionType.ToString();
    }

    private string FormatCost(AbilitySO ability)
    {
        if (ability == null)
            return "-";

        bool hasMana = ability.manaCost > 0;
        bool hasStamina = ability.staminaCost > 0;

        if (hasMana && hasStamina)
            return $"Mana {ability.manaCost} / Energia {ability.staminaCost}";

        if (hasMana)
            return $"Mana {ability.manaCost}";

        if (hasStamina)
            return $"Energia {ability.staminaCost}";

        return "Gratis";
    }

    private void AutoBind()
    {
        if (priorityImage == null)
            priorityImage = FindChild<Image>("Image_Priority");

        if (priorityImage == null)
            priorityImage = FindChild<Image>("PriorAct");

        if (activeButton == null)
            activeButton = FindChild<Button>("Button_Active");

        if (activeButton == null)
        {
            Transform activeTransform = FindChildTransform("PriorAct");
            if (activeTransform != null)
                activeButton = activeTransform.GetComponent<Button>();
        }

        if (activeImage == null)
            activeImage = FindChild<Image>("Image_Active");

        if (activeImage == null)
        {
            Transform activeTransform = FindChildTransform("PriorAct");
            if (activeTransform != null)
                activeImage = activeTransform.GetComponentInChildren<Image>(true);
        }

        Transform conditionTransform = FindChildTransform("Condition");
        if (conditionButton == null && conditionTransform != null)
            conditionButton = conditionTransform.GetComponent<Button>();

        if (conditionText == null && conditionTransform != null)
            conditionText = conditionTransform.GetComponentInChildren<TMP_Text>(true);

        Transform abilityTransform = FindChildTransform("Ability");
        if (editAbilityButton == null && abilityTransform != null)
            editAbilityButton = abilityTransform.GetComponent<Button>();

        if (abilityIcon == null && abilityTransform != null)
            abilityIcon = abilityTransform.GetComponentInChildren<Image>(true);

        if (abilityText == null && abilityTransform != null)
            abilityText = abilityTransform.GetComponentInChildren<TMP_Text>(true);

        if (costText == null)
            costText = FindChild<TMP_Text>("Text_Cost");

        if (moveUpButton == null)
            moveUpButton = FindChild<Button>("Button_MoveUp");

        if (moveDownButton == null)
            moveDownButton = FindChild<Button>("Button_MoveDown");
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
