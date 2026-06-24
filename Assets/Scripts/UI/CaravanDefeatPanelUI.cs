using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaravanDefeatPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "MainMenuScene";

    private void Awake()
    {
        EnsureBuilt();
        Hide();
    }

    public static CaravanDefeatPanelUI GetOrCreate()
    {
        CaravanDefeatPanelUI existing = FindObjectOfType<CaravanDefeatPanelUI>();

        if (existing != null)
            return existing;

        GameObject go = new GameObject("CaravanDefeatPanelUI");
        return go.AddComponent<CaravanDefeatPanelUI>();
    }

    public void Show(string reason)
    {
        EnsureBuilt();
        EnsureEventSystem();

        if (GameSfxPlayer.Instance != null)
            GameSfxPlayer.Instance.PlayDefeat();

        if (titleText != null)
            titleText.text = "La caravana fue derrotada";

        if (bodyText != null)
            bodyText.text = string.IsNullOrWhiteSpace(reason)
                ? "La expedicion termino."
                : reason;

        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(ReturnToMainMenu);
            menuButton.onClick.AddListener(ReturnToMainMenu);
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Hide()
    {
        EnsureBuilt();

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    private void EnsureBuilt()
    {
        if (panelRoot != null && titleText != null && bodyText != null && menuButton != null)
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
        titleText = CreateText("Title", panelRoot.transform, "La caravana fue derrotada", 32, TextAlignmentOptions.Center);
        bodyText = CreateText("Body", panelRoot.transform, "", 22, TextAlignmentOptions.Center);
        menuButton = CreateButton(panelRoot.transform);
    }

    private GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("Panel_CaravanDefeat");
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.07f, 0.04f, 0.04f, 0.95f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(620f, 320f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 18f;
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
        tmp.color = new Color(0.96f, 0.86f, 0.74f, 1f);
        tmp.enableWordWrapping = true;

        LayoutElement layout = textGO.AddComponent<LayoutElement>();
        layout.minHeight = name == "Title" ? 48f : 120f;
        layout.flexibleWidth = 1f;

        return tmp;
    }

    private Button CreateButton(Transform parent)
    {
        GameObject buttonGO = new GameObject("Button_ReturnToMenu");
        buttonGO.transform.SetParent(parent, false);

        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.48f, 0.22f, 0.15f, 1f);

        Button button = buttonGO.AddComponent<Button>();

        LayoutElement layout = buttonGO.AddComponent<LayoutElement>();
        layout.preferredWidth = 240f;
        layout.preferredHeight = 48f;

        TMP_Text label = CreateText("ReturnToMenu", buttonGO.transform, "Volver al menu", 20, TextAlignmentOptions.Center);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        LayoutElement labelLayout = label.GetComponent<LayoutElement>();
        if (labelLayout != null)
            Destroy(labelLayout);

        return button;
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
            return;

        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();
    }
}
