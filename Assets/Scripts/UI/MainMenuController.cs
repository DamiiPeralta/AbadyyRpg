using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    public string startSceneName = "WorldMapScene";

    [Header("Cover")]
    public Sprite coverSprite;

    [Header("Text")]
    public string gameTitle = "Abadyy RPG";
    [TextArea] public string aboutText = "Demo de RPG de caravana, exploracion y combate tactico.";

    [Header("New Game")]
    public bool resetRuntimeStateOnStart = true;

    private GameObject aboutPanel;

    private readonly Color buttonColor = new Color(0.18f, 0.13f, 0.09f, 0.92f);
    private readonly Color buttonHoverColor = new Color(0.34f, 0.24f, 0.14f, 0.96f);
    private readonly Color buttonPressedColor = new Color(0.52f, 0.36f, 0.18f, 1f);
    private readonly Color textColor = new Color(0.95f, 0.88f, 0.72f, 1f);

    private void Start()
    {
        GameSfxPlayer.GetOrCreate();
        BuildUI();
    }

    public void StartGame()
    {
        if (resetRuntimeStateOnStart)
            ResetPersistentRuntimeState();

        SceneManager.LoadScene(startSceneName);
    }

    public void ShowAbout()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(true);
    }

    public void HideAbout()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResetPersistentRuntimeState()
    {
        DestroySingleton(GameRunState.Instance);
        DestroySingleton(PartyRuntimeState.Instance);
        DestroySingleton(InventoryRuntimeState.Instance);
        DestroySingleton(CaravanState.Instance);
    }

    private void DestroySingleton(MonoBehaviour singleton)
    {
        if (singleton != null)
            Destroy(singleton.gameObject);
    }

    private void BuildUI()
    {
        Canvas canvas = CreateCanvas();
        CreateBackground(canvas.transform);
        CreateShade(canvas.transform);
        CreateMenu(canvas.transform);
        CreateAboutPanel(canvas.transform);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas_MainMenu");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return canvas;
    }

    private void CreateBackground(Transform parent)
    {
        GameObject go = CreateUIObject("Image_CoverBackground", parent);
        Image image = go.AddComponent<Image>();
        image.sprite = coverSprite;
        image.color = coverSprite != null ? Color.white : new Color(0.07f, 0.06f, 0.05f, 1f);
        image.raycastTarget = false;
        image.preserveAspect = false;
        Stretch(go.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    private void CreateShade(Transform parent)
    {
        GameObject go = CreateUIObject("Image_MenuShade", parent);
        Image image = go.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.48f);
        image.raycastTarget = false;
        Stretch(go.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    private void CreateMenu(Transform parent)
    {
        GameObject root = CreateUIObject("Panel_Menu", parent);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.06f, 0.14f);
        rect.anchorMax = new Vector2(0.42f, 0.88f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = root.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.LowerLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        TMP_Text title = CreateText("Text_Title", root.transform, gameTitle, 64, FontStyles.Bold, TextAlignmentOptions.Left);
        title.gameObject.AddComponent<LayoutElement>().preferredHeight = 180f;

        CreateButton("Empezar", root.transform, StartGame);
        CreateButton("About", root.transform, ShowAbout);
        CreateButton("Salir", root.transform, ExitGame);
    }

    private void CreateAboutPanel(Transform parent)
    {
        aboutPanel = CreateUIObject("Panel_About", parent);
        aboutPanel.SetActive(false);

        Image image = aboutPanel.AddComponent<Image>();
        image.color = new Color(0.08f, 0.07f, 0.06f, 0.94f);

        RectTransform rect = aboutPanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(640f, 360f);

        VerticalLayoutGroup layout = aboutPanel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;

        TMP_Text title = CreateText("Text_AboutTitle", aboutPanel.transform, "About", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        title.gameObject.AddComponent<LayoutElement>().preferredHeight = 52f;

        TMP_Text body = CreateText("Text_AboutBody", aboutPanel.transform, aboutText, 22, FontStyles.Normal, TextAlignmentOptions.Center);
        body.gameObject.AddComponent<LayoutElement>().preferredHeight = 150f;

        CreateButton("Cerrar", aboutPanel.transform, HideAbout, 220f, 48f);
    }

    private Button CreateButton(string label, Transform parent, UnityEngine.Events.UnityAction action, float width = 320f, float height = 56f)
    {
        GameObject go = CreateUIObject("Button_" + label, parent);
        Image image = go.AddComponent<Image>();
        image.color = buttonColor;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = buttonPressedColor;
        colors.selectedColor = buttonHoverColor;
        colors.colorMultiplier = 1f;
        button.colors = colors;

        LayoutElement layout = go.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        layout.preferredHeight = height;

        TMP_Text text = CreateText("Text_" + label, go.transform, label, 24, FontStyles.Bold, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        return button;
    }

    private TMP_Text CreateText(string name, Transform parent, string value, int size, FontStyles style, TextAlignmentOptions alignment)
    {
        GameObject go = CreateUIObject(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = textColor;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        return text;
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
    }
}
