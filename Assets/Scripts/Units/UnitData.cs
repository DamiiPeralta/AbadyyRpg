using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    [Header("Identidad")]
    public string unitName = "Unidad";
    [TextArea] public string description;

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
    public int maxStamina = 100;
    public int startStamina = -1;
    public int maxMana = 100;
    public int startMana = -1;

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

        unit.strength = strength;
        unit.dexterity = dexterity;
        unit.intelligence = intelligence;
        unit.constitution = constitution;

        unit.level = Mathf.Max(1, level);
        unit.experience = Mathf.Max(0, experience);
        unit.maxStamina = Mathf.Max(1, maxStamina);
        unit.currentStamina = startStamina >= 0
            ? Mathf.Clamp(startStamina, 0, unit.maxStamina)
            : unit.maxStamina;
        unit.maxMana = Mathf.Max(0, maxMana);
        unit.currentMana = startMana >= 0
            ? Mathf.Clamp(startMana, 0, unit.maxMana)
            : unit.maxMana;

        unit.baseMaxPhysicalArmor = maxPhysicalArmor;
        unit.baseMaxMagicalArmor = maxMagicalArmor;

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

        if (abilities != null && abilities.Count > 0)
            unit.abilities = new List<AbilitySO>(abilities);

        if (tactics != null && tactics.Count > 0)
        {
            unit.tactics = new List<TacticRule>();

            foreach (TacticRule tactic in tactics)
            {
                if (tactic != null)
                    unit.tactics.Add(tactic.Clone());
            }
        }

        unit.consumable1 = consumable1;
        unit.consumable2 = consumable2;

        return unit;
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
