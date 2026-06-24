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

        CreateTier1Enemies();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Enemigos de demo creados/actualizados en Assets/GameData/Enemies.");
    }

    private static void CreateTier1Enemies()
    {
        CreateEnemy("Enemy_Rata_Gigante_T1", "enemy_rata_gigante_t1", "Rata Gigante", EnemyTier.Basic, EnemyKind.Beast, 1, 8, 1, 3, 0, 4, 6, 0, 1, 0, true, 3, 1, 0, 0, 0, 1, 0);
        CreateEnemy("Enemy_Esqueleto_T1", "enemy_esqueleto_t1", "Esqueleto", EnemyTier.Common, EnemyKind.Aberration, 2, 16, 2, 1, 0, 6, 8, 0, 5, 1, true, 6, 0, 0, 1, 1, 0, 0);
        CreateEnemy("Enemy_Gusano_T1", "enemy_gusano_t1", "Gusano", EnemyTier.Dangerous, EnemyKind.Beast, 3, 28, 3, 1, 1, 8, 10, 0, 3, 4, true, 10, 1, 0, 0, 0, 1, 1);
        CreateEnemy("Enemy_Demonio_Menor_T1", "enemy_demonio_menor_t1", "Demonio Menor", EnemyTier.Boss, EnemyKind.Cult, 4, 60, 2, 2, 4, 14, 8, 14, 5, 8, true, 25, 0, 0, 0, 1, 1, 3);
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
        int stone,
        int iron,
        int leather,
        int crystals)
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
        enemy.reward = BuildReward(xp, gold, food, wood, stone, iron, leather, crystals);
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
                return AssetDatabase.LoadAssetAtPath<Sprite>(enemyName == "Gusano" ? "Assets/Sprites/GeneratedUI/enemigos/Bestia del Monte.png" : "Assets/Sprites/GeneratedUI/enemigos/Perro.png");
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
            case "Rata Gigante":
                return "Assets/Sprites/GeneratedUI/enemigos/Perro.png";
            case "Esqueleto":
                return "Assets/Sprites/GeneratedUI/enemigos/Bandido.png";
            case "Gusano":
                return "Assets/Sprites/GeneratedUI/enemigos/Bestia del Monte.png";
            case "Demonio Menor":
                return "Assets/Sprites/GeneratedUI/enemigos/Cultista Menor.png";
            default:
                return null;
        }
    }

    private static RewardData BuildReward(int xp, int gold, int food, int wood, int stone, int iron, int leather, int crystals)
    {
        RewardData reward = new RewardData
        {
            experience = xp,
            gold = gold,
            food = food,
            wood = wood,
            stone = stone,
            iron = iron,
            leather = leather,
            crystals = crystals
        };

        return reward;
    }

    private static List<AbilitySO> BuildAbilities(EnemyKind kind, EnemyTier tier)
    {
        List<AbilitySO> abilities = new List<AbilitySO>();

        switch (kind)
        {
            case EnemyKind.Human:
                break;
            case EnemyKind.Beast:
                AddAbility(abilities, tier >= EnemyTier.Dangerous ? "Ability_Enemy_Deep_Bite_T1" : "Ability_Enemy_Rat_Bite_T1");
                break;
            case EnemyKind.Cult:
                AddAbility(abilities, "Ability_Enemy_Infernal_Lash_T1");
                AddAbility(abilities, "Ability_Enemy_Claw_T1");
                break;
            case EnemyKind.Aberration:
                AddAbility(abilities, "Ability_Enemy_Rusty_Blow_T1");
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
