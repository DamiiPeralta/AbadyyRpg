using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeUI : MonoBehaviour
{
    public static ScreenFadeUI Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float sleepFadeOutSeconds = 0.6f;
    [SerializeField] private float sleepHoldSeconds = 0.25f;
    [SerializeField] private float sleepFadeInSeconds = 0.8f;

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureBuilt();
    }

    public static ScreenFadeUI GetOrCreate()
    {
        if (Instance != null)
            return Instance;

        ScreenFadeUI existing = FindObjectOfType<ScreenFadeUI>();
        if (existing != null)
            return existing;

        GameObject go = new GameObject("ScreenFadeUI");
        return go.AddComponent<ScreenFadeUI>();
    }

    public void PlaySleepTransition(Action onBlackScreen)
    {
        EnsureBuilt();

        if (GameSfxPlayer.Instance != null)
            GameSfxPlayer.Instance.PlaySleep();

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(SleepTransitionCoroutine(onBlackScreen));
    }

    private IEnumerator SleepTransitionCoroutine(Action onBlackScreen)
    {
        SetBlocking(true);
        yield return FadeTo(1f, sleepFadeOutSeconds);

        onBlackScreen?.Invoke();

        if (sleepHoldSeconds > 0f)
            yield return new WaitForSecondsRealtime(sleepHoldSeconds);

        yield return FadeTo(0f, sleepFadeInSeconds);
        SetBlocking(false);
        transitionCoroutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float seconds)
    {
        EnsureBuilt();

        float startAlpha = fadeGroup.alpha;
        float duration = Mathf.Max(0.01f, seconds);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        fadeGroup.alpha = targetAlpha;
    }

    private void SetBlocking(bool value)
    {
        EnsureBuilt();
        fadeGroup.blocksRaycasts = value;
        fadeGroup.interactable = value;
    }

    private void EnsureBuilt()
    {
        if (fadeGroup != null)
            return;

        GameObject canvasGO = new GameObject("ScreenFadeCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imageGO = new GameObject("Image_FadeBlack");
        imageGO.transform.SetParent(canvasGO.transform, false);

        Image image = imageGO.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rect = imageGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeGroup = imageGO.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
        fadeGroup.interactable = false;
    }
}
