using UnityEngine;
using UnityEditor;

public static class StarterAbilityCreator
{
    private const string FolderPath = "Assets/Abilities/StarterParty";

    [MenuItem("CaravanRPG/Create Starter Abilities")]
    public static void CreateStarterAbilities()
    {
        EnsureFolderExists();

        // PARTY
        CreateKnightTaunt();
        CreateKnightRegeneration();
        CreateKnightPowerStrike();
        CreateKnightGuard();

        CreateAssassinQuickStab();
        CreateAssassinSmokeBomb();
        CreateAssassinPoisonStrike();
        CreateAssassinExecution();

        CreateMageArcaneExplosion();
        CreateMageAreaHeal();
        CreateMageFireball();

        // ENEMIGOS
        CreateSkeletonBoneStrike();
        CreateSkeletonStunBlow();

        CreateRatBite();
        CreateRatPoisonBite();

        CreateLesserDemonClaw();
        CreateLesserDemonDarkBolt();

        CreateWormBite();
        CreateWormToxicSpit();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Starter abilities creadas/actualizadas en: " + FolderPath);
    }

    private static void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Abilities"))
            AssetDatabase.CreateFolder("Assets", "Abilities");

        if (!AssetDatabase.IsValidFolder(FolderPath))
            AssetDatabase.CreateFolder("Assets/Abilities", "StarterParty");
    }

    private static AbilitySO CreateAbility(string assetName)
    {
        string path = $"{FolderPath}/{assetName}.asset";

        AbilitySO ability = AssetDatabase.LoadAssetAtPath<AbilitySO>(path);

        if (ability == null)
        {
            ability = ScriptableObject.CreateInstance<AbilitySO>();
            AssetDatabase.CreateAsset(ability, path);
        }

        ResetAbility(ability);

        return ability;
    }

    private static void ResetAbility(AbilitySO ability)
    {
        ability.abilityName = "New Ability";
        ability.description = "";

        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

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

        ability.generatesTaunt = true;
        ability.customTauntGenerated = -1;
        ability.selfTauntChange = 0;
        ability.targetTauntChange = 0;
        ability.singleAttackTaunt = 30;
        ability.areaAttackTaunt = 40;
        ability.singleHealTaunt = 30;
        ability.areaHealTaunt = 40;
        ability.armorRestoreTaunt = 20;
        ability.statusOnlyTaunt = 20;

        ability.appliesStatusEffect = false;
        ability.statusDuration = 1;
        ability.statusValue = 0;

        ability.strengthToStatusValue = 0f;
        ability.dexterityToStatusValue = 0f;
        ability.intelligenceToStatusValue = 0f;
        ability.constitutionToStatusValue = 0f;
    }

    // =====================================================
    // PARTY - CABALLEROS
    // =====================================================

    private static void CreateKnightTaunt()
    {
        AbilitySO ability = CreateAbility("Burla_Caballero");

        ability.abilityName = "Burla";
        ability.description = "El caballero fuerza la atención enemiga y genera mucha amenaza.";
        ability.targetType = AbilityTarget.Self;
        ability.targetMode = AbilityTargetMode.Single;

        ability.customTauntGenerated = 60;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateKnightRegeneration()
    {
        AbilitySO ability = CreateAbility("Regeneracion_Caballero");

        ability.abilityName = "Regeneración de Caballero";
        ability.description = "El caballero recupera vida durante varios turnos. Escala con Constitución.";
        ability.targetType = AbilityTarget.Self;
        ability.targetMode = AbilityTargetMode.Single;

        ability.appliesStatusEffect = true;
        ability.statusEffectType = StatusEffectType.Regeneration;
        ability.statusDuration = 3;
        ability.statusValue = 4;
        ability.constitutionToStatusValue = 0.25f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateKnightPowerStrike()
    {
        AbilitySO ability = CreateAbility("Golpe_Poderoso");

        ability.abilityName = "Golpe Poderoso";
        ability.description = "Ataque físico pesado. Escala fuerte con Fuerza.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 4;
        ability.physicalPowerMultiplier = 0.5f;
        ability.strengthToPhysicalDamage = 3.0f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateKnightGuard()
    {
        AbilitySO ability = CreateAbility("Guardia");

        ability.abilityName = "Guardia";
        ability.description = "Restaura armadura física. Escala con Constitución.";
        ability.targetType = AbilityTarget.Self;
        ability.targetMode = AbilityTargetMode.Single;

        ability.restorePhysicalArmor = 8;
        ability.constitutionToPhysicalArmorRestore = 0.8f;

        EditorUtility.SetDirty(ability);
    }

    // =====================================================
    // PARTY - ASESINO
    // =====================================================

    private static void CreateAssassinSmokeBomb()
    {
        AbilitySO ability = CreateAbility("Bomba_De_Humo");

        ability.abilityName = "Bomba de Humo";
        ability.description = "El asesino se oculta. Reduce su máximo de taunt temporalmente.";
        ability.targetType = AbilityTarget.Self;
        ability.targetMode = AbilityTargetMode.Single;

        ability.appliesStatusEffect = true;
        ability.statusEffectType = StatusEffectType.Invisibility;
        ability.statusDuration = 3;
        ability.statusValue = 20;
        ability.customTauntGenerated = 0;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateAssassinQuickStab()
    {
        AbilitySO ability = CreateAbility("Punialada_Rapida");

        ability.abilityName = "Puñalada Rápida";
        ability.description = "Ataque físico veloz. Escala con Destreza y algo de Fuerza.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 2;
        ability.strengthToPhysicalDamage = 1.0f;
        ability.dexterityToPhysicalDamage = 1.4f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateAssassinPoisonStrike()
    {
        AbilitySO ability = CreateAbility("Hoja_Envenenada");

        ability.abilityName = "Hoja Envenenada";
        ability.description = "Ataque rápido que aplica veneno. Daño y veneno escalan con Destreza.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 2;
        ability.dexterityToPhysicalDamage = 1.0f;

        ability.appliesStatusEffect = true;
        ability.statusEffectType = StatusEffectType.Poison;
        ability.statusDuration = 4;
        ability.statusValue = 3;
        ability.dexterityToStatusValue = 0.2f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateAssassinExecution()
    {
        AbilitySO ability = CreateAbility("Ejecucion");

        ability.abilityName = "Ejecución";
        ability.description = "Ataque físico brutal. Escala con Fuerza y Destreza.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 6;
        ability.strengthToPhysicalDamage = 2.2f;
        ability.dexterityToPhysicalDamage = 1.2f;

        EditorUtility.SetDirty(ability);
    }

    // =====================================================
    // PARTY - MAGO
    // =====================================================

    private static void CreateMageArcaneExplosion()
    {
        AbilitySO ability = CreateAbility("Explosion_Arcana");

        ability.abilityName = "Explosión Arcana";
        ability.description = "Daño mágico de área. Escala con Inteligencia.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.All;

        ability.flatMagicalDamage = 4;
        ability.magicalPowerMultiplier = 0.3f;
        ability.intelligenceToMagicalDamage = 1.6f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateMageAreaHeal()
    {
        AbilitySO ability = CreateAbility("Curacion_De_Area");

        ability.abilityName = "Curación de Área";
        ability.description = "Cura a todos los aliados. Escala con Inteligencia.";
        ability.targetType = AbilityTarget.Ally;
        ability.targetMode = AbilityTargetMode.All;

        ability.flatHeal = 4;
        ability.healPercent = 0.10f;
        ability.intelligenceToHeal = 1.2f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateMageFireball()
    {
        AbilitySO ability = CreateAbility("Bola_De_Fuego");

        ability.abilityName = "Bola de Fuego";
        ability.description = "Daño mágico fuerte contra un enemigo. Escala mucho con Inteligencia.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatMagicalDamage = 8;
        ability.magicalPowerMultiplier = 0.5f;
        ability.intelligenceToMagicalDamage = 2.4f;

        EditorUtility.SetDirty(ability);
    }

    // =====================================================
    // ENEMIGO - ESQUELETO
    // =====================================================

    private static void CreateSkeletonBoneStrike()
    {
        AbilitySO ability = CreateAbility("Skeleton_Bone_Strike");

        ability.abilityName = "Golpe de Hueso";
        ability.description = "Ataque físico simple de esqueleto.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 3;
        ability.strengthToPhysicalDamage = 1.6f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateSkeletonStunBlow()
    {
        AbilitySO ability = CreateAbility("Skeleton_Stun_Blow");

        ability.abilityName = "Golpe Aturdidor";
        ability.description = "Ataque físico que aturde al objetivo.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 2;
        ability.strengthToPhysicalDamage = 1.2f;

        ability.appliesStatusEffect = true;
        ability.statusEffectType = StatusEffectType.Stun;
        ability.statusDuration = 1;
        ability.statusValue = 0;

        EditorUtility.SetDirty(ability);
    }

    // =====================================================
    // ENEMIGO - RATA
    // =====================================================

    private static void CreateRatBite()
    {
        AbilitySO ability = CreateAbility("Rat_Bite");

        ability.abilityName = "Mordida";
        ability.description = "Ataque rápido y débil de rata.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 1;
        ability.dexterityToPhysicalDamage = 1.1f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateRatPoisonBite()
    {
        AbilitySO ability = CreateAbility("Rat_Poison_Bite");

        ability.abilityName = "Mordida Infecciosa";
        ability.description = "Ataque débil que aplica veneno.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 1;
        ability.dexterityToPhysicalDamage = 0.8f;

        ability.appliesStatusEffect = true;
        ability.statusEffectType = StatusEffectType.Poison;
        ability.statusDuration = 3;
        ability.statusValue = 2;
        ability.dexterityToStatusValue = 0.15f;

        EditorUtility.SetDirty(ability);
    }

    // =====================================================
    // ENEMIGO - DEMONIO MENOR
    // =====================================================

    private static void CreateLesserDemonClaw()
    {
        AbilitySO ability = CreateAbility("Lesser_Demon_Claw");

        ability.abilityName = "Garra Demoníaca";
        ability.description = "Ataque híbrido físico y mágico.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 3;
        ability.flatMagicalDamage = 3;
        ability.strengthToPhysicalDamage = 1.0f;
        ability.intelligenceToMagicalDamage = 1.3f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateLesserDemonDarkBolt()
    {
        AbilitySO ability = CreateAbility("Lesser_Demon_Dark_Bolt");

        ability.abilityName = "Llamarada Oscura";
        ability.description = "Daño mágico fuerte de demonio menor.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatMagicalDamage = 5;
        ability.intelligenceToMagicalDamage = 2.0f;

        EditorUtility.SetDirty(ability);
    }

    // =====================================================
    // ENEMIGO - GUSANO
    // =====================================================

    private static void CreateWormBite()
    {
        AbilitySO ability = CreateAbility("Worm_Bite");

        ability.abilityName = "Mordida Pesada";
        ability.description = "Ataque físico lento pero fuerte.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatPhysicalDamage = 5;
        ability.strengthToPhysicalDamage = 2.2f;
        ability.constitutionToPhysicalDamage = 0.4f;

        EditorUtility.SetDirty(ability);
    }

    private static void CreateWormToxicSpit()
    {
        AbilitySO ability = CreateAbility("Worm_Toxic_Spit");

        ability.abilityName = "Escupitajo Tóxico";
        ability.description = "Aplica veneno. El gusano desgasta lentamente.";
        ability.targetType = AbilityTarget.Enemy;
        ability.targetMode = AbilityTargetMode.Single;

        ability.flatMagicalDamage = 2;
        ability.constitutionToMagicalDamage = 0.5f;

        ability.appliesStatusEffect = true;
        ability.statusEffectType = StatusEffectType.Poison;
        ability.statusDuration = 4;
        ability.statusValue = 3;
        ability.constitutionToStatusValue = 0.15f;

        EditorUtility.SetDirty(ability);
    }
}