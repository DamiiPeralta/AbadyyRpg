using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class UnitLevelGrowth
{
    public int strength;
    public int dexterity;
    public int intelligence;
    public int constitution;
    public int stamina;
    public int mana;
    public int physicalArmor;
    public int magicalArmor;
}

public class Unit
{
    public string unitName;

    // =========================
    // ATRIBUTOS BASE
    // =========================
    public int strength;
    public int dexterity;
    public int intelligence;
    public int constitution;

    // =========================
    // PROGRESO PERSISTENTE
    // =========================
    public int level = 1;
    public int experience = 0;
    public int maxLevel = 5;
    public List<int> experienceByLevel = new List<int> { 0, 40, 100, 180, 300 };
    public List<UnitLevelGrowth> levelGrowths = new List<UnitLevelGrowth>();
    public int strengthGrowthPerLevel = 1;
    public int dexterityGrowthPerLevel = 1;
    public int intelligenceGrowthPerLevel = 1;
    public int constitutionGrowthPerLevel = 1;
    public int staminaGrowthPerLevel = 5;
    public int manaGrowthPerLevel = 5;
    public int physicalArmorGrowthPerLevel = 0;
    public int magicalArmorGrowthPerLevel = 0;
    public int maxStamina = 100;
    public int currentStamina = 100;
    public int maxMana = 100;
    public int currentMana = 100;

    // =========================
    // STATS DERIVADOS
    // =========================
    public int maxHP;
    public int currentHP;

    public int physicalDamage;
    public int magicalDamage;
    public int physicalDamageMin;
    public int physicalDamageMax;
    public int magicalDamageMin;
    public int magicalDamageMax;

    public int speed;

    public int baseMaxPhysicalArmor;
    public int baseMaxMagicalArmor;

    public int maxPhysicalArmor;
    public int currentPhysicalArmor;
    public int maxMagicalArmor;
    public int currentMagicalArmor;

    // =========================
    // EQUIPAMIENTO / OBJETOS
    // =========================
    public Dictionary<EquipmentSlot, EquipmentItem> equipment = new Dictionary<EquipmentSlot, EquipmentItem>();

    public ConsumableItem consumable1;
    public ConsumableItem consumable2;

    public int turnMeter = 0;

    // =========================
    // TAUNT / AMENAZA
    // =========================
    public int currentTaunt = 0;
    public bool isFrontLine = true;

    public int frontMaxTaunt = 100;
    public int backMaxTaunt = 80;
    public int temporaryMaxTauntPenalty = 0;

    public int TotalTaunt => currentTaunt;

    public int CurrentMaxTaunt
    {
        get
        {
            int baseMax = isFrontLine ? frontMaxTaunt : backMaxTaunt;
            return Mathf.Clamp(baseMax - temporaryMaxTauntPenalty, 0, frontMaxTaunt);
        }
    }

    public bool isAlive;

    public UnitData unitData;
    public UnitView unitView;

    public List<AbilitySO> abilities = new List<AbilitySO>();
    public List<TacticRule> tactics = new List<TacticRule>();
    private int nextAbilityIndex = 0;

    public bool skipNextTurn = false;
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    public Unit(string name)
    {
        unitName = name;
        isAlive = true;
    }

    // =========================
    // PROGRESION
    // =========================
    public void ConfigureProgression(
        int maxLevel,
        List<int> experienceByLevel,
        int strengthGrowthPerLevel,
        int dexterityGrowthPerLevel,
        int intelligenceGrowthPerLevel,
        int constitutionGrowthPerLevel,
        int staminaGrowthPerLevel,
        int manaGrowthPerLevel,
        int physicalArmorGrowthPerLevel,
        int magicalArmorGrowthPerLevel,
        List<UnitLevelGrowth> levelGrowths = null)
    {
        this.maxLevel = Mathf.Max(1, maxLevel);
        this.experienceByLevel = NormalizeExperienceTable(experienceByLevel, this.maxLevel);
        this.levelGrowths = levelGrowths != null ? new List<UnitLevelGrowth>(levelGrowths) : new List<UnitLevelGrowth>();
        this.strengthGrowthPerLevel = strengthGrowthPerLevel;
        this.dexterityGrowthPerLevel = dexterityGrowthPerLevel;
        this.intelligenceGrowthPerLevel = intelligenceGrowthPerLevel;
        this.constitutionGrowthPerLevel = constitutionGrowthPerLevel;
        this.staminaGrowthPerLevel = staminaGrowthPerLevel;
        this.manaGrowthPerLevel = manaGrowthPerLevel;
        this.physicalArmorGrowthPerLevel = physicalArmorGrowthPerLevel;
        this.magicalArmorGrowthPerLevel = magicalArmorGrowthPerLevel;
        level = Mathf.Clamp(level, 1, this.maxLevel);
    }

