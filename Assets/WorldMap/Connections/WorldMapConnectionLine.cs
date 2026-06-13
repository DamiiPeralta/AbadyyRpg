using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class WorldMapConnectionLine : MonoBehaviour
{
    [Header("Nodes")]
    public WorldMapNode fromNode;
    public WorldMapNode toNode;

    [Header("Visual")]
    public float width = 0.035f;
    public Color blockedColor = new Color(0.25f, 0.22f, 0.18f, 0.65f);
    public Color undiscoveredColor = new Color(0.72f, 0.56f, 0.24f, 0.75f);
    public Color availableColor = new Color(0.95f, 0.72f, 0.24f, 0.95f);
    public Color visitedColor = new Color(0.42f, 0.76f, 0.45f, 0.9f);
    public Color currentColor = new Color(1f, 0.86f, 0.32f, 1f);

    private LineRenderer lineRenderer;

    private void Awake()
    {
        Setup();
        Refresh();
    }

    private void OnEnable()
    {
        Setup();
        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void OnValidate()
    {
        Setup();
        Refresh();
    }

    public void Refresh()
    {
        Setup();

        if (lineRenderer == null)
            return;

        bool hasNodes = fromNode != null && toNode != null;
        lineRenderer.enabled = hasNodes;

        if (!hasNodes)
            return;

        lineRenderer.SetPosition(0, fromNode.transform.position);
        lineRenderer.SetPosition(1, toNode.transform.position);

        Color color = GetStateColor();
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void Setup()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
            return;

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.numCapVertices = 2;
        lineRenderer.numCornerVertices = 2;
        lineRenderer.sortingOrder = -1;

        if (lineRenderer.sharedMaterial == null)
            lineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    private Color GetStateColor()
    {
        if (fromNode == null || toNode == null)
            return blockedColor;

        if (!fromNode.isUnlocked || !toNode.isUnlocked)
            return blockedColor;

        if (fromNode.isCurrent || toNode.isCurrent)
            return currentColor;

        if (fromNode.isVisited && toNode.isVisited)
            return visitedColor;

        if (fromNode.isVisited || toNode.isVisited)
            return availableColor;

        return undiscoveredColor;
    }
}
