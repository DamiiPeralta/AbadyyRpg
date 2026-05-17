using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AbilityTarget
{
    Self,
    Ally,
    Enemy
}

public enum AbilityTargetMode
{
    Single,
    All
}

[CreateAssetMenu(menuName = "Abilities/GenericAbility")]
public class AbilitySO : ScriptableObject
{
    [Header("Información básica")]
    public string abilityName = "New Ability";
    public Sprite icon;

    [TextArea]
    public string description;

    public AbilityTarget targetType = AbilityTarget.Enemy;
    public AbilityTargetMode targetMode = AbilityTargetMode.Single;

    [Header("Costes")]
    public int staminaCost = 0;
    public int manaCost = 0;

    [Header("Daño físico")]
    public int flatPhysicalDamage = 0;
    public float physicalPowerMultiplier = 0f;
    public float strengthToPhysicalDamage = 0f;
    public float dexterityToPhysicalDamage = 0f;
    public float intelligenceToPhysicalDamage = 0f;
    public float constitutionToPhysicalDamage = 0f;

    [Header("Daño mágico")]
    public int flatMagicalDamage = 0;
    public float magicalPowerMultiplier = 0f;
    public float strengthToMagicalDamage = 0f;
    public float dexterityToMagicalDamage = 0f;
    public float intelligenceToMagicalDamage = 0f;
    public float constitutionToMagicalDamage = 0f;

    [Header("Curación")]
    public int flatHeal = 0;

    [Range(0f, 1f)]
    public float healPercent = 0f;

    public float strengthToHeal = 0f;
    public float dexterityToHeal = 0f;
    public float intelligenceToHeal = 0f;
    public float constitutionToHeal = 0f;

    [Header("Restaurar armadura")]
    public int restorePhysicalArmor = 0;
    public int restoreMagicalArmor = 0;

    public float strengthToPhysicalArmorRestore = 0f;
    public float dexterityToPhysicalArmorRestore = 0f;
    public float intelligenceToPhysicalArmorRestore = 0f;
    public float constitutionToPhysicalArmorRestore = 0f;

    public float strengthToMagicalArmorRestore = 0f;
    public float dexterityToMagicalArmorRestore = 0f;
    public float intelligenceToMagicalArmorRestore = 0f;
    public float constitutionToMagicalArmorRestore = 0f;

    [Header("Vampirismo")]
    [Range(0f, 1f)]
    public float vampirismPercent = 0f;

    [Header("Estado")]
    public bool appliesStatusEffect = false;
    public StatusEffectType statusEffectType;
    public int statusDuration = 1;
    public int statusValue = 0;

    public float strengthToStatusValue = 0f;
    public float dexterityToStatusValue = 0f;
    public float intelligenceToStatusValue = 0f;
    public float constitutionToStatusValue = 0f;

    [Header("Taunt")]
    public bool generatesTaunt = true;

    [Tooltip("Si es 0 o más, reemplaza la generación automática. Ej: Burla = 60, Bomba de humo = 0.")]
    public int customTauntGenerated = -1;

    [Tooltip("Cambio directo sobre el taunt del usuario. Puede ser negativo.")]
    public int selfTauntChange = 0;

    [Tooltip("Cambio directo sobre el taunt del objetivo. Puede ser negativo.")]
    public int targetTauntChange = 0;

    public int singleAttackTaunt = 30;
    public int areaAttackTaunt = 40;
    public int singleHealTaunt = 30;
    public int areaHealTaunt = 40;
    public int armorRestoreTaunt = 20;
    public int statusOnlyTaunt = 20;

