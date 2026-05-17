using UnityEngine;

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : EquipmentItem
{
    [Header("Armor")]

    // Opcional: tipo de armadura (para UI, lógica futura, etc.)
    public ArmorType armorType;

    [Header("Defensa base")]
    public int physicalArmor = 0;
    public int magicalArmor = 0;
}
