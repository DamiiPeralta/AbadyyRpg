using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPickerModalUI : MonoBehaviour
{
    public GameObject modalRoot;
    public Transform contentRoot;
    public Button closeButton;
    public Button abilityButtonPrefab;
    public TMP_Text titleText;

    private Unit unit;
    private Action<AbilitySO> onPicked;

    private void Awake()
    {
        AutoBind();

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        Close();
    }

    public void Open(Unit unit, Action<AbilitySO> onPicked)
    {
        this.unit = unit;
        this.onPicked = onPicked;

        if (modalRoot != null)
            modalRoot.SetActive(true);
        else
            gameObject.SetActive(true);

        if (titleText != null)
            titleText.text = "Elegir habilidad";

        Rebuild();
    }

    public void Close()
    {
        if (modalRoot != null)
            modalRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void Rebuild()
    {
        if (contentRoot == null)
            contentRoot = transform;

        ClearContent();

        CreateAbilityButton(null, true);

        if (unit == null || unit.abilities == null || unit.abilities.Count == 0)
            return;

        foreach (AbilitySO ability in unit.abilities)
        {
            if (ability == null)
                continue;

            CreateAbilityButton(ability, true);
        }
    }

    private void ClearContent()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = contentRoot.GetChild(i);

            if (abilityButtonPrefab != null && child == abilityButtonPrefab.transform)
            {
                child.gameObject.SetActive(false);
                continue;
            }

            Destroy(child.gameObject);
        }
    }

    private void CreateAbilityButton(AbilitySO ability, bool canPick)
    {
        Button button = abilityButtonPrefab != null
            ? Instantiate(abilityButtonPrefab, contentRoot)
            : CreateRuntimeButton(contentRoot);

        button.gameObject.SetActive(true);

        FillAbilityButton(button.gameObject, ability);

        button.interactable = canPick;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onPicked?.Invoke(ability);
            Close();
        });
    }

    private void FillAbilityButton(GameObject root, AbilitySO ability)
    {
        TMP_Text nameText = FindChild<TMP_Text>(root.transform, "Text_AbilityName");
        TMP_Text typeText = FindChild<TMP_Text>(root.transform, "Text_AbilityType");
        TMP_Text costText = FindChild<TMP_Text>(root.transform, "Text_AbilityCost");
        Image iconImage = FindChild<Image>(root.transform, "AbilityIcon");

        if (ability == null)
        {
            SetText(nameText, "Sin habilidad");
            SetText(typeText, "-");
            SetText(costText, "-");

            if (iconImage != null)
                iconImage.enabled = false;

            TMP_Text fallbackText = root.GetComponentInChildren<TMP_Text>(true);
            if (fallbackText != null && nameText == null)
                fallbackText.text = "Sin habilidad";

            return;
        }

        SetText(nameText, ability.abilityName);
        SetText(typeText, FormatAbilityType(ability));
        SetText(costText, FormatCost(ability));

        if (iconImage != null)
        {
            iconImage.enabled = ability.icon != null;

            if (ability.icon != null)
                iconImage.sprite = ability.icon;
        }

        TMP_Text fallback = root.GetComponentInChildren<TMP_Text>(true);
        if (fallback != null && nameText == null)
            fallback.text = $"{ability.abilityName}    {FormatAbilityType(ability)}    {FormatCost(ability)}";
    }

    private string FormatAbilityType(AbilitySO ability)
    {
        if (ability == null)
            return "-";

        if (ability.cleansesNegativeStatus)
            return "Limpieza";

        if (ability.targetType == AbilityTarget.Ally && (ability.flatHeal > 0 || ability.healPercent > 0f))
            return "Curacion";

        if (ability.restorePhysicalArmor > 0 || ability.restoreMagicalArmor > 0)
            return "Defensa";

        if (ability.appliesStatusEffect)
            return "Estado";

        if (ability.targetType == AbilityTarget.Enemy)
            return "Ataque";

        if (ability.targetType == AbilityTarget.Self)
            return "Personal";

        return ability.targetType.ToString();
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private Button CreateRuntimeButton(Transform parent)
    {
        GameObject go = new GameObject("AbilityButton", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        image.color = new Color(0.18f, 0.10f, 0.06f, 0.95f);

        GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(go.transform, false);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 4f);
        textRect.offsetMax = new Vector2(-12f, -4f);

        TextMeshProUGUI text = textGO.GetComponent<TextMeshProUGUI>();
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.MidlineLeft;

        LayoutElement layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = 42f;

        return go.GetComponent<Button>();
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
        if (modalRoot == null)
            modalRoot = gameObject;

        if (contentRoot == null)
            contentRoot = FindChildTransform("Content");

        if (contentRoot == null)
            contentRoot = FindChildTransform("AbilityList");

        if (closeButton == null)
            closeButton = FindChild<Button>("Button_Close");

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_Title");
    }

    private T FindChild<T>(string childName) where T : Component
    {
        Transform child = FindChildTransform(transform, childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private T FindChild<T>(Transform root, string childName) where T : Component
    {
        Transform child = FindChildTransform(root, childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindChildTransform(string childName)
    {
        return FindChildTransform(transform, childName);
    }

    private Transform FindChildTransform(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child;
        }

        return null;
    }
}
