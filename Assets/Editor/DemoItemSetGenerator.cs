using UnityEditor;
using UnityEngine;

public static class DemoItemSetGenerator
{
    private const string WeaponsFolder = "Assets/GameData/Items/Weapons";
    private const string ArmorsFolder = "Assets/GameData/Items/Armors";
    private const string ConsumablesFolder = "Assets/GameData/Items/Consumables";
    private const string MaterialsFolder = "Assets/GameData/Items/Materials";

    [MenuItem("CaravanRPG/Game Data/Create Demo Item Set")]
    public static void CreateDemoItemSet()
    {
        EnsureFolder("Assets/GameData");
        EnsureFolder("Assets/GameData/Items");
        EnsureFolder(WeaponsFolder);
        EnsureFolder(ArmorsFolder);
        EnsureFolder(ConsumablesFolder);
        EnsureFolder(MaterialsFolder);

        CreateWeapons();
        CreateArmors();
        CreateConsumables();
        CreateMaterials();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Set minimo de items de demo creado/actualizado en Assets/GameData/Items.");
    }

    private static void CreateWeapons()
    {
        CreateWeapon("Weapon_Rusty_Sword", "Rusty_Sword", "Espada Oxidada", ItemTier.Tier1, WeaponType.Sword, 1, 4, 8, 0, 0, 12);
        CreateWeapon("Weapon_Iron_Sword", "Iron_Sword", "Espada de Hierro", ItemTier.Tier2, WeaponType.Sword, 2, 9, 15, 0, 0, 35);
        CreateWeapon("Weapon_Steel_Sword", "Steel_Sword", "Espada de Acero", ItemTier.Tier3, WeaponType.Sword, 3, 16, 24, 0, 0, 80);
        CreateWeapon("Weapon_Sword_Of_Light", "Sword_Of_Light", "Espada de Luz", ItemTier.Special, WeaponType.Sword, 3, 12, 20, 5, 11, 120);

        CreateWeapon("Weapon_Dull_Dagger", "Dull_Dagger", "Daga Mellada", ItemTier.Tier1, WeaponType.Dagger, 1, 3, 7, 0, 0, 10);
        CreateWeapon("Weapon_Iron_Dagger", "Iron_Dagger", "Daga de Hierro", ItemTier.Tier2, WeaponType.Dagger, 2, 7, 13, 0, 0, 30);
        CreateWeapon("Weapon_Steel_Dagger", "Steel_Dagger", "Daga de Acero", ItemTier.Tier3, WeaponType.Dagger, 3, 12, 20, 0, 0, 70);
        CreateWeapon("Weapon_Blackblood_Dagger", "Blackblood_Dagger", "Daga de Sangre Negra", ItemTier.Special, WeaponType.Dagger, 3, 10, 16, 0, 0, 110);

        CreateWeapon("Weapon_Broken_Staff", "Broken_Staff", "Baston Partido", ItemTier.Tier1, WeaponType.Staff, 1, 0, 0, 4, 8, 12);
        CreateWeapon("Weapon_Marked_Oak_Staff", "Marked_Oak_Staff", "Baston de Roble Marcado", ItemTier.Tier2, WeaponType.Staff, 2, 0, 0, 9, 15, 35);
        CreateWeapon("Weapon_Circle_Staff", "Circle_Staff", "Baston del Circulo", ItemTier.Tier3, WeaponType.Staff, 3, 0, 0, 16, 24, 80);
        CreateWeapon("Weapon_Hunger_Grimoire", "Hunger_Grimoire", "Grimorio del Hambre", ItemTier.Special, WeaponType.Staff, 3, 0, 0, 10, 18, 120);

        CreateWeapon("Weapon_Broken_Symbol", "Broken_Symbol", "Simbolo Quebrado", ItemTier.Tier1, WeaponType.Staff, 1, 0, 0, 3, 7, 12);
        CreateWeapon("Weapon_Iron_Scepter", "Iron_Scepter", "Cetro de Hierro", ItemTier.Tier2, WeaponType.Staff, 2, 0, 0, 7, 13, 35);
        CreateWeapon("Weapon_Consecrated_Scepter", "Consecrated_Scepter", "Cetro Consagrado", ItemTier.Tier3, WeaponType.Staff, 3, 0, 0, 12, 20, 80);
        CreateWeapon("Weapon_Bell_Of_Fallen", "Bell_Of_Fallen", "Campana de los Caidos", ItemTier.Special, WeaponType.Staff, 3, 0, 0, 9, 17, 120);
    }

