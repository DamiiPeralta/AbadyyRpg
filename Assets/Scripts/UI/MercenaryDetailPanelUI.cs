using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryDetailPanelUI : MonoBehaviour
{
    private readonly Color positivePreviewColor = new Color(0.43f, 1f, 0.48f, 1f);
    private readonly Color negativePreviewColor = new Color(1f, 0.42f, 0.42f, 1f);

    [Header("Header")]
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text classText;
    public TMP_Text levelText;
    public TMP_Text stateText;
    public Slider xpSlider;
    public TMP_Text xpText;

    [Header("Resources")]
    public Slider hpSlider;
    public TMP_Text hpText;
    public Slider staminaSlider;
    public TMP_Text staminaText;
    public Slider manaSlider;
    public TMP_Text manaText;
    public Slider physicalArmorSlider;
    public TMP_Text physicalArmorText;
    public Slider magicalArmorSlider;
    public TMP_Text magicalArmorText;

    [Header("Stats")]
    public TMP_Text strengthText;
    public TMP_Text dexterityText;
    public TMP_Text intelligenceText;
    public TMP_Text constitutionText;
    public TMP_Text speedText;
    public TMP_Text physicalAttackText;
    public TMP_Text magicalAttackText;

    public void Show(Unit unit)
    {
        if (unit == null)
        {
            Clear();
            return;
        }

        if (portraitImage != null && unit.unitData != null && unit.unitData.icon != null)
            portraitImage.sprite = unit.unitData.icon;

        SetText(nameText, unit.unitName);
        SetText(classText, unit.unitData != null ? unit.unitData.name : "Clase");
        SetText(levelText, $"Nivel {unit.level}");
        SetText(stateText, unit.isAlive ? "Listo para combatir" : "Fuera de combate");

        SetSlider(xpSlider, unit.experience, GetNextLevelExperience(unit));
        SetText(xpText, $"{unit.experience}/{GetNextLevelExperience(unit)}");

        SetSlider(hpSlider, unit.currentHP, unit.maxHP);
        SetText(hpText, $"{unit.currentHP}/{unit.maxHP}");

        SetSlider(staminaSlider, unit.currentStamina, unit.maxStamina);
        SetText(staminaText, $"{unit.currentStamina}/{unit.maxStamina}");

        SetSlider(manaSlider, unit.currentMana, unit.maxMana);
        SetText(manaText, $"{unit.currentMana}/{unit.maxMana}");

        SetSlider(physicalArmorSlider, unit.currentPhysicalArmor, unit.maxPhysicalArmor);
        SetText(physicalArmorText, $"{unit.currentPhysicalArmor}/{unit.maxPhysicalArmor}");

        SetSlider(magicalArmorSlider, unit.currentMagicalArmor, unit.maxMagicalArmor);
        SetText(magicalArmorText, $"{unit.currentMagicalArmor}/{unit.maxMagicalArmor}");

        SetText(strengthText, unit.strength.ToString("00"));
        SetText(dexterityText, unit.dexterity.ToString("00"));
        SetText(intelligenceText, unit.intelligence.ToString("00"));
        SetText(constitutionText, unit.constitution.ToString("00"));
        SetText(speedText, unit.speed.ToString("00"));
        SetText(physicalAttackText, $"{unit.physicalDamageMin} - {unit.physicalDamageMax}");
        SetText(magicalAttackText, $"{unit.magicalDamageMin} - {unit.magicalDamageMax}");
    }

    public void ShowEquipmentPreview(Unit unit, EquipmentSlot slot, EquipmentItem candidate)
    {
        Show(unit);

        if (unit == null || candidate == null)
            return;

        EquipmentItem current = unit.GetEquippedItem(slot);

        int maxHpDelta = GetStatEffectDelta(current, candidate, StatType.MaxHP);
        int speedDelta = GetStatEffectDelta(current, candidate, StatType.Speed);
        int physicalDamageDelta = GetStatEffectDelta(current, candidate, StatType.PhysicalDamage);
        int magicalDamageDelta = GetStatEffectDelta(current, candidate, StatType.MagicalDamage);
        int physicalArmorDelta = GetStatEffectDelta(current, candidate, StatType.PhysicalArmor);
        int magicalArmorDelta = GetStatEffectDelta(current, candidate, StatType.MagicalArmor);

        if (candidate is Weapon candidateWeapon || current is Weapon)
        {
            Weapon currentWeapon = current as Weapon;
            Weapon nextWeapon = candidate as Weapon;

            int currentPhysicalMin = unit.physicalDamageMin - (currentWeapon != null ? currentWeapon.physicalDamageMin : 0);
            int currentPhysicalMax = unit.physicalDamageMax - (currentWeapon != null ? currentWeapon.physicalDamageMax : 0);
            int currentMagicalMin = unit.magicalDamageMin - (currentWeapon != null ? currentWeapon.magicalDamageMin : 0);
            int currentMagicalMax = unit.magicalDamageMax - (currentWeapon != null ? currentWeapon.magicalDamageMax : 0);

            int nextPhysicalMin = currentPhysicalMin + (nextWeapon != null ? nextWeapon.physicalDamageMin : 0);
            int nextPhysicalMax = currentPhysicalMax + (nextWeapon != null ? nextWeapon.physicalDamageMax : 0);
            int nextMagicalMin = currentMagicalMin + (nextWeapon != null ? nextWeapon.magicalDamageMin : 0);
            int nextMagicalMax = currentMagicalMax + (nextWeapon != null ? nextWeapon.magicalDamageMax : 0);

            int physicalDelta = GetAverage(nextPhysicalMin, nextPhysicalMax) - GetAverage(unit.physicalDamageMin, unit.physicalDamageMax);
            int magicalDelta = GetAverage(nextMagicalMin, nextMagicalMax) - GetAverage(unit.magicalDamageMin, unit.magicalDamageMax);

            SetPreviewText(physicalAttackText, $"{nextPhysicalMin} - {nextPhysicalMax}", physicalDelta + physicalDamageDelta);
            SetPreviewText(magicalAttackText, $"{nextMagicalMin} - {nextMagicalMax}", magicalDelta + magicalDamageDelta);
        }
        else
        {
            ApplyPreviewDelta(physicalAttackText, physicalDamageDelta);
            ApplyPreviewDelta(magicalAttackText, magicalDamageDelta);
        }

        if (candidate is Armor candidateArmor || current is Armor)
        {
            Armor currentArmor = current as Armor;
            Armor nextArmor = candidate as Armor;

            int basePhysicalArmor = unit.maxPhysicalArmor - (currentArmor != null ? currentArmor.physicalArmor : 0);
            int baseMagicalArmor = unit.maxMagicalArmor - (currentArmor != null ? currentArmor.magicalArmor : 0);
            int nextPhysicalArmor = basePhysicalArmor + (nextArmor != null ? nextArmor.physicalArmor : 0);
            int nextMagicalArmor = baseMagicalArmor + (nextArmor != null ? nextArmor.magicalArmor : 0);

            SetPreviewText(physicalArmorText, $"{unit.currentPhysicalArmor}/{nextPhysicalArmor}", nextPhysicalArmor - unit.maxPhysicalArmor + physicalArmorDelta);
            SetPreviewText(magicalArmorText, $"{unit.currentMagicalArmor}/{nextMagicalArmor}", nextMagicalArmor - unit.maxMagicalArmor + magicalArmorDelta);
        }
        else
        {
            ApplyPreviewDelta(physicalArmorText, physicalArmorDelta);
            ApplyPreviewDelta(magicalArmorText, magicalArmorDelta);
        }

        ApplyPreviewDelta(hpText, maxHpDelta);
        ApplyPreviewDelta(speedText, speedDelta);
    }

    public void Clear()
    {
        SetText(nameText, "-");
        SetText(classText, "-");
        SetText(levelText, "Nivel -");
        SetText(stateText, "Sin mercenario seleccionado");
        SetText(xpText, "0/0");
        SetText(hpText, "0/0");
        SetText(staminaText, "0/0");
        SetText(manaText, "0/0");
        SetText(physicalArmorText, "0/0");
        SetText(magicalArmorText, "0/0");
    }

    private int GetNextLevelExperience(Unit unit)
    {
        return Mathf.Max(1, unit.level * 1000);
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private void ApplyPreviewDelta(TMP_Text text, int delta)
    {
        if (text == null || delta == 0)
            return;

        SetPreviewText(text, text.text, delta);
    }

    private void SetPreviewText(TMP_Text text, string value, int delta)
    {
        if (text == null)
            return;

        if (delta == 0)
        {
            text.text = value;
            return;
        }

        string color = ColorUtility.ToHtmlStringRGB(delta > 0 ? positivePreviewColor : negativePreviewColor);
        string sign = delta > 0 ? "+" : "";
        text.text = $"<color=#{color}>{value} ({sign}{delta})</color>";
    }

    private int GetAverage(int min, int max)
    {
        return Mathf.RoundToInt((min + max) * 0.5f);
    }

    private int GetStatEffectDelta(EquipmentItem current, EquipmentItem candidate, StatType statType)
    {
        return GetFlatStatEffect(candidate, statType) - GetFlatStatEffect(current, statType);
    }

    private int GetFlatStatEffect(EquipmentItem item, StatType statType)
    {
        if (item == null || item.effects == null)
            return 0;

        int value = 0;

        foreach (ItemEffect effect in item.effects)
        {
            if (effect == null || effect.effectType != EffectType.ModifyStat || effect.statType != statType)
                continue;

            if (!effect.isPercent)
                value += effect.value;
        }

        return value;
    }

    private void SetSlider(Slider slider, int current, int maximum)
    {
        if (slider == null)
            return;

        slider.minValue = 0;
        slider.maxValue = Mathf.Max(1, maximum);
        slider.value = Mathf.Clamp(current, 0, slider.maxValue);
    }
}
