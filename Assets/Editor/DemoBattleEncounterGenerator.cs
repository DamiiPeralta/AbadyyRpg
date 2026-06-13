using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DemoBattleEncounterGenerator
{
    private const string BattlesFolder = "Assets/GameData/Battles";
    private const string EnemiesFolder = "Assets/GameData/Enemies";
    private const string DatabasePath = BattlesFolder + "/BattleEncounterDatabase.asset";

    [MenuItem("CaravanRPG/Game Data/Create Demo Battle Encounters")]
    public static void CreateDemoBattleEncounters()
    {
        EnsureFolder("Assets/GameData");
        EnsureFolder(BattlesFolder);

        List<BattleEncounterSO> encounters = new List<BattleEncounterSO>
        {
            CreateEncounter("Encounter_Road_Ambush_01", "road_ambush_01", "Emboscada del Camino", "Primer combate facil contra bandidos.", ("Enemy_Bandido_Raso", 2)),
            CreateEncounter("Encounter_Hungry_Beasts_01", "hungry_beasts_01", "Bestias Hambrientas", "Bestias fragiles y rapidas.", ("Enemy_Perro_Hambriento", 2), ("Enemy_Cuervo_Carronero", 1)),
            CreateEncounter("Encounter_Bandit_Patrol_01", "bandit_patrol_01", "Patrulla Bandida", "Encuentro comun para probar taunt.", ("Enemy_Saqueador", 1), ("Enemy_Ballestero", 1)),
            CreateEncounter("Encounter_Bandit_Shield_01", "bandit_shield_01", "Escuderos del Camino", "Introduce armadura fisica alta.", ("Enemy_Escudero_Bandido", 1), ("Enemy_Bandido_Raso", 1), ("Enemy_Cuervo_Carronero", 1)),
            CreateEncounter("Encounter_Cult_Rites_01", "cult_rites_01", "Rito Menor del Culto", "Dano magico y soporte enemigo.", ("Enemy_Cultista_Menor", 1), ("Enemy_Acolito_Corrupto", 1)),
            CreateEncounter("Encounter_Deserter_Group_01", "deserter_group_01", "Grupo Desertor", "Combate fisico de media demo.", ("Enemy_Maton_Desertor", 1), ("Enemy_Saqueador", 1), ("Enemy_Ballestero", 1)),
            CreateEncounter("Encounter_Mountain_Beast_01", "mountain_beast_01", "Bestia del Monte", "Presion fisica fuerte.", ("Enemy_Bestia_Del_Monte", 1), ("Enemy_Perro_Hambriento", 2)),
            CreateEncounter("Encounter_Elite_Captain_01", "elite_captain_01", "Capitan Desertor", "Elite humano.", ("Enemy_Capitan_Desertor", 1), ("Enemy_Bandido_Raso", 1), ("Enemy_Ballestero", 1)),
            CreateEncounter("Encounter_Elite_Witch_01", "elite_witch_01", "Bruja del Circulo", "Elite magico.", ("Enemy_Bruja_Del_Circulo", 1), ("Enemy_Cultista_Menor", 2)),
            CreateEncounter("Encounter_Boss_RoadLord_01", "boss_road_lord_01", "Senor del Camino", "Jefe regional.", ("Enemy_Senor_Del_Camino", 1)),
            CreateEncounter("Encounter_Final_AbbeyThing_01", "final_abbey_thing_01", "La Cosa Bajo la Abadia", "Jefe final de demo.", ("Enemy_Cosa_Bajo_Abadia", 1))
        };

        BattleEncounterDatabase database = AssetDatabase.LoadAssetAtPath<BattleEncounterDatabase>(DatabasePath);

        if (database == null)
        {
            database = ScriptableObject.CreateInstance<BattleEncounterDatabase>();
            AssetDatabase.CreateAsset(database, DatabasePath);
        }

        database.encounters = encounters;
        EditorUtility.SetDirty(database);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Encuentros de batalla de demo creados/actualizados en Assets/GameData/Battles.");
    }

    private static BattleEncounterSO CreateEncounter(string assetName, string encounterId, string encounterName, string description, params (string enemyAssetName, int amount)[] enemyEntries)
    {
        BattleEncounterSO encounter = LoadOrCreate<BattleEncounterSO>($"{BattlesFolder}/{assetName}.asset");
        encounter.encounterId = encounterId;
        encounter.encounterName = encounterName;
        encounter.description = description;
        encounter.enemies = new List<BattleEnemyEntry>();
        encounter.reward = new RewardData();

        foreach ((string enemyAssetName, int amount) entry in enemyEntries)
        {
            EnemyDefinitionSO enemy = AssetDatabase.LoadAssetAtPath<EnemyDefinitionSO>($"{EnemiesFolder}/{entry.enemyAssetName}.asset");

            if (enemy == null)
                continue;

            encounter.enemies.Add(new BattleEnemyEntry
            {
                enemy = enemy,
                amount = Mathf.Max(1, entry.amount)
            });

            AddEnemyReward(encounter.reward, enemy.reward, Mathf.Max(1, entry.amount));
        }

        EditorUtility.SetDirty(encounter);
        return encounter;
    }

    private static void AddEnemyReward(RewardData target, RewardData source, int multiplier)
    {
        if (target == null || source == null)
            return;

        target.gold += source.gold * multiplier;
        target.food += source.food * multiplier;
        target.wood += source.wood * multiplier;
        target.iron += source.iron * multiplier;
        target.leather += source.leather * multiplier;
        target.experience += source.experience * multiplier;

        if (source.items == null)
            return;

        foreach (RewardItemEntry item in source.items)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.itemId) || item.amount <= 0)
                continue;

            target.items.Add(new RewardItemEntry
            {
                itemId = item.itemId,
                amount = item.amount * multiplier
            });
        }
    }

    private static T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (asset != null)
            return asset;

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = System.IO.Path.GetFileName(path);

        if (!string.IsNullOrWhiteSpace(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folder);
    }
}