    public int AddExperience(int amount)
    {
        if (amount <= 0)
            return 0;

        int previousLevel = level;
        experience = Mathf.Max(0, experience + amount);

        while (CanLevelUp())
        {
            ApplyLevelUp();
            LearnClassAbilitiesForCurrentLevel();
        }

        int levelsGained = level - previousLevel;

        if (levelsGained > 0)
            Debug.Log($"{unitName} subio a nivel {level}. XP: {experience}.");

        return levelsGained;
    }

    public bool CanLevelUp()
    {
        if (level >= maxLevel)
            return false;

        return experience >= GetExperienceRequiredForLevel(level + 1);
    }

    public int GetExperienceRequiredForLevel(int targetLevel)
    {
        int clampedLevel = Mathf.Clamp(targetLevel, 1, maxLevel);

        if (experienceByLevel == null || experienceByLevel.Count == 0)
            experienceByLevel = NormalizeExperienceTable(null, maxLevel);

        int index = clampedLevel - 1;
        return index < experienceByLevel.Count ? experienceByLevel[index] : experienceByLevel[experienceByLevel.Count - 1];
    }

    public int GetExperienceRequiredForNextLevel()
    {
        if (level >= maxLevel)
            return 0;

        return GetExperienceRequiredForLevel(level + 1);
    }

    public int GetExperienceRemainingForNextLevel()
    {
        if (level >= maxLevel)
            return 0;

        return Mathf.Max(0, GetExperienceRequiredForNextLevel() - experience);
    }

    private void ApplyLevelUp()
    {
        int previousMaxHP = maxHP;
        int previousMaxStamina = maxStamina;
        int previousMaxMana = maxMana;
        int previousMaxPhysicalArmor = maxPhysicalArmor;
        int previousMaxMagicalArmor = maxMagicalArmor;

        level++;
        UnitLevelGrowth growth = GetGrowthForLevel(level);

        strength += growth.strength;
        dexterity += growth.dexterity;
        intelligence += growth.intelligence;
        constitution += growth.constitution;
        maxStamina = Mathf.Max(1, maxStamina + growth.stamina);
        maxMana = Mathf.Max(0, maxMana + growth.mana);
        baseMaxPhysicalArmor = Mathf.Max(0, baseMaxPhysicalArmor + growth.physicalArmor);
        baseMaxMagicalArmor = Mathf.Max(0, baseMaxMagicalArmor + growth.magicalArmor);

        RecalculateStats();

        if (isAlive)
            currentHP = Mathf.Clamp(currentHP + Mathf.Max(0, maxHP - previousMaxHP), 1, maxHP);

        currentStamina = Mathf.Clamp(currentStamina + Mathf.Max(0, maxStamina - previousMaxStamina), 0, maxStamina);
        currentMana = Mathf.Clamp(currentMana + Mathf.Max(0, maxMana - previousMaxMana), 0, maxMana);
        currentPhysicalArmor = Mathf.Clamp(currentPhysicalArmor + Mathf.Max(0, maxPhysicalArmor - previousMaxPhysicalArmor), 0, maxPhysicalArmor);
        currentMagicalArmor = Mathf.Clamp(currentMagicalArmor + Mathf.Max(0, maxMagicalArmor - previousMaxMagicalArmor), 0, maxMagicalArmor);
    }

    private List<int> NormalizeExperienceTable(List<int> source, int maxLevel)
    {
        List<int> normalized = source != null && source.Count > 0
            ? new List<int>(source)
            : new List<int> { 0, 40, 100, 180, 300 };

        while (normalized.Count < maxLevel)
        {
            int last = normalized.Count > 0 ? normalized[normalized.Count - 1] : 0;
            int increment = 100 + normalized.Count * 50;
            normalized.Add(last + increment);
        }

        for (int i = 0; i < normalized.Count; i++)
            normalized[i] = Mathf.Max(0, normalized[i]);

        normalized[0] = 0;
        return normalized;
    }

