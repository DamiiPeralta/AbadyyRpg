using TMPro;
using UnityEngine;

public class SuppliesResourcesPanelUI : MonoBehaviour
{
    [Header("Cantidades")]
    public TMP_Text goldAmountText;
    public TMP_Text foodAmountText;
    public TMP_Text woodAmountText;
    public TMP_Text stoneAmountText;
    public TMP_Text ironAmountText;
    public TMP_Text leatherAmountText;
    public TMP_Text crystalsAmountText;

    [Header("Opcional: nombres")]
    public TMP_Text goldNameText;
    public TMP_Text foodNameText;
    public TMP_Text woodNameText;
    public TMP_Text stoneNameText;
    public TMP_Text ironNameText;
    public TMP_Text leatherNameText;
    public TMP_Text crystalsNameText;

    private void Awake()
    {
        AutoBind();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null)
        {
            SetAllAmounts("0");
            return;
        }

        SetText(goldNameText, "Oro");
        SetText(foodNameText, "Comida");
        SetText(woodNameText, "Madera");
        SetText(stoneNameText, "Piedra");
        SetText(ironNameText, "Hierro");
        SetText(leatherNameText, "Cuero");
        SetText(crystalsNameText, "Cristal");

        SetText(goldAmountText, inventory.gold.ToString());
        SetText(foodAmountText, inventory.food.ToString());
        SetText(woodAmountText, inventory.wood.ToString());
        SetText(stoneAmountText, inventory.stone.ToString());
        SetText(ironAmountText, inventory.iron.ToString());
        SetText(leatherAmountText, inventory.leather.ToString());
        SetText(crystalsAmountText, inventory.crystals.ToString());
    }

    private void SetAllAmounts(string value)
    {
        SetText(goldAmountText, value);
        SetText(foodAmountText, value);
        SetText(woodAmountText, value);
        SetText(stoneAmountText, value);
        SetText(ironAmountText, value);
        SetText(leatherAmountText, value);
        SetText(crystalsAmountText, value);
    }

    private void AutoBind()
    {
        if (goldNameText == null)
            goldNameText = FindChild<TMP_Text>("Text_GoldText");

        if (goldAmountText == null)
            goldAmountText = FindChild<TMP_Text>("Text_GoldQuan");

        if (foodNameText == null)
            foodNameText = FindChild<TMP_Text>("Text_FoodText");

        if (foodAmountText == null)
            foodAmountText = FindChild<TMP_Text>("Text_FoodQuan");

        if (woodNameText == null)
            woodNameText = FindChild<TMP_Text>("Text_WoodText");

        if (woodAmountText == null)
            woodAmountText = FindChild<TMP_Text>("Text_WoodQuan");

        if (stoneNameText == null)
            stoneNameText = FindChild<TMP_Text>("Text_StoneText");

        if (stoneAmountText == null)
            stoneAmountText = FindChild<TMP_Text>("Text_StoneQuan");

        if (ironNameText == null)
            ironNameText = FindChild<TMP_Text>("Text_IronText");

        if (ironAmountText == null)
            ironAmountText = FindChild<TMP_Text>("Text_IronQuan");

        if (leatherNameText == null)
            leatherNameText = FindChild<TMP_Text>("Text_LetherText");

        if (leatherAmountText == null)
            leatherAmountText = FindChild<TMP_Text>("Text_LetherQuan");

        if (crystalsNameText == null)
            crystalsNameText = FindChild<TMP_Text>("Text_CrystalText");

        if (crystalsAmountText == null)
            crystalsAmountText = FindChild<TMP_Text>("Text_CrystalQuan");
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private T FindChild<T>(string childName) where T : Component
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child.GetComponent<T>();
        }

        return null;
    }
}
