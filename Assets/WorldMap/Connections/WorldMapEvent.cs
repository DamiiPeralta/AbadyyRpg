using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldMapEvent
{
    [Header("Presentacion")]
    public string title;
    public string eventType;

    [TextArea(3, 8)]
    public string description;

    public Sprite illustration;

    [Header("Opciones")]
    public List<WorldMapEventOption> options = new List<WorldMapEventOption>();
}

[System.Serializable]
public class WorldMapEventOption
{
    [Header("Presentacion")]
    public string title;

    [TextArea(2, 4)]
    public string description;

    public Sprite icon;

    [Header("Costos - Recursos")]
    public int goldCost;
    public int foodCost;
    public int woodCost;
    public int stoneCost;
    public int ironCost;
    public int leatherCost;
    public int crystalsCost;

    [Header("Costos - Caravana")]
    public int caravanStaminaCost;
    public int hoursCost;

    [Header("Costos - Items")]
    public List<EventOptionItemCost> itemCosts = new List<EventOptionItemCost>();

    [Header("Acciones")]
    public List<WorldMapNode> nodesToUnlock = new List<WorldMapNode>();
    public string flagToAdd;
    public List<string> requiredFlags = new List<string>();
    public List<string> blockedByFlags = new List<string>();

    [Tooltip("Si esta activo, el evento queda cerrado despues de elegir esta opcion.")]
    public bool completesEvent = true;

    [Header("Combat")]
    [Tooltip("Si esta activo, esta opcion inicia un combate.")]
    public bool startsCombat = false;

    [Tooltip("Grupo de enemigos que se carga en BattleSetup.")]
    public string battleGroupId = "TestBattle";

    [Header("Reward")]
    public RewardData reward;

    [Header("Movimiento")]
    public bool returnToStartNode = false;
}

[System.Serializable]
public class EventOptionItemCost
{
    public ItemBase item;
    public string itemId;
    public int amount = 1;

    public string GetItemId()
    {
        if (item != null && !string.IsNullOrWhiteSpace(item.itemId))
            return item.itemId;

        return itemId;
    }

    public ItemBase GetItem()
    {
        if (item != null)
            return item;

        return ItemDatabase.Instance != null ? ItemDatabase.Instance.GetItemById(itemId) : null;
    }
}