    public UnitLevelGrowth GetGrowthForLevel(int targetLevel)
    {
        int index = Mathf.Max(0, targetLevel - 2);

        if (levelGrowths != null && index < levelGrowths.Count && levelGrowths[index] != null)
            return levelGrowths[index];

        return new UnitLevelGrowth
        {
            strength = strengthGrowthPerLevel,
            dexterity = dexterityGrowthPerLevel,
            intelligence = intelligenceGrowthPerLevel,
            constitution = constitutionGrowthPerLevel,
            stamina = staminaGrowthPerLevel,
            mana = manaGrowthPerLevel,
            physicalArmor = physicalArmorGrowthPerLevel,
            magicalArmor = magicalArmorGrowthPerLevel
        };
    }

    private void LearnClassAbilitiesForCurrentLevel()
    {
        if (unitData == null || !unitData.includeClassAbilities || unitData.characterClass == null)
            return;

        List<AbilitySO> classAbilities = unitData.characterClass.GetAbilitiesForLevel(level);

        if (classAbilities == null || classAbilities.Count == 0)
            return;

        if (abilities == null)
            abilities = new List<AbilitySO>();

        foreach (AbilitySO ability in classAbilities)
        {
            if (ability != null && !abilities.Contains(ability))
                abilities.Add(ability);
        }
    }

    // =========================
    // RECALCULAR STATS
    // =========================
    public void RecalculateStats()
    {
        maxHP = constitution * 2;
        physicalDamageMin = strength;
        physicalDamageMax = Mathf.CeilToInt(strength * 1.5f);
        magicalDamageMin = intelligence;
        magicalDamageMax = Mathf.CeilToInt(intelligence * 1.5f);

        speed = 10 + dexterity;

        maxPhysicalArmor = baseMaxPhysicalArmor;
        maxMagicalArmor = baseMaxMagicalArmor;

        ApplyEquipmentBaseStats();

        physicalDamage = GetAverageDamage(physicalDamageMin, physicalDamageMax);
        magicalDamage = GetAverageDamage(magicalDamageMin, magicalDamageMax);

        ApplyEquipmentStatEffects(false);
        ApplyEquipmentStatEffects(true);
        ApplyStatusStatEffects();

        maxHP = Mathf.Max(1, maxHP);
        physicalDamage = Mathf.Max(0, physicalDamage);
        magicalDamage = Mathf.Max(0, magicalDamage);
        speed = Mathf.Clamp(speed, 1, 999);
        maxPhysicalArmor = Mathf.Max(0, maxPhysicalArmor);
        maxMagicalArmor = Mathf.Max(0, maxMagicalArmor);
        physicalDamageMin = Mathf.Max(0, physicalDamageMin);
        physicalDamageMax = Mathf.Max(physicalDamageMin, physicalDamageMax);
        magicalDamageMin = Mathf.Max(0, magicalDamageMin);
        magicalDamageMax = Mathf.Max(magicalDamageMin, magicalDamageMax);

        if (currentHP <= 0 && isAlive)
            currentHP = maxHP;

        if (currentHP > maxHP)
            currentHP = maxHP;

        if (currentPhysicalArmor > maxPhysicalArmor)
            currentPhysicalArmor = maxPhysicalArmor;

        if (currentMagicalArmor > maxMagicalArmor)
            currentMagicalArmor = maxMagicalArmor;

        ClampTauntToMax();
    }

    private void ApplyEquipmentBaseStats()
    {
        foreach (var pair in equipment)
        {
            EquipmentItem item = pair.Value;

            if (item is Weapon weapon)
            {
                physicalDamageMin += weapon.physicalDamageMin;
                physicalDamageMax += weapon.physicalDamageMax;
                magicalDamageMin += weapon.magicalDamageMin;
                magicalDamageMax += weapon.magicalDamageMax;
            }
            else if (item is Armor armor)
            {
                maxPhysicalArmor += armor.physicalArmor;
                maxMagicalArmor += armor.magicalArmor;
            }
        }
    }

    private int GetAverageDamage(int min, int max)
    {
        int low = Mathf.Min(min, max);
        int high = Mathf.Max(min, max);
        return Mathf.RoundToInt((low + high) * 0.5f);
    }

    public int RollPhysicalDamage()
    {
        return Random.Range(physicalDamageMin, physicalDamageMax + 1);
    }

    public int RollMagicalDamage()
    {
        return Random.Range(magicalDamageMin, magicalDamageMax + 1);
    }

    private void ApplyEquipmentStatEffects(bool percentEffects)
    {
        foreach (var pair in equipment)
        {
            EquipmentItem item = pair.Value;

            if (item == null || item.effects == null)
                continue;

            foreach (ItemEffect effect in item.effects)
            {
                if (effect == null || effect.effectType != EffectType.ModifyStat)
                    continue;

                if (effect.isPercent != percentEffects)
                    continue;

                ApplyStatModifier(effect.statType, effect.value, effect.isPercent);
            }
        }
    }

    private void ApplyStatModifier(StatType statType, int value, bool isPercent)
    {
        switch (statType)
        {
            case StatType.MaxHP:
                maxHP = ModifyValue(maxHP, value, isPercent);
                break;

            case StatType.PhysicalDamage:
                physicalDamage = ModifyValue(physicalDamage, value, isPercent);
                break;

            case StatType.MagicalDamage:
                magicalDamage = ModifyValue(magicalDamage, value, isPercent);
                break;

            case StatType.Speed:
                speed = ModifyValue(speed, value, isPercent);
                break;

            case StatType.PhysicalArmor:
                maxPhysicalArmor = ModifyValue(maxPhysicalArmor, value, isPercent);
                break;

            case StatType.MagicalArmor:
                maxMagicalArmor = ModifyValue(maxMagicalArmor, value, isPercent);
                break;
        }
    }

    private int ModifyValue(int currentValue, int modifier, bool isPercent)
    {
        if (isPercent)
            return currentValue + Mathf.RoundToInt(currentValue * (modifier / 100f));

        return currentValue + modifier;
    }

    private void ApplyStatusStatEffects()
    {
        int attackDown = GetTotalStatusValue(StatusEffectType.AttackDown);

        if (attackDown <= 0)
            return;

        physicalDamageMin -= attackDown;
        physicalDamageMax -= attackDown;
        magicalDamageMin -= attackDown;
        magicalDamageMax -= attackDown;
        physicalDamage -= attackDown;
        magicalDamage -= attackDown;
    }

    public void ResetTurnMeter()
    {
        turnMeter = 0;
    }

    // =========================
    // TAUNT
    // =========================
    public void ResetTaunt()
    {
        currentTaunt = 0;
        temporaryMaxTauntPenalty = 0;
    }

    public void SetFrontLine(bool value)
    {
        isFrontLine = value;
        ClampTauntToMax();
    }

    public void AddTaunt(int amount)
    {
        int before = currentTaunt;
        currentTaunt = Mathf.Clamp(currentTaunt + amount, 0, CurrentMaxTaunt);
        int realChange = currentTaunt - before;

        if (unitView != null && realChange != 0)
        {
            if (realChange > 0)
                unitView.ShowStatusText("TAUNT +" + realChange);
            else
                unitView.ShowStatusText("TAUNT " + realChange);
        }
    }

    public void ReduceTaunt(int amount)
    {
        if (amount <= 0)
            return;

        AddTaunt(-amount);
    }

    public void DecayTaunt(int amount)
    {
        ReduceTaunt(amount);
    }

    public void SetTemporaryMaxTauntPenalty(int amount)
    {
        temporaryMaxTauntPenalty = Mathf.Max(0, amount);
        ClampTauntToMax();
    }

    public void ClampTauntToMax()
    {
        currentTaunt = Mathf.Clamp(currentTaunt, 0, CurrentMaxTaunt);
    }

    public void SetPositionTauntModifier(int amount)
    {
        // Compatibilidad con scripts viejos.
        // El sistema nuevo usa isFrontLine y CurrentMaxTaunt.
    }

    // =========================
    // EQUIPAMIENTO
    // =========================
    public bool EquipItem(EquipmentItem item)
    {
        if (item == null)
            return false;

        int previousMaxPhysicalArmor = maxPhysicalArmor;
        int previousMaxMagicalArmor = maxMagicalArmor;

        equipment[item.slot] = item;
        RecalculateStats();
        PreserveArmorAfterEquipmentChange(previousMaxPhysicalArmor, previousMaxMagicalArmor);
        return true;
    }

    public EquipmentItem UnequipItem(EquipmentSlot slot)
    {
        if (!equipment.TryGetValue(slot, out EquipmentItem item))
            return null;

        int previousMaxPhysicalArmor = maxPhysicalArmor;
        int previousMaxMagicalArmor = maxMagicalArmor;

        equipment.Remove(slot);
        RecalculateStats();
        PreserveArmorAfterEquipmentChange(previousMaxPhysicalArmor, previousMaxMagicalArmor);
        return item;
    }

    private void PreserveArmorAfterEquipmentChange(int previousMaxPhysicalArmor, int previousMaxMagicalArmor)
    {
        currentPhysicalArmor = PreserveArmorValue(currentPhysicalArmor, previousMaxPhysicalArmor, maxPhysicalArmor);
        currentMagicalArmor = PreserveArmorValue(currentMagicalArmor, previousMaxMagicalArmor, maxMagicalArmor);
    }

    private int PreserveArmorValue(int current, int previousMax, int newMax)
    {
        if (newMax <= 0)
            return 0;

        int delta = newMax - previousMax;

        if (delta > 0)
            return Mathf.Clamp(current + delta, 0, newMax);

        return Mathf.Clamp(current, 0, newMax);
    }

    public EquipmentItem GetEquippedItem(EquipmentSlot slot)
    {
        equipment.TryGetValue(slot, out EquipmentItem item);
        return item;
    }

    // =========================
    // CONSUMIBLES
    // =========================
    public void SetConsumable1(ConsumableItem item)
    {
        consumable1 = item;
    }

    public void SetConsumable2(ConsumableItem item)
    {
        consumable2 = item;
    }

    public ConsumableItem RemoveConsumable1()
    {
        ConsumableItem item = consumable1;
        consumable1 = null;
        return item;
    }

    public ConsumableItem RemoveConsumable2()
    {
        ConsumableItem item = consumable2;
        consumable2 = null;
        return item;
    }

    // =========================
    // HABILIDADES
    // =========================
    public AbilitySO GetNextAbility()
    {
        if (abilities == null || abilities.Count == 0)
            return null;

        return GetNextAffordableAbility();
    }

    public AbilitySO GetAbilityForTurn(List<Unit> allies, List<Unit> enemies, BattleManager battleManager)
    {
        if (tactics != null && tactics.Count > 0)
        {
            List<TacticRule> orderedRules = tactics
                .Where(rule => rule != null && rule.isActive && CanPayAbilityCost(rule.ability))
                .OrderBy(rule => rule.priority)
                .Take(TacticRules.MaxRules)
                .ToList();

            foreach (TacticRule rule in orderedRules)
            {
                if (DoesTacticConditionPass(rule, allies, enemies))
                    return rule.ability;
            }
        }

        bool isEnemy = battleManager != null &&
                       battleManager.enemyUnits != null &&
                       battleManager.enemyUnits.Contains(this);

        return isEnemy ? GetNextAbility() : null;
    }

    private AbilitySO GetNextAffordableAbility()
    {
        if (abilities == null || abilities.Count == 0)
            return null;

        for (int i = 0; i < abilities.Count; i++)
        {
            int index = (nextAbilityIndex + i) % abilities.Count;
            AbilitySO ability = abilities[index];

            if (!CanPayAbilityCost(ability))
                continue;

            nextAbilityIndex = (index + 1) % abilities.Count;
            return ability;
        }

        return null;
    }

