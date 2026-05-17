using System.Collections.Generic;
using UnityEngine;

public class StarterPartyPresetApplier : MonoBehaviour
{
    [Header("Unidades")]
    public UnitData mainKnight;
    public UnitData offKnight;
    public UnitData assassin;
    public UnitData mage;

    [Header("Habilidades Caballero Principal")]
    public AbilitySO knightTaunt;
    public AbilitySO knightRegeneration;
    public AbilitySO knightPowerStrike;

    [Header("Habilidades Caballero Secundario")]
    public AbilitySO offKnightPowerStrike;
    public AbilitySO offKnightArmorBreak;
    public AbilitySO offKnightGuard;

    [Header("Habilidades Asesino")]
    public AbilitySO assassinQuickStab;
    public AbilitySO assassinPoisonStrike;
    public AbilitySO assassinSmokeBomb;
    public AbilitySO assassinFinisher;

    [Header("Habilidades Mago")]
    public AbilitySO mageArcaneExplosion;
    public AbilitySO mageAreaHeal;
    public AbilitySO mageFireball;

    [ContextMenu("Apply Starter Party Presets")]
    public void ApplyStarterPartyPresets()
    {
        ApplyMainKnight();
        ApplyOffKnight();
        ApplyAssassin();
        ApplyMage();

        Debug.Log("Starter party presets aplicados.");
    }

    private void ApplyMainKnight()
    {
        if (mainKnight == null)
            return;

        mainKnight.unitName = "Caballero Guardián";
        mainKnight.description = "Tanque principal. Aguanta golpes, protege al grupo y se regenera.";

        mainKnight.strength = 14;
        mainKnight.dexterity = 8;
        mainKnight.intelligence = 4;
        mainKnight.constitution = 18;

        mainKnight.maxPhysicalArmor = 40;
        mainKnight.maxMagicalArmor = 10;

        mainKnight.startHP = -1;
        mainKnight.startPhysicalArmor = -1;
        mainKnight.startMagicalArmor = -1;

        mainKnight.abilities = CleanAbilityList(new List<AbilitySO>
        {
            knightTaunt,
            knightRegeneration,
            knightPowerStrike
        });
    }

    private void ApplyOffKnight()
    {
        if (offKnight == null)
            return;

        offKnight.unitName = "Caballero de Acero";
        offKnight.description = "Guerrero pesado. Menos resistente que el tanque principal, pero pega más fuerte.";

        offKnight.strength = 16;
        offKnight.dexterity = 9;
        offKnight.intelligence = 3;
        offKnight.constitution = 15;

        offKnight.maxPhysicalArmor = 30;
        offKnight.maxMagicalArmor = 5;

        offKnight.startHP = -1;
        offKnight.startPhysicalArmor = -1;
        offKnight.startMagicalArmor = -1;

        offKnight.abilities = CleanAbilityList(new List<AbilitySO>
        {
            offKnightPowerStrike,
            offKnightArmorBreak,
            offKnightGuard
        });
    }

    private void ApplyAssassin()
    {
        if (assassin == null)
            return;

        assassin.unitName = "Asesino";
        assassin.description = "Unidad rápida y frágil. Hace mucho daño físico y aplica veneno.";

        assassin.strength = 18;
        assassin.dexterity = 22;
        assassin.intelligence = 2;
        assassin.constitution = 8;

        assassin.maxPhysicalArmor = 5;
        assassin.maxMagicalArmor = 0;

        assassin.startHP = -1;
        assassin.startPhysicalArmor = -1;
        assassin.startMagicalArmor = -1;

        assassin.abilities = CleanAbilityList(new List<AbilitySO>
        {
            assassinQuickStab,
            assassinPoisonStrike,
            assassinSmokeBomb,
            assassinFinisher
        });
    }

    private void ApplyMage()
    {
        if (mage == null)
            return;

        mage.unitName = "Mago de Retaguardia";
        mage.description = "Caster frágil. Cura al grupo y hace daño mágico de área.";

        mage.strength = 3;
        mage.dexterity = 10;
        mage.intelligence = 20;
        mage.constitution = 9;

        mage.maxPhysicalArmor = 0;
        mage.maxMagicalArmor = 20;

        mage.startHP = -1;
        mage.startPhysicalArmor = -1;
        mage.startMagicalArmor = -1;

        mage.abilities = CleanAbilityList(new List<AbilitySO>
        {
            mageArcaneExplosion,
            mageAreaHeal,
            mageFireball
        });
    }

    private List<AbilitySO> CleanAbilityList(List<AbilitySO> list)
    {
        List<AbilitySO> clean = new List<AbilitySO>();

        foreach (AbilitySO ability in list)
        {
            if (ability != null)
                clean.Add(ability);
        }

        return clean;
    }
}