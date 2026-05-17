using System.Collections.Generic;
using UnityEngine;

public class WorldMapNode : MonoBehaviour
{
    [Header("Info")]
    public string nodeId;
    public string nodeName = "Node";
    [TextArea] public string description;

    [Header("Conexiones")]
    public List<WorldMapNode> connectedNodes = new List<WorldMapNode>();

    [Header("Estado")]
    public bool isUnlocked = true;
    public bool isVisited = false;
    public bool isCurrent = false;

    [Header("Combat")]
    public bool isCombatNode = false;
    public string battleGroupId = "TestBattle";
    public List<WorldMapNode> unlockOnCombatVictory = new List<WorldMapNode>();
    public bool isCompleted = false;

    [Header("Rewards")]
    public RewardData combatVictoryReward;

    [Header("Sprites")]
    public Sprite lockedSprite;
    public Sprite unvisitedSprite;
    public Sprite visitedSprite;
    public Sprite currentSprite;

    [Header("Evento")]
    public bool hasEvent = false;
    public bool eventCompleted = false;
    public WorldMapEvent nodeEvent;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        RefreshVisual();
    }

    private void OnMouseDown()
    {
        if (WorldMapManager.Instance == null)
        {
            Debug.LogError("No existe WorldMapManager en la escena.");
            return;
        }

        WorldMapManager.Instance.TryMoveToNode(this);
    }

    public bool IsConnectedTo(WorldMapNode otherNode)
    {
        return connectedNodes.Contains(otherNode);
    }

    public void SetVisited(bool value)
    {
        isVisited = value;
        RefreshVisual();
    }

    public void SetCurrent(bool value)
    {
        isCurrent = value;
        RefreshVisual();
    }

    public void SetUnlocked(bool value)
    {
        isUnlocked = value;
        RefreshVisual();
    }

    public void RefreshVisual()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (!isUnlocked && lockedSprite != null)
        {
            spriteRenderer.sprite = lockedSprite;
            return;
        }

        if (isCurrent && currentSprite != null)
        {
            spriteRenderer.sprite = currentSprite;
            return;
        }

        if (isVisited && visitedSprite != null)
        {
            spriteRenderer.sprite = visitedSprite;
            return;
        }

        if (unvisitedSprite != null)
        {
            spriteRenderer.sprite = unvisitedSprite;
        }
    }
}