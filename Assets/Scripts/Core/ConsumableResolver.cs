using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ConsumableResolver
{
    public static bool TryUseFirstValidConsumable(
        Unit user,
        List<Unit> allies,
        List<Unit> enemies,
        BattleManager battleManager
    )
    {
        if (user == null || !user.isAlive)
            return false;

        if (TryUseSlot1(user, allies, enemies, battleManager))
            return true;

        if (TryUseSlot2(user, allies, enemies, battleManager))
            return true;

        return false;
    }

    private static bool TryUseSlot1(
        Unit user,
        List<Unit> allies,
        List<Unit> enemies,
        BattleManager battleManager
    )
    {
        ConsumableItem item = user.consumable1;

        if (item == null)
            return false;

        if (!CanUse(user, allies, item))
            return false;

        bool used = Execute(user, allies, enemies, item, battleManager);

        if (used)
        {
            Debug.Log($"{user.unitName} consumió {item.itemName} del slot Consumable1");

            user.RemoveConsumable1();

            // 🔥 sincroniza con el inspector
            if (user.unitData != null)
                user.unitData.ClearRuntimeConsumable1();
        }

        return used;
    }

    private static bool TryUseSlot2(
        Unit user,
        List<Unit> allies,
        List<Unit> enemies,
        BattleManager battleManager
    )
    {
        ConsumableItem item = user.consumable2;

        if (item == null)
            return false;

        if (!CanUse(user, allies, item))
            return false;

        bool used = Execute(user, allies, enemies, item, battleManager);

        if (used)
        {
            Debug.Log($"{user.unitName} consumió {item.itemName} del slot Consumable2");

            user.RemoveConsumable2();

            // 🔥 sincroniza con el inspector
            if (user.unitData != null)
                user.unitData.ClearRuntimeConsumable2();
        }

        return used;
    }

    private static bool CanUse(Unit user, List<Unit> allies, ConsumableItem item)
    {
        switch (item.useCondition)
        {
            case ConsumableUseCondition.Always:
                return true;

            case ConsumableUseCondition.SelfHpBelowPercent:
                return user.GetHPPercentage() <= item.hpThreshold;

            case ConsumableUseCondition.AnyAllyDead:
                return allies.Any(a => a != null && !a.isAlive);

            case ConsumableUseCondition.SelfHasPoison:
                return user.HasStatus(StatusEffectType.Poison);

            case ConsumableUseCondition.SelfHasStun:
                return user.HasStatus(StatusEffectType.Stun);

            case ConsumableUseCondition.Never:
            default:
                return false;
        }
    }

    private static bool Execute(
        Unit user,
        List<Unit> allies,
        List<Unit> enemies,
        ConsumableItem item,
        BattleManager battleManager
    )
    {
        if (user.unitView != null)
        {
            user.unitView.ShowItemText(item.itemName);
            user.unitView.FlashHit(new Color(1f, 0.65f, 0f));
        }

        Debug.Log($"{user.unitName} usa consumible: {item.itemName}");

        switch (item.consumableEffectType)
        {
            case ConsumableEffectType.HealSelf:
                return HealSelf(user, item, battleManager);

            case ConsumableEffectType.ReviveAlly:
                return ReviveAlly(user, allies, item, battleManager);

            case ConsumableEffectType.DamageAllEnemies:
                return DamageAllEnemies(enemies, item, battleManager);

            case ConsumableEffectType.ApplyStatusToSelf:
                return ApplyStatusToSelf(user, item);

            case ConsumableEffectType.CleanseSelfStatus:
                return CleanseSelfStatus(user, item);

            default:
                return false;
        }
    }

    private static bool HealSelf(Unit user, ConsumableItem item, BattleManager battleManager)
    {
        int beforeHP = user.currentHP;

        user.Heal(item.value);

        int realHeal = user.currentHP - beforeHP;

        if (realHeal <= 0)
            return false;

        if (user.unitView != null)
        {
            user.unitView.ShowDamageText(realHeal, DamageFeedbackType.Heal);
            user.unitView.HealMotion();
        }

        battleManager.UpdateUnitVisualsIfExists(user);
        return true;
    }

    private static bool ReviveAlly(
        Unit user,
        List<Unit> allies,
        ConsumableItem item,
        BattleManager battleManager
    )
    {
        Unit deadAlly = allies.FirstOrDefault(a => a != null && !a.isAlive);

        if (deadAlly == null)
            return false;

        deadAlly.isAlive = true;
        deadAlly.currentHP = Mathf.Clamp(item.value, 1, deadAlly.maxHP);

        if (deadAlly.unitView != null)
        {
            deadAlly.unitView.ShowStatusText("REVIVE");
            deadAlly.unitView.ShowDamageText(deadAlly.currentHP, DamageFeedbackType.Heal);
            deadAlly.unitView.HealMotion();
            deadAlly.unitView.FlashHit(Color.yellow);
            deadAlly.unitView.UpdateVisuals();
        }

        battleManager.UpdateUnitVisualsIfExists(deadAlly);

        Debug.Log($"{user.unitName} revive a {deadAlly.unitName}");

        return true;
    }

    private static bool DamageAllEnemies(
        List<Unit> enemies,
        ConsumableItem item,
        BattleManager battleManager
    )
    {
        bool hitSomeone = false;

        foreach (Unit enemy in enemies)
        {
            if (enemy == null || !enemy.isAlive)
                continue;

            var (_, _, hpDamage) = enemy.TakeDamage(0, item.value);

            hitSomeone = true;

            if (enemy.unitView != null)
            {
                enemy.unitView.ShowStatusText("MOLOTOV");
                enemy.unitView.ShowDamageText(hpDamage, DamageFeedbackType.HP);
                enemy.unitView.FlashHit(new Color(1f, 0.35f, 0f));
                enemy.unitView.ShakeOnHit();
            }

            battleManager.UpdateUnitVisualsIfExists(enemy);
        }

        return hitSomeone;
    }

    private static bool ApplyStatusToSelf(Unit user, ConsumableItem item)
    {
        StatusEffect effect = new StatusEffect(
            item.statusEffectType,
            item.statusDuration,
            item.statusValue
        );

        user.AddStatusEffect(effect);

        if (user.unitView != null)
            user.unitView.ShowStatusText(item.statusEffectType.ToString().ToUpper());

        return true;
    }

    private static bool CleanseSelfStatus(Unit user, ConsumableItem item)
    {
        if (user.activeEffects == null)
            return false;

        int removed = user.activeEffects.RemoveAll(e => e.type == item.statusEffectType);

        if (removed <= 0)
            return false;

        if (user.unitView != null)
        {
            user.unitView.ShowStatusText("CLEANSE");
            user.unitView.RefreshStatusVisuals(user.activeEffects);
        }

        return true;
    }
}