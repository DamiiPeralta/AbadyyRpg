using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoVictoryPanelUI : MonoBehaviour
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

    public static DemoVictoryPanelUI GetOrCreate()
    {
        DemoVictoryPanelUI existing = FindObjectOfType<DemoVictoryPanelUI>();

        if (existing != null)
            return existing;

        GameObject go = new GameObject("DemoVictoryPanelUI");
        return go.AddComponent<DemoVictoryPanelUI>();
    }

    public void Show()
    {
        EnsureBuilt();
        EnsureEventSystem();

        if (GameSfxPlayer.Instance != null)
            GameSfxPlayer.Instance.PlayVictory();

        if (titleText != null)
            titleText.text = "Completaste la demo!";

        if (bodyText != null)
            bodyText.text = BuildRunSummary();

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

    private string BuildRunSummary()
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("La caravana alcanzo el paso fronterizo y dejo atras la amenaza del portal.");
        builder.AppendLine();
        AppendPartySummary(builder);
        AppendInventorySummary(builder);
        AppendCaravanSummary(builder);
        builder.AppendLine();
        builder.AppendLine("Completaste la demo! Muchas gracias por jugar.");

        return builder.ToString();
    }

    private void AppendPartySummary(StringBuilder builder)
    {
        PartyRuntimeState party = PartyRuntimeState.Instance;
        List<Unit> roster = party != null ? party.GetRoster() : null;

        builder.AppendLine("Expedicionarios:");

        if (roster == null || roster.Count == 0)
        {
            builder.AppendLine("- Sin expedicionarios registrados.");
            return;
        }

        foreach (Unit unit in roster)
        {
            if (unit == null)
                continue;

            builder.AppendLine($"- {unit.unitName}: nivel {unit.level}, XP {unit.experience}, HP {unit.currentHP}/{unit.maxHP}");
            builder.AppendLine($"  Equipo: {BuildEquipmentSummary(unit)}");
            builder.AppendLine($"  Tacticas: {BuildTacticsSummary(unit)}");
        }
    }

    private void AppendInventorySummary(StringBuilder builder)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        builder.AppendLine();
        builder.AppendLine("Inventario:");

        if (inventory == null)
        {
            builder.AppendLine("- Sin inventario registrado.");
            return;
        }

        builder.AppendLine($"- Oro: {inventory.gold}");
        builder.AppendLine($"- Recursos: comida {inventory.food}, madera {inventory.wood}, piedra {inventory.stone}, hierro {inventory.iron}, cuero {inventory.leather}, cristales {inventory.crystals}");

        if (inventory.items == null || inventory.items.Count == 0)
        {
            builder.AppendLine("- Items: ninguno.");
            return;
        }

        builder.AppendLine("- Items:");

        foreach (InventoryEntry entry in inventory.items)
        {
            if (entry == null || entry.amount <= 0)
                continue;

            builder.AppendLine($"  - {GetItemName(entry.itemId)} x{entry.amount:00}");
        }
    }

    private void AppendCaravanSummary(StringBuilder builder)
    {
        CaravanState caravan = CaravanState.Instance;

        if (caravan == null)
            return;

        builder.AppendLine();
        builder.AppendLine($"Caravana: dia {caravan.day}, hora {caravan.hour:00}:00, energia {caravan.caravanStamina}/{caravan.maxCaravanStamina}");
    }

    private string BuildEquipmentSummary(Unit unit)
    {
        if (unit.equipment == null || unit.equipment.Count == 0)
            return "sin equipo";

        List<string> parts = new List<string>();

        foreach (KeyValuePair<EquipmentSlot, EquipmentItem> pair in unit.equipment)
        {
            if (pair.Value != null)
                parts.Add($"{pair.Key}: {pair.Value.itemName}");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "sin equipo";
    }

    private string BuildTacticsSummary(Unit unit)
    {
        if (unit.tactics == null || unit.tactics.Count == 0)
            return "sin tacticas";

        List<string> parts = new List<string>();

        foreach (TacticRule tactic in unit.tactics)
        {
            if (tactic == null || !tactic.isActive)
                continue;

            string abilityName = tactic.ability != null ? tactic.ability.abilityName : "sin habilidad";
            parts.Add($"{FormatCondition(tactic)} -> {abilityName}");
        }

        return parts.Count > 0 ? string.Join("; ", parts) : "sin tacticas activas";
    }

    private string FormatCondition(TacticRule tactic)
    {
        switch (tactic.conditionType)
        {
            case TacticConditionType.Always:
                return "Siempre";
            case TacticConditionType.SelfHpBelowPercent:
                return $"Yo HP < {tactic.thresholdPercent}%";
            case TacticConditionType.AllyHpBelowPercent:
                return $"Aliado HP < {tactic.thresholdPercent}%";
            case TacticConditionType.EnemyHpBelowPercent:
                return $"Enemigo HP < {tactic.thresholdPercent}%";
            case TacticConditionType.SelfPhysicalArmorBelowPercent:
                return $"Yo armadura fisica < {tactic.thresholdPercent}%";
            case TacticConditionType.AllyPhysicalArmorBelowPercent:
                return $"Aliado armadura fisica < {tactic.thresholdPercent}%";
            case TacticConditionType.SelfStaminaAbovePercent:
                return $"Yo energia >= {tactic.thresholdPercent}%";
            default:
                return tactic.conditionType.ToString();
        }
    }

    private string GetItemName(string itemId)
    {
        if (ItemDatabase.Instance != null)
        {
            ItemBase item = ItemDatabase.Instance.GetItemById(itemId);

            if (item != null && !string.IsNullOrWhiteSpace(item.itemName))
                return item.itemName;
        }

        return string.IsNullOrWhiteSpace(itemId) ? "Item" : itemId;
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
        titleText = CreateText("Title", panelRoot.transform, "Completaste la demo!", 32, TextAlignmentOptions.Center);
        bodyText = CreateScrollableBody(panelRoot.transform);
        menuButton = CreateButton(panelRoot.transform);
    }

    private GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("Panel_DemoVictory");
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.05f, 0.08f, 0.06f, 0.96f);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(760f, 560f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        return panel;
    }

    private TMP_Text CreateScrollableBody(Transform parent)
    {
        GameObject scrollGO = new GameObject("Scroll_RunSummary");
        scrollGO.transform.SetParent(parent, false);

        Image scrollImage = scrollGO.AddComponent<Image>();
        scrollImage.color = new Color(0f, 0f, 0f, 0.18f);

        ScrollRect scrollRect = scrollGO.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        LayoutElement scrollLayout = scrollGO.AddComponent<LayoutElement>();
        scrollLayout.preferredHeight = 360f;
        scrollLayout.flexibleWidth = 1f;

        GameObject viewportGO = new GameObject("Viewport");
        viewportGO.transform.SetParent(scrollGO.transform, false);
        RectTransform viewportRect = viewportGO.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(12f, 8f);
        viewportRect.offsetMax = new Vector2(-12f, -8f);
        viewportGO.AddComponent<RectMask2D>();

        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(viewportGO.transform, false);
        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        ContentSizeFitter fitter = contentGO.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI tmp = contentGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "";
        tmp.fontSize = 19;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.color = new Color(0.92f, 0.90f, 0.80f, 1f);
        tmp.enableWordWrapping = true;

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;

        return tmp;
    }

    private TMP_Text CreateText(string name, Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textGO = new GameObject("Text_" + name);
        textGO.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = new Color(0.96f, 0.88f, 0.65f, 1f);
        tmp.enableWordWrapping = true;

        LayoutElement layout = textGO.AddComponent<LayoutElement>();
        layout.minHeight = 44f;
        layout.flexibleWidth = 1f;

        return tmp;
    }

    private Button CreateButton(Transform parent)
    {
        GameObject buttonGO = new GameObject("Button_ReturnToMenu");
        buttonGO.transform.SetParent(parent, false);

        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.28f, 0.44f, 0.22f, 1f);

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
