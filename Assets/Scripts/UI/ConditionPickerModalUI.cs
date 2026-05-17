using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionPickerModalUI : MonoBehaviour
{
    public GameObject modalRoot;
    public Transform contentRoot;
    public Button closeButton;
    public Button conditionButtonPrefab;
    public TMP_Text titleText;

    private Action<TacticConditionType> onPicked;

    private void Awake()
    {
        AutoBind();

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        Close();
    }

    public void Open(Action<TacticConditionType> onPicked)
    {
        this.onPicked = onPicked;

        if (modalRoot != null)
            modalRoot.SetActive(true);
        else
            gameObject.SetActive(true);

        if (titleText != null)
            titleText.text = "Elegir condicion";

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

        foreach (TacticConditionType conditionType in Enum.GetValues(typeof(TacticConditionType)))
            CreateConditionButton(conditionType, FormatCondition(conditionType, 30));
    }

    private void ClearContent()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = contentRoot.GetChild(i);

            if (conditionButtonPrefab != null && child == conditionButtonPrefab.transform)
            {
                child.gameObject.SetActive(false);
                continue;
            }

            Destroy(child.gameObject);
        }
    }

    private void CreateConditionButton(TacticConditionType conditionType, string label)
    {
        Button button = conditionButtonPrefab != null
            ? Instantiate(conditionButtonPrefab, contentRoot)
            : CreateRuntimeButton(contentRoot);

        button.gameObject.SetActive(true);

        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
            text.text = label;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onPicked?.Invoke(conditionType);
            Close();
        });
    }

    private Button CreateRuntimeButton(Transform parent)
    {
        GameObject go = new GameObject("ConditionButton", typeof(RectTransform), typeof(Image), typeof(Button));
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

    private string FormatCondition(TacticConditionType conditionType, int thresholdPercent)
    {
        switch (conditionType)
        {
            case TacticConditionType.Always:
                return "Sin condicion";
            case TacticConditionType.SelfHpBelowPercent:
                return $"Yo HP < {thresholdPercent}%";
            case TacticConditionType.AllyHpBelowPercent:
                return $"Aliado HP < {thresholdPercent}%";
            case TacticConditionType.EnemyHpBelowPercent:
                return $"Enemigo HP < {thresholdPercent}%";
            case TacticConditionType.SelfPhysicalArmorBelowPercent:
                return $"Yo arm. fisica < {thresholdPercent}%";
            case TacticConditionType.AllyPhysicalArmorBelowPercent:
                return $"Aliado arm. fisica < {thresholdPercent}%";
            case TacticConditionType.SelfStaminaAbovePercent:
                return $"Yo energia >= {thresholdPercent}%";
        }

        return conditionType.ToString();
    }

    private void AutoBind()
    {
        if (modalRoot == null)
            modalRoot = gameObject;

        if (contentRoot == null)
            contentRoot = FindChildTransform("Content");

        if (contentRoot == null)
            contentRoot = FindChildTransform("ConditionList");

        if (closeButton == null)
            closeButton = FindChild<Button>("Button_Close");

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_Title");
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
