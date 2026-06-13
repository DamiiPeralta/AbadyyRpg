using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DemoMercenaryPartyGenerator
{
    private const string CharactersFolder = "Assets/GameData/Characters";
    private const string MercenariesFolder = "Assets/GameData/Characters/Mercenaries";
    private const int DemoLevel = 3;

    [MenuItem("CaravanRPG/Game Data/Create Demo Mercenary Party")]
    public static void CreateDemoMercenaryParty()
    {
        DemoCharacterClassGenerator.CreateDemoClasses();
        DemoAbilitySetGenerator.CreateDemoAbilitySet();
        DemoItemSetGenerator.CreateDemoItemSet();

        EnsureFolder("Assets/GameData");
        EnsureFolder(CharactersFolder);
        EnsureFolder(MercenariesFolder);

        GameObject defender = CreateMercenary(
            "Merc_Defensor_Base",
            "Defensor",
            "Defensor de Valdoran",
            "Tanque inicial. Usa placas oxidadas, sostiene la linea y recupera armadura fisica.",
            "Class_Defensor",
            "Weapon_Rusty_Sword",
            "Armor_Rusty_Plates",
            "Assets/Prefabs/Mercs/Caballero.prefab");

        GameObject assassin = CreateMercenary(
            "Merc_Asesino_Base",
            "Asesino",
            "Asesino de Claravalle",
            "DPS fisico inicial. Usa daga mellada, remata enemigos heridos y baja su amenaza.",
            "Class_Asesino",
            "Weapon_Dull_Dagger",
            "Armor_Worn_Leather",
            "Assets/Prefabs/Mercs/Asesino.prefab");

        GameObject mage = CreateMercenary(
            "Merc_MagoDelCirculo_Base",
            "Mago del Circulo",
            "Mago del Circulo",
            "DPS magico inicial. Usa baston partido, marca objetivos y drena esencia cuando peligra.",
            "Class_MagoDelCirculo",
            "Weapon_Broken_Staff",
            "Armor_Torn_Robe",
            "Assets/Prefabs/Mercs/Mago.prefab");

        GameObject acolyte = CreateMercenary(
            "Merc_Acolita_Base",
            "Acolita",
            "Acolita de la Vigilia",
            "Soporte inicial. Usa simbolo quebrado, cura heridas y remienda armadura fisica.",
            "Class_Acolita",
            "Weapon_Broken_Symbol",
            "Armor_Patched_Vestment",
            "Assets/Prefabs/Mercs/Mago.prefab");

        AssignToOpenSceneStartingSetup(new List<GameObject> { defender, assassin, mage, acolyte });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Party demo de mercenarios creada/actualizada con equipo tier 1, habilidades y tacticas de clase.");
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
        ConsumableItem potion = LoadConsumable("Consumable_Health_Potion");

        unitData.unitName = unitName;
        unitData.description = description;
        unitData.characterClass = characterClass;
        unitData.useClassBaseStats = true;
        unitData.useClassProgression = true;
        unitData.includeClassAbilities = true;
        unitData.includeClassSuggestedTactics = true;
        unitData.level = DemoLevel;
        unitData.experience = 0;
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
        unitData.consumable1 = potion;
        unitData.consumable2 = null;
        unitData.abilities.Clear();
        unitData.tactics.Clear();

        if (unitData.battleSprite == null && spriteRenderer.sprite != null)
            unitData.battleSprite = spriteRenderer.sprite;

        if (unitData.icon == null)
            unitData.icon = unitData.battleSprite;

        if (unitData.battleSprite != null)
            spriteRenderer.sprite = unitData.battleSprite;

        ApplyAcolyteTintIfNeeded(objectName, spriteRenderer);

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

    private static void ApplyAcolyteTintIfNeeded(string objectName, SpriteRenderer spriteRenderer)
    {
        if (objectName != "Acolita" || spriteRenderer == null)
            return;

        spriteRenderer.color = new Color(0.95f, 0.9f, 0.75f, 1f);
    }

    private static CharacterClassSO LoadClass(string assetName)
    {
        return AssetDatabase.LoadAssetAtPath<CharacterClassSO>($"Assets/GameData/Classes/{assetName}.asset");
    }

    private static Weapon LoadWeapon(string assetName)
    {
        return AssetDatabase.LoadAssetAtPath<Weapon>($"Assets/GameData/Items/Weapons/{assetName}.asset");
    }

    private static Armor LoadArmor(string assetName)
    {
        return AssetDatabase.LoadAssetAtPath<Armor>($"Assets/GameData/Items/Armors/{assetName}.asset");
    }

    private static ConsumableItem LoadConsumable(string assetName)
    {
        return AssetDatabase.LoadAssetAtPath<ConsumableItem>($"Assets/GameData/Items/Consumables/{assetName}.asset");
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
