using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryDetailPanelUI : MonoBehaviour
{
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

    private void SetSlider(Slider slider, int current, int maximum)
    {
        if (slider == null)
            return;

        slider.minValue = 0;
        slider.maxValue = Mathf.Max(1, maximum);
        slider.value = Mathf.Clamp(current, 0, slider.maxValue);
    }
}