    private bool DoesTacticConditionPass(TacticRule rule, List<Unit> allies, List<Unit> enemies)
    {
        int threshold = Mathf.Clamp(rule.thresholdPercent, 0, 100);

        switch (rule.conditionType)
        {
            case TacticConditionType.Always:
                return true;

            case TacticConditionType.SelfHpBelowPercent:
                return GetPercent(currentHP, maxHP) < threshold;

            case TacticConditionType.AllyHpBelowPercent:
                return HasLivingUnitBelowPercent(allies, threshold, unit => GetPercent(unit.currentHP, unit.maxHP));

            case TacticConditionType.EnemyHpBelowPercent:
                return HasLivingUnitBelowPercent(enemies, threshold, unit => GetPercent(unit.currentHP, unit.maxHP));

            case TacticConditionType.SelfPhysicalArmorBelowPercent:
                return GetPercent(currentPhysicalArmor, maxPhysicalArmor) < threshold;

            case TacticConditionType.AllyPhysicalArmorBelowPercent:
                return HasLivingUnitBelowPercent(allies, threshold, unit => GetPercent(unit.currentPhysicalArmor, unit.maxPhysicalArmor));

            case TacticConditionType.SelfStaminaAbovePercent:
                return GetPercent(currentStamina, maxStamina) >= threshold;
        }

        return false;
    }

    private bool HasLivingUnitBelowPercent(List<Unit> units, int threshold, System.Func<Unit, float> getPercent)
    {
        if (units == null)
            return false;

        foreach (Unit unit in units)
        {
            if (unit != null && unit.isAlive && getPercent(unit) < threshold)
                return true;
        }

        return false;
    }

    private float GetPercent(int current, int maximum)
    {
        if (maximum <= 0)
            return 0f;

        return (current * 100f) / maximum;
    }

    public void ResetAbilities()
    {
        nextAbilityIndex = 0;
    }

    // =========================
    // VIDA
    // =========================
    public float GetHPPercentage()
    {
        if (maxHP <= 0)
            return 0f;

        return (float)currentHP / maxHP;
    }

    public void Heal(int amount)
    {
        if (!isAlive)
            return;

        int previousHP = currentHP;

        currentHP += amount;

        if (currentHP > maxHP)
            currentHP = maxHP;

        if (currentHP > previousHP && GameSfxPlayer.Instance != null)
            GameSfxPlayer.Instance.PlayHeal();
    }

    public void RestoreStamina(int amount)
    {
        if (amount <= 0)
            return;

        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    }

    public bool SpendStamina(int amount)
    {
        if (amount <= 0)
            return true;

        if (currentStamina < amount)
            return false;

        currentStamina -= amount;
        return true;
    }

    public void RestoreMana(int amount)
    {
        if (amount <= 0)
            return;

        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
    }

    public bool SpendMana(int amount)
    {
        if (amount <= 0)
            return true;

        if (currentMana < amount)
            return false;

        currentMana -= amount;
        return true;
    }

    public bool CanPayAbilityCost(AbilitySO ability)
    {
        if (ability == null)
            return false;

        return currentStamina >= Mathf.Max(0, ability.staminaCost) &&
               currentMana >= Mathf.Max(0, ability.manaCost);
    }

    public bool SpendAbilityCost(AbilitySO ability)
    {
        if (!CanPayAbilityCost(ability))
            return false;

        SpendStamina(Mathf.Max(0, ability.staminaCost));
        SpendMana(Mathf.Max(0, ability.manaCost));
        return true;
    }

    public void FullHeal()
    {
        currentHP = maxHP;
        isAlive = true;

        if (unitView != null)
            unitView.RefreshStatusVisuals(activeEffects);
    }