    public void Execute(Unit user, List<Unit> allies, List<Unit> enemies, BattleManager battleManager)
    {
        List<Unit> targets = SelectTargets(user, allies, enemies, battleManager);

        if (targets == null || targets.Count == 0)
            return;

        if (!user.SpendAbilityCost(this))
        {
            Debug.Log($"{user.unitName} no tiene recursos para usar {abilityName}.");
            return;
        }

        Debug.Log($"{user.unitName} usa {abilityName}");

        int totalHpDamageDealt = 0;
        int totalHealDone = 0;
        int totalArmorRestored = 0;
        bool appliedAnyStatus = false;

        if (selfTauntChange != 0)
            user.AddTaunt(selfTauntChange);

        foreach (Unit target in targets)
        {
            if (target == null || !target.isAlive)
                continue;

            Debug.Log($"   Objetivo: {target.unitName}");

            if (targetTauntChange != 0)
                target.AddTaunt(targetTauntChange);

            int healAmount = CalculateHeal(user, target);

            if (healAmount > 0)
            {
                int beforeHP = target.currentHP;

                target.Heal(healAmount);

                int realHeal = target.currentHP - beforeHP;
                totalHealDone += realHeal;

                if (target.unitView != null)
                {
                    target.unitView.HealMotion();
                    target.unitView.ShowDamageText(realHeal, DamageFeedbackType.Heal);
                    target.unitView.FlashHit(Color.cyan);
                }

                battleManager.UpdateUnitVisualsIfExists(target);
            }

            int physicalArmorRestore = CalculatePhysicalArmorRestore(user);

            if (physicalArmorRestore > 0)
            {
                int restored = target.RestorePhysicalArmor(physicalArmorRestore);
                totalArmorRestored += restored;

                if (restored > 0 && target.unitView != null)
                {
                    target.unitView.ShowStatusText("PHYS ARMOR");
                    target.unitView.ShowDamageText(restored, DamageFeedbackType.PhysicalArmorRestore);
                }

                battleManager.UpdateUnitVisualsIfExists(target);
            }

            int magicalArmorRestore = CalculateMagicalArmorRestore(user);

            if (magicalArmorRestore > 0)
            {
                int restored = target.RestoreMagicalArmor(magicalArmorRestore);
                totalArmorRestored += restored;

                if (restored > 0 && target.unitView != null)
                {
                    target.unitView.ShowStatusText("MAG ARMOR");
                    target.unitView.ShowDamageText(restored, DamageFeedbackType.MagicalArmorRestore);
                }

                battleManager.UpdateUnitVisualsIfExists(target);
            }

            int physicalDamage = CalculatePhysicalDamage(user);
            int magicalDamage = CalculateMagicalDamage(user);

            if (physicalDamage > 0 || magicalDamage > 0)
            {
                var (physAbs, magAbs, hpDamage) = target.TakeDamage(physicalDamage, magicalDamage);

                totalHpDamageDealt += hpDamage;

                if (target.unitView != null)
                {
                    target.unitView.ShowDamageBreakdown(physAbs, magAbs, hpDamage);
                    target.unitView.FlashHit(new Color(1f, 0.5f, 0f));
                    target.unitView.ShakeOnHit();
                }

                Debug.Log($"   Daño físico: {physicalDamage} | Daño mágico: {magicalDamage}");
                Debug.Log($"   Armadura física absorbió: {physAbs} | Armadura mágica absorbió: {magAbs} | Daño HP: {hpDamage}");

                battleManager.UpdateUnitVisualsIfExists(target);
            }

            if (appliesStatusEffect && target.isAlive)
            {
                int finalStatusValue = CalculateStatusValue(user);
                StatusEffect effect = new StatusEffect(statusEffectType, statusDuration, finalStatusValue);
                target.AddStatusEffect(effect);
                appliedAnyStatus = true;

                Debug.Log($"   {target.unitName} recibe {statusEffectType} ({statusDuration} turnos, valor {finalStatusValue})");
            }
        }

        if (vampirismPercent > 0f && totalHpDamageDealt > 0 && user.isAlive)
        {
            int vampHeal = Mathf.CeilToInt(totalHpDamageDealt * vampirismPercent);
            int beforeHP = user.currentHP;

            user.Heal(vampHeal);

            int realHeal = user.currentHP - beforeHP;
            totalHealDone += realHeal;

            if (user.unitView != null)
            {
                user.unitView.ShowStatusText("VAMP");
                user.unitView.ShowDamageText(realHeal, DamageFeedbackType.Heal);
                user.unitView.HealMotion();
            }

            battleManager.UpdateUnitVisualsIfExists(user);
        }

        GenerateTauntIfPlayer(user, battleManager, totalHpDamageDealt, totalHealDone, totalArmorRestored, appliedAnyStatus);
    }

    private void GenerateTauntIfPlayer(Unit user, BattleManager battleManager, int hpDamage, int healing, int armorRestored, bool appliedAnyStatus)
    {
        if (!generatesTaunt)
            return;

        if (battleManager == null || battleManager.playerUnits == null)
            return;

        if (!battleManager.playerUnits.Contains(user))
            return;

        int generated = 0;

        if (customTauntGenerated >= 0)
        {
            generated = customTauntGenerated;
        }
        else if (hpDamage > 0)
        {
            generated = targetMode == AbilityTargetMode.All ? areaAttackTaunt : singleAttackTaunt;
        }
        else if (healing > 0)
        {
            generated = targetMode == AbilityTargetMode.All ? areaHealTaunt : singleHealTaunt;
        }
        else if (armorRestored > 0)
        {
            generated = armorRestoreTaunt;
        }
        else if (appliedAnyStatus)
        {
            generated = statusOnlyTaunt;
        }

        if (generated > 0)
            user.AddTaunt(generated);
    }

