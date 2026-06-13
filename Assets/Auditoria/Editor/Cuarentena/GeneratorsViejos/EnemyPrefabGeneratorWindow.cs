using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyPrefabGeneratorWindow : EditorWindow
{
    private const string AbilityFolder = "Assets/Abilities/StarterParty";
    private const string OutputFolder = "Assets/Prefabs/Enemies";

    private GameObject baseEnemyPrefab;

    [MenuItem("CaravanRPG/Create Enemy Prefabs From Base")]
    public static void OpenWindow()
    {
        GetWindow<EnemyPrefabGeneratorWindow>("Enemy Prefab Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enemy Prefab Generator", EditorStyles.boldLabel);

        baseEnemyPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Base Enemy Prefab",
            baseEnemyPrefab,
            typeof(GameObject),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Create Enemy Prefabs"))
        {
            if (baseEnemyPrefab == null)
            {
                Debug.LogError("Asigná un prefab base primero.");
                return;
            }

            CreateEnemyPrefabs();
        }
    }

    private void CreateEnemyPrefabs()
    {
        EnsureFolderExists();

        CreateEnemyPrefab(
            "Skeleton",
            "Esqueleto",
            "Resistente físico, puede aturdir.",
            10, 8, 2, 12,
            15, 0,
            new List<AbilitySO>
            {
                LoadAbility("Skeleton_Bone_Strike"),
                LoadAbility("Skeleton_Stun_Blow")
            }
        );

        CreateEnemyPrefab(
            "Rat",
            "Rata",
            "Muy rápida y frágil. Molesta con veneno.",
            6, 18, 1, 6,
            0, 0,
            new List<AbilitySO>
            {
                LoadAbility("Rat_Bite"),
                LoadAbility("Rat_Poison_Bite")
            }
        );

        CreateEnemyPrefab(
            "LesserDemon",
            "Demonio Menor",
            "Enemigo híbrido. Hace daño físico y mágico.",
            10, 10, 14, 10,
            5, 10,
            new List<AbilitySO>
            {
                LoadAbility("Lesser_Demon_Claw"),
                LoadAbility("Lesser_Demon_Dark_Bolt")
            }
        );

        CreateEnemyPrefab(
            "Worm",
            "Gusano",
            "Lento, resistente y venenoso.",
            14, 4, 4, 18,
            10, 5,
            new List<AbilitySO>
            {
                LoadAbility("Worm_Bite"),
                LoadAbility("Worm_Toxic_Spit")
            }
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Prefabs enemigos creados en: " + OutputFolder);
    }

    private void CreateEnemyPrefab(
        string prefabName,
        string unitName,
        string description,
        int strength,
        int dexterity,
        int intelligence,
        int constitution,
        int physicalArmor,
        int magicalArmor,
        List<AbilitySO> abilities
    )
    {
        string path = $"{OutputFolder}/{prefabName}.prefab";

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(baseEnemyPrefab);

        if (instance == null)
        {
            instance = Instantiate(baseEnemyPrefab);
        }

        instance.name = prefabName;

        UnitData unitData = instance.GetComponent<UnitData>();

        if (unitData == null)
        {
            unitData = instance.AddComponent<UnitData>();
        }

        unitData.unitName = unitName;
        unitData.description = description;

        unitData.strength = strength;
        unitData.dexterity = dexterity;
        unitData.intelligence = intelligence;
        unitData.constitution = constitution;

        unitData.maxPhysicalArmor = physicalArmor;
        unitData.maxMagicalArmor = magicalArmor;

        unitData.startHP = -1;
        unitData.startPhysicalArmor = -1;
        unitData.startMagicalArmor = -1;

        unitData.abilities = CleanAbilityList(abilities);

        PrefabUtility.SaveAsPrefabAsset(instance, path);
        DestroyImmediate(instance);

        Debug.Log($"Creado prefab enemigo: {path}");
    }

    private AbilitySO LoadAbility(string assetName)
    {
        string path = $"{AbilityFolder}/{assetName}.asset";
        AbilitySO ability = AssetDatabase.LoadAssetAtPath<AbilitySO>(path);

        if (ability == null)
        {
            Debug.LogWarning($"No se encontró ability: {path}");
        }

        return ability;
    }

    private List<AbilitySO> CleanAbilityList(List<AbilitySO> list)
    {
        List<AbilitySO> clean = new List<AbilitySO>();

        foreach (AbilitySO ability in list)
        {
            if (ability != null)
                clean.Add(ability);
        }

        return clean;
    }

    private void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        if (!AssetDatabase.IsValidFolder(OutputFolder))
            AssetDatabase.CreateFolder("Assets/Prefabs", "Enemies");
    }
}