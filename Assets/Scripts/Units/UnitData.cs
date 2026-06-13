using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    [Header("Identidad")]
    public string unitName = "Unidad";
    [TextArea] public string description;
    public CharacterClassSO characterClass;

    [Header("Usar datos de clase")]
    public bool useClassBaseStats = false;
    public bool useClassProgression = false;
    public bool includeClassAbilities = true;
    public bool includeClassSuggestedTactics = false;

    [Header("Sprites")]
    public Sprite icon;          // Retrato para UI
    public Sprite battleSprite;  // Sprite visible en combate

    [Header("Atributos")]
    public int strength = 10;
    public int dexterity = 10;
    public int intelligence = 10;
    public int constitution = 10;

    [Header("Progreso inicial")]
    public int level = 1;
    public int experience = 0;
    public int maxLevel = 5;
    public List<int> experienceByLevel = new List<int> { 0, 100, 250, 450, 700 };
    public int maxStamina = 100;
    public int startStamina = -1;
    public int maxMana = 100;
    public int startMana = -1;

    [Header("Crecimiento por nivel")]
    public int strengthGrowthPerLevel = 1;
    public int dexterityGrowthPerLevel = 1;
    public int intelligenceGrowthPerLevel = 1;
    public int constitutionGrowthPerLevel = 1;
    public int staminaGrowthPerLevel = 5;
    public int manaGrowthPerLevel = 5;
    public int physicalArmorGrowthPerLevel = 0;
    public int magicalArmorGrowthPerLevel = 0;

    [Header("Armaduras base")]
    public int maxPhysicalArmor = 0;
    public int maxMagicalArmor = 0;

    [Header("Valores iniciales")]
    public int startHP = -1;
    public int startPhysicalArmor = -1;
    public int startMagicalArmor = -1;

    [Header("Habilidades")]
    public List<AbilitySO> abilities = new List<AbilitySO>();

    [Header("Tacticas")]
    public List<TacticRule> tactics = new List<TacticRule>();

    [Header("Equipo inicial")]
    public EquipmentItem helmet;
    public EquipmentItem chest;
    public EquipmentItem feet;
    public EquipmentItem hands;
    public EquipmentItem rightHand;
    public EquipmentItem leftHand;
    public EquipmentItem ring;
    public EquipmentItem amulet;

    [Header("Consumibles iniciales")]
    public ConsumableItem consumable1;
    public ConsumableItem consumable2;

    public Unit CreateUnit()
    {
        Unit unit = new Unit(unitName);

        unit.unitData = this;

        unit.strength = useClassBaseStats && characterClass != null ? characterClass.strength : strength;
        unit.dexterity = useClassBaseStats && characterClass != null ? characterClass.dexterity : dexterity;
        unit.intelligence = useClassBaseStats && characterClass != null ? characterClass.intelligence : intelligence;
        unit.constitution = useClassBaseStats && characterClass != null ? characterClass.constitution : constitution;

        unit.level = Mathf.Max(1, level);
        unit.experience = Mathf.Max(0, experience);
        unit.ConfigureProgression(
            useClassProgression && characterClass != null ? characterClass.maxLevel : maxLevel,
            useClassProgression && characterClass != null ? characterClass.experienceByLevel : experienceByLevel,
            useClassProgression && characterClass != null ? characterClass.strengthGrowthPerLevel : strengthGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.dexterityGrowthPerLevel : dexterityGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.intelligenceGrowthPerLevel : intelligenceGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.constitutionGrowthPerLevel : constitutionGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.staminaGrowthPerLevel : staminaGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.manaGrowthPerLevel : manaGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.physicalArmorGrowthPerLevel : physicalArmorGrowthPerLevel,
            useClassProgression && characterClass != null ? characterClass.magicalArmorGrowthPerLevel : magicalArmorGrowthPerLevel);
        unit.maxStamina = Mathf.Max(1, useClassBaseStats && characterClass != null ? characterClass.maxStamina : maxStamina);
        unit.currentStamina = startStamina >= 0
            ? Mathf.Clamp(startStamina, 0, unit.maxStamina)
            : unit.maxStamina;
        unit.maxMana = Mathf.Max(0, useClassBaseStats && characterClass != null ? characterClass.maxMana : maxMana);
        unit.currentMana = startMana >= 0
            ? Mathf.Clamp(startMana, 0, unit.maxMana)
            : unit.maxMana;

        unit.baseMaxPhysicalArmor = useClassBaseStats && characterClass != null ? characterClass.basePhysicalArmor : maxPhysicalArmor;
        unit.baseMaxMagicalArmor = useClassBaseStats && characterClass != null ? characterClass.baseMagicalArmor : maxMagicalArmor;

        unit.EquipItem(helmet);
        unit.EquipItem(chest);
        unit.EquipItem(feet);
        unit.EquipItem(hands);
        unit.EquipItem(rightHand);
        unit.EquipItem(leftHand);
        unit.EquipItem(ring);
        unit.EquipItem(amulet);

        unit.RecalculateStats();

        if (startHP >= 0)
            unit.currentHP = Mathf.Clamp(startHP, 0, unit.maxHP);
        else
            unit.currentHP = unit.maxHP;

        if (unit.currentHP <= 0)
            unit.isAlive = false;

        if (startPhysicalArmor >= 0)
            unit.currentPhysicalArmor = Mathf.Clamp(startPhysicalArmor, 0, unit.maxPhysicalArmor);
        else
            unit.currentPhysicalArmor = unit.maxPhysicalArmor;

        if (startMagicalArmor >= 0)
            unit.currentMagicalArmor = Mathf.Clamp(startMagicalArmor, 0, unit.maxMagicalArmor);
        else
            unit.currentMagicalArmor = unit.maxMagicalArmor;

        unit.abilities = BuildAbilitiesForUnit(unit.level);

        unit.tactics = BuildTacticsForUnit();

        unit.consumable1 = consumable1;
        unit.consumable2 = consumable2;

        return unit;
    }

    private List<AbilitySO> BuildAbilitiesForUnit(int unitLevel)
    {
        List<AbilitySO> result = new List<AbilitySO>();

        if (includeClassAbilities && characterClass != null)
            AddUniqueAbilities(result, characterClass.GetAbilitiesForLevel(unitLevel));

        if (abilities != null)
            AddUniqueAbilities(result, abilities);

        return result;
    }

    private List<TacticRule> BuildTacticsForUnit()
    {
        List<TacticRule> result = new List<TacticRule>();

        if (includeClassSuggestedTactics && characterClass != null)
            result.AddRange(characterClass.CloneSuggestedTactics());

        if (tactics != null)
        {
            foreach (TacticRule tactic in tactics)
            {
                if (tactic != null)
                    result.Add(tactic.Clone());
            }
        }

        return result;
    }

    private void AddUniqueAbilities(List<AbilitySO> target, List<AbilitySO> source)
    {
        if (target == null || source == null)
            return;

        foreach (AbilitySO ability in source)
        {
            if (ability != null && !target.Contains(ability))
                target.Add(ability);
        }
    }

    public void ClearRuntimeConsumable1()
    {
        consumable1 = null;
    }

    public void ClearRuntimeConsumable2()
    {
        consumable2 = null;
    }
}
