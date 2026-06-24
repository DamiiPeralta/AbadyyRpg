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
            CreateEncounter("Encounter_Rata_01_T1", "encounter_rata_01_t1", "Rata solitaria", "Tutorial de combate contra una rata gigante.", ("Enemy_Rata_Gigante_T1", 1)),
            CreateEncounter("Encounter_Ratas_02_T1", "encounter_ratas_02_t1", "Dos ratas gigantes", "Primer desgaste real contra dos enemigos rapidos.", ("Enemy_Rata_Gigante_T1", 2)),
            CreateEncounter("Encounter_Rata_Esqueleto_T1", "encounter_rata_esqueleto_t1", "Rata y esqueleto", "Prueba de prioridad contra desgaste y armadura fisica.", ("Enemy_Rata_Gigante_T1", 1), ("Enemy_Esqueleto_T1", 1)),
            CreateEncounter("Encounter_Esqueleto_01_T1", "encounter_esqueleto_01_t1", "Esqueleto", "Test simple de armadura fisica alta.", ("Enemy_Esqueleto_T1", 1)),
            CreateEncounter("Encounter_Gusano_01_T1", "encounter_gusano_01_t1", "Gusano", "Mini-check de preparacion fisica y sustain.", ("Enemy_Gusano_T1", 1)),
            CreateEncounter("Encounter_Esqueleto_Gusano_T1", "encounter_esqueleto_gusano_t1", "Esqueleto y gusano", "Pelea de preparacion final antes del jefe.", ("Enemy_Esqueleto_T1", 1), ("Enemy_Gusano_T1", 1)),
            CreateEncounter("Encounter_Demonio_Menor_T1", "encounter_demonio_menor_t1", "Demonio menor", "Jefe Tier 1. Combate final de la demo simplificada.", ("Enemy_Demonio_Menor_T1", 1))
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
        target.stone += source.stone * multiplier;
        target.iron += source.iron * multiplier;
        target.leather += source.leather * multiplier;
        target.crystals += source.crystals * multiplier;
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