    private int CalculatePhysicalDamage(Unit user)
    {
        float value = flatPhysicalDamage + user.RollPhysicalDamage() * physicalPowerMultiplier;
        float attributeMultiplier = CalculateAttributeMultiplier(
            user,
            strengthToPhysicalDamage,
            dexterityToPhysicalDamage,
            intelligenceToPhysicalDamage,
            constitutionToPhysicalDamage
        );

        value *= attributeMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(value));
    }

    private int CalculateMagicalDamage(Unit user)
    {
        float value = flatMagicalDamage + user.RollMagicalDamage() * magicalPowerMultiplier;
        float attributeMultiplier = CalculateAttributeMultiplier(
            user,
            strengthToMagicalDamage,
            dexterityToMagicalDamage,
            intelligenceToMagicalDamage,
            constitutionToMagicalDamage
        );

        value *= attributeMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(value));
    }

    private float CalculateAttributeMultiplier(
        Unit user,
        float strengthScale,
        float dexterityScale,
        float intelligenceScale,
        float constitutionScale)
    {
        float percent =
            user.strength * strengthScale +
            user.dexterity * dexterityScale +
            user.intelligence * intelligenceScale +
            user.constitution * constitutionScale;

        return Mathf.Max(0f, 1f + percent / 100f);
    }

    private int CalculateHeal(Unit user, Unit target)
    {
        float value = flatHeal;

        if (healPercent > 0f)
            value += target.maxHP * healPercent;

        value += user.strength * strengthToHeal;
        value += user.dexterity * dexterityToHeal;
        value += user.intelligence * intelligenceToHeal;
        value += user.constitution * constitutionToHeal;

        return Mathf.Max(0, Mathf.RoundToInt(value));
    }

    private int CalculatePhysicalArmorRestore(Unit user)
    {
        float value = restorePhysicalArmor;

        value += user.strength * strengthToPhysicalArmorRestore;
        value += user.dexterity * dexterityToPhysicalArmorRestore;
        value += user.intelligence * intelligenceToPhysicalArmorRestore;
        value += user.constitution * constitutionToPhysicalArmorRestore;

        return Mathf.Max(0, Mathf.RoundToInt(value));
    }

    private int CalculateMagicalArmorRestore(Unit user)
    {
        float value = restoreMagicalArmor;

        value += user.strength * strengthToMagicalArmorRestore;
        value += user.dexterity * dexterityToMagicalArmorRestore;
        value += user.intelligence * intelligenceToMagicalArmorRestore;
        value += user.constitution * constitutionToMagicalArmorRestore;

        return Mathf.Max(0, Mathf.RoundToInt(value));
    }

    private int CalculateStatusValue(Unit user)
    {
        float value = statusValue;

        value += user.strength * strengthToStatusValue;
        value += user.dexterity * dexterityToStatusValue;
        value += user.intelligence * intelligenceToStatusValue;
        value += user.constitution * constitutionToStatusValue;

        return Mathf.Max(0, Mathf.RoundToInt(value));
    }

    private List<Unit> SelectTargets(Unit user, List<Unit> allies, List<Unit> enemies, BattleManager battleManager)
    {
        List<Unit> source = new List<Unit>();

        switch (targetType)
        {
            case AbilityTarget.Self:
                source.Add(user);
                break;

            case AbilityTarget.Ally:
                source = allies;
                break;

            case AbilityTarget.Enemy:
                source = enemies;
                break;
        }

        List<Unit> validTargets = new List<Unit>();

        foreach (Unit unit in source)
        {
            if (unit != null && unit.isAlive)
                validTargets.Add(unit);
        }

        if (targetMode == AbilityTargetMode.All)
            return validTargets;

        if (validTargets.Count == 0)
            return new List<Unit>();

        bool userIsEnemy = battleManager != null &&
                           battleManager.enemyUnits != null &&
                           battleManager.enemyUnits.Contains(user);

        if (targetType == AbilityTarget.Ally)
        {
            Unit allyTarget = validTargets
                .OrderBy(u => GetHealthPercent(u))
                .ThenBy(u => u.currentHP)
                .FirstOrDefault();

            return new List<Unit> { allyTarget };
        }

        if (userIsEnemy && targetType == AbilityTarget.Enemy)
        {
            Unit tauntTarget = validTargets
                .OrderByDescending(u => u.TotalTaunt)
                .ThenByDescending(u => u.currentHP)
                .FirstOrDefault();

            return new List<Unit> { tauntTarget };
        }

        if (targetType == AbilityTarget.Enemy)
        {
            Unit enemyTarget = validTargets
                .OrderBy(u => GetHealthPercent(u))
                .ThenBy(u => u.currentHP)
                .FirstOrDefault();

            return new List<Unit> { enemyTarget };
        }

        return new List<Unit> { validTargets[0] };
    }

    private float GetHealthPercent(Unit unit)
    {
        if (unit == null || unit.maxHP <= 0)
            return 0f;

        return (float)unit.currentHP / unit.maxHP;
    }
}
