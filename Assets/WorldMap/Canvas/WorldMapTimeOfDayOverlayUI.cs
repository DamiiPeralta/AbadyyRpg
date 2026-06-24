using UnityEngine;
using UnityEngine.UI;

public class WorldMapTimeOfDayOverlayUI : MonoBehaviour
{
    [Header("Overlay Colors")]
    public Color dayColor = new Color(1f, 1f, 1f, 0f);
    public Color dawnColor = new Color(0.95f, 0.55f, 0.28f, 0.14f);
    public Color duskColor = new Color(0.95f, 0.42f, 0.18f, 0.16f);
    public Color nightColor = new Color(0.03f, 0.08f, 0.22f, 0.34f);

    [Header("Hours")]
    public int dawnStartHour = 5;
    public int dayStartHour = 7;
    public int duskStartHour = 18;
    public int nightStartHour = 21;

    [Header("Smoothing")]
    public float smoothSeconds = 0.35f;

    private Image overlayImage;
    private Color targetColor;
    private float lastRefreshTime;

    private void Awake()
    {
        EnsureBuilt();
        RefreshImmediate();
    }

    private void Update()
    {
        if (Time.unscaledTime - lastRefreshTime > 0.5f)
            Refresh();

        if (overlayImage == null)
            return;

        float lerpSpeed = smoothSeconds <= 0f ? 1f : Time.unscaledDeltaTime / smoothSeconds;
        overlayImage.color = Color.Lerp(overlayImage.color, targetColor, Mathf.Clamp01(lerpSpeed));
    }

    public static WorldMapTimeOfDayOverlayUI GetOrCreate()
    {
        WorldMapTimeOfDayOverlayUI existing = FindObjectOfType<WorldMapTimeOfDayOverlayUI>();

        if (existing != null)
            return existing;

        GameObject go = new GameObject("WorldMapTimeOfDayOverlayUI");
        return go.AddComponent<WorldMapTimeOfDayOverlayUI>();
    }

    public void Refresh()
    {
        EnsureBuilt();
        targetColor = GetColorForCurrentHour();
        lastRefreshTime = Time.unscaledTime;
    }

    public void RefreshImmediate()
    {
        Refresh();

        if (overlayImage != null)
            overlayImage.color = targetColor;
    }

    private Color GetColorForCurrentHour()
    {
        int hour = CaravanState.Instance != null ? CaravanState.Instance.hour : 8;
        hour = ((hour % 24) + 24) % 24;

        if (hour >= dawnStartHour && hour < dayStartHour)
            return dawnColor;

        if (hour >= dayStartHour && hour < duskStartHour)
            return dayColor;

        if (hour >= duskStartHour && hour < nightStartHour)
            return duskColor;

        return nightColor;
    }

    private void EnsureBuilt()
    {
        if (overlayImage != null)
            return;

        GameObject canvasGO = new GameObject("WorldMapTimeOfDayCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 25000;

        GameObject imageGO = new GameObject("Image_TimeOfDayTint");
        imageGO.transform.SetParent(canvasGO.transform, false);

        overlayImage = imageGO.AddComponent<Image>();
        overlayImage.color = dayColor;
        overlayImage.raycastTarget = false;

        RectTransform rect = imageGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
