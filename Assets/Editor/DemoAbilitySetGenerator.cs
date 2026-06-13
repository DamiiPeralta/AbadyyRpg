using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DemoAbilitySetGenerator
{
    private const string AbilitiesFolder = "Assets/GameData/Abilities";
    private const string ClassesFolder = "Assets/GameData/Classes";

    [MenuItem("CaravanRPG/Game Data/Create Demo Ability Set")]
    public static void CreateDemoAbilitySet()
    {
        EnsureFolder("Assets/GameData");
        EnsureFolder(AbilitiesFolder);

        CreateDefenderAbilities();
        CreateAssassinAbilities();
        CreateMageAbilities();
        CreateAcolyteAbilities();
        UpdateDemoClasses();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Habilidades de demo creadas/actualizadas en Assets/GameData/Abilities.");
    }

    private static void CreateDefenderAbilities()
    {
        AbilitySO shieldBash = CreateAbility("Ability_Defender_ShieldBash", "Golpe de Escudo", "Dano fisico bajo y genera amenaza.", AbilityTarget.Enemy, AbilityTargetMode.Single, 15, 0);
        shieldBash.flatPhysicalDamage = 4;
        shieldBash.physicalPowerMultiplier = 0.6f;
        shieldBash.strengthToPhysicalDamage = 1f;
        shieldBash.customTauntGenerated = 35;

        AbilitySO defensiveStance = CreateAbility("Ability_Defender_DefensiveStance", "Postura Defensiva", "Restaura armadura fisica propia.", AbilityTarget.Self, AbilityTargetMode.Single, 20, 0);
        defensiveStance.restorePhysicalArmor = 35;
        defensiveStance.constitutionToPhysicalArmorRestore = 1.5f;
        defensiveStance.customTauntGenerated = 25;

        AbilitySO provoke = CreateAbility("Ability_Defender_Provoke", "Provocar", "Aumenta mucho la amenaza propia.", AbilityTarget.Self, AbilityTargetMode.Single, 15, 0);
        provoke.selfTauntChange = 60;
        provoke.customTauntGenerated = 0;

        AbilitySO guardBreaker = CreateAbility("Ability_Defender_GuardBreaker", "Romper Guardia", "Dano fisico medio para presionar armadura fisica.", AbilityTarget.Enemy, AbilityTargetMode.Single, 25, 0);
        guardBreaker.flatPhysicalDamage = 8;
        guardBreaker.physicalPowerMultiplier = 0.9f;
        guardBreaker.strengthToPhysicalDamage = 1.5f;
        guardBreaker.customTauntGenerated = 35;
    }

    private static void CreateAssassinAbilities()
    {
        AbilitySO quickCut = CreateAbility("Ability_Assassin_QuickCut", "Corte Rapido", "Dano fisico medio con bajo coste.", AbilityTarget.Enemy, AbilityTargetMode.Single, 12, 0);
        quickCut.flatPhysicalDamage = 5;
        quickCut.physicalPowerMultiplier = 0.8f;
        quickCut.dexterityToPhysicalDamage = 1.5f;
        quickCut.customTauntGenerated = 20;

        AbilitySO execution = CreateAbility("Ability_Assassin_Execution", "Ejecucion", "Dano fisico alto para rematar objetivos.", AbilityTarget.Enemy, AbilityTargetMode.Single, 24, 0);
        execution.flatPhysicalDamage = 14;
        execution.physicalPowerMultiplier = 1.1f;
        execution.dexterityToPhysicalDamage = 2f;
        execution.customTauntGenerated = 35;

        AbilitySO bleed = CreateAbility("Ability_Assassin_Bleed", "Sangrado", "Dano fisico bajo y dano por turno.", AbilityTarget.Enemy, AbilityTargetMode.Single, 18, 0);
        bleed.flatPhysicalDamage = 4;
        bleed.physicalPowerMultiplier = 0.45f;
        bleed.dexterityToPhysicalDamage = 1f;
        bleed.appliesStatusEffect = true;
        bleed.statusEffectType = StatusEffectType.Poison;
        bleed.statusDuration = 3;
        bleed.statusValue = 6;
        bleed.dexterityToStatusValue = 0.5f;
        bleed.customTauntGenerated = 25;

        AbilitySO vanish = CreateAbility("Ability_Assassin_Vanish", "Desaparecer", "Baja amenaza y aplica invisibilidad temporal.", AbilityTarget.Self, AbilityTargetMode.Single, 20, 0);
        vanish.selfTauntChange = -45;
        vanish.appliesStatusEffect = true;
        vanish.statusEffectType = StatusEffectType.Invisibility;
        vanish.statusDuration = 2;
        vanish.statusValue = 35;
        vanish.generatesTaunt = false;
    }

    private static void CreateMageAbilities()
    {
        AbilitySO arcaneDart = CreateAbility("Ability_Mage_ArcaneDart", "Dardo Arcano", "Dano magico medio a un enemigo.", AbilityTarget.Enemy, AbilityTargetMode.Single, 0, 14);
        arcaneDart.flatMagicalDamage = 6;
        arcaneDart.magicalPowerMultiplier = 0.8f;
        arcaneDart.intelligenceToMagicalDamage = 1.5f;
        arcaneDart.customTauntGenerated = 25;

        AbilitySO circleMark = CreateAbility("Ability_Mage_CircleMark", "Marca del Circulo", "Dano magico bajo y dano por turno.", AbilityTarget.Enemy, AbilityTargetMode.Single, 0, 18);
        circleMark.flatMagicalDamage = 3;
        circleMark.magicalPowerMultiplier = 0.45f;
        circleMark.intelligenceToMagicalDamage = 1f;
        circleMark.appliesStatusEffect = true;
        circleMark.statusEffectType = StatusEffectType.Poison;
        circleMark.statusDuration = 3;
        circleMark.statusValue = 7;
        circleMark.intelligenceToStatusValue = 0.6f;
        circleMark.customTauntGenerated = 30;

        AbilitySO essenceDrain = CreateAbility("Ability_Mage_EssenceDrain", "Drenar Esencia", "Dano magico con vampirismo.", AbilityTarget.Enemy, AbilityTargetMode.Single, 0, 24);
        essenceDrain.flatMagicalDamage = 7;
        essenceDrain.magicalPowerMultiplier = 0.75f;
        essenceDrain.intelligenceToMagicalDamage = 1.3f;
        essenceDrain.vampirismPercent = 0.45f;
        essenceDrain.customTauntGenerated = 30;

        AbilitySO magicPulse = CreateAbility("Ability_Mage_MagicPulse", "Pulso Magico", "Dano magico bajo a todos los enemigos.", AbilityTarget.Enemy, AbilityTargetMode.All, 0, 32);
        magicPulse.flatMagicalDamage = 4;
        magicPulse.magicalPowerMultiplier = 0.45f;
        magicPulse.intelligenceToMagicalDamage = 1f;
        magicPulse.customTauntGenerated = 45;
    }

    private static void CreateAcolyteAbilities()
    {
        AbilitySO healWound = CreateAbility("Ability_Acolyte_HealWound", "Curar Herida", "Cura HP a un aliado.", AbilityTarget.Ally, AbilityTargetMode.Single, 0, 18);
        healWound.flatHeal = 22;
        healWound.healPercent = 0.08f;
        healWound.intelligenceToHeal = 1.2f;
        healWound.customTauntGenerated = 30;

        AbilitySO minorBlessing = CreateAbility("Ability_Acolyte_MinorBlessing", "Bendicion Menor", "Aplica regeneracion a un aliado.", AbilityTarget.Ally, AbilityTargetMode.Single, 0, 16);
        minorBlessing.appliesStatusEffect = true;
        minorBlessing.statusEffectType = StatusEffectType.Regeneration;
        minorBlessing.statusDuration = 3;
        minorBlessing.statusValue = 8;
        minorBlessing.intelligenceToStatusValue = 0.4f;
        minorBlessing.customTauntGenerated = 25;

        AbilitySO mendArmor = CreateAbility("Ability_Acolyte_MendArmor", "Remendar Armadura", "Restaura armadura fisica a un aliado.", AbilityTarget.Ally, AbilityTargetMode.Single, 0, 20);
        mendArmor.restorePhysicalArmor = 28;
        mendArmor.intelligenceToPhysicalArmorRestore = 1f;
        mendArmor.customTauntGenerated = 25;

        AbilitySO vigorPrayer = CreateAbility("Ability_Acolyte_VigorPrayer", "Oracion de Vigor", "Cura poco y aplica regeneracion a todos los aliados.", AbilityTarget.Ally, AbilityTargetMode.All, 0, 30);
        vigorPrayer.flatHeal = 8;
        vigorPrayer.intelligenceToHeal = 0.5f;
        vigorPrayer.appliesStatusEffect = true;
        vigorPrayer.statusEffectType = StatusEffectType.Regeneration;
        vigorPrayer.statusDuration = 2;
        vigorPrayer.statusValue = 5;
        vigorPrayer.intelligenceToStatusValue = 0.25f;
        vigorPrayer.customTauntGenerated = 40;
    }

    private static AbilitySO CreateAbility(string assetName, string abilityName, string description, AbilityTarget target, AbilityTargetMode targetMode, int staminaCost, int manaCost)
    {
        AbilitySO ability = LoadOrCreate<AbilitySO>($"{AbilitiesFolder}/{assetName}.asset");
        ResetAbility(ability);
        ability.abilityName = abilityName;
        ability.description = description;
        ability.targetType = target;
        ability.targetMode = targetMode;
        ability.staminaCost = staminaCost;
        ability.manaCost = manaCost;
        EditorUtility.SetDirty(ability);
        return ability;
    }

    private static void ResetAbility(AbilitySO ability)
    {
        ability.icon = null;
        ability.description = string.Empty;
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;
        ability.staminaCost = 0;
        ability.manaCost = 0;
        ability.flatPhysicalDamage = 0;
        ability.physicalPowerMultiplier = 0f;
        ability.strengthToPhysicalDamage = 0f;
        ability.dexterityToPhysicalDamage = 0f;
        ability.intelligenceToPhysicalDamage = 0f;
        ability.constitutionToPhysicalDamage = 0f;
        ability.flatMagicalDamage = 0;
        ability.magicalPowerMultiplier = 0f;
        ability.strengthToMagicalDamage = 0f;
        ability.dexterityToMagicalDamage = 0f;
        ability.intelligenceToMagicalDamage = 0f;
        ability.constitutionToMagicalDamage = 0f;
        ability.flatHeal = 0;
        ability.healPercent = 0f;
        ability.strengthToHeal = 0f;
        ability.dexterityToHeal = 0f;
        ability.intelligenceToHeal = 0f;
        ability.constitutionToHeal = 0f;
        ability.restorePhysicalArmor = 0;
        ability.restoreMagicalArmor = 0;
        ability.strengthToPhysicalArmorRestore = 0f;
        ability.dexterityToPhysicalArmorRestore = 0f;
        ability.intelligenceToPhysicalArmorRestore = 0f;
        ability.constitutionToPhysicalArmorRestore = 0f;
        ability.strengthToMagicalArmorRestore = 0f;
        ability.dexterityToMagicalArmorRestore = 0f;
        ability.intelligenceToMagicalArmorRestore = 0f;
        ability.constitutionToMagicalArmorRestore = 0f;
        ability.vampirismPercent = 0f;
        ability.appliesStatusEffect = false;
        ability.statusEffectType = StatusEffectType.Poison;
        ability.statusDuration = 1;
        ability.statusValue = 0;
        ability.strengthToStatusValue = 0f;
        ability.dexterityToStatusValue = 0f;
        ability.intelligenceToStatusValue = 0f;
        ability.constitutionToStatusValue = 0f;
        ability.generatesTaunt = true;
        ability.customTauntGenerated = -1;
        ability.selfTauntChange = 0;
        ability.targetTauntChange = 0;
    }

    private static void UpdateDemoClasses()
    {
        UpdateClass("Class_Defensor", new[]
        {
            ("Ability_Defender_ShieldBash", 1),
            ("Ability_Defender_DefensiveStance", 1),
            ("Ability_Defender_Provoke", 2),
            ("Ability_Defender_GuardBreaker", 3)
        }, new[]
        {
            CreateTactic(TacticConditionType.SelfPhysicalArmorBelowPercent, 40, "Ability_Defender_DefensiveStance", 1),
            CreateTactic(TacticConditionType.AllyHpBelowPercent, 50, "Ability_Defender_Provoke", 2),
            CreateTactic(TacticConditionType.Always, 100, "Ability_Defender_ShieldBash", 5)
        });

        UpdateClass("Class_Asesino", new[]
        {
            ("Ability_Assassin_QuickCut", 1),
            ("Ability_Assassin_Execution", 1),
            ("Ability_Assassin_Bleed", 2),
            ("Ability_Assassin_Vanish", 3)
        }, new[]
        {
            CreateTactic(TacticConditionType.SelfHpBelowPercent, 35, "Ability_Assassin_Vanish", 1),
            CreateTactic(TacticConditionType.EnemyHpBelowPercent, 35, "Ability_Assassin_Execution", 2),
            CreateTactic(TacticConditionType.Always, 100, "Ability_Assassin_QuickCut", 5)
        });

        UpdateClass("Class_MagoDelCirculo", new[]
        {
            ("Ability_Mage_ArcaneDart", 1),
            ("Ability_Mage_CircleMark", 1),
            ("Ability_Mage_EssenceDrain", 2),
            ("Ability_Mage_MagicPulse", 3)
        }, new[]
        {
            CreateTactic(TacticConditionType.SelfHpBelowPercent, 45, "Ability_Mage_EssenceDrain", 1),
            CreateTactic(TacticConditionType.Always, 100, "Ability_Mage_CircleMark", 3),
            CreateTactic(TacticConditionType.Always, 100, "Ability_Mage_ArcaneDart", 5)
        });

        UpdateClass("Class_Acolita", new[]
        {
            ("Ability_Acolyte_HealWound", 1),
            ("Ability_Acolyte_MinorBlessing", 1),
            ("Ability_Acolyte_MendArmor", 2),
            ("Ability_Acolyte_VigorPrayer", 3)
        }, new[]
        {
            CreateTactic(TacticConditionType.AllyHpBelowPercent, 35, "Ability_Acolyte_HealWound", 1),
            CreateTactic(TacticConditionType.AllyPhysicalArmorBelowPercent, 35, "Ability_Acolyte_MendArmor", 2),
            CreateTactic(TacticConditionType.AllyHpBelowPercent, 70, "Ability_Acolyte_MinorBlessing", 3)
        });
    }

    private static void UpdateClass(string classAssetName, (string abilityAssetName, int level)[] unlocks, TacticRule[] tactics)
    {
        CharacterClassSO characterClass = AssetDatabase.LoadAssetAtPath<CharacterClassSO>($"{ClassesFolder}/{classAssetName}.asset");

        if (characterClass == null)
            return;

        characterClass.abilityUnlocks = new List<ClassAbilityUnlock>();

        foreach ((string abilityAssetName, int level) unlock in unlocks)
        {
            AbilitySO ability = LoadAbility(unlock.abilityAssetName);

            if (ability == null)
                continue;

            characterClass.abilityUnlocks.Add(new ClassAbilityUnlock
            {
                level = unlock.level,
                ability = ability
            });
        }

        characterClass.suggestedTactics = new List<TacticRule>();

        foreach (TacticRule tactic in tactics)
        {
            if (tactic != null && tactic.ability != null)
                characterClass.suggestedTactics.Add(tactic);
        }

        EditorUtility.SetDirty(characterClass);
    }

    private static TacticRule CreateTactic(TacticConditionType condition, int threshold, string abilityAssetName, int priority)
    {
        return new TacticRule
        {
            priority = priority,
            isActive = true,
            conditionType = condition,
            thresholdPercent = threshold,
            ability = LoadAbility(abilityAssetName)
        };
    }

    private static AbilitySO LoadAbility(string assetName)
    {
        return AssetDatabase.LoadAssetAtPath<AbilitySO>($"{AbilitiesFolder}/{assetName}.asset");
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
