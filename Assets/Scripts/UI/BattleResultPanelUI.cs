using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleResultPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button continueButton;
    [SerializeField] private string fallbackReturnSceneName = "WorldMapScene";

    private string returnSceneName;

    private void Awake()
    {
        EnsureBuilt();
        Hide();
    }

    public static BattleResultPanelUI GetOrCreate()
    {
        BattleResultPanelUI existing = FindObjectOfType<BattleResultPanelUI>();

        if (existing != null)
            return existing;

        GameObject go = new GameObject("BattleResultPanelUI");
        return go.AddComponent<BattleResultPanelUI>();
    }

    public void ShowVictory(string rewardText, string sceneName)
    {
        Show(
            "Victoria",
            string.IsNullOrWhiteSpace(rewardText) ? "Combate completado." : rewardText,
            sceneName
        );
    }

    public void ShowDefeat(string sceneName)
    {
        Show(
            "Derrota",
            "El combate termino. La caravana vuelve al mapa.",
            sceneName
        );
    }

    public void Hide()
    {
        EnsureBuilt();

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void Show(string title, string body, string sceneName)
    {
        EnsureBuilt();
        EnsureEventSystem();

        returnSceneName = string.IsNullOrWhiteSpace(sceneName)
            ? fallbackReturnSceneName
            : sceneName;

        if (titleText != null)
            titleText.text = title;

        if (bodyText != null)
            bodyText.text = body;

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(ContinueToWorldMap);
            continueButton.onClick.AddListener(ContinueToWorldMap);
        }

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    private void ContinueToWorldMap()
    {
        string sceneToLoad = string.IsNullOrWhiteSpace(returnSceneName)
            ? fallbackReturnSceneName
            : returnSceneName;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void EnsureBuilt()
    {
        if (panelRoot != null && titleText != null && bodyText != null && continueButton != null)
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
        titleText = CreateText("Title", panelRoot.transform, "Fin del combate", 32, TextAlignmentOptions.Center);
        bodyText = CreateText("Body", panelRoot.transform, "", 22, TextAlignmentOptions.Center);
        continueButton = CreateButton(panelRoot.transform);
    }

    private GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("Panel_BattleResult");
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.08f, 0.07f, 0.06f, 0.92f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(560f, 340f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(28, 28, 24, 24);
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
        tmp.color = new Color(0.95f, 0.9f, 0.78f, 1f);
        tmp.enableWordWrapping = true;

        LayoutElement layout = textGO.AddComponent<LayoutElement>();
        layout.minHeight = name == "Title" ? 46f : 140f;
        layout.flexibleWidth = 1f;

        return tmp;
    }

    private Button CreateButton(Transform parent)
    {
        GameObject buttonGO = new GameObject("Button_Continue");
        buttonGO.transform.SetParent(parent, false);

        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.55f, 0.42f, 0.22f, 1f);

        Button button = buttonGO.AddComponent<Button>();

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(220f, 48f);

        LayoutElement layout = buttonGO.AddComponent<LayoutElement>();
        layout.preferredWidth = 220f;
        layout.preferredHeight = 48f;

        TMP_Text label = CreateText("Continue", buttonGO.transform, "Continuar", 20, TextAlignmentOptions.Center);
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
