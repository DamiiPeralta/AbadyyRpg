using UnityEngine;
using UnityEditor;

public static class StarterItemCreator
{
    private const string FolderPath = "Assets/Items/Starter";

    [MenuItem("CaravanRPG/Create Starter Items")]
    public static void CreateStarterItems()
    {
        EnsureFolderExists();

        // ARMADURAS
        CreateHeavyArmor();
        CreateLightArmor();
        CreateMageRobe();

        // CONSUMIBLES
        CreateHealthPotion();
        CreateReviveScroll();
        CreateMolotov();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Starter items creados/actualizados.");
    }

    private static void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Items"))
            AssetDatabase.CreateFolder("Assets", "Items");

        if (!AssetDatabase.IsValidFolder(FolderPath))
            AssetDatabase.CreateFolder("Assets/Items", "Starter");
    }

    private static T CreateItem<T>(string assetName) where T : ItemBase
    {
        string path = $"{FolderPath}/{assetName}.asset";

        T item = AssetDatabase.LoadAssetAtPath<T>(path);

        if (item == null)
        {
            item = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(item, path);
        }

        return item;
    }

    // =========================
    // ARMADURAS
    // =========================

    private static void CreateHeavyArmor()
    {
        Armor armor = CreateItem<Armor>("Armor_Heavy");

        armor.itemName = "Armadura Pesada";
        armor.description = "Altísima defensa física, buena defensa mágica.";
        armor.itemType = ItemType.Armor;
        armor.slot = EquipmentSlot.Chest;

        armor.effects.Clear();

        armor.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = StatType.PhysicalArmor,
            value = 100
        });

        armor.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = StatType.MagicalArmor,
            value = 30
        });

        EditorUtility.SetDirty(armor);
    }

    private static void CreateLightArmor()
    {
        Armor armor = CreateItem<Armor>("Armor_Light");

        armor.itemName = "Armadura Ligera";
        armor.description = "Defensa equilibrada física y mágica.";
        armor.itemType = ItemType.Armor;
        armor.slot = EquipmentSlot.Chest;

        armor.effects.Clear();

        armor.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = StatType.PhysicalArmor,
            value = 50
        });

        armor.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = StatType.MagicalArmor,
            value = 50
        });

        EditorUtility.SetDirty(armor);
    }

    private static void CreateMageRobe()
    {
        Armor armor = CreateItem<Armor>("Armor_Robe");

        armor.itemName = "Túnica Arcana";
        armor.description = "Baja defensa física, altísima defensa mágica.";
        armor.itemType = ItemType.Armor;
        armor.slot = EquipmentSlot.Chest;

        armor.effects.Clear();

        armor.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = StatType.PhysicalArmor,
            value = 30
        });

        armor.effects.Add(new ItemEffect
        {
            effectType = EffectType.ModifyStat,
            statType = StatType.MagicalArmor,
            value = 150
        });

        EditorUtility.SetDirty(armor);
    }

    // =========================
    // CONSUMIBLES
    // =========================

    private static void CreateHealthPotion()
    {
        ConsumableItem item = CreateItem<ConsumableItem>("Potion_Health");

        item.itemName = "Poción de Vida";
        item.description = "Se usa automáticamente si la vida está por debajo del 30%.";
        item.itemType = ItemType.Consumable;

        item.useCondition = ConsumableUseCondition.SelfHpBelowPercent;
        item.hpThreshold = 0.3f;

        item.consumableEffectType = ConsumableEffectType.HealSelf;
        item.value = 30;

        item.effects.Clear();

        item.effects.Add(new ItemEffect
        {
            effectType = EffectType.Heal,
            value = 30
        });

        EditorUtility.SetDirty(item);
    }

    private static void CreateReviveScroll()
    {
        ConsumableItem item = CreateItem<ConsumableItem>("Scroll_Revive");

        item.itemName = "Pergamino de Resurrección";
        item.description = "Revive automáticamente a un aliado caído.";
        item.itemType = ItemType.Consumable;

        item.useCondition = ConsumableUseCondition.AnyAllyDead;

        item.consumableEffectType = ConsumableEffectType.ReviveAlly;
        item.value = 30;

        item.effects.Clear();

        EditorUtility.SetDirty(item);
    }

    private static void CreateMolotov()
    {
        ConsumableItem item = CreateItem<ConsumableItem>("Molotov");

        item.itemName = "Molotov";
        item.description = "Se lanza automáticamente y hace daño mágico a todos los enemigos.";
        item.itemType = ItemType.Consumable;

        item.useCondition = ConsumableUseCondition.Always;

        item.consumableEffectType = ConsumableEffectType.DamageAllEnemies;
        item.value = 15;

        item.effects.Clear();

        EditorUtility.SetDirty(item);
    }
}