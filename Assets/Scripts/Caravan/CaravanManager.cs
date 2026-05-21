using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CaravanManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject firePanel;
    public GameObject rosterPanel;
    public GameObject suppliesPanel;
    public GameObject artisansPanel;

    [Header("Texts")]
    public TMP_Text caravanText;
    public TMP_Text suppliesText;
    public TMP_Text rosterText;
    public TMP_Text messageText;

    [Header("Rest Settings")]
    [Range(0f, 1f)]
    public float restHealPercent = 0.20f;

    [Header("Generated UI")]
    public bool buildGeneratedUI = true;

    private TMP_Text actionTitleText;
    private TMP_Text actionBodyText;
    private GameObject generatedRoot;
    private string currentSection = "Suministros";

    private readonly Color backgroundColor = new Color(0.09f, 0.08f, 0.07f, 1f);
    private readonly Color cardColor = new Color(0.17f, 0.14f, 0.11f, 0.94f);
    private readonly Color cardAccentColor = new Color(0.54f, 0.35f, 0.16f, 1f);
    private readonly Color buttonColor = new Color(0.29f, 0.21f, 0.14f, 1f);
    private readonly Color buttonHoverColor = new Color(0.42f, 0.29f, 0.17f, 1f);
    private readonly Color textColor = new Color(0.93f, 0.88f, 0.78f, 1f);
    private readonly Color mutedTextColor = new Color(0.72f, 0.66f, 0.57f, 1f);

    private void Start()
    {
        if (buildGeneratedUI)
            BuildGeneratedUI();

        ShowSuppliesPanel();
        RefreshAllUI();
    }

    public void ShowFirePanel()
    {
        currentSection = "Hoguera";
        SetPanelState(firePanel, true);
        SetPanelState(rosterPanel, false);
        SetPanelState(suppliesPanel, false);
        SetPanelState(artisansPanel, false);
        RefreshActionPanel();
        RefreshAllUI();
    }

    public void ShowRosterPanel()
    {
        currentSection = "Barracas";
        SetPanelState(firePanel, false);
        SetPanelState(rosterPanel, true);
        SetPanelState(suppliesPanel, false);
        SetPanelState(artisansPanel, false);
        RefreshActionPanel();
        RefreshAllUI();
    }

    public void ShowSuppliesPanel()
    {
        currentSection = "Suministros";
        SetPanelState(firePanel, false);
        SetPanelState(rosterPanel, false);
        SetPanelState(suppliesPanel, true);
        SetPanelState(artisansPanel, false);
        RefreshActionPanel();
        RefreshAllUI();
    }

    public void ShowArtisansPanel()
    {
        currentSection = "Artesanos";
        SetPanelState(firePanel, false);
        SetPanelState(rosterPanel, false);
        SetPanelState(suppliesPanel, false);
        SetPanelState(artisansPanel, true);
        RefreshActionPanel();
        RefreshAllUI();
    }

    public void HideAllPanels()
    {
        SetPanelState(firePanel, false);
        SetPanelState(rosterPanel, false);
        SetPanelState(suppliesPanel, false);
        SetPanelState(artisansPanel, false);
    }

    private void SetPanelState(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    public void RefreshAllUI()
    {
        RefreshCaravanUI();
        RefreshSuppliesUI();
        RefreshRosterUI();
        RefreshActionPanel();
    }

    public void RefreshCaravanUI()
    {
        if (caravanText == null)
            return;

        if (CaravanState.Instance == null)
        {
            caravanText.text = "CaravanState no encontrado.";
            return;
        }

        CaravanState caravan = CaravanState.Instance;

        caravanText.text =
            $"Dia {caravan.day}\n" +
            $"Moral: {caravan.morale}/100\n" +
            $"Stamina de viaje: {caravan.caravanStamina}/{caravan.maxCaravanStamina}\n" +
            $"Carga: {caravan.currentCarry}/{caravan.maxCarry}";
    }

    public void RefreshSuppliesUI()
    {
        if (suppliesText == null)
            return;

        if (InventoryRuntimeState.Instance == null)
        {
            suppliesText.text = "InventoryRuntimeState no encontrado.";
            return;
        }

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        StringBuilder builder = new StringBuilder();

        builder.AppendLine($"Oro        {inventory.gold}");
        builder.AppendLine($"Comida     {inventory.food}");
        builder.AppendLine($"Madera     {inventory.wood}");
        builder.AppendLine($"Piedra     {inventory.stone}");
        builder.AppendLine($"Hierro     {inventory.iron}");
        builder.AppendLine($"Cuero      {inventory.leather}");
        builder.AppendLine($"Cristales  {inventory.crystals}");
        builder.AppendLine();
        builder.AppendLine("Items");

        if (inventory.items == null || inventory.items.Count == 0)
        {
            builder.AppendLine("Sin items en la caravana.");
        }
        else
        {
            foreach (InventoryEntry entry in inventory.items)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.itemId) || entry.amount <= 0)
                    continue;

                string displayName = entry.itemId;

                if (ItemDatabase.Instance != null)
                {
                    ItemBase item = ItemDatabase.Instance.GetItemById(entry.itemId);

                    if (item != null && !string.IsNullOrWhiteSpace(item.itemName))
                        displayName = item.itemName;
                }

                builder.AppendLine($"{displayName} x{entry.amount}");
            }
        }

        suppliesText.text = builder.ToString();
    }

    public void RefreshRosterUI()
    {
        if (rosterText == null)
            return;

        if (PartyRuntimeState.Instance == null || !PartyRuntimeState.Instance.HasParty())
        {
            rosterText.text = "No hay party runtime todavia.";
            return;
        }

        StringBuilder builder = new StringBuilder();

        foreach (Unit unit in PartyRuntimeState.Instance.GetCurrentParty())
        {
            if (unit == null)
                continue;

            EquipmentItem weapon = unit.GetEquippedItem(EquipmentSlot.RightHand);
            EquipmentItem armor = unit.GetEquippedItem(EquipmentSlot.Chest);

            builder.AppendLine(unit.unitName.ToUpperInvariant());
            builder.AppendLine($"Nivel {unit.level}  XP {unit.experience}  {(unit.isAlive ? "Vivo" : "Muerto")}");
            builder.AppendLine($"HP {unit.currentHP}/{unit.maxHP}  STA {unit.currentStamina}/{unit.maxStamina}  MANA {unit.currentMana}/{unit.maxMana}");
            builder.AppendLine($"Armadura F {unit.currentPhysicalArmor}/{unit.maxPhysicalArmor}  M {unit.currentMagicalArmor}/{unit.maxMagicalArmor}");
            builder.AppendLine($"Arma: {(weapon != null ? weapon.itemName : "Sin arma")}");
            builder.AppendLine($"Armadura: {(armor != null ? armor.itemName : "Sin armadura")}");
            builder.AppendLine();
        }

        rosterText.text = builder.ToString();
    }

    public void Rest()
    {
        if (InventoryRuntimeState.Instance == null)
        {
            SetMessage("No existe InventoryRuntimeState.");
            return;
        }

        if (CaravanState.Instance == null)
        {
            SetMessage("No existe CaravanState.");
            return;
        }

        if (PartyRuntimeState.Instance == null || !PartyRuntimeState.Instance.HasParty())
        {
            SetMessage("No hay party para descansar.");
            return;
        }

        int foodCost = PartyRuntimeState.Instance.GetLivingMembersCount();

        if (foodCost <= 0)
        {
            SetMessage("No hay miembros vivos para descansar.");
            return;
        }

        bool paidFood = InventoryRuntimeState.Instance.SpendFood(foodCost);

        if (!paidFood)
        {
            SetMessage($"No hay comida suficiente. Necesitas {foodCost}.");
            return;
        }

        PartyRuntimeState.Instance.HealAllLivingPercent(restHealPercent);
        PartyRuntimeState.Instance.RestoreAllLivingStamina();
        PartyRuntimeState.Instance.RestoreAllLivingMana();
        CaravanState.Instance.RestoreStamina();
        CaravanState.Instance.AdvanceDay();

        SetMessage($"Descanso completo. Comida consumida: {foodCost}.");

        RefreshAllUI();
    }

    public void ReturnToWorldMap()
    {
        SceneManager.LoadScene("WorldMapScene");
    }

    private void BuildGeneratedUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning("CaravanManager: no se encontro Canvas para construir UI.");
            return;
        }

        DisableExistingCanvasChildren(canvas);

        generatedRoot = CreateUIObject("GeneratedCaravanUI", canvas.transform);
        Stretch(generatedRoot.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image rootImage = generatedRoot.AddComponent<Image>();
        rootImage.color = backgroundColor;

        GameObject topBar = CreatePanel("TopBar", generatedRoot.transform, new Color(0.12f, 0.10f, 0.08f, 1f));
        Stretch(topBar.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -82f), Vector2.zero);
        HorizontalLayoutGroup topLayout = topBar.AddComponent<HorizontalLayoutGroup>();
        topLayout.padding = new RectOffset(28, 28, 16, 16);
        topLayout.spacing = 14;
        topLayout.childControlWidth = false;
        topLayout.childControlHeight = true;
        topLayout.childForceExpandWidth = false;

        CreateTitleText("Caravana", topBar.transform, 30, 260);
        CreateButton("Hoguera", topBar.transform, ShowFirePanel);
        CreateButton("Barracas", topBar.transform, ShowRosterPanel);
        CreateButton("Suministros", topBar.transform, ShowSuppliesPanel);
        CreateButton("Artesanos", topBar.transform, ShowArtisansPanel);
        CreateButton("Descansar", topBar.transform, Rest);
        CreateButton("Volver al mapa", topBar.transform, ReturnToWorldMap, 210);

        GameObject content = CreateUIObject("Content", generatedRoot.transform);
        Stretch(content.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(28f, 28f), new Vector2(-28f, -104f));

        GameObject rosterCard = CreateCard("Barracas", content.transform, new Vector2(0f, 0f), new Vector2(0.46f, 1f), new Vector2(0f, 0f), new Vector2(-14f, 0f));
        rosterPanel = rosterCard;
        rosterText = CreateBodyText("RosterText", rosterCard.transform, 20);

        GameObject caravanCard = CreateCard("Estado de la caravana", content.transform, new Vector2(0.48f, 0.52f), new Vector2(1f, 1f), new Vector2(14f, 0f), Vector2.zero);
        caravanText = CreateBodyText("CaravanText", caravanCard.transform, 24);

        GameObject suppliesCard = CreateCard("Suministros", content.transform, new Vector2(0.48f, 0f), new Vector2(0.72f, 0.5f), new Vector2(14f, 0f), new Vector2(-10f, 0f));
        suppliesPanel = suppliesCard;
        suppliesText = CreateBodyText("SuppliesText", suppliesCard.transform, 22);

        GameObject artisansCard = CreateCard("Artesanos", content.transform, new Vector2(0.48f, 0f), new Vector2(0.72f, 0.5f), new Vector2(14f, 0f), new Vector2(-10f, 0f));
        artisansPanel = artisansCard;

        GameObject actionCard = CreateCard("Acciones", content.transform, new Vector2(0.735f, 0f), new Vector2(1f, 0.5f), new Vector2(10f, 0f), Vector2.zero);
        firePanel = actionCard;
        actionTitleText = CreateText("ActionTitle", actionCard.transform, "", 24, FontStyles.Bold, textColor, TextAlignmentOptions.TopLeft);
        Stretch(actionTitleText.rectTransform, Vector2.zero, Vector2.one, new Vector2(22f, 102f), new Vector2(-22f, -68f));
        actionBodyText = CreateText("ActionBody", actionCard.transform, "", 18, FontStyles.Normal, mutedTextColor, TextAlignmentOptions.TopLeft);
        Stretch(actionBodyText.rectTransform, Vector2.zero, Vector2.one, new Vector2(22f, 22f), new Vector2(-22f, -122f));

        GameObject messagePanel = CreatePanel("MessagePanel", generatedRoot.transform, new Color(0.11f, 0.09f, 0.07f, 0.96f));
        Stretch(messagePanel.GetComponent<RectTransform>(), new Vector2(0.48f, 0f), new Vector2(1f, 0f), new Vector2(42f, 28f), new Vector2(-28f, 84f));
        messageText = CreateText("MessageText", messagePanel.transform, "La caravana esta lista.", 18, FontStyles.Normal, textColor, TextAlignmentOptions.MidlineLeft);
        Stretch(messageText.rectTransform, Vector2.zero, Vector2.one, new Vector2(18f, 8f), new Vector2(-18f, -8f));
    }

    private void DisableExistingCanvasChildren(Canvas canvas)
    {
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            Transform child = canvas.transform.GetChild(i);

            if (child != null)
                child.gameObject.SetActive(false);
        }
    }

    private GameObject CreateCard(string title, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject card = CreatePanel(title + "Card", parent, cardColor);
        Stretch(card.GetComponent<RectTransform>(), anchorMin, anchorMax, offsetMin, offsetMax);

        GameObject accent = CreatePanel(title + "Accent", card.transform, cardAccentColor);
        Stretch(accent.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, new Vector2(0f, -4f));

        TMP_Text header = CreateText(title + "Header", card.transform, title, 24, FontStyles.Bold, textColor, TextAlignmentOptions.TopLeft);
        Stretch(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(22f, -62f), new Vector2(-22f, -18f));

        return card;
    }

    private TMP_Text CreateBodyText(string name, Transform parent, int size)
    {
        TMP_Text text = CreateText(name, parent, "", size, FontStyles.Normal, textColor, TextAlignmentOptions.TopLeft);
        Stretch(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(22f, 20f), new Vector2(-22f, -74f));
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Ellipsis;
        return text;
    }

    private TMP_Text CreateTitleText(string text, Transform parent, int size, float width)
    {
        TMP_Text title = CreateText(text + "Title", parent, text, size, FontStyles.Bold, textColor, TextAlignmentOptions.MidlineLeft);
        LayoutElement layout = title.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        layout.minWidth = width;
        return title;
    }

    private void CreateButton(string label, Transform parent, UnityEngine.Events.UnityAction action, float width = 170f)
    {
        GameObject go = CreatePanel(label + "Button", parent, buttonColor);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = go.GetComponent<Image>();
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = cardAccentColor;
        colors.selectedColor = buttonHoverColor;
        colors.colorMultiplier = 1f;
        button.colors = colors;

        LayoutElement layout = go.AddComponent<LayoutElement>();
        layout.preferredWidth = width;
        layout.minWidth = width;

        TMP_Text text = CreateText(label + "Label", go.transform, label, 18, FontStyles.Bold, textColor, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = CreateUIObject(name, parent);
        Image image = go.AddComponent<Image>();
        image.color = color;
        return go;
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private TMP_Text CreateText(string name, Transform parent, string value, int size, FontStyles style, Color color, TextAlignmentOptions alignment)
    {
        GameObject go = CreateUIObject(name, parent);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        return text;
    }

    private void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
    }

    private void RefreshActionPanel()
    {
        if (actionTitleText == null || actionBodyText == null)
            return;

        actionTitleText.text = currentSection;

        if (currentSection == "Hoguera")
        {
            int foodCost = PartyRuntimeState.Instance != null ? PartyRuntimeState.Instance.GetLivingMembersCount() : 0;
            int healPercent = Mathf.RoundToInt(restHealPercent * 100f);

            actionBodyText.text =
                $"Descansar consume {foodCost} de comida.\n" +
                $"Restaura {healPercent}% de HP, toda la stamina individual y la stamina de viaje.\n\n" +
                "Usa el boton Descansar cuando quieras cerrar el dia.";
        }
        else if (currentSection == "Barracas")
        {
            actionBodyText.text =
                "Estado de la compania, vida actual, stamina, armadura y equipo principal.\n\n" +
                "Esta lista lee directamente de PartyRuntimeState.";
        }
        else if (currentSection == "Artesanos")
        {
            actionBodyText.text =
                "Procesa materiales de la caravana y transforma recursos en equipo o suministros.\n\n" +
                "Las recetas disponibles dependen del artesano seleccionado.";
        }
        else
        {
            actionBodyText.text =
                "Recursos disponibles para viaje, descanso y mejoras futuras.\n\n" +
                "Los items vienen de InventoryRuntimeState.";
        }
    }

    private void SetMessage(string message)
    {
        Debug.Log(message);

        if (messageText != null)
            messageText.text = message;
    }
}
