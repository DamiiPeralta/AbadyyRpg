using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : EquipmentItem
{
    [Header("Weapon")]

    // Opcional: tipo de arma (para animaciones, UI, etc.)
    public WeaponType weaponType;

    [Header("Daño base")]
    public int physicalDamageMin = 0;
    public int physicalDamageMax = 0;
    public int magicalDamageMin = 0;
    public int magicalDamageMax = 0;

    public int RollPhysicalDamage()
    {
        return RollRange(physicalDamageMin, physicalDamageMax);
    }

    public int RollMagicalDamage()
    {
        return RollRange(magicalDamageMin, magicalDamageMax);
    }

    public int GetAveragePhysicalDamage()
    {
        return GetAverageRange(physicalDamageMin, physicalDamageMax);
    }

    public int GetAverageMagicalDamage()
    {
        return GetAverageRange(magicalDamageMin, magicalDamageMax);
    }

    private int RollRange(int min, int max)
    {
        int low = Mathf.Min(min, max);
        int high = Mathf.Max(min, max);
        return Random.Range(low, high + 1);
    }

    private int GetAverageRange(int min, int max)
    {
        int low = Mathf.Min(min, max);
        int high = Mathf.Max(min, max);
        return Mathf.RoundToInt((low + high) * 0.5f);
    }
}
