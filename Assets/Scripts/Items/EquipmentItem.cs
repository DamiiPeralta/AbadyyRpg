using UnityEngine;

public abstract class EquipmentItem : ItemBase
{
    [Header("Equipamiento")]

    // En qué slot se puede equipar
    public EquipmentSlot slot;

    // Para balance / progresión (opcional)
    public int levelRequirement;

}