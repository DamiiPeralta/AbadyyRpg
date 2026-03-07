using System.Collections.Generic;
using UnityEngine;

// Contenedor de configuración para una batalla: prefabs y posiciones de aliados/enemigos
public class BattleSetup : MonoBehaviour
{
    [Header("Aliados")]
    public List<GameObject> allyPrefabs = new List<GameObject>();
    public List<Transform> allyPositions = new List<Transform>();

    [Header("Enemigos")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<Transform> enemyPositions = new List<Transform>();
}
