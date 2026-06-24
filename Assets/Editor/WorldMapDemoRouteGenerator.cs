using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WorldMapDemoRouteGenerator
{
    private const string ParentName = "Generated_DemoEventRoute";
    private const string NodePrefabPath = "Assets/WorldMap/Nodes/NodePrefab.prefab";
    private const string WorldMapScenePath = "Assets/Scenes/WorldMapScene.unity";

    private const string FlagBridgeRepaired = "flag_puente_reparado";
    private const string FlagBossClueFound = "flag_pista_jefe_encontrada";
    private const string FlagBossRouteUnlocked = "flag_ruta_jefe_desbloqueada";
    private const string FlagBossDefeated = "flag_jefe_derrotado";
    private const string FlagFrontierOpen = "flag_frontera_abierta";

    private static readonly List<NodeSpec> ZoneNodes = new List<NodeSpec>
    {
        new NodeSpec("node_valdoran", "Ciudad de Valdoran", "Base de la caravana, preparacion y crafting.", WorldMapTerrainType.Pueblo, new Vector3(-5.9f, 2.25f, 0f), true),
        new NodeSpec("node_camino_01", "Camino a Claravalle I", "Primer tramo seguro fuera de Valdoran.", WorldMapTerrainType.Camino, new Vector3(-5.25f, 0.75f, 0f), true),
        new NodeSpec("node_camino_02", "Camino a Claravalle II", "Segundo tramo antes del puente viejo.", WorldMapTerrainType.Camino, new Vector3(-4.65f, -0.25f, 0f), true),
        new NodeSpec("node_puente_roto", "Puente roto", "Obstaculo inicial de recursos y tiempo.", WorldMapTerrainType.RioBajo, new Vector3(-3.45f, -0.95f, 0f), true),
        new NodeSpec("node_cruce_central", "Cruce del Vigia", "Cruce principal de la mini zona.", WorldMapTerrainType.Camino, new Vector3(-1.05f, -0.25f, 0f), false),
        new NodeSpec("node_bosque_aldheron", "Bosque de Aldheron", "Farm temprano con ratas y esqueletos.", WorldMapTerrainType.Bosque, new Vector3(-1.95f, 1.15f, 0f), false),
        new NodeSpec("node_claro_rocas", "Claro de las Rocas", "Farm medio y prueba de builds.", WorldMapTerrainType.Camino, new Vector3(-2.55f, -2.05f, 0f), false),
        new NodeSpec("node_torre_vigia", "Torre vigia abandonada", "Evento para encontrar la pista del jefe.", WorldMapTerrainType.Ruinas, new Vector3(-0.65f, 2.25f, 0f), false),
        new NodeSpec("node_mina_hierro", "Mina de Hierro Negro", "Farm de hierro y cristal para preparacion final.", WorldMapTerrainType.Ruinas, new Vector3(1.95f, 1.75f, 0f), false),
        new NodeSpec("node_collinasombra", "Aldea de Collinasombra", "Advertencia final antes del portal.", WorldMapTerrainType.Pueblo, new Vector3(3.75f, -0.85f, 0f), false),
        new NodeSpec("node_portal_excavadores", "Portal de los Excavadores", "Jefe de la demo Tier 1.", WorldMapTerrainType.Ruinas, new Vector3(5.05f, 0.95f, 0f), false),
        new NodeSpec("node_frontera_abierta", "Frontera abierta", "Cierre de demo tras derrotar al jefe.", WorldMapTerrainType.CaminoMontañoso, new Vector3(6.15f, 2.1f, 0f), false)
    };

    private static readonly string[,] ZoneConnections =
    {
        { "node_valdoran", "node_camino_01" },
        { "node_camino_01", "node_camino_02" },
        { "node_camino_02", "node_puente_roto" },
        { "node_puente_roto", "node_cruce_central" },
        { "node_cruce_central", "node_bosque_aldheron" },
        { "node_bosque_aldheron", "node_claro_rocas" },
        { "node_cruce_central", "node_torre_vigia" },
        { "node_torre_vigia", "node_mina_hierro" },
        { "node_cruce_central", "node_collinasombra" },
        { "node_collinasombra", "node_portal_excavadores" },
        { "node_portal_excavadores", "node_frontera_abierta" }
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
        Debug.Log("Mini zona demo Tier 1 generada: 12 nodos, 10 caminos logicos, eventos, combates y jefe final.");
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
            node.isCurrent = spec.id == "node_valdoran";
            node.hasEvent = false;
            node.eventCompleted = false;
            node.nodeEvent = null;
            node.isCombatNode = false;
            node.isRepeatableCombat = false;
            node.battleGroupId = "";
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
        ConfigureCombat(nodes, "node_bosque_aldheron", "encounter_ratas_02_t1");
        ConfigureCombat(nodes, "node_claro_rocas", "encounter_rata_esqueleto_t1");
        ConfigureCombat(nodes, "node_mina_hierro", "encounter_esqueleto_gusano_t1");
    }

    private static void ConfigureCombat(Dictionary<string, WorldMapNode> nodes, string nodeId, string battleGroupId)
    {
        WorldMapNode node = nodes[nodeId];
        BattleEncounterSO encounter = LoadEncounter(battleGroupId);

        node.isCombatNode = true;
        node.isRepeatableCombat = true;
        node.battleGroupId = battleGroupId;
        node.combatVictoryReward = encounter != null ? encounter.reward : null;
        EditorUtility.SetDirty(node);
    }

    private static void ConfigureEvents(Dictionary<string, WorldMapNode> nodes)
    {
        ConfigureBridgeEvent(nodes);
        ConfigureBossClueEvent(nodes);
        ConfigureCollinasombraEvent(nodes);
        ConfigureBossEvent(nodes);
        ConfigureFinalEvent(nodes);
    }

    private static void ConfigureBridgeEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["node_puente_roto"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Puente roto",
            eventType = "Evento de Camino",
            description = "El puente cruje bajo el peso de la caravana. Cruzarlo sin repararlo puede dejar la expedicion varada.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_broken_bridge_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Reparar el puente",
                    "Usar materiales y tiempo para cruzar sin perder suministros.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_reparar_transparent.png",
                    FlagBridgeRepaired,
                    Reward(experience: 5),
                    nodes["node_cruce_central"],
                    nodes["node_bosque_aldheron"],
                    nodes["node_torre_vigia"],
                    wood: 1,
                    iron: 1,
                    stamina: 2,
                    hours: 2),
                Option(
                    "Forzar el cruce",
                    "Cruzar igual. No bloquea la demo, pero consume energia y tiempo.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    FlagBridgeRepaired,
                    null,
                    nodes["node_cruce_central"],
                    nodes["node_bosque_aldheron"],
                    nodes["node_torre_vigia"],
                    stamina: 3,
                    hours: 2),
                Option(
                    "Volver a Valdoran",
                    "No cambia nada. Mejor preparar recursos antes de cruzar.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_pueblo_transparent.png",
                    null,
                    null)
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureBossClueEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["node_torre_vigia"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Torre vigia abandonada",
            eventType = "Descubrimiento",
            description = "Desde la torre se ven marcas recientes hacia las montanas. Alguien esta usando el viejo portal como refugio.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_ruins_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Revisar la torre",
                    "Encuentra la pista del jefe y desbloquea la ruta hacia Collinasombra y la mina.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    FlagBossClueFound,
                    Reward(experience: 10),
                    nodes["node_mina_hierro"],
                    nodes["node_collinasombra"],
                    hours: 1),
                Option(
                    "Saquear suministros",
                    "Tomar recursos menores. La pista queda pendiente para volver luego.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_recursos_transparent.png",
                    null,
                    Reward(wood: 1, stone: 1),
                    null,
                    hours: 1,
                    completesEvent: false),
                Option(
                    "Irse",
                    "No cambia nada.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    null,
                    null,
                    null,
                    completesEvent: false)
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureCollinasombraEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["node_collinasombra"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Aldea de Collinasombra",
            eventType = "Advertencia",
            description = "Los aldeanos hablan de fuego azul en las ruinas. Las heridas no parecen hechas por acero.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_village_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Escuchar advertencias",
                    "La caravana confirma que el jefe usa dano magico y abre el camino al portal.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_pueblo_transparent.png",
                    FlagBossRouteUnlocked,
                    Reward(experience: 10),
                    nodes["node_portal_excavadores"]),
                Option(
                    "Pedir ayuda menor",
                    "Comprar una racion antes de seguir.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_recursos_transparent.png",
                    null,
                    Reward(food: 1),
                    null,
                    gold: 5,
                    completesEvent: false),
                Option(
                    "Volver a preparar",
                    "No cambia nada.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    null,
                    null,
                    null,
                    completesEvent: false)
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureBossEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["node_portal_excavadores"];
        node.unlockOnCombatVictory.Add(nodes["node_frontera_abierta"]);
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Portal de los Excavadores",
            eventType = "Jefe",
            description = "El aire vibra con calor azul. El demonio menor espera entre piedras quebradas.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_ruins_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Enfrentar al demonio menor",
                    "Inicia el combate final de la demo Tier 1.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_combate_transparent.png",
                    null,
                    null,
                    null,
                    requiredFlag: FlagBossRouteUnlocked,
                    startsCombat: true,
                    battleGroupId: "encounter_demonio_menor_t1")
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static void ConfigureFinalEvent(Dictionary<string, WorldMapNode> nodes)
    {
        WorldMapNode node = nodes["node_frontera_abierta"];
        node.hasEvent = true;
        node.nodeEvent = new WorldMapEvent
        {
            title = "Frontera abierta",
            eventType = "Cierre de Demo",
            description = "Con el demonio derrotado, la caravana puede seguir mas alla del valle.",
            illustration = LoadSprite("Assets/Sprites/GeneratedUI/Ilistrations/event_illustration_caravan_road_transparent.png"),
            options = new List<WorldMapEventOption>
            {
                Option(
                    "Cerrar expedicion",
                    "Marca la frontera como abierta.",
                    "Assets/Sprites/GeneratedUI/Iconos/event_icon_camino_transparent.png",
                    FlagFrontierOpen,
                    Reward(gold: 10, experience: 20),
                    null,
                    requiredFlag: FlagBossDefeated)
            }
        };

        EditorUtility.SetDirty(node);
    }

    private static WorldMapEventOption Option(
        string title,
        string description,
        string iconPath,
        string flag,
        RewardData reward,
        WorldMapNode unlockA = null,
        WorldMapNode unlockB = null,
        WorldMapNode unlockC = null,
        int gold = 0,
        int food = 0,
        int wood = 0,
        int stone = 0,
        int iron = 0,
        int leather = 0,
        int crystals = 0,
        int stamina = 0,
        int hours = 0,
        string requiredFlag = null,
        bool completesEvent = true,
        bool startsCombat = false,
        string battleGroupId = "")
    {
        WorldMapEventOption option = new WorldMapEventOption
        {
            title = title,
            description = description,
            icon = LoadSprite(iconPath),
            goldCost = gold,
            foodCost = food,
            woodCost = wood,
            stoneCost = stone,
            ironCost = iron,
            leatherCost = leather,
            crystalsCost = crystals,
            caravanStaminaCost = stamina,
            hoursCost = hours,
            flagToAdd = flag,
            reward = reward,
            completesEvent = completesEvent,
            startsCombat = startsCombat,
            battleGroupId = string.IsNullOrWhiteSpace(battleGroupId) ? "TestBattle" : battleGroupId
        };

        if (unlockA != null)
            option.nodesToUnlock.Add(unlockA);

        if (unlockB != null)
            option.nodesToUnlock.Add(unlockB);

        if (unlockC != null)
            option.nodesToUnlock.Add(unlockC);

        if (!string.IsNullOrWhiteSpace(requiredFlag))
            option.requiredFlags.Add(requiredFlag);

        return option;
    }

    private static RewardData Reward(int gold = 0, int food = 0, int wood = 0, int stone = 0, int iron = 0, int leather = 0, int crystals = 0, int experience = 0)
    {
        return new RewardData
        {
            gold = gold,
            food = food,
            wood = wood,
            stone = stone,
            iron = iron,
            leather = leather,
            crystals = crystals,
            experience = experience
        };
    }

    private static BattleEncounterSO LoadEncounter(string encounterId)
    {
        string[] guids = AssetDatabase.FindAssets("t:BattleEncounterSO", new[] { "Assets/GameData/Battles" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BattleEncounterSO encounter = AssetDatabase.LoadAssetAtPath<BattleEncounterSO>(path);

            if (encounter != null && encounter.encounterId == encounterId)
                return encounter;
        }

        return null;
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
        GameObject lineObject = new GameObject($"Road_{index + 1:00}_{fromNode.nodeId}_to_{toNode.nodeId}");
        Undo.RegisterCreatedObjectUndo(lineObject, "Create demo map connection line");
        lineObject.transform.SetParent(parent);
        lineObject.transform.position = Vector3.zero;

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));

        WorldMapConnectionLine connectionLine = lineObject.AddComponent<WorldMapConnectionLine>();
        connectionLine.fromNode = fromNode;
        connectionLine.toNode = toNode;
        connectionLine.blockedColor = new Color(0.42f, 0.08f, 0.06f, 0.75f);
        connectionLine.undiscoveredColor = new Color(0.25f, 0.22f, 0.18f, 0.5f);
        connectionLine.availableColor = new Color(0.95f, 0.72f, 0.24f, 0.95f);
        connectionLine.visitedColor = new Color(1f, 0.86f, 0.32f, 1f);
        connectionLine.currentColor = new Color(1f, 0.93f, 0.55f, 1f);
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

        manager.currentNode = nodesById["node_valdoran"];
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
