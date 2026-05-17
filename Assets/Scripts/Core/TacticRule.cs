using System;
using UnityEngine;

public enum TacticConditionType
{
    Always,
    SelfHpBelowPercent,
    AllyHpBelowPercent,
    EnemyHpBelowPercent,
    SelfPhysicalArmorBelowPercent,
    AllyPhysicalArmorBelowPercent,
    SelfStaminaAbovePercent
}

public static class TacticRules
{
    public const int MaxRules = 5;
}

[Serializable]
public class TacticRule
{
    public int priority = 1;
    public bool isActive = true;
    public TacticConditionType conditionType = TacticConditionType.Always;

    [Range(0, 100)]
    public int thresholdPercent = 30;

    public AbilitySO ability;

    public TacticRule Clone()
    {
        return new TacticRule
        {
            priority = priority,
            isActive = isActive,
            conditionType = conditionType,
            thresholdPercent = thresholdPercent,
            ability = ability
        };
    }
}
