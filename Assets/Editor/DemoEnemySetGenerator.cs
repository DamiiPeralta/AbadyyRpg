using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DemoEnemySetGenerator
{
    private const string EnemiesFolder = "Assets/GameData/Enemies";

    [MenuItem("CaravanRPG/Game Data/Create Demo Enemy Set")]
    public static void CreateDemoEnemySet()
    {
        EnsureFolder("Assets/GameData");
        EnsureFolder(EnemiesFolder);

        CreateNormalEnemies();
        CreateEliteEnemies();
        CreateBosses();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Enemigos de demo creados/actualizados en Assets/GameData/Enemies.");
    }

    private static void CreateNormalEnemies()
    {
        CreateEnemy("Enemy_Bandido_Raso", "bandido_raso", "Bandido Raso", EnemyTier.Basic, EnemyKind.Human, 1, 20, 8, 8, 2, 7, 80, 0, 10, 0, true, 6, 0, 1, 0, 0);
        CreateEnemy("Enemy_Perro_Hambriento", "perro_hambriento", "Perro Hambriento", EnemyTier.Basic, EnemyKind.Beast, 1, 15, 7, 12, 1, 6, 90, 0, 5, 0, true, 0, 1, 0, 0, 0);
        CreateEnemy("Enemy_Cuervo_Carronero", "cuervo_carronero", "Cuervo Carronero", EnemyTier.Basic, EnemyKind.Beast, 1, 15, 5, 14, 2, 5, 70, 0, 0, 8, false, 0, 0, 0, 0, 1);

        CreateEnemy("Enemy_Saqueador", "saqueador", "Saqueador", EnemyTier.Common, EnemyKind.Human, 2, 35, 12, 7, 2, 10, 90, 0, 25, 5, true, 12, 0, 0, 1, 0);
        CreateEnemy("Enemy_Ballestero", "ballestero", "Ballestero", EnemyTier.Common, EnemyKind.Human, 2, 35, 10, 11, 2, 7, 90, 0, 12, 5, false, 10, 0, 1, 1, 0);
        CreateEnemy("Enemy_Escudero_Bandido", "escudero_bandido", "Escudero Bandido", EnemyTier.Common, EnemyKind.Human, 2, 40, 8, 6, 2, 13, 100, 0, 55, 8, true, 14, 0, 0, 2, 0);
        CreateEnemy("Enemy_Cultista_Menor", "cultista_menor", "Cultista Menor", EnemyTier.Common, EnemyKind.Cult, 2, 40, 3, 8, 12, 7, 50, 100, 5, 35, false, 8, 0, 0, 0, 1);

        CreateEnemy("Enemy_Maton_Desertor", "maton_desertor", "Maton Desertor", EnemyTier.Dangerous, EnemyKind.Human, 3, 55, 14, 9, 3, 12, 110, 0, 40, 10, true, 20, 0, 1, 2, 0);
        CreateEnemy("Enemy_Bestia_Del_Monte", "bestia_del_monte", "Bestia del Monte", EnemyTier.Dangerous, EnemyKind.Beast, 3, 60, 16, 10, 1, 14, 120, 0, 30, 0, true, 0, 2, 0, 0, 0);
        CreateEnemy("Enemy_Acolito_Corrupto", "acolito_corrupto", "Acolito Corrupto", EnemyTier.Dangerous, EnemyKind.Cult, 3, 60, 4, 9, 14, 10, 60, 120, 10, 45, false, 12, 0, 0, 0, 2);
    }

    private static void CreateEliteEnemies()
    {
        CreateEnemy("Enemy_Capitan_Desertor", "capitan_desertor", "Capitan Desertor", EnemyTier.Elite, EnemyKind.Human, 4, 140, 18, 12, 4, 16, 140, 20, 90, 20, true, 55, 0, 2, 3, 0);
        CreateEnemy("Enemy_Bruja_Del_Circulo", "bruja_del_circulo", "Bruja del Circulo", EnemyTier.Elite, EnemyKind.Cult, 4, 140, 5, 12, 18, 12, 70, 160, 20, 95, false, 35, 0, 0, 0, 4);
    }

    private static void CreateBosses()
    {
        CreateEnemy("Enemy_Senor_Del_Camino", "senor_del_camino", "Senor del Camino", EnemyTier.Boss, EnemyKind.Human, 5, 240, 22, 12, 6, 20, 170, 40, 150, 45, true, 100, 2, 3, 4, 1);
        CreateEnemy("Enemy_Cosa_Bajo_Abadia", "cosa_bajo_abadia", "La Cosa Bajo la Abadia", EnemyTier.FinalBoss, EnemyKind.Aberration, 5, 0, 20, 14, 20, 22, 180, 180, 130, 130, true, 150, 0, 3, 3, 6);
    }

    private static void CreateEnemy(
        string assetName,
        string enemyId,
        string enemyName,
        EnemyTier tier,
        EnemyKind kind,
        int level,
        int xp,
        int strength,
        int dexterity,
        int intelligence,
        int constitution,
        int stamina,
        int mana,
        int physicalArmor,
        int magicalArmor,
        bool isFrontLine,
        int gold,
        int food,
        int wood,
        int iron,
        int leatherOrCrystals)
    {
        EnemyDefinitionSO enemy = LoadOrCreate<EnemyDefinitionSO>($"{EnemiesFolder}/{assetName}.asset");
        enemy.enemyId = enemyId;
        enemy.enemyName = enemyName;
        enemy.tier = tier;
        enemy.kind = kind;
        enemy.description = BuildDescription(tier, kind);
        enemy.level = level;
        enemy.experienceReward = xp;
        enemy.strength = strength;
        enemy.dexterity = dexterity;
        enemy.intelligence = intelligence;
        enemy.constitution = constitution;
        enemy.maxStamina = stamina;
        enemy.maxMana = mana;
        enemy.physicalArmor = physicalArmor;
        enemy.magicalArmor = magicalArmor;
        enemy.isFrontLine = isFrontLine;
        enemy.battleSprite = LoadEnemySprite(enemyName, kind);
        enemy.icon = enemy.battleSprite;
        enemy.visualPrefab = null;
        enemy.reward = BuildReward(xp, gold, food, wood, iron, kind == EnemyKind.Cult || kind == EnemyKind.Aberration ? 0 : leatherOrCrystals, kind == EnemyKind.Cult || kind == EnemyKind.Aberration ? leatherOrCrystals : 0);
        enemy.abilities = BuildAbilities(kind, tier);
        enemy.tactics = BuildTactics(enemy.abilities);
        EditorUtility.SetDirty(enemy);
    }

    private static Sprite LoadEnemySprite(string enemyName, EnemyKind kind)
    {
        string path = GetEnemySpritePath(enemyName);
        Sprite sprite = !string.IsNullOrWhiteSpace(path) ? AssetDatabase.LoadAssetAtPath<Sprite>(path) : null;

        if (sprite != null)
            return sprite;

        switch (kind)
        {
            case EnemyKind.Beast:
                return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GeneratedUI/enemigos/Perro.png");
            case EnemyKind.Cult:
                return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GeneratedUI/enemigos/Cultista Menor.png");
            case EnemyKind.Aberration:
                return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GeneratedUI/enemigos/la cosa bajo la abadia.png");
            default:
                return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GeneratedUI/enemigos/Bandido.png");
        }
    }

    private static string GetEnemySpritePath(string enemyName)
    {
        switch (enemyName)
        {
            case "Bandido Raso":
                return "Assets/Sprites/GeneratedUI/enemigos/Bandido.png";
            case "Perro Hambriento":
                return "Assets/Sprites/GeneratedUI/enemigos/Perro.png";
            case "Cuervo Carronero":
                return "Assets/Sprites/GeneratedUI/enemigos/Cuervo.png";
            case "Saqueador":
                return "Assets/Sprites/GeneratedUI/enemigos/Saqueador.png";
            case "Ballestero":
                return "Assets/Sprites/GeneratedUI/enemigos/Ballestero.png";
            case "Escudero Bandido":
                return "Assets/Sprites/GeneratedUI/enemigos/Bandido.png";
            case "Cultista Menor":
                return "Assets/Sprites/GeneratedUI/enemigos/Cultista Menor.png";
            case "Maton Desertor":
                return "Assets/Sprites/GeneratedUI/enemigos/Maton Desertor.png";
            case "Bestia del Monte":
                return "Assets/Sprites/GeneratedUI/enemigos/Bestia del Monte.png";
            case "Acolito Corrupto":
                return "Assets/Sprites/GeneratedUI/enemigos/Acolito Corrupto.png";
            case "Capitan Desertor":
                return "Assets/Sprites/GeneratedUI/enemigos/Capitan Desertor.png";
            case "Bruja del Circulo":
                return "Assets/Sprites/GeneratedUI/enemigos/Bruja del Circulo.png";
            case "Senor del Camino":
                return "Assets/Sprites/GeneratedUI/enemigos/se?or del camino.png";
            case "La Cosa Bajo la Abadia":
                return "Assets/Sprites/GeneratedUI/enemigos/la cosa bajo la abadia.png";
            default:
                return null;
        }
    }

    private static RewardData BuildReward(int xp, int gold, int food, int wood, int iron, int leather, int crystals)
    {
        RewardData reward = new RewardData
        {
            experience = xp,
            gold = gold,
            food = food,
            wood = wood,
            iron = iron,
            leather = leather
        };

        if (crystals > 0)
            reward.items.Add(new RewardItemEntry { itemId = "Crystal_Shard", amount = crystals });

        return reward;
    }

    private static List<AbilitySO> BuildAbilities(EnemyKind kind, EnemyTier tier)
    {
        List<AbilitySO> abilities = new List<AbilitySO>();

        switch (kind)
        {
            case EnemyKind.Human:
                AddAbility(abilities, "Ability_Assassin_QuickCut");
                if (tier >= EnemyTier.Dangerous)
                    AddAbility(abilities, "Ability_Defender_GuardBreaker");
                break;
            case EnemyKind.Beast:
                AddAbility(abilities, "Ability_Assassin_QuickCut");
                if (tier >= EnemyTier.Dangerous)
                    AddAbility(abilities, "Ability_Assassin_Bleed");
                break;
            case EnemyKind.Cult:
                AddAbility(abilities, "Ability_Mage_ArcaneDart");
                if (tier >= EnemyTier.Dangerous)
                    AddAbility(abilities, "Ability_Mage_CircleMark");
                if (tier >= EnemyTier.Elite)
                    AddAbility(abilities, "Ability_Mage_MagicPulse");
                break;
            case EnemyKind.Aberration:
                AddAbility(abilities, "Ability_Mage_MagicPulse");
                AddAbility(abilities, "Ability_Assassin_Bleed");
                AddAbility(abilities, "Ability_Mage_EssenceDrain");
                break;
        }

        return abilities;
    }

    private static List<TacticRule> BuildTactics(List<AbilitySO> abilities)
    {
        List<TacticRule> tactics = new List<TacticRule>();

        if (abilities == null || abilities.Count == 0)
            return tactics;

        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] == null)
                continue;

            tactics.Add(new TacticRule
            {
                priority = i + 1,
                isActive = true,
                conditionType = TacticConditionType.Always,
                thresholdPercent = 100,
                ability = abilities[i]
            });
        }

        return tactics;
    }

    private static void AddAbility(List<AbilitySO> abilities, string assetName)
    {
        AbilitySO ability = AssetDatabase.LoadAssetAtPath<AbilitySO>($"Assets/GameData/Abilities/{assetName}.asset");

        if (ability != null && !abilities.Contains(ability))
            abilities.Add(ability);
    }

    private static string BuildDescription(EnemyTier tier, EnemyKind kind)
    {
        return $"{kind} de tier {(int)tier}. Definicion inicial para balance de demo.";
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
