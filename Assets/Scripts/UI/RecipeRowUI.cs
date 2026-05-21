using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeRowUI : MonoBehaviour
{
    public Button selectButton;
    public Image iconImage;
    public TMP_Text recipeNameText;
    public TMP_Text primaryStatText;
    public TMP_Text secondaryStatText;
    public Image primaryStatIconImage;
    public Image secondaryStatIconImage;

    [Header("Stat Icons")]
    public Sprite physicalDamageIcon;
    public Sprite magicalDamageIcon;
    public Sprite physicalArmorIcon;
    public Sprite magicalArmorIcon;

    private CraftingRecipeSO recipe;
    private Action<CraftingRecipeSO> onSelected;

    private void Awake()
    {
        AutoBind();
    }

    public void Bind(CraftingRecipeSO recipe, Action<CraftingRecipeSO> onSelected)
    {
        this.recipe = recipe;
        this.onSelected = onSelected;

        Refresh();

        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(HandleSelected);
            selectButton.onClick.AddListener(HandleSelected);
        }
    }

    public void Refresh()
    {
        SetText(recipeNameText, recipe != null ? recipe.GetDisplayName() : "-");
        SetMainIcon();
        SetStats();
    }

    private void SetMainIcon()
    {
        if (iconImage == null)
            return;

        Sprite sprite = recipe != null ? recipe.GetIcon() : null;
        iconImage.enabled = sprite != null;

        if (sprite != null)
            iconImage.sprite = sprite;
    }

    private void SetStats()
    {
        ItemBase previewItem = recipe != null ? recipe.previewItem : null;

        if (previewItem == null && recipe != null)
            previewItem = recipe.GetFirstOutputItem();

        if (previewItem is Weapon weapon)
        {
            SetText(primaryStatText, $"{weapon.physicalDamageMin} - {weapon.physicalDamageMax}");
            SetText(secondaryStatText, $"{weapon.magicalDamageMin} - {weapon.magicalDamageMax}");
            SetIcon(primaryStatIconImage, physicalDamageIcon);
            SetIcon(secondaryStatIconImage, magicalDamageIcon);
            return;
        }

        if (previewItem is Armor armor)
        {
            SetText(primaryStatText, armor.physicalArmor.ToString("00"));
            SetText(secondaryStatText, armor.magicalArmor.ToString("00"));
            SetIcon(primaryStatIconImage, physicalArmorIcon);
            SetIcon(secondaryStatIconImage, magicalArmorIcon);
            return;
        }

        SetText(primaryStatText, string.Empty);
        SetText(secondaryStatText, string.Empty);
        SetIcon(primaryStatIconImage, null);
        SetIcon(secondaryStatIconImage, null);
    }

    private void HandleSelected()
    {
        onSelected?.Invoke(recipe);
    }

    private void AutoBind()
    {
        if (selectButton == null)
            selectButton = GetComponent<Button>();

        if (iconImage == null)
            iconImage = FindChild<Image>("Image_ItemIcon");

        if (recipeNameText == null)
            recipeNameText = FindChild<TMP_Text>("Text_ItemName");

        if (primaryStatText == null)
            primaryStatText = FindChild<TMP_Text>("Text_PrincipalStatistic1");

        if (primaryStatText == null)
            primaryStatText = FindChild<TMP_Text>("Text_PrimaryStat");

        if (secondaryStatText == null)
            secondaryStatText = FindChild<TMP_Text>("Text_PrincipalStatistic2");

        if (secondaryStatText == null)
            secondaryStatText = FindChild<TMP_Text>("Text_SecondaryStat");

        if (primaryStatIconImage == null)
            primaryStatIconImage = FindChild<Image>("Image_PrincipalIcon1");

        if (secondaryStatIconImage == null)
            secondaryStatIconImage = FindChild<Image>("Image_PrincipalIcon2");
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private void SetIcon(Image image, Sprite sprite)
    {
        if (image == null)
            return;

        image.enabled = sprite != null;

        if (sprite != null)
            image.sprite = sprite;
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
