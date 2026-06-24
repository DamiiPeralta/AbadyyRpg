using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftingResultPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float visibleSeconds = 2f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        EnsureBuilt();
        Hide();
    }

    public static CraftingResultPanelUI GetOrCreate()
    {
        CraftingResultPanelUI existing = FindObjectOfType<CraftingResultPanelUI>();

        if (existing != null)
            return existing;

        GameObject go = new GameObject("CraftingResultPanelUI");
        return go.AddComponent<CraftingResultPanelUI>();
    }

    public void ShowSuccess(string body)
    {
        Show("Creacion completada", body);
    }

    public void ShowFailure(string body)
    {
        Show("No se pudo crear", body);
    }

    public void Hide()
    {
        EnsureBuilt();

        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    private void Show(string title, string body)
    {
        EnsureBuilt();

        if (titleText != null)
            titleText.text = title;

        if (bodyText != null)
            bodyText.text = string.IsNullOrWhiteSpace(body) ? "-" : body;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, visibleSeconds));
        Hide();
        hideCoroutine = null;
    }

    private void EnsureBuilt()
    {
        if (panelRoot != null && titleText != null && bodyText != null && canvasGroup != null)
            return;

        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        else if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        panelRoot = CreatePanel(canvas.transform);
        canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        titleText = CreateText("Title", panelRoot.transform, "Crafting", 22, TextAlignmentOptions.Center);
        bodyText = CreateText("Body", panelRoot.transform, "", 18, TextAlignmentOptions.Center);
    }

    private GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("Panel_CraftingResult");
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.08f, 0.07f, 0.06f, 0.92f);
        image.raycastTarget = false;

        CanvasGroup group = panel.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -72f);
        rect.sizeDelta = new Vector2(460f, 128f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(22, 22, 14, 14);
        layout.spacing = 6f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        return panel;
    }

    private TMP_Text CreateText(string name, Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textGO = new GameObject("Text_" + name);
        textGO.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = new Color(0.95f, 0.9f, 0.78f, 1f);
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;

        LayoutElement layout = textGO.AddComponent<LayoutElement>();
        layout.minHeight = name == "Title" ? 30f : 52f;
        layout.flexibleWidth = 1f;

        return tmp;
    }
}