    private static void CreateArmors()
    {
        CreateArmor("Armor_Rusty_Plates", "Rusty_Plates", "Placas Oxidadas", ItemTier.Tier1, ArmorType.Heavy, 1, 80, 20, 35);
        CreateArmor("Armor_Iron_Plates", "Iron_Plates", "Placas de Hierro", ItemTier.Tier2, ArmorType.Heavy, 2, 115, 30, 80);
        CreateArmor("Armor_Steel_Plates", "Steel_Plates", "Placas de Acero", ItemTier.Tier3, ArmorType.Heavy, 3, 155, 45, 150);
        CreateArmor("Armor_SelfRepair_Plates", "SelfRepair_Plates", "Armadura Autorreparadora", ItemTier.Special, ArmorType.Heavy, 3, 130, 55, 200);

        CreateArmor("Armor_Worn_Leather", "Worn_Leather", "Cuero Gastado", ItemTier.Tier1, ArmorType.Light, 1, 35, 15, 25);
        CreateArmor("Armor_Boiled_Leather", "Boiled_Leather", "Cuero Hervido", ItemTier.Tier2, ArmorType.Light, 2, 55, 25, 60);
        CreateArmor("Armor_Reinforced_Leather", "Reinforced_Leather", "Cuero Reforzado", ItemTier.Tier3, ArmorType.Light, 3, 80, 35, 120);
        CreateArmor("Armor_Stalker_Mantle", "Stalker_Mantle", "Manto del Acechador", ItemTier.Special, ArmorType.Light, 3, 60, 40, 170);

        CreateArmor("Armor_Torn_Robe", "Torn_Robe", "Tunica Rasgada", ItemTier.Tier1, ArmorType.Robe, 1, 10, 50, 25);
        CreateArmor("Armor_Circle_Robe", "Circle_Robe", "Tunica del Circulo", ItemTier.Tier2, ArmorType.Robe, 2, 20, 80, 65);
        CreateArmor("Armor_Crystal_Robe", "Crystal_Robe", "Tunica de Cristal", ItemTier.Tier3, ArmorType.Robe, 3, 35, 115, 130);
        CreateArmor("Armor_Antimagic_Mantle", "Antimagic_Mantle", "Manto Antimagia", ItemTier.Special, ArmorType.Robe, 3, 20, 145, 190);

        CreateArmor("Armor_Patched_Vestment", "Patched_Vestment", "Vestidura Remendada", ItemTier.Tier1, ArmorType.Medium, 1, 25, 35, 25);
        CreateArmor("Armor_Light_Iron_Mail", "Light_Iron_Mail", "Cota Liviana", ItemTier.Tier2, ArmorType.Medium, 2, 40, 60, 65);
        CreateArmor("Armor_Consecrated_Vestment", "Consecrated_Vestment", "Vestidura Consagrada", ItemTier.Tier3, ArmorType.Medium, 3, 65, 90, 130);
        CreateArmor("Armor_Serene_Flame_Habit", "Serene_Flame_Habit", "Habito de la Llama Serena", ItemTier.Special, ArmorType.Medium, 3, 50, 100, 190);
    }

    private static void CreateConsumables()
    {
        CreateConsumable("Consumable_Health_Potion", "Potion_Health", "Pocion de Salud", "Restaura HP al usuario.", ItemTier.Tier1, ConsumableEffectType.HealSelf, 40, 18);
        CreateConsumable("Consumable_Greater_Health_Potion", "Potion_Health_Greater", "Pocion de Salud Mayor", "Restaura mucho HP al usuario.", ItemTier.Tier2, ConsumableEffectType.HealSelf, 80, 45);
        CreateConsumable("Consumable_Revive_Scroll", "Scroll_Revive", "Pergamino de Revivir", "Revive a un aliado caido.", ItemTier.Special, ConsumableEffectType.ReviveAlly, 35, 100);
    }

