using System.Collections.Generic;
using UnityEngine;

public enum CharacterClassRole
{
    Tank,
    PhysicalDps,
    MagicalDps,
    Support
}

[System.Serializable]
public class ClassAbilityUnlock
{
    public int level = 1;
    public AbilitySO ability;
}

[CreateAssetMenu(menuName = "Caravan RPG/Character Class")]
public class CharacterClassSO : ScriptableObject
{
    [Header("Identity")]
    public string classId;
    public string className;
    public CharacterClassRole role;

    [TextArea(3, 8)]
    public string description;

    [Header("Base Attributes")]
    public int strength = 10;
    public int dexterity = 10;
    public int intelligence = 10;
    public int constitution = 10;

    [Header("Base Resources")]
    public int maxStamina = 100;
    public int maxMana = 100;
    public int basePhysicalArmor = 0;
    public int baseMagicalArmor = 0;

    [Header("Progression")]
    public int maxLevel = 5;
    public List<int> experienceByLevel = new List<int> { 0, 40, 100, 180, 300 };

    [Header("Growth Per Level")]
    public int strengthGrowthPerLevel = 1;
    public int dexterityGrowthPerLevel = 1;
    public int intelligenceGrowthPerLevel = 1;
    public int constitutionGrowthPerLevel = 1;
    public int staminaGrowthPerLevel = 5;
    public int manaGrowthPerLevel = 5;
    public int physicalArmorGrowthPerLevel = 0;
    public int magicalArmorGrowthPerLevel = 0;
    public List<UnitLevelGrowth> levelGrowths = new List<UnitLevelGrowth>();

    [Header("Abilities")]
    public List<ClassAbilityUnlock> abilityUnlocks = new List<ClassAbilityUnlock>();

    [Header("Suggested Tactics")]
    public List<TacticRule> suggestedTactics = new List<TacticRule>();

    public List<AbilitySO> GetAbilitiesForLevel(int level)
    {
        List<AbilitySO> result = new List<AbilitySO>();

        if (abilityUnlocks == null)
            return result;

        foreach (ClassAbilityUnlock unlock in abilityUnlocks)
        {
            if (unlock == null || unlock.ability == null)
                continue;

            if (Mathf.Max(1, unlock.level) <= level && !result.Contains(unlock.ability))
                result.Add(unlock.ability);
        }

        return result;
    }

    public List<TacticRule> CloneSuggestedTactics()
    {
        List<TacticRule> result = new List<TacticRule>();

        if (suggestedTactics == null)
            return result;

        foreach (TacticRule tactic in suggestedTactics)
        {
            if (tactic != null)
                result.Add(tactic.Clone());
        }

        return result;
    }
}
