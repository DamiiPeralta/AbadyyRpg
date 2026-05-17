using System;

[Serializable]
public class ItemEffect
{
    public EffectType effectType;
    public StatType statType;

    public int value;
    public bool isPercent;
}