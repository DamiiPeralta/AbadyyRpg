using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SuppliesScrollViewSetup : MonoBehaviour
{
    public RectTransform viewport;
    public RectTransform content;
    public ScrollRect scrollRect;

    [Header("Layout")]
    public float spacing = 6f;
    public int paddingLeft = 10;
    public int paddingRight = 10;
    public int paddingTop = 10;
    public int paddingBottom = 8;

    [ContextMenu("Setup Supplies Scroll View")]
    public void Setup()
    {
        RectTransform root = GetComponent<RectTransform>();

        if (root == null)
            return;

        scrollRect = GetOrAdd<ScrollRect>(gameObject);
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.scrollSensitivity = 24f;

        RemoveRootLayoutComponents();

        viewport = EnsureChildRect("Viewport", root);
        Stretch(viewport, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image viewportImage = GetOrAdd<Image>(viewport.gameObject);
        viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
        viewportImage.raycastTarget = true;

        RectMask2D mask = GetOrAdd<RectMask2D>(viewport.gameObject);
        mask.padding = Vector4.zero;
        mask.softness = Vector2Int.zero;

        content = EnsureChildRect("Content", viewport);
        Stretch(content, new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        content.pivot = new Vector2(0.5f, 1f);

        VerticalLayoutGroup layout = GetOrAdd<VerticalLayoutGroup>(content.gameObject);
        layout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = GetOrAdd<ContentSizeFitter>(content.gameObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        MoveExistingRowsIntoContent(root);

        scrollRect.viewport = viewport;
        scrollRect.content = content;
    }

    private void MoveExistingRowsIntoContent(RectTransform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform child = root.GetChild(i);

            if (child == viewport)
                continue;

            child.SetParent(content, false);
        }
    }

    private void RemoveRootLayoutComponents()
    {
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();

        if (gridLayout != null)
            DestroyImmediate(gridLayout);

        VerticalLayoutGroup verticalLayout = GetComponent<VerticalLayoutGroup>();

        if (verticalLayout != null)
            DestroyImmediate(verticalLayout);

        ContentSizeFitter sizeFitter = GetComponent<ContentSizeFitter>();

        if (sizeFitter != null)
            DestroyImmediate(sizeFitter);
    }

    private RectTransform EnsureChildRect(string childName, Transform parent)
    {
        Transform existing = parent.Find(childName);

        if (existing != null)
            return existing.GetComponent<RectTransform>();

        GameObject go = new GameObject(childName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private T GetOrAdd<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();

        if (component == null)
            component = target.AddComponent<T>();

        return component;
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
