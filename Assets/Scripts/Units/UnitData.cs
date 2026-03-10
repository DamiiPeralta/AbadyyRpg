using System.Collections.Generic;
using UnityEngine;

// Componente que va en cada prefab de unidad para almacenar sus datos
public class UnitData : MonoBehaviour
{
    [Header("Identidad")]
    public string unitName = "Unidad";
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Estadísticas básicas")]
    public int maxHP = 100;
    public int speed = 10;
    public int baseDamage = 10;   // todavía se puede usar si se necesita un valor general

    [Header("Daño")]
    public int physicalDamage = 10;
    public int magicalDamage = 0;

    [Header("Armaduras (total)")]
    public int maxPhysicalArmor = 0;
    public int maxMagicalArmor = 0;

    [Header("Valores iniciales (opcional)")]
    public int startHP = -1;               // si es -1 usaremos maxHP
    public int startPhysicalArmor = -1;    // si es -1 usaremos maxPhysicalArmor
    public int startMagicalArmor = -1;     // si es -1 usaremos maxMagicalArmor

    [Header("Habilidades")]
    public List<AbilitySO> abilities = new List<AbilitySO>();

    // Crea una instancia de Unit basada en estos datos
    public Unit CreateUnit()
    {
        Unit unit = new Unit(unitName, maxHP, speed, physicalDamage, magicalDamage, maxPhysicalArmor, maxMagicalArmor);

        // ajustar valores iniciales si se especificaron
        if (startHP >= 0)
            unit.currentHP = Mathf.Min(startHP, unit.maxHP);
        if (startPhysicalArmor >= 0)
            unit.currentPhysicalArmor = Mathf.Min(startPhysicalArmor, unit.maxPhysicalArmor);
        if (startMagicalArmor >= 0)
            unit.currentMagicalArmor = Mathf.Min(startMagicalArmor, unit.maxMagicalArmor);

        // copiar la lista de habilidades si hay alguna
        if (abilities != null && abilities.Count > 0)
        {
            unit.abilities = new List<AbilitySO>(abilities);
        }

        return unit;
    }
}
