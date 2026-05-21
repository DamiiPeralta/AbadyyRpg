using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeStackSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text amountText;

    [Header("Fallback Icons")]
    public Sprite goldIcon;
    public Sprite foodIcon;
    public Sprite woodIcon;
    public Sprite stoneIcon;
    public Sprite ironIcon;
    public Sprite leatherIcon;
    public Sprite crystalsIcon;

    private readonly Color enoughColor = new Color(0.85f, 0.97f, 0.78f, 1f);
    private readonly Color missingColor = new Color(1f, 0.42f, 0.42f, 1f);
    private readonly Color neutralColor = Color.white;

    private void Awake()
    {
        AutoBind();
    }

    public void Show(RecipeStack stack, int multiplier, bool showOwned)
    {
        if (stack == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);

        int amount = Mathf.Max(0, stack.amount) * Mathf.Max(1, multiplier);
        int owned = showOwned ? GetOwnedAmount(stack) : 0;

        SetText(nameText, GetStackName(stack));
        SetText(amountText, showOwned ? $"{amount}/{owned}" : $"x{amount}");
        SetIcon(GetStackIcon(stack));

        if (amountText != null)
            amountText.color = showOwned ? (owned >= amount ? enoughColor : missingColor) : neutralColor;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private int GetOwnedAmount(RecipeStack stack)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (stack == null || inventory == null)
            return 0;

        switch (stack.stackType)
        {
            case RecipeStackType.Item:
                return inventory.GetAmount(stack.GetItemId());
            case RecipeStackType.Gold:
                return inventory.gold;
            case RecipeStackType.Food:
                return inventory.food;
            case RecipeStackType.Wood:
                return inventory.wood;
            case RecipeStackType.Stone:
                return inventory.stone;
            case RecipeStackType.Iron:
                return inventory.iron;
            case RecipeStackType.Leather:
                return inventory.leather;
            case RecipeStackType.Crystals:
                return inventory.crystals;
            default:
                return 0;
        }
    }

    private string GetStackName(RecipeStack stack)
    {
        if (stack == null)
            return "-";

        if (stack.stackType == RecipeStackType.Item)
        {
            ItemBase item = stack.GetItem();
            return item != null && !string.IsNullOrWhiteSpace(item.itemName) ? item.itemName : stack.GetItemId();
        }

        switch (stack.stackType)
        {
            case RecipeStackType.Gold:
                return "Oro";
            case RecipeStackType.Food:
                return "Comida";
            case RecipeStackType.Wood:
                return "Madera";
            case RecipeStackType.Stone:
                return "Piedra";
            case RecipeStackType.Iron:
                return "Hierro";
            case RecipeStackType.Leather:
                return "Cuero";
            case RecipeStackType.Crystals:
                return "Cristales";
            default:
                return stack.stackType.ToString();
        }
    }

    private Sprite GetStackIcon(RecipeStack stack)
    {
        if (stack == null)
            return null;

        if (stack.stackType == RecipeStackType.Item)
        {
            ItemBase item = stack.GetItem();
            return item != null ? item.icon : null;
        }

        switch (stack.stackType)
        {
            case RecipeStackType.Gold:
                return goldIcon;
            case RecipeStackType.Food:
                return foodIcon;
            case RecipeStackType.Wood:
                return woodIcon;
            case RecipeStackType.Stone:
                return stoneIcon;
            case RecipeStackType.Iron:
                return ironIcon;
            case RecipeStackType.Leather:
                return leatherIcon;
            case RecipeStackType.Crystals:
                return crystalsIcon;
            default:
                return null;
        }
    }

    private void SetIcon(Sprite sprite)
    {
        if (iconImage == null)
            return;

        iconImage.enabled = sprite != null;

        if (sprite != null)
            iconImage.sprite = sprite;
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private void AutoBind()
    {
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>(true);

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        if (nameText == null && texts.Length > 0)
            nameText = texts[0];

        if (amountText == null && texts.Length > 1)
            amountText = texts[1];
    }
}
