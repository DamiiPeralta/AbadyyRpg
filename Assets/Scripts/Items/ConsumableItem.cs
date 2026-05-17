using UnityEngine;

public enum ConsumableUseCondition
{
    Never,
    Always,
    SelfHpBelowPercent,
    AnyAllyDead,
    SelfHasPoison,
    SelfHasStun
}

public enum ConsumableEffectType
{
    None,
    HealSelf,
    ReviveAlly,
    DamageAllEnemies,
    ApplyStatusToSelf,
    CleanseSelfStatus
}

[CreateAssetMenu(menuName = "Items/Consumable")]
public class ConsumableItem : ItemBase
{
    [Header("Condición")]
    public ConsumableUseCondition useCondition = ConsumableUseCondition.Never;

    [Range(0f, 1f)]
    public float hpThreshold = 0.3f;

    [Header("Efecto")]
    public ConsumableEffectType consumableEffectType = ConsumableEffectType.None;

    public int value = 0;

    [Header("Status opcional")]
    public StatusEffectType statusEffectType;
    public int statusDuration = 1;
    public int statusValue = 0;
}