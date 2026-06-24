using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapHudUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statusText;
    public Button searchResourcesButton;
    public TMP_Text searchResourcesButtonText;
    public TMP_Text searchResourcesInfoText;

    private Action onSearchResources;

    private void Awake()
    {
        EnsureResourceSearchUI();
    }

    public void BindSearchResources(Action onSearchResources)
    {
        this.onSearchResources = onSearchResources;
        HookSearchResourcesButton();
    }

    public void Refresh(WorldMapNode currentNode)
    {
        EnsureResourceSearchUI();

        if (statusText == null)
            return;

        CaravanState caravan = CaravanState.Instance;
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        int day = caravan != null ? caravan.day : 1;
        int hour = caravan != null ? caravan.hour : 8;
        int stamina = caravan != null ? caravan.caravanStamina : 0;
        int maxStamina = caravan != null ? caravan.maxCaravanStamina : 0;
        int gold = inventory != null ? inventory.gold : 0;
        string location = currentNode != null ? currentNode.nodeName : "Sin ubicacion";

        string result = GameRunState.Instance != null && GameRunState.Instance.expeditionFailed
            ? " | EXPEDICION FRACASADA"
            : "";

        statusText.text = $"Dia {day} | Hora {hour:00} | Stamina {stamina}/{maxStamina} | Oro {gold} | {location}{result}";

        RefreshSearchResourcesUI(currentNode);
    }

    private void RefreshSearchResourcesUI(WorldMapNode currentNode)
    {
        bool canSearch = currentNode != null &&
                         currentNode.allowsResourceSearch &&
                         currentNode.resourceSearchDrops != null &&
                         currentNode.resourceSearchDrops.Count > 0;

        if (searchResourcesButton != null)
        {
            searchResourcesButton.gameObject.SetActive(canSearch);
            searchResourcesButton.interactable = canSearch;
        }

        string resourcesText = CaravanResourceSearchUtility.BuildPossibleResourcesText(currentNode);

        if (searchResourcesButtonText != null)
            searchResourcesButtonText.text = string.IsNullOrWhiteSpace(resourcesText)
                ? "Buscar recursos"
                : $"Buscar: {resourcesText}";

        if (searchResourcesInfoText != null)
        {
            searchResourcesInfoText.gameObject.SetActive(canSearch);
            searchResourcesInfoText.text = canSearch
                ? $"Posibles recursos: {resourcesText}"
                : string.Empty;
        }
    }

    private void EnsureResourceSearchUI()
    {
        if (searchResourcesButton != null && searchResourcesButtonText != null)
            return;

        Transform existing = FindChildTransform("Button_SearchResources");

        if (existing != null)
        {
            searchResourcesButton = existing.GetComponent<Button>();
            searchResourcesButtonText = existing.GetComponentInChildren<TMP_Text>(true);
            HookSearchResourcesButton();
            return;
        }

        GameObject buttonGO = new GameObject("Button_SearchResources");
        buttonGO.transform.SetParent(transform, false);

        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.25f, 0.20f, 0.12f, 0.94f);

        searchResourcesButton = buttonGO.AddComponent<Button>();

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-24f, 54f);
        rect.sizeDelta = new Vector2(260f, 42f);

        GameObject textGO = new GameObject("Text_SearchResources");
        textGO.transform.SetParent(buttonGO.transform, false);

        searchResourcesButtonText = textGO.AddComponent<TextMeshProUGUI>();
        searchResourcesButtonText.text = "Buscar recursos";
        searchResourcesButtonText.fontSize = 17;
        searchResourcesButtonText.alignment = TextAlignmentOptions.Center;
        searchResourcesButtonText.color = new Color(0.95f, 0.90f, 0.78f, 1f);
        searchResourcesButtonText.enableWordWrapping = false;
        searchResourcesButtonText.overflowMode = TextOverflowModes.Ellipsis;

        RectTransform textRect = searchResourcesButtonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10f, 4f);
        textRect.offsetMax = new Vector2(-10f, -4f);

        if (searchResourcesInfoText == null)
        {
            GameObject infoGO = new GameObject("Text_SearchResourcesInfo");
            infoGO.transform.SetParent(transform, false);

            searchResourcesInfoText = infoGO.AddComponent<TextMeshProUGUI>();
            searchResourcesInfoText.fontSize = 14;
            searchResourcesInfoText.alignment = TextAlignmentOptions.Right;
            searchResourcesInfoText.color = new Color(0.86f, 0.80f, 0.68f, 1f);
            searchResourcesInfoText.enableWordWrapping = false;
            searchResourcesInfoText.overflowMode = TextOverflowModes.Ellipsis;

            RectTransform infoRect = searchResourcesInfoText.GetComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(1f, 0f);
            infoRect.anchorMax = new Vector2(1f, 0f);
            infoRect.pivot = new Vector2(1f, 0f);
            infoRect.anchoredPosition = new Vector2(-24f, 28f);
            infoRect.sizeDelta = new Vector2(320f, 24f);
        }

        HookSearchResourcesButton();
    }

    private void HookSearchResourcesButton()
    {
        if (searchResourcesButton == null)
            return;

        searchResourcesButton.onClick.RemoveListener(HandleSearchResourcesClicked);
        searchResourcesButton.onClick.AddListener(HandleSearchResourcesClicked);
    }

    private void HandleSearchResourcesClicked()
    {
        onSearchResources?.Invoke();
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
