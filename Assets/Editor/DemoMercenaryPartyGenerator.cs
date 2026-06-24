using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DemoMercenaryPartyGenerator
{
    private const string CharactersFolder = "Assets/GameData/Characters";
    private const string MercenariesFolder = "Assets/GameData/Characters/Mercenaries";
    private const int DemoLevel = 1;

    [MenuItem("CaravanRPG/Game Data/Create Demo Mercenary Party")]
    public static void CreateDemoMercenaryParty()
    {
        DemoCharacterClassGenerator.CreateDemoClasses();
        DemoAbilitySetGenerator.CreateDemoAbilitySet();
        DemoItemSetGenerator.CreateDemoItemSet();

        EnsureFolder("Assets/GameData");
        EnsureFolder(CharactersFolder);
        EnsureFolder(MercenariesFolder);

        GameObject expedicionario = CreateMercenary(
            "Merc_Defensor_Base",
            "Expedicionario",
            "Expedicionario",
            "Personaje flexible de la demo Tier 1. Empieza sin equipo ni consumibles.",
            string.Empty,
            string.Empty,
            string.Empty,
            "Assets/Prefabs/Mercs/Caballero.prefab");

        AssignToOpenSceneStartingSetup(new List<GameObject> { expedicionario });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Party demo Tier 1 creada/actualizada con 1 expedicionario sin equipo inicial.");
    }

    private static GameObject CreateMercenary(
        string prefabName,
        string objectName,
        string unitName,
        string description,
        string classAssetName,
        string weaponAssetName,
        string armorAssetName,
        string visualTemplatePath)
    {
        string prefabPath = $"{MercenariesFolder}/{prefabName}.prefab";
        GameObject template = AssetDatabase.LoadAssetAtPath<GameObject>(visualTemplatePath);
        GameObject workingObject = template != null
            ? (GameObject)PrefabUtility.InstantiatePrefab(template)
            : new GameObject(objectName);

        workingObject.name = objectName;

        SpriteRenderer spriteRenderer = workingObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = workingObject.AddComponent<SpriteRenderer>();

        UnitView unitView = workingObject.GetComponent<UnitView>();
        if (unitView == null)
            unitView = workingObject.AddComponent<UnitView>();

        UnitData unitData = workingObject.GetComponent<UnitData>();
        if (unitData == null)
            unitData = workingObject.AddComponent<UnitData>();

        CharacterClassSO characterClass = LoadClass(classAssetName);
        Weapon weapon = LoadWeapon(weaponAssetName);
        Armor armor = LoadArmor(armorAssetName);

        unitData.unitName = unitName;
        unitData.description = description;
        unitData.characterClass = characterClass;
        unitData.useClassBaseStats = characterClass != null;
        unitData.useClassProgression = characterClass != null;
        unitData.includeClassAbilities = characterClass != null;
        unitData.includeClassSuggestedTactics = characterClass != null;
        unitData.strength = 2;
        unitData.dexterity = 2;
        unitData.intelligence = 2;
        unitData.constitution = 5;
        unitData.level = DemoLevel;
        unitData.experience = 0;
        unitData.maxLevel = 5;
        unitData.experienceByLevel = new List<int> { 0, 40, 100, 180, 300 };
        unitData.strengthGrowthPerLevel = 0;
        unitData.dexterityGrowthPerLevel = 0;
        unitData.intelligenceGrowthPerLevel = 0;
        unitData.constitutionGrowthPerLevel = 0;
        unitData.staminaGrowthPerLevel = 0;
        unitData.manaGrowthPerLevel = 0;
        unitData.physicalArmorGrowthPerLevel = 0;
        unitData.magicalArmorGrowthPerLevel = 0;
        unitData.levelGrowths = CreateDemoLevelGrowths();
        unitData.maxStamina = 10;
        unitData.maxMana = 10;
        unitData.maxPhysicalArmor = 0;
        unitData.maxMagicalArmor = 0;
        unitData.startHP = -1;
        unitData.startStamina = -1;
        unitData.startMana = -1;
        unitData.startPhysicalArmor = -1;
        unitData.startMagicalArmor = -1;
        unitData.helmet = null;
        unitData.chest = armor;
        unitData.feet = null;
        unitData.hands = null;
        unitData.rightHand = weapon;
        unitData.leftHand = null;
        unitData.ring = null;
        unitData.amulet = null;
        unitData.consumable1 = null;
        unitData.consumable2 = null;
        unitData.abilities.Clear();
        AddAbility(unitData.abilities, "Ability_Generic_PowerStrike_T1");
        AddAbility(unitData.abilities, "Ability_Generic_MagicMissile_T1");
        AddAbility(unitData.abilities, "Ability_Generic_Heal_T1");
        AddAbility(unitData.abilities, "Ability_Generic_Regeneration_T1");
        AddAbility(unitData.abilities, "Ability_Generic_Guard_T1");
        AddAbility(unitData.abilities, "Ability_Generic_MagicShield_T1");
        AddAbility(unitData.abilities, "Ability_Generic_Cleanse_T1");
        AddAbility(unitData.abilities, "Ability_Generic_ArmBreaker_T1");
        unitData.tactics.Clear();

        if (unitData.battleSprite == null && spriteRenderer.sprite != null)
            unitData.battleSprite = spriteRenderer.sprite;

        if (unitData.icon == null)
            unitData.icon = unitData.battleSprite;

        if (unitData.battleSprite != null)
            spriteRenderer.sprite = unitData.battleSprite;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(workingObject, prefabPath);
        Object.DestroyImmediate(workingObject);
        return prefab;
    }

    private static void AssignToOpenSceneStartingSetup(List<GameObject> mercenaryPrefabs)
    {
        BattleSetup[] battleSetups = Object.FindObjectsOfType<BattleSetup>();

        foreach (BattleSetup setup in battleSetups)
        {
            if (setup == null)
                continue;

            if (setup.enemyPositions != null && setup.enemyPositions.Count > 0)
                continue;

            setup.allyPrefabs = new List<GameObject>(mercenaryPrefabs);
            EditorUtility.SetDirty(setup);
            Debug.Log($"Starting party asignada en {setup.gameObject.name}.");
        }

        if (battleSetups.Length > 0)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private static CharacterClassSO LoadClass(string assetName)
    {
        if (string.IsNullOrWhiteSpace(assetName))
            return null;

        return AssetDatabase.LoadAssetAtPath<CharacterClassSO>($"Assets/GameData/Classes/{assetName}.asset");
    }

    private static Weapon LoadWeapon(string assetName)
    {
        if (string.IsNullOrWhiteSpace(assetName))
            return null;

        return AssetDatabase.LoadAssetAtPath<Weapon>($"Assets/GameData/Items/Weapons/{assetName}.asset");
    }

    private static Armor LoadArmor(string assetName)
    {
        if (string.IsNullOrWhiteSpace(assetName))
            return null;

        return AssetDatabase.LoadAssetAtPath<Armor>($"Assets/GameData/Items/Armors/{assetName}.asset");
    }

    private static void AddAbility(List<AbilitySO> abilities, string assetName)
    {
        AbilitySO ability = AssetDatabase.LoadAssetAtPath<AbilitySO>($"Assets/GameData/Abilities/{assetName}.asset");

        if (ability != null)
            abilities.Add(ability);
    }

    private static List<UnitLevelGrowth> CreateDemoLevelGrowths()
    {
        return new List<UnitLevelGrowth>
        {
            new UnitLevelGrowth { strength = 1, constitution = 1, stamina = 1, mana = 1 },
            new UnitLevelGrowth { dexterity = 1, intelligence = 1, stamina = 1, mana = 1 },
            new UnitLevelGrowth { strength = 1, intelligence = 1, constitution = 1 },
            new UnitLevelGrowth { dexterity = 1, stamina = 1, mana = 1 }
        };
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