    private static void CreateMaterials()
    {
        CreateMaterial("Material_Iron_Ore", "Iron_Ore", "Mineral de Hierro", ItemTier.Tier1, 6);
        CreateMaterial("Material_Leather_Strip", "Leather_Strip", "Tira de Cuero", ItemTier.Tier1, 5);
        CreateMaterial("Material_Coal", "Coal", "Carbon", ItemTier.Tier1, 4);
        CreateMaterial("Material_Arcane_Dust", "Arcane_Dust", "Polvo Arcano", ItemTier.Tier2, 14);
        CreateMaterial("Material_Crystal_Shard", "Crystal_Shard", "Fragmento de Cristal", ItemTier.Tier2, 18);
    }

    private static void CreateWeapon(string assetName, string itemId, string itemName, ItemTier tier, WeaponType weaponType, int levelRequirement, int physicalMin, int physicalMax, int magicalMin, int magicalMax, int sellValue)
    {
        Weapon weapon = LoadOrCreate<Weapon>($"{WeaponsFolder}/{assetName}.asset");
        weapon.itemId = itemId;
        weapon.itemName = itemName;
        weapon.description = BuildWeaponDescription(physicalMin, physicalMax, magicalMin, magicalMax);
        weapon.itemType = ItemType.Weapon;
        weapon.itemTier = tier;
        weapon.slot = EquipmentSlot.RightHand;
        weapon.levelRequirement = levelRequirement;
        weapon.weaponType = weaponType;
        weapon.physicalDamageMin = physicalMin;
        weapon.physicalDamageMax = physicalMax;
        weapon.magicalDamageMin = magicalMin;
        weapon.magicalDamageMax = magicalMax;
        weapon.sellValue = sellValue;
        EditorUtility.SetDirty(weapon);
    }

    private static void CreateArmor(string assetName, string itemId, string itemName, ItemTier tier, ArmorType armorType, int levelRequirement, int physicalArmor, int magicalArmor, int sellValue)
    {
        Armor armor = LoadOrCreate<Armor>($"{ArmorsFolder}/{assetName}.asset");
        armor.itemId = itemId;
        armor.itemName = itemName;
        armor.description = $"Armadura fisica {physicalArmor}. Armadura magica {magicalArmor}.";
        armor.itemType = ItemType.Armor;
        armor.itemTier = tier;
        armor.slot = EquipmentSlot.Chest;
        armor.levelRequirement = levelRequirement;
        armor.armorType = armorType;
        armor.physicalArmor = physicalArmor;
        armor.magicalArmor = magicalArmor;
        armor.sellValue = sellValue;
        EditorUtility.SetDirty(armor);
    }

    private static void CreateConsumable(string assetName, string itemId, string itemName, string description, ItemTier tier, ConsumableEffectType effectType, int value, int sellValue)
    {
        ConsumableItem consumable = LoadOrCreate<ConsumableItem>($"{ConsumablesFolder}/{assetName}.asset");
        consumable.itemId = itemId;
        consumable.itemName = itemName;
        consumable.description = description;
        consumable.itemType = ItemType.Consumable;
        consumable.itemTier = tier;
        consumable.useCondition = ConsumableUseCondition.SelfHpBelowPercent;
        consumable.hpThreshold = 0.35f;
        consumable.consumableEffectType = effectType;
        consumable.value = value;
        consumable.sellValue = sellValue;
        EditorUtility.SetDirty(consumable);
    }

    private static void CreateMaterial(string assetName, string itemId, string itemName, ItemTier tier, int sellValue)
    {
        MaterialItem material = LoadOrCreate<MaterialItem>($"{MaterialsFolder}/{assetName}.asset");
        material.itemId = itemId;
        material.itemName = itemName;
        material.description = "Material usado para crafting y contratos.";
        material.itemType = ItemType.Material;
        material.itemTier = tier;
        material.sellValue = sellValue;
        EditorUtility.SetDirty(material);
    }

    private static T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (asset != null)
            return asset;

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static string BuildWeaponDescription(int physicalMin, int physicalMax, int magicalMin, int magicalMax)
    {
        string description = "";

        if (physicalMax > 0)
            description += $"Dano fisico {physicalMin}-{physicalMax}.";

        if (magicalMax > 0)
        {
            if (!string.IsNullOrWhiteSpace(description))
                description += " ";

            description += $"Dano magico {magicalMin}-{magicalMax}.";
        }

        return string.IsNullOrWhiteSpace(description) ? "Arma de soporte." : description;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = System.IO.Path.GetFileName(path);

        if (!string.IsNullOrWhiteSpace(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folder);
    }
}
