#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Herramienta simple para crear assets de AbilitySO de muestra
public static class AbilityCreator
{
    private static AbilitySO Create(string name, float dmgMult = 1f, float healPct = 0f, bool skip = false, AbilityTarget target = AbilityTarget.Enemy)
    {
        AbilitySO asset = ScriptableObject.CreateInstance<AbilitySO>();
        asset.abilityName = name;
        asset.damageMultiplier = dmgMult;
        asset.healPercent = healPct;
        asset.skipTargetTurn = skip;
        asset.targetType = target;

        string basePath = "Assets/Abilities";
        if (!AssetDatabase.IsValidFolder(basePath))
        {
            AssetDatabase.CreateFolder("Assets", "Abilities");
        }
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(basePath + "/" + name + ".asset");
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }

    [MenuItem("Assets/Create/Abilities/Create Sample Abilities")]
    public static void CreateSamples()
    {
        Create("Ataque Básico", 1f, 0f, false, AbilityTarget.Enemy);
        Create("Ataque Poderoso", 2f, 0f, false, AbilityTarget.Enemy);
        Create("Curar 30%", 0f, 0.3f, false, AbilityTarget.Self);
        Create("Golpe Incapacitante", 1f, 0f, true, AbilityTarget.Enemy);
        Debug.Log("Muestras de habilidades creadas en Assets/Abilities");
    }
}
#endif