    // =========================
    // DAÑO
    // =========================
    public (int physArmorAbsorbed, int magArmorAbsorbed, int hpDamage) TakeDamage(int physicalDmg, int magicalDmg)
    {
        int physArmorAbsorbed = Mathf.Min(currentPhysicalArmor, physicalDmg);
        currentPhysicalArmor -= physArmorAbsorbed;
        int physRemainder = physicalDmg - physArmorAbsorbed;

        int magArmorAbsorbed = Mathf.Min(currentMagicalArmor, magicalDmg);
        currentMagicalArmor -= magArmorAbsorbed;
        int magRemainder = magicalDmg - magArmorAbsorbed;

        int totalHPDamage = physRemainder + magRemainder;
        currentHP -= totalHPDamage;

        if (currentHP < 0)
            currentHP = 0;

        if (currentHP == 0)
        {
            isAlive = false;

            TryUseAutoReviveConsumable();
        }

        return (physArmorAbsorbed, magArmorAbsorbed, totalHPDamage);
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, 0);
    }

    private bool TryUseAutoReviveConsumable()
    {
        ConsumableItem reviveItem = null;
        int slot = 0;

        if (IsAutoReviveConsumable(consumable1))
        {
            reviveItem = consumable1;
            slot = 1;
        }
        else if (IsAutoReviveConsumable(consumable2))
        {
            reviveItem = consumable2;
            slot = 2;
        }

        if (reviveItem == null)
            return false;

        float revivePercent = reviveItem.hpThreshold > 0f ? reviveItem.hpThreshold : 0.3f;
        int revivedHP = Mathf.Clamp(Mathf.CeilToInt(maxHP * revivePercent), 1, maxHP);

        if (slot == 1)
        {
            RemoveConsumable1();

            if (unitData != null)
                unitData.ClearRuntimeConsumable1();
        }
        else
        {
            RemoveConsumable2();

            if (unitData != null)
                unitData.ClearRuntimeConsumable2();
        }

        isAlive = true;
        currentHP = revivedHP;

        if (unitView != null)
        {
            unitView.ShowItemText(reviveItem.itemName);
            unitView.ShowStatusText("REVIVE");
            unitView.ShowDamageText(revivedHP, DamageFeedbackType.Heal);
            unitView.HealMotion();
            unitView.FlashHit(Color.yellow);
            unitView.UpdateVisuals();
            unitView.RefreshStatusVisuals(activeEffects);
        }

        Debug.Log($"{unitName} revivio automaticamente con {revivedHP}/{maxHP} HP usando {reviveItem.itemName}.");
        return true;
    }

    private bool IsAutoReviveConsumable(ConsumableItem item)
    {
        return item != null &&
               item.consumableEffectType == ConsumableEffectType.ReviveAlly &&
               item.useCondition == ConsumableUseCondition.AnyAllyDead;
    }

    // =========================
    // ARMADURA
    // =========================
    public int RestorePhysicalArmor(int amount)
    {
        if (!isAlive)
            return 0;

        int before = currentPhysicalArmor;
        currentPhysicalArmor += amount;

        if (currentPhysicalArmor > maxPhysicalArmor)
            currentPhysicalArmor = maxPhysicalArmor;

        return currentPhysicalArmor - before;
    }

    public int RestoreMagicalArmor(int amount)
    {
        if (!isAlive)
            return 0;

        int before = currentMagicalArmor;
        currentMagicalArmor += amount;

        if (currentMagicalArmor > maxMagicalArmor)
            currentMagicalArmor = maxMagicalArmor;

        return currentMagicalArmor - before;
    }

    // =========================
    // STATUS EFFECTS
    // =========================
    public void AddStatusEffect(StatusEffect effect)
    {
        if (effect == null)
            return;

        activeEffects.Add(effect);
        RefreshTemporaryTauntMaxFromEffects();

        if (unitView != null)
        {
            unitView.ShowStatusText(effect.type.ToString().ToUpper());
            unitView.RefreshStatusVisuals(activeEffects);
        }
    }

    public void ProcessStatusEffects()
    {
        if (!isAlive)
            return;

        int totalPoisonDamage = 0;
        int totalRegenerationHeal = 0;
        bool hasStun = false;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = activeEffects[i];

            switch (effect.type)
            {
                case StatusEffectType.Poison:
                    totalPoisonDamage += effect.value;
                    break;

                case StatusEffectType.Regeneration:
                    totalRegenerationHeal += effect.value;
                    break;

                case StatusEffectType.Stun:
                    hasStun = true;
                    break;
            }

            effect.duration--;

            if (effect.IsExpired())
                activeEffects.RemoveAt(i);
        }

        RefreshTemporaryTauntMaxFromEffects();

        if (totalPoisonDamage > 0)
        {
            TakeDamage(totalPoisonDamage, 0);

            if (unitView != null)
            {
                unitView.ShowStatusText("POISON");
                unitView.ShowDamageText(totalPoisonDamage, DamageFeedbackType.HP);
                unitView.ShakeOnHit();
            }
        }

        if (!isAlive)
        {
            if (unitView != null)
                unitView.RefreshStatusVisuals(activeEffects);

            return;
        }

        if (totalRegenerationHeal > 0)
        {
            Heal(totalRegenerationHeal);

            if (unitView != null)
            {
                unitView.ShowStatusText("REGEN");
                unitView.ShowDamageText(totalRegenerationHeal, DamageFeedbackType.Heal);
                unitView.HealMotion();
            }
        }

        if (hasStun)
        {
            skipNextTurn = true;

            if (unitView != null)
                unitView.ShowStatusText("STUN");
        }

        if (unitView != null)
            unitView.RefreshStatusVisuals(activeEffects);
    }

    private void RefreshTemporaryTauntMaxFromEffects()
    {
        int penalty = 0;

        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.type == StatusEffectType.Invisibility)
                penalty += Mathf.Max(0, effect.value);
        }

        SetTemporaryMaxTauntPenalty(penalty);
    }

    public bool HasStatus(StatusEffectType type)
    {
        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.type == type)
                return true;
        }

        return false;
    }

    public int RemoveNegativeStatusEffects()
    {
        if (activeEffects == null || activeEffects.Count == 0)
            return 0;

        int removed = activeEffects.RemoveAll(IsNegativeStatusEffect);

        if (removed > 0)
        {
            RefreshTemporaryTauntMaxFromEffects();

            if (unitView != null)
            {
                unitView.ShowStatusText("CLEANSE");
                unitView.RefreshStatusVisuals(activeEffects);
            }
        }

        return removed;
    }

    private bool IsNegativeStatusEffect(StatusEffect effect)
    {
        if (effect == null)
            return false;

        switch (effect.type)
        {
            case StatusEffectType.Poison:
            case StatusEffectType.Stun:
            case StatusEffectType.AttackDown:
                return true;

            default:
                return false;
        }
    }

    private int GetTotalStatusValue(StatusEffectType type)
    {
        int total = 0;

        if (activeEffects == null)
            return total;

        foreach (StatusEffect effect in activeEffects)
        {
            if (effect != null && effect.type == type)
                total += Mathf.Max(0, effect.value);
        }

        return total;
    }

    public int CountStatusStacks(StatusEffectType type)
    {
        int count = 0;

        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.type == type)
                count++;
        }

        return count;
    }

    // =========================
    // INFO
    // =========================
    public string GetInfo()
    {
        string info =
            $"{unitName} | HP: {currentHP}/{maxHP}" +
            $" | SPD: {speed}" +
            $" | STR: {strength}" +
            $" | DEX: {dexterity}" +
            $" | INT: {intelligence}" +
            $" | CON: {constitution}" +
            $" | LVL: {level}" +
            $" | XP: {experience}" +
            $" | STA: {currentStamina}/{maxStamina}" +
            $" | MANA: {currentMana}/{maxMana}" +
            $" | Phys DMG: {physicalDamageMin}-{physicalDamageMax}" +
            $" | Mag DMG: {magicalDamageMin}-{magicalDamageMax}" +
            $" | Phys Armor: {currentPhysicalArmor}/{maxPhysicalArmor}" +
            $" | Mag Armor: {currentMagicalArmor}/{maxMagicalArmor}" +
            $" | Pos: {(isFrontLine ? "Front" : "Back")}" +
            $" | Taunt: {currentTaunt}/{CurrentMaxTaunt}";

        if (abilities != null && abilities.Count > 0)
        {
            info += " | Abilities:";
            foreach (var a in abilities)
                info += " " + a.abilityName;
        }

        if (equipment != null && equipment.Count > 0)
        {
            info += " | Equipment:";
            foreach (var pair in equipment)
            {
                if (pair.Value != null)
                    info += $" {pair.Key}:{pair.Value.itemName}";
            }
        }

        if (consumable1 != null || consumable2 != null)
        {
            info += " | Consumables:";

            if (consumable1 != null)
                info += $" Slot1:{consumable1.itemName}";

            if (consumable2 != null)
                info += $" Slot2:{consumable2.itemName}";
        }

        if (activeEffects != null && activeEffects.Count > 0)
        {
            info += " | Effects:";
            foreach (var e in activeEffects)
                info += $" {e.type}({e.duration})";
        }

        return info;
    }
}
