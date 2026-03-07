using UnityEngine;

// Componente que va en cada prefab de unidad para almacenar sus datos
public class UnitData : MonoBehaviour
{
    public string unitName = "Unidad";
    public int maxHP = 100;
    public int speed = 10;
    public int baseDamage = 10;

    // Crea una instancia de Unit basada en estos datos
    public Unit CreateUnit()
    {
        Unit unit = new Unit(unitName, maxHP, speed, baseDamage);
        return unit;
    }
}
