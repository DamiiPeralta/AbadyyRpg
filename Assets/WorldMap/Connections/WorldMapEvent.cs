using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldMapEvent
{
    [Header("Texto")]
    [TextArea(3, 8)]
    public string eventText;

    [Header("Opciones")]
    public WorldMapEventOption option1;
    public WorldMapEventOption option2;
}

[System.Serializable]
public class WorldMapEventOption
{
    [Header("Texto")]
    public string optionText;

    [Header("Acciones")]
    public List<WorldMapNode> nodesToUnlock = new List<WorldMapNode>();
    public string flagToAdd;

    [Tooltip("Si está activo, el evento queda cerrado después de elegir esta opción.")]
    public bool completesEvent = true;

    [Header("Combat")]
    [Tooltip("Si está activo, esta opción inicia un combate.")]
    public bool startsCombat = false;

    [Tooltip("Grupo de enemigos que se carga en BattleSetup.")]
    public string battleGroupId = "TestBattle";

    [Header("Reward")]
    public RewardData reward;

    [Header("Movimiento")]
    public bool returnToStartNode = false;
}