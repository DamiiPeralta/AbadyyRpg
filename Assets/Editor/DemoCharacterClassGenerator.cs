using UnityEditor;
using UnityEngine;

public static class DemoCharacterClassGenerator
{
    private const string ClassesFolder = "Assets/GameData/Classes";

    [MenuItem("CaravanRPG/Game Data/Create Demo Character Classes")]
    public static void CreateDemoClasses()
    {
        EnsureFolder("Assets/GameData");
        EnsureFolder(ClassesFolder);

        CreateOrUpdateClass(
            "Class_Defensor",
            "defensor",
            "Defensor",
            CharacterClassRole.Tank,
            "Tanque principal. Sostiene la linea, atrae ataques y mantiene activa su armadura fisica.",
            8,
            7,
            2,
            13,
            110,
            20,
            0,
            0,
            1,
            1,
            0,
            2,
            6,
            2,
            2,
            0);

        CreateOrUpdateClass(
            "Class_Asesino",
            "asesino",
            "Asesino",
            CharacterClassRole.PhysicalDps,
            "DPS fisico rapido. Elimina objetivos vulnerables y remata enemigos heridos.",
            14,
            15,
            1,
            9,
            120,
            10,
            0,
            0,
            2,
            2,
            0,
            1,
            7,
            1,
            0,
            0);

        CreateOrUpdateClass(
            "Class_MagoDelCirculo",
            "mago_del_circulo",
            "Mago del Circulo",
            CharacterClassRole.MagicalDps,
            "DPS magico. Presiona armadura magica, aplica estados y resuelve grupos con dano en area.",
            3,
            10,
            13,
            7,
            60,
            130,
            0,
            0,
            0,
            1,
            2,
            1,
            2,
            8,
            0,
            1);

        CreateOrUpdateClass(
            "Class_Acolita",
            "acolita",
            "Acolita",
            CharacterClassRole.Support,
            "Sanadora y booster. Mantiene viva a la party, repara defensas y aumenta el margen de error.",
            4,
            12,
            12,
            9,
            70,
            130,
            0,
            0,
            0,
            1,
            2,
            1,
            2,
            8,
            1,
            1);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Clases de demo creadas/actualizadas en Assets/GameData/Classes.");
    }

    private static void CreateOrUpdateClass(
        string assetName,
        string classId,
        string className,
        CharacterClassRole role,
        string description,
        int strength,
        int dexterity,
        int intelligence,
        int constitution,
        int maxStamina,
        int maxMana,
        int basePhysicalArmor,
        int baseMagicalArmor,
        int strengthGrowth,
        int dexterityGrowth,
        int intelligenceGrowth,
        int constitutionGrowth,
        int staminaGrowth,
        int manaGrowth,
        int physicalArmorGrowth,
        int magicalArmorGrowth)
    {
        string path = $"{ClassesFolder}/{assetName}.asset";
        CharacterClassSO characterClass = AssetDatabase.LoadAssetAtPath<CharacterClassSO>(path);

        if (characterClass == null)
        {
            characterClass = ScriptableObject.CreateInstance<CharacterClassSO>();
            AssetDatabase.CreateAsset(characterClass, path);
        }

        characterClass.classId = classId;
        characterClass.className = className;
        characterClass.role = role;
        characterClass.description = description;
        characterClass.strength = strength;
        characterClass.dexterity = dexterity;
        characterClass.intelligence = intelligence;
        characterClass.constitution = constitution;
        characterClass.maxStamina = maxStamina;
        characterClass.maxMana = maxMana;
        characterClass.basePhysicalArmor = basePhysicalArmor;
        characterClass.baseMagicalArmor = baseMagicalArmor;
        characterClass.maxLevel = 5;
        characterClass.experienceByLevel = new System.Collections.Generic.List<int> { 0, 100, 250, 450, 700 };
        characterClass.strengthGrowthPerLevel = strengthGrowth;
        characterClass.dexterityGrowthPerLevel = dexterityGrowth;
        characterClass.intelligenceGrowthPerLevel = intelligenceGrowth;
        characterClass.constitutionGrowthPerLevel = constitutionGrowth;
        characterClass.staminaGrowthPerLevel = staminaGrowth;
        characterClass.manaGrowthPerLevel = manaGrowth;
        characterClass.physicalArmorGrowthPerLevel = physicalArmorGrowth;
        characterClass.magicalArmorGrowthPerLevel = magicalArmorGrowth;

        EditorUtility.SetDirty(characterClass);
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
