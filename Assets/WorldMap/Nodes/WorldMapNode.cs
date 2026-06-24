using System.Collections.Generic;
using UnityEngine;

public enum WorldMapTerrainType
{
    Camino,
    Pueblo,
    Bosque,
    BosqueProfundo,
    CaminoMontañoso,
    Montaña,
    RioBajo,
    Pantano,
    Ruinas
}

public class WorldMapNode : MonoBehaviour
{
    [Header("Info")]
    public string nodeId;
    public string nodeName = "Node";
    [TextArea] public string description;

    [Header("Viaje")]
    public WorldMapTerrainType terrainType = WorldMapTerrainType.Camino;
    public bool useTerrainDefaultTravelCosts = true;
    public int travelStaminaCost = 1;
    public int travelHourCost = 1;

    [Header("Conexiones")]
    public List<WorldMapNode> connectedNodes = new List<WorldMapNode>();

    [Header("Estado")]
    public bool isUnlocked = true;
    public bool isVisited = false;
    public bool isCurrent = false;

    [Header("Mercado")]
    public bool allowsTrading = false;
    [TextArea] public string noTradingMessage = "No puedes vender aqui, no hay mercado.";

    [Header("Buscar recursos")]
    public bool useTerrainDefaultResourceSearch = true;
    public bool allowsResourceSearch = false;
    public int resourceSearchStaminaCost = 4;
    public int resourceSearchHourCost = 2;
    public List<WorldMapResourceDrop> resourceSearchDrops = new List<WorldMapResourceDrop>();

    [Header("Combat")]
    public bool isCombatNode = false;
    [Tooltip("Permite volver a iniciar combate al entrar al nodo aunque ya se haya completado antes.")]
    public bool isRepeatableCombat = false;
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
        if (useTerrainDefaultResourceSearch)
            ApplyDefaultResourceSearch();

        spriteRenderer = GetComponent<SpriteRenderer>();
        RefreshVisual();
    }

    private void OnValidate()
    {
        if (useTerrainDefaultTravelCosts)
            ApplyDefaultTravelCosts();

        if (useTerrainDefaultResourceSearch)
            ApplyDefaultResourceSearch();
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

    public void ApplyDefaultTravelCosts()
    {
        switch (terrainType)
        {
            case WorldMapTerrainType.Pueblo:
                travelStaminaCost = 0;
                travelHourCost = 0;
                break;
            case WorldMapTerrainType.Bosque:
                travelStaminaCost = 3;
                travelHourCost = 3;
                break;
            case WorldMapTerrainType.BosqueProfundo:
                travelStaminaCost = 5;
                travelHourCost = 5;
                break;
            case WorldMapTerrainType.CaminoMontañoso:
                travelStaminaCost = 3;
                travelHourCost = 3;
                break;
            case WorldMapTerrainType.Montaña:
                travelStaminaCost = 6;
                travelHourCost = 6;
                break;
            case WorldMapTerrainType.RioBajo:
                travelStaminaCost = 4;
                travelHourCost = 3;
                break;
            case WorldMapTerrainType.Pantano:
                travelStaminaCost = 5;
                travelHourCost = 4;
                break;
            case WorldMapTerrainType.Ruinas:
                travelStaminaCost = 2;
                travelHourCost = 2;
                break;
            default:
                travelStaminaCost = 1;
                travelHourCost = 1;
                break;
        }
    }

    public void ApplyDefaultResourceSearch()
    {
        resourceSearchStaminaCost = 4;
        resourceSearchHourCost = 2;

        if (resourceSearchDrops == null)
            resourceSearchDrops = new List<WorldMapResourceDrop>();

        resourceSearchDrops.Clear();

        switch (terrainType)
        {
            case WorldMapTerrainType.Pueblo:
                allowsResourceSearch = false;
                break;

            case WorldMapTerrainType.Bosque:
                allowsResourceSearch = true;
                AddResourceDrop(CaravanResourceType.Wood, 1, 2, 5);
                AddResourceDrop(CaravanResourceType.Food, 1, 1, 2);
                AddResourceDrop(CaravanResourceType.Leather, 1, 1, 1);
                break;

            case WorldMapTerrainType.BosqueProfundo:
                allowsResourceSearch = true;
                resourceSearchStaminaCost = 5;
                resourceSearchHourCost = 3;
                AddResourceDrop(CaravanResourceType.Wood, 2, 3, 5);
                AddResourceDrop(CaravanResourceType.Food, 1, 2, 2);
                AddResourceDrop(CaravanResourceType.Leather, 1, 2, 2);
                break;

            case WorldMapTerrainType.Camino:
            case WorldMapTerrainType.CaminoMontañoso:
                allowsResourceSearch = true;
                AddResourceDrop(CaravanResourceType.Stone, 1, 2, 3);
                AddResourceDrop(CaravanResourceType.Wood, 1, 1, 2);
                break;

            case WorldMapTerrainType.Montaña:
                allowsResourceSearch = true;
                resourceSearchStaminaCost = 5;
                resourceSearchHourCost = 3;
                AddResourceDrop(CaravanResourceType.Stone, 2, 3, 4);
                AddResourceDrop(CaravanResourceType.Iron, 1, 2, 2);
                AddResourceDrop(CaravanResourceType.Crystals, 1, 1, 1);
                break;

            case WorldMapTerrainType.RioBajo:
                allowsResourceSearch = true;
                AddResourceDrop(CaravanResourceType.Food, 1, 2, 4);
                AddResourceDrop(CaravanResourceType.Stone, 1, 1, 1);
                break;

            case WorldMapTerrainType.Pantano:
                allowsResourceSearch = true;
                resourceSearchStaminaCost = 5;
                AddResourceDrop(CaravanResourceType.Food, 1, 1, 2);
                AddResourceDrop(CaravanResourceType.Leather, 1, 2, 3);
                AddResourceDrop(CaravanResourceType.Wood, 1, 1, 1);
                break;

            case WorldMapTerrainType.Ruinas:
                allowsResourceSearch = true;
                AddResourceDrop(CaravanResourceType.Stone, 1, 2, 3);
                AddResourceDrop(CaravanResourceType.Crystals, 1, 2, 2);
                AddResourceDrop(CaravanResourceType.Iron, 1, 1, 1);
                break;
        }
    }

    private void AddResourceDrop(CaravanResourceType resourceType, int minAmount, int maxAmount, int weight)
    {
        resourceSearchDrops.Add(new WorldMapResourceDrop
        {
            resourceType = resourceType,
            minAmount = minAmount,
            maxAmount = maxAmount,
            weight = weight
        });
    }
}
