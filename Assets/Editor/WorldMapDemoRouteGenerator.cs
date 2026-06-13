using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WorldMapDemoRouteGenerator
{
    private const string ParentName = "Generated_DemoEventRoute";
    private const string NodePrefabPath = "Assets/WorldMap/Nodes/NodePrefab.prefab";
    private const string WorldMapScenePath = "Assets/Scenes/WorldMapScene.unity";

    private static readonly List<NodeSpec> ZoneNodes = new List<NodeSpec>
    {
        new NodeSpec("demo_start", "Ciudad de Valdoran", "La caravana parte desde la ciudad amurallada hacia las tierras fronterizas.", WorldMapTerrainType.Pueblo, new Vector3(-5.65f, 2.25f, 0f), true),
        new NodeSpec("demo_crossroads", "Puente de Valdoran", "El puente exterior conecta la ciudad con la red de caminos del valle.", WorldMapTerrainType.RioBajo, new Vector3(-4.55f, 0.65f, 0f), true),
        new NodeSpec("demo_bridge_event", "Aldea de Claravalle", "La aldea marca el primer punto de decision de la ruta.", WorldMapTerrainType.Pueblo, new Vector3(-5.45f, -0.35f, 0f), true),
        new NodeSpec("demo_low_forest_combat", "Bosque de Aldheron", "Criaturas hambrientas rondan entre los arboles del bosque central.", WorldMapTerrainType.Bosque, new Vector3(-0.95f, 0.85f, 0f), false),
        new NodeSpec("demo_old_cache_event", "Torre Vigia Abandonada", "La torre conserva suministros y rastros de patrullas antiguas.", WorldMapTerrainType.Ruinas, new Vector3(-1.25f, 2.45f, 0f), false),
        new NodeSpec("demo_river_ambush", "Claro de las Rocas", "Un claro abierto junto al camino sur, perfecto para una emboscada.", WorldMapTerrainType.Camino, new Vector3(-3.35f, -2.0f, 0f), false),
        new NodeSpec("demo_bandit_camp", "Campamento del Vigia", "El humo de una fogata delata un grupo armado en el centro del valle.", WorldMapTerrainType.Camino, new Vector3(0.05f, -0.55f, 0f), false),
        new NodeSpec("demo_shrine_event", "Cruce de los Susurros", "Un altar olvidado descansa junto al puente del bosque profundo.", WorldMapTerrainType.Ruinas, new Vector3(0.55f, -2.0f, 0f), false),
        new NodeSpec("demo_swamp_beast", "Bosque Profundo de Umbria", "Las sombras se cierran entre arboles viejos y caminos torcidos.", WorldMapTerrainType.BosqueProfundo, new Vector3(-0.35f, -3.15f, 0f), false),
        new NodeSpec("demo_broken_market", "Mina de Hierro Negro", "La mina abre una ruta dura hacia la cordillera y sus portales antiguos.", WorldMapTerrainType.Ruinas, new Vector3(2.15f, 2.2f, 0f), false),
        new NodeSpec("demo_ruin_shortcut", "Portal de los Excavadores", "Una construccion vieja entre montanas sirve como atajo peligroso.", WorldMapTerrainType.Ruinas, new Vector3(4.95f, 0.95f, 0f), false),
        new NodeSpec("demo_sealed_gate_event", "Aldea de Collinasombra", "El camino hacia el este queda cerrado por simbolos y patrullas.", WorldMapTerrainType.Pueblo, new Vector3(3.85f, -0.95f, 0f), false),
        new NodeSpec("demo_gate_guard", "Pantano de las Sombras Lentas", "El ultimo grupo protege la entrada al tramo maldito.", WorldMapTerrainType.Pantano, new Vector3(5.15f, -2.9f, 0f), false),
        new NodeSpec("demo_abbey_threshold", "Cordillera de Piedra Umbria", "Un respiro corto antes del jefe entre riscos y niebla.", WorldMapTerrainType.Ruinas, new Vector3(5.75f, 2.35f, 0f), false),
        new NodeSpec("demo_final_boss", "La Cosa Bajo la Cordillera", "El objetivo final de la run simulada.", WorldMapTerrainType.Ruinas, new Vector3(6.55f, 1.35f, 0f), false)
    };

    private static readonly string[,] ZoneConnections =
    {
        { "demo_start", "demo_crossroads" },
        { "demo_crossroads", "demo_bridge_event" },
        { "demo_bridge_event", "demo_low_forest_combat" },
        { "demo_bridge_event", "demo_river_ambush" },
        { "demo_low_forest_combat", "demo_old_cache_event" },
        { "demo_old_cache_event", "demo_bandit_camp" },
        { "demo_old_cache_event", "demo_broken_market" },
        { "demo_broken_market", "demo_ruin_shortcut" },
        { "demo_ruin_shortcut", "demo_sealed_gate_event" },
        { "demo_river_ambush", "demo_shrine_event" },
        { "demo_shrine_event", "demo_swamp_beast" },
        { "demo_swamp_beast", "demo_sealed_gate_event" },
        { "demo_bandit_camp", "demo_sealed_gate_event" },
        { "demo_sealed_gate_event", "demo_gate_guard" },
        { "demo_gate_guard", "demo_abbey_threshold" },
        { "demo_abbey_threshold", "demo_final_boss" }
    };

    [MenuItem("CaravanRPG/World Map/Generate Demo Event Route")]
    public static void Generate()
    {
        GenerateInActiveScene();
    }

    [MenuItem("CaravanRPG/World Map/Generate Demo Event Route And Save Scene")]
    public static void GenerateAndSaveWorldMapScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EditorSceneManager.OpenScene(WorldMapScenePath);
        GenerateInActiveScene();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    public static void GenerateInActiveScene()
    {
        WorldMapNode prefab = AssetDatabase.LoadAssetAtPath<WorldMapNode>(NodePrefabPath);

        if (prefab == null)
        {
            Debug.LogError($"No se encontro el prefab de nodo en {NodePrefabPath}.");
            return;
        }

        DeleteExistingGeneratedRoute();

        GameObject parent = new GameObject(ParentName);
        Undo.RegisterCreatedObjectUndo(parent, "Create demo event route");

        Dictionary<string, WorldMapNode> nodesById = CreateNodes(prefab, parent.transform);
        ConnectZone(nodesById);
        ConfigureCombats(nodesById);
        ConfigureEvents(nodesById);
        CreateConnectionLines(parent.transform, nodesById);
        ConfigureWorldMapManager(nodesById);

        EditorUtility.SetDirty(parent);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Mini zona demo generada: eventos, rutas opcionales, combates y jefe final.");
    }

    private static Dictionary<string, WorldMapNode> CreateNodes(WorldMapNode prefab, Transform parent)
    {
        Dictionary<string, WorldMapNode> nodesById = new Dictionary<string, WorldMapNode>();

        foreach (NodeSpec spec in ZoneNodes)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
            Undo.RegisterCreatedObjectUndo(instance, "Create demo map node");
            instance.transform.SetParent(parent);
            instance.transform.position = spec.position;
            instance.name = spec.id;

            WorldMapNode node = instance.GetComponent<WorldMapNode>();
            node.nodeId = spec.id;
            node.nodeName = spec.name;
            node.description = spec.description;
            node.terrainType = spec.terrainType;
            node.useTerrainDefaultTravelCosts = true;
            node.ApplyDefaultTravelCosts();
            node.connectedNodes.Clear();
            node.isUnlocked = spec.unlockedAtStart;
            node.isVisited = false;
            node.isCurrent = spec.id == "demo_start";
            node.hasEvent = false;
            node.eventCompleted = false;
            node.nodeEvent = null;
            node.isCombatNode = false;
            node.battleGroupId = "TestBattle";
            node.isCompleted = false;
            node.combatVictoryReward = null;
            node.unlockOnCombatVictory.Clear();

            nodesById.Add(spec.id, node);
            EditorUtility.SetDirty(node);
        }

        return nodesById;
    }

    private static void ConnectZone(Dictionary<string, WorldMapNode> nodesById)
    {
        for (int i = 0; i < ZoneConnections.GetLength(0); i++)
        {
            WorldMapNode from = nodesById[ZoneConnections[i, 0]];
            WorldMapNode to = nodesById[ZoneConnections[i, 1]];

            AddConnection(from, to);
            AddConnection(to, from);
        }
    }

    private static void ConfigureCombats(Dictionary<string, WorldMapNode> nodes)
    {
        ConfigureCombat(nodes, "demo_low_forest_combat", "hungry_beasts_01", "demo_old_cache_event", Reward(8, 1, 0, 0, 0, 25));
        ConfigureCombat(nodes, "demo_river_ambush", "road_ambush_01", "demo_shrine_event", Reward(10, 0, 1, 0, 0, 30));
        ConfigureCombat(nodes, "demo_bandit_camp", "bandit_patrol_01", "demo_sealed_gate_event", Reward(25, 0, 0, 1, 0, 45));
        ConfigureCombat(nodes, "demo_swamp_beast", "mountain_beast_01", "demo_sealed_gate_event", Reward(18, 1, 0, 0, 1, 50));
        ConfigureCombat(nodes, "demo_ruin_shortcut", "cult_rites_01", "demo_sealed_gate_event", Reward(20, 0, 0, 0, 1, 45));
        ConfigureCombat(nodes, "demo_gate_guard", "elite_captain_01", "demo_abbey_threshold", Reward(35, 0, 0, 2, 1, 80));
        ConfigureCombat(nodes, "demo_final_boss", "final_abbey_thing_01", null, Reward(120, 2, 2, 2, 2, 180));
    }

    private static void ConfigureCombat(Dictionary<string, WorldMapNode> nodes, string nodeId, string battleGroupId, string unlockNodeId, RewardData reward)
    {
        WorldMapNode node = nodes[nodeId];
        node.isCombatNode = true;
        node.battleGroupId = battleGroupId;
        node.combatVictoryReward = reward;

        if (!string.IsNullOrWhiteSpace(unlockNodeId))
            node.unlockOnCombatVictory.Add(nodes[unlockNodeId]);

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureEvents(Dictionary<string, WorldMapNode> nodes)
    {
        ConfigureBridgeEvent(nodes);
        ConfigureCacheEvent(nodes);
        ConfigureShrineEvent(nodes);
        ConfigureMarketEvent(nodes);
        ConfigureGateEvent(nodes);
        ConfigureThresholdEvent(nodes);
    }

    private static void ConfigureBridgeEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["demo_bridge_event"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Rutas partidas en Claravalle",
            eventType = "Evento de Camino",
            description = "El camino norte esta danado por lluvias y ruedas hundidas. Repararlo abre la ruta del bosque; rodear manda a la caravana por el claro del sur.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_broken_bridge_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Reparar el camino norte",
                    "Gastar materiales para abrir Bosque de Aldheron y mantener tambien visible el paso del sur.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_reparar_transparent.png",
                    nodes["demo_low_forest_combat"],
                    nodes["demo_river_ambush"],
                    "demo_puente_reparado",
                    Reward(6, 0, 0, 0, 0, 15),
                    wood: 2,
                    iron: 1,
                    stamina: 2,
                    hours: 2
                ),
                Option(
                    "Rodear por el claro",
                    "Evitar el gasto de materiales. Es mas lento y deja la ruta del bosque para despues.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    nodes["demo_river_ambush"],
                    null,
                    "demo_puente_rodeado",
                    null,
                    stamina: 1,
                    hours: 3
                )
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureCacheEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["demo_old_cache_event"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Torre vigia abandonada",
            eventType = "Recursos",
            description = "La puerta esta trabada, pero dentro quedan cajas viejas, mapas rotos y marcas recientes.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_market_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Forzar la puerta",
                    "Recuperar suministros y revelar el campamento del vigia que usaba el lugar.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_recursos_transparent.png",
                    nodes["demo_bandit_camp"],
                    nodes["demo_broken_market"],
                    "demo_almacen_saqueado",
                    Reward(18, 1, 2, 1, 0, 35),
                    stamina: 2,
                    hours: 1
                ),
                Option(
                    "Marcar el lugar y seguir",
                    "No arriesgarse con las cajas. Abre solo la ruta dura hacia la mina.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    nodes["demo_broken_market"],
                    null,
                    "demo_almacen_ignorado",
                    Reward(4, 0, 0, 0, 0, 10),
                    hours: 1
                )
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureShrineEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["demo_shrine_event"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Cruce de los Susurros",
            eventType = "Evento de Fe",
            description = "El altar tiene una hendidura con la misma forma que el sello de Collinasombra.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_ruins_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Copiar el sello",
                    "La caravana obtiene la marca necesaria para abrir el paso de Collinasombra sin forzarlo.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_descanso_transparent.png",
                    nodes["demo_swamp_beast"],
                    null,
                    "demo_sello_santuario",
                    Reward(5, 0, 0, 0, 0, 25),
                    hours: 1
                ),
                Option(
                    "Descansar un momento",
                    "Recuperar fuerzas y seguir por el pantano sin aprender el sello.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_descanso_transparent.png",
                    nodes["demo_swamp_beast"],
                    null,
                    "demo_descanso_santuario",
                    Reward(0, 1, 0, 0, 0, 15),
                    hours: 2
                )
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureMarketEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["demo_broken_market"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Mina de Hierro Negro",
            eventType = "Ruta Opcional",
            description = "Una senda minera permite alcanzar el portal de los excavadores por una ruta peligrosa.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_market_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Abrir el atajo",
                    "Gastar tiempo limpiando escombros para alcanzar una ruta alternativa hacia Collinasombra.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    nodes["demo_ruin_shortcut"],
                    null,
                    "demo_atajo_abierto",
                    Reward(8, 0, 1, 0, 0, 20),
                    stamina: 1,
                    hours: 2
                )
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureGateEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["demo_sealed_gate_event"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Paso sellado de Collinasombra",
            eventType = "Bloqueo",
            description = "El paso puede abrirse con el sello de los Susurros o romperse a fuerza bruta. Detras espera la guardia del pantano.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_ruins_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Abrir con el sello",
                    "Usar la marca copiada en el cruce para evitar perder recursos.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_pueblo_transparent.png",
                    nodes["demo_gate_guard"],
                    null,
                    "demo_porton_abierto_con_sello",
                    Reward(12, 0, 0, 0, 0, 35),
                    hours: 1,
                    requiredFlag: "demo_sello_santuario"
                ),
                Option(
                    "Romper las cadenas",
                    "Forzar el paso gastando hierro y stamina. Sirve si la run no consiguio el sello.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_reparar_transparent.png",
                    nodes["demo_gate_guard"],
                    null,
                    "demo_porton_forzado",
                    null,
                    iron: 2,
                    stamina: 3,
                    hours: 2
                )
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureThresholdEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["demo_abbey_threshold"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Umbral de Piedra Umbria",
            eventType = "Preparacion",
            description = "El paso interior esta abierto. La caravana puede tomar aire antes del jefe.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_ruins_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Preparar el asalto final",
                    "Ordenar equipo, repartir antorchas y avanzar hacia el jefe.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_combate_transparent.png",
                    nodes["demo_final_boss"],
                    null,
                    "demo_jefe_desbloqueado",
                    Reward(20, 1, 0, 1, 0, 40),
                    hours: 1
                )
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static WorldMapEventOption Option(
        string title,
        string description,
        string iconPath,
        WorldMapNode unlockA,
        WorldMapNode unlockB,
        string flag,
        RewardData reward,
        int gold = 0,
        int food = 0,
        int wood = 0,
        int iron = 0,
        int leather = 0,
        int stamina = 0,
        int hours = 0,
        string requiredFlag = null)
    {
        WorldMapEventOption option = new WorldMapEventOption
        {
            title = title,
            description = description,
            icon = LoadSprite(iconPath),
            goldCost = gold,
            foodCost = food,
            woodCost = wood,
            ironCost = iron,
            leatherCost = leather,
            caravanStaminaCost = stamina,
            hoursCost = hours,
            flagToAdd = flag,
            reward = reward,
            completesEvent = true
        };

        if (unlockA != null)
            option.nodesToUnlock.Add(unlockA);

        if (unlockB != null)
            option.nodesToUnlock.Add(unlockB);

        if (!string.IsNullOrWhiteSpace(requiredFlag))
            option.requiredFlags.Add(requiredFlag);

        return option;
    }

    private static RewardData Reward(int gold, int food, int wood, int iron, int leather, int experience)
    {
        return new RewardData
        {
            gold = gold,
            food = food,
            wood = wood,
            iron = iron,
            leather = leather,
            experience = experience
        };
    }

    private static void CreateConnectionLines(Transform parent, Dictionary<string, WorldMapNode> nodesById)
    {
        GameObject lineRoot = new GameObject("Connections");
        Undo.RegisterCreatedObjectUndo(lineRoot, "Create demo map connection lines");
        lineRoot.transform.SetParent(parent);
        lineRoot.transform.localPosition = Vector3.zero;

        for (int i = 0; i < ZoneConnections.GetLength(0); i++)
        {
            WorldMapNode from = nodesById[ZoneConnections[i, 0]];
            WorldMapNode to = nodesById[ZoneConnections[i, 1]];
            CreateConnectionLine(lineRoot.transform, from, to, i);
        }
    }

    private static void CreateConnectionLine(Transform parent, WorldMapNode fromNode, WorldMapNode toNode, int index)
    {
        GameObject lineObject = new GameObject($"Connection_{index + 1:00}_{fromNode.nodeId}_to_{toNode.nodeId}");
        Undo.RegisterCreatedObjectUndo(lineObject, "Create demo map connection line");
        lineObject.transform.SetParent(parent);
        lineObject.transform.position = Vector3.zero;

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));

        WorldMapConnectionLine connectionLine = lineObject.AddComponent<WorldMapConnectionLine>();
        connectionLine.fromNode = fromNode;
        connectionLine.toNode = toNode;
        connectionLine.Refresh();

        EditorUtility.SetDirty(connectionLine);
    }

    private static void ConfigureWorldMapManager(Dictionary<string, WorldMapNode> nodesById)
    {
        WorldMapManager manager = Object.FindObjectOfType<WorldMapManager>();

        if (manager == null)
        {
            Debug.LogWarning("No se encontro WorldMapManager. La zona se creo, pero no se asigno currentNode/allNodes.");
            return;
        }

        manager.currentNode = nodesById["demo_start"];
        manager.allNodes = new List<WorldMapNode>();

        foreach (NodeSpec spec in ZoneNodes)
            manager.allNodes.Add(nodesById[spec.id]);

        EditorUtility.SetDirty(manager);
    }

    private static void AddConnection(WorldMapNode from, WorldMapNode to)
    {
        if (from != null && to != null && !from.connectedNodes.Contains(to))
            from.connectedNodes.Add(to);
    }

    private static void DeleteExistingGeneratedRoute()
    {
        GameObject existing = GameObject.Find(ParentName);

        if (existing != null)
            Undo.DestroyObjectImmediate(existing);
    }

    private static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private class NodeSpec
    {
        public readonly string id;
        public readonly string name;
        public readonly string description;
        public readonly WorldMapTerrainType terrainType;
        public readonly Vector3 position;
        public readonly bool unlockedAtStart;

        public NodeSpec(string id, string name, string description, WorldMapTerrainType terrainType, Vector3 position, bool unlockedAtStart)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.terrainType = terrainType;
            this.position = position;
            this.unlockedAtStart = unlockedAtStart;
        }
    }
}
