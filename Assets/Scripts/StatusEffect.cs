public enum StatusEffectType
{
    Poison,
    Regeneration,
    Stun,
    Invisibility
}

public class StatusEffect
{
    public StatusEffectType type;
    public int duration;
    public int value;

    public StatusEffect(StatusEffectType type, int duration, int value)
    {
        this.type = type;
        this.duration = duration;
        this.value = value;
    }

    public bool IsExpired()
    {
        return duration <= 0;
    }
}
