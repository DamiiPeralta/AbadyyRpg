using System.Collections.Generic;
using UnityEngine;

public enum EnemyTier
{
    Basic = 1,
    Common = 2,
    Dangerous = 3,
    Elite = 4,
    Boss = 5,
    FinalBoss = 6
}

public enum EnemyKind
{
    Human,
    Beast,
    Cult,
    Aberration
}

[CreateAssetMenu(menuName = "Caravan RPG/Enemy Definition")]
public class EnemyDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    public string enemyId;
    public string enemyName;
    public EnemyTier tier = EnemyTier.Basic;
    public EnemyKind kind = EnemyKind.Human;

    [TextArea(3, 8)]
    public string description;

    [Header("Visuals")]
    public Sprite icon;
    public Sprite battleSprite;
    public GameObject visualPrefab;

    [Header("Progression")]
    public int level = 1;
    public int experienceReward = 10;

    [Header("Attributes")]
    public int strength = 8;
    public int dexterity = 8;
    public int intelligence = 5;
    public int constitution = 8;

    [Header("Resources")]
    public int maxStamina = 80;
    public int maxMana = 20;

    [Header("Armor")]
    public int physicalArmor = 0;
    public int magicalArmor = 0;

    [Header("Combat")]
    public bool isFrontLine = true;
    public List<AbilitySO> abilities = new List<AbilitySO>();
    public List<TacticRule> tactics = new List<TacticRule>();

    [Header("Rewards")]
    public RewardData reward = new RewardData();

    public Unit CreateUnit()
    {
        Unit unit = new Unit(enemyName);
        unit.strength = strength;
        unit.dexterity = dexterity;
        unit.intelligence = intelligence;
        unit.constitution = constitution;
        unit.level = Mathf.Max(1, level);
        unit.maxStamina = Mathf.Max(1, maxStamina);
        unit.currentStamina = unit.maxStamina;
        unit.maxMana = Mathf.Max(0, maxMana);
        unit.currentMana = unit.maxMana;
        unit.baseMaxPhysicalArmor = Mathf.Max(0, physicalArmor);
        unit.baseMaxMagicalArmor = Mathf.Max(0, magicalArmor);
        unit.isFrontLine = isFrontLine;
        unit.isAlive = true;

        unit.ConfigureProgression(
            unit.level,
            new List<int> { 0 },
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0);

        unit.RecalculateStats();
        unit.currentHP = unit.maxHP;
        unit.currentPhysicalArmor = unit.maxPhysicalArmor;
        unit.currentMagicalArmor = unit.maxMagicalArmor;

        if (abilities != null)
            unit.abilities = new List<AbilitySO>(abilities);

        unit.tactics = new List<TacticRule>();

        if (tactics != null)
        {
            foreach (TacticRule tactic in tactics)
            {
                if (tactic != null)
                    unit.tactics.Add(tactic.Clone());
            }
        }

        return unit;
    }
}
