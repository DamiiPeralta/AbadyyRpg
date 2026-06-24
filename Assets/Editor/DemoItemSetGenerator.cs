using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DemoItemSetGenerator
{
    private const string WeaponsFolder = "Assets/GameData/Items/Weapons";
    private const string ArmorsFolder = "Assets/GameData/Items/Armors";
    private const string ConsumablesFolder = "Assets/GameData/Items/Consumables";
    private const string MaterialsFolder = "Assets/GameData/Items/Materials";
    private const string CraftingFolder = "Assets/GameData/Crafting/Tier1";

    [MenuItem("CaravanRPG/Game Data/Create Demo Item Set")]
    public static void CreateDemoItemSet()
    {
        EnsureFolder("Assets/GameData");
        EnsureFolder("Assets/GameData/Items");
        EnsureFolder(WeaponsFolder);
        EnsureFolder(ArmorsFolder);
        EnsureFolder(ConsumablesFolder);
        EnsureFolder(MaterialsFolder);
        EnsureFolder("Assets/GameData/Crafting");
        EnsureFolder(CraftingFolder);

        CreateWeapons();
        CreateArmors();
        CreateConsumables();
        CreateMaterials();
        CreateRecipes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Set minimo de items de demo creado/actualizado en Assets/GameData/Items.");
    }

    private static void CreateWeapons()
    {
        CreateWeapon("Weapon_Rusty_Sword", "weapon_espada_oxidada_t1", "Espada Oxidada", "Dano fisico estable. +1 dano fisico por fuerza.", ItemTier.Tier1, WeaponType.Sword, 1, 2, 3, 0, 0, 6, StatType.PhysicalDamage, 1);
        CreateWeapon("Weapon_Iron_Sword", "Iron_Sword", "Espada de Hierro", ItemTier.Tier2, WeaponType.Sword, 2, 9, 15, 0, 0, 35);
        CreateWeapon("Weapon_Steel_Sword", "Steel_Sword", "Espada de Acero", ItemTier.Tier3, WeaponType.Sword, 3, 16, 24, 0, 0, 80);
        CreateWeapon("Weapon_Sword_Of_Light", "Sword_Of_Light", "Espada de Luz", ItemTier.Special, WeaponType.Sword, 3, 12, 20, 5, 11, 120);

        CreateWeapon("Weapon_Dull_Dagger", "weapon_daga_mellada_t1", "Daga Mellada", "Arma rapida y barata. +1 velocidad por destreza.", ItemTier.Tier1, WeaponType.Dagger, 1, 1, 2, 0, 0, 4, StatType.Speed, 1);
        CreateWeapon("Weapon_Iron_Dagger", "Iron_Dagger", "Daga de Hierro", ItemTier.Tier2, WeaponType.Dagger, 2, 7, 13, 0, 0, 30);
        CreateWeapon("Weapon_Steel_Dagger", "Steel_Dagger", "Daga de Acero", ItemTier.Tier3, WeaponType.Dagger, 3, 12, 20, 0, 0, 70);
        CreateWeapon("Weapon_Blackblood_Dagger", "Blackblood_Dagger", "Daga de Sangre Negra", ItemTier.Special, WeaponType.Dagger, 3, 10, 16, 0, 0, 110);

        CreateWeapon("Weapon_Broken_Staff", "weapon_baston_partido_t1", "Baston Partido", "Dano magico estable. +1 dano magico por inteligencia.", ItemTier.Tier1, WeaponType.Staff, 1, 0, 0, 2, 3, 6, StatType.MagicalDamage, 1);
        CreateWeapon("Weapon_Marked_Oak_Staff", "Marked_Oak_Staff", "Baston de Roble Marcado", ItemTier.Tier2, WeaponType.Staff, 2, 0, 0, 9, 15, 35);
        CreateWeapon("Weapon_Circle_Staff", "Circle_Staff", "Baston del Circulo", ItemTier.Tier3, WeaponType.Staff, 3, 0, 0, 16, 24, 80);
        CreateWeapon("Weapon_Hunger_Grimoire", "Hunger_Grimoire", "Grimorio del Hambre", ItemTier.Special, WeaponType.Staff, 3, 0, 0, 10, 18, 120);

        CreateWeapon("Weapon_Broken_Symbol", "weapon_simbolo_quebrado_t1", "Simbolo Quebrado", "Arma de soporte. +2 HP por constitucion.", ItemTier.Tier1, WeaponType.Staff, 1, 0, 0, 1, 2, 5, StatType.MaxHP, 2);
        CreateWeapon("Weapon_Iron_Scepter", "Iron_Scepter", "Cetro de Hierro", ItemTier.Tier2, WeaponType.Staff, 2, 0, 0, 7, 13, 35);
        CreateWeapon("Weapon_Consecrated_Scepter", "Consecrated_Scepter", "Cetro Consagrado", ItemTier.Tier3, WeaponType.Staff, 3, 0, 0, 12, 20, 80);
        CreateWeapon("Weapon_Bell_Of_Fallen", "Bell_Of_Fallen", "Campana de los Caidos", ItemTier.Special, WeaponType.Staff, 3, 0, 0, 9, 17, 120);
    }

    private static void CreateArmors()
    {
        CreateArmor("Armor_Rusty_Plates", "armor_placas_oxidadas_t1", "Placas Oxidadas", "Defensa fisica pesada. +2 HP, -1 velocidad.", ItemTier.Tier1, ArmorType.Heavy, 1, 5, 1, 8, StatType.MaxHP, 2, StatType.Speed, -1);
        CreateArmor("Armor_Iron_Plates", "Iron_Plates", "Placas de Hierro", ItemTier.Tier2, ArmorType.Heavy, 2, 115, 30, 80);
        CreateArmor("Armor_Steel_Plates", "Steel_Plates", "Placas de Acero", ItemTier.Tier3, ArmorType.Heavy, 3, 155, 45, 150);
        CreateArmor("Armor_SelfRepair_Plates", "SelfRepair_Plates", "Armadura Autorreparadora", ItemTier.Special, ArmorType.Heavy, 3, 130, 55, 200);

        CreateArmor("Armor_Worn_Leather", "armor_cuero_gastado_t1", "Cuero Gastado", "Armadura ligera. +1 velocidad.", ItemTier.Tier1, ArmorType.Light, 1, 3, 1, 6, StatType.Speed, 1);
        CreateArmor("Armor_Boiled_Leather", "Boiled_Leather", "Cuero Hervido", ItemTier.Tier2, ArmorType.Light, 2, 55, 25, 60);
        CreateArmor("Armor_Reinforced_Leather", "Reinforced_Leather", "Cuero Reforzado", ItemTier.Tier3, ArmorType.Light, 3, 80, 35, 120);
        CreateArmor("Armor_Stalker_Mantle", "Stalker_Mantle", "Manto del Acechador", ItemTier.Special, ArmorType.Light, 3, 60, 40, 170);

        CreateArmor("Armor_Torn_Robe", "armor_tunica_rasgada_t1", "Tunica Rasgada", "Proteccion magica barata. +1 dano magico.", ItemTier.Tier1, ArmorType.Robe, 1, 1, 4, 7, StatType.MagicalDamage, 1);
        CreateArmor("Armor_Circle_Robe", "Circle_Robe", "Tunica del Circulo", ItemTier.Tier2, ArmorType.Robe, 2, 20, 80, 65);
        CreateArmor("Armor_Crystal_Robe", "Crystal_Robe", "Tunica de Cristal", ItemTier.Tier3, ArmorType.Robe, 3, 35, 115, 130);
        CreateArmor("Armor_Antimagic_Mantle", "Antimagic_Mantle", "Manto Antimagia", ItemTier.Special, ArmorType.Robe, 3, 20, 145, 190);

        CreateArmor("Armor_Patched_Vestment", "armor_vestidura_remendada_t1", "Vestidura Remendada", "Defensa equilibrada. +2 HP.", ItemTier.Tier1, ArmorType.Medium, 1, 2, 3, 7, StatType.MaxHP, 2);
        CreateArmor("Armor_Light_Iron_Mail", "Light_Iron_Mail", "Cota Liviana", ItemTier.Tier2, ArmorType.Medium, 2, 40, 60, 65);
        CreateArmor("Armor_Consecrated_Vestment", "Consecrated_Vestment", "Vestidura Consagrada", ItemTier.Tier3, ArmorType.Medium, 3, 65, 90, 130);
        CreateArmor("Armor_Serene_Flame_Habit", "Serene_Flame_Habit", "Habito de la Llama Serena", ItemTier.Special, ArmorType.Medium, 3, 50, 100, 190);
    }

    private static void CreateConsumables()
    {
        CreateConsumable("Consumable_Health_Potion", "consumable_pocion_salud_t1", "Pocion de Salud", "Cura 4 HP al usuario.", ItemTier.Tier1, ConsumableUseCondition.SelfHpBelowPercent, 0.4f, ConsumableEffectType.HealSelf, 4, 3);
        CreateConsumable("Consumable_Molotov", "consumable_molotov_t1", "Molotov", "Hace 3 de dano magico a todos los enemigos.", ItemTier.Tier1, ConsumableUseCondition.Always, 0.3f, ConsumableEffectType.DamageAllEnemies, 3, 4);
        CreateConsumable("Consumable_Revive_Scroll", "consumable_pergamino_revivir_t1", "Pergamino de Revivir", "Revive automaticamente al caer con 30% de HP y se consume.", ItemTier.Tier1, ConsumableUseCondition.AnyAllyDead, 0.3f, ConsumableEffectType.ReviveAlly, 0, 8);
        CreateConsumable("Consumable_Greater_Health_Potion", "Potion_Health_Greater", "Pocion de Salud Mayor", "Restaura mucho HP al usuario.", ItemTier.Tier2, ConsumableEffectType.HealSelf, 80, 45);
    }

    private static void CreateMaterials()
    {
        CreateMaterial("Material_Iron_Ore", "Iron_Ore", "Mineral de Hierro", ItemTier.Tier1, 6);
        CreateMaterial("Material_Leather_Strip", "Leather_Strip", "Tira de Cuero", ItemTier.Tier1, 5);
        CreateMaterial("Material_Coal", "Coal", "Carbon", ItemTier.Tier1, 4);
        CreateMaterial("Material_Arcane_Dust", "Arcane_Dust", "Polvo Arcano", ItemTier.Tier2, 14);
        CreateMaterial("Material_Crystal_Shard", "Crystal_Shard", "Fragmento de Cristal", ItemTier.Tier2, 18);
    }

    private static void CreateRecipes()
    {
        CreateRecipe("Recipe_Daga_Mellada_T1", "recipe_daga_mellada_t1", "Daga Mellada", "Forja una daga barata para builds rapidas.", CraftingStationType.Blacksmith, "Weapon_Dull_Dagger",
            Inputs(Stack(RecipeStackType.Gold, 8), Stack(RecipeStackType.Iron, 1), Stack(RecipeStackType.Leather, 1)));
        CreateRecipe("Recipe_Espada_Oxidada_T1", "recipe_espada_oxidada_t1", "Espada Oxidada", "Forja una espada simple de dano fisico estable.", CraftingStationType.Blacksmith, "Weapon_Rusty_Sword",
            Inputs(Stack(RecipeStackType.Gold, 12), Stack(RecipeStackType.Iron, 2), Stack(RecipeStackType.Wood, 1)));
        CreateRecipe("Recipe_Baston_Partido_T1", "recipe_baston_partido_t1", "Baston Partido", "Arma magica inicial para enemigos con armadura fisica.", CraftingStationType.RunicMage, "Weapon_Broken_Staff",
            Inputs(Stack(RecipeStackType.Gold, 12), Stack(RecipeStackType.Wood, 2), Stack(RecipeStackType.Crystals, 1)));
        CreateRecipe("Recipe_Simbolo_Quebrado_T1", "recipe_simbolo_quebrado_t1", "Simbolo Quebrado", "Foco defensivo para sostener combates largos.", CraftingStationType.RunicMage, "Weapon_Broken_Symbol",
            Inputs(Stack(RecipeStackType.Gold, 10), Stack(RecipeStackType.Stone, 1), Stack(RecipeStackType.Crystals, 1)));

        CreateRecipe("Recipe_Placas_Oxidadas_T1", "recipe_placas_oxidadas_t1", "Placas Oxidadas", "Armadura pesada contra dano fisico.", CraftingStationType.Blacksmith, "Armor_Rusty_Plates",
            Inputs(Stack(RecipeStackType.Gold, 16), Stack(RecipeStackType.Iron, 3), Stack(RecipeStackType.Stone, 1)));
        CreateRecipe("Recipe_Cuero_Gastado_T1", "recipe_cuero_gastado_t1", "Cuero Gastado", "Armadura ligera para actuar antes.", CraftingStationType.Blacksmith, "Armor_Worn_Leather",
            Inputs(Stack(RecipeStackType.Gold, 12), Stack(RecipeStackType.Leather, 3)));
        CreateRecipe("Recipe_Tunica_Rasgada_T1", "recipe_tunica_rasgada_t1", "Tunica Rasgada", "Proteccion magica inicial.", CraftingStationType.RunicMage, "Armor_Torn_Robe",
            Inputs(Stack(RecipeStackType.Gold, 14), Stack(RecipeStackType.Leather, 2), Stack(RecipeStackType.Crystals, 1)));
        CreateRecipe("Recipe_Vestidura_Remendada_T1", "recipe_vestidura_remendada_t1", "Vestidura Remendada", "Armadura equilibrada de soporte.", CraftingStationType.RunicMage, "Armor_Patched_Vestment",
            Inputs(Stack(RecipeStackType.Gold, 14), Stack(RecipeStackType.Leather, 1), Stack(RecipeStackType.Wood, 1), Stack(RecipeStackType.Crystals, 1)));

        CreateRecipe("Recipe_Pocion_Salud_T1", "recipe_pocion_salud_t1", "Pocion de Salud", "Prepara una pocion de emergencia.", CraftingStationType.Alchemist, "Consumable_Health_Potion",
            Inputs(Stack(RecipeStackType.Gold, 6), Stack(RecipeStackType.Food, 1), Stack(RecipeStackType.Crystals, 1)));
        CreateRecipe("Recipe_Molotov_T1", "recipe_molotov_t1", "Molotov", "Prepara un consumible ofensivo simple.", CraftingStationType.Alchemist, "Consumable_Molotov",
            Inputs(Stack(RecipeStackType.Gold, 8), Stack(RecipeStackType.Wood, 1), Stack(RecipeStackType.Leather, 1), Stack(RecipeStackType.Crystals, 1)));
        CreateRecipe("Recipe_Pergamino_Revivir_T1", "recipe_pergamino_revivir_t1", "Pergamino de Revivir", "Prepara un seguro de emergencia.", CraftingStationType.RunicMage, "Consumable_Revive_Scroll",
            Inputs(Stack(RecipeStackType.Gold, 16), Stack(RecipeStackType.Stone, 1), Stack(RecipeStackType.Crystals, 2)));
    }

    private static void CreateRecipe(string assetName, string recipeId, string recipeName, string description, CraftingStationType stationType, string outputAssetName, List<RecipeStack> inputs)
    {
        ItemBase output = FindItem(outputAssetName);
        CraftingRecipeSO recipe = LoadOrCreate<CraftingRecipeSO>($"{CraftingFolder}/{assetName}.asset");
        recipe.recipeId = recipeId;
        recipe.recipeName = recipeName;
        recipe.description = description;
        recipe.stationType = stationType;
        recipe.previewItem = output;
        recipe.inputs = inputs;
        recipe.outputs = Inputs(ItemStack(output, 1));
        EditorUtility.SetDirty(recipe);
    }

    private static List<RecipeStack> Inputs(params RecipeStack[] stacks)
    {
        return new List<RecipeStack>(stacks);
    }

    private static RecipeStack Stack(RecipeStackType stackType, int amount)
    {
        return new RecipeStack
        {
            stackType = stackType,
            amount = amount
        };
    }

    private static RecipeStack ItemStack(ItemBase item, int amount)
    {
        return new RecipeStack
        {
            stackType = RecipeStackType.Item,
            item = item,
            itemId = item != null ? item.itemId : "",
            amount = amount
        };
    }

    private static void CreateWeapon(string assetName, string itemId, string itemName, ItemTier tier, WeaponType weaponType, int levelRequirement, int physicalMin, int physicalMax, int magicalMin, int magicalMax, int sellValue)
    {
        CreateWeapon(assetName, itemId, itemName, BuildWeaponDescription(physicalMin, physicalMax, magicalMin, magicalMax), tier, weaponType, levelRequirement, physicalMin, physicalMax, magicalMin, magicalMax, sellValue, null, 0);
    }

    private static void CreateWeapon(string assetName, string itemId, string itemName, string description, ItemTier tier, WeaponType weaponType, int levelRequirement, int physicalMin, int physicalMax, int magicalMin, int magicalMax, int sellValue, StatType? bonusStat, int bonusValue)
    {
        Weapon weapon = LoadOrCreate<Weapon>($"{WeaponsFolder}/{assetName}.asset");
        weapon.itemId = itemId;
        weapon.itemName = itemName;
        weapon.description = $"{BuildWeaponDescription(physicalMin, physicalMax, magicalMin, magicalMax)} {description}";
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
        SetSingleStatEffect(weapon, bonusStat, bonusValue);
        EditorUtility.SetDirty(weapon);
    }

    private static void CreateArmor(string assetName, string itemId, string itemName, ItemTier tier, ArmorType armorType, int levelRequirement, int physicalArmor, int magicalArmor, int sellValue)
    {
        CreateArmor(assetName, itemId, itemName, $"Armadura fisica {physicalArmor}. Armadura magica {magicalArmor}.", tier, armorType, levelRequirement, physicalArmor, magicalArmor, sellValue, null, 0, null, 0);
    }

    private static void CreateArmor(string assetName, string itemId, string itemName, string description, ItemTier tier, ArmorType armorType, int levelRequirement, int physicalArmor, int magicalArmor, int sellValue, StatType? bonusStat, int bonusValue, StatType? secondBonusStat = null, int secondBonusValue = 0)
    {
        Armor armor = LoadOrCreate<Armor>($"{ArmorsFolder}/{assetName}.asset");
        armor.itemId = itemId;
        armor.itemName = itemName;
        armor.description = description;
        armor.itemType = ItemType.Armor;
        armor.itemTier = tier;
        armor.slot = EquipmentSlot.Chest;
        armor.levelRequirement = levelRequirement;
        armor.armorType = armorType;
        armor.physicalArmor = physicalArmor;
        armor.magicalArmor = magicalArmor;
        armor.sellValue = sellValue;
        SetStatEffects(armor, bonusStat, bonusValue, secondBonusStat, secondBonusValue);
        EditorUtility.SetDirty(armor);
    }

    private static void CreateConsumable(string assetName, string itemId, string itemName, string description, ItemTier tier, ConsumableEffectType effectType, int value, int sellValue)
    {
        CreateConsumable(assetName, itemId, itemName, description, tier, ConsumableUseCondition.SelfHpBelowPercent, 0.35f, effectType, value, sellValue);
    }

    private static void CreateConsumable(string assetName, string itemId, string itemName, string description, ItemTier tier, ConsumableUseCondition useCondition, float hpThreshold, ConsumableEffectType effectType, int value, int sellValue)
    {
        ConsumableItem consumable = LoadOrCreate<ConsumableItem>($"{ConsumablesFolder}/{assetName}.asset");
        consumable.itemId = itemId;
        consumable.itemName = itemName;
        consumable.description = description;
        consumable.itemType = ItemType.Consumable;
        consumable.itemTier = tier;
        consumable.useCondition = useCondition;
        consumable.hpThreshold = hpThreshold;
        consumable.consumableEffectType = effectType;
        consumable.value = value;
        consumable.sellValue = sellValue;
        consumable.effects.Clear();
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

    private static void SetSingleStatEffect(ItemBase item, StatType? statType, int value)
    {
        SetStatEffects(item, statType, value, null, 0);
    }

    private static void SetStatEffects(ItemBase item, StatType? statType, int value, StatType? secondStatType, int secondValue)
    {
        item.effects.Clear();

        AddStatEffect(item, statType, value);
        AddStatEffect(item, secondStatType, secondValue);
    }

    private static void AddStatEffect(ItemBase item, StatType? statType, int value)
    {
        if (!statType.HasValue || value == 0)
            return;

        item.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = statType.Value,
            value = value,
            isPercent = false
        });
    }

    private static ItemBase FindItem(string assetName)
    {
        string[] paths =
        {
            $"{WeaponsFolder}/{assetName}.asset",
            $"{ArmorsFolder}/{assetName}.asset",
            $"{ConsumablesFolder}/{assetName}.asset",
            $"{MaterialsFolder}/{assetName}.asset"
        };

        foreach (string path in paths)
        {
            ItemBase item = AssetDatabase.LoadAssetAtPath<ItemBase>(path);

            if (item != null)
                return item;
        }

        return null;
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
