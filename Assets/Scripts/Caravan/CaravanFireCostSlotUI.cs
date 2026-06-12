using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaravanFireCostSlotUI : MonoBehaviour
{
    [Header("Refs")]
    public Image resourceIcon;
    public TMP_Text amountText;

    [Header("Colors")]
    public Color availableTextColor = Color.white;
    public Color missingTextColor = new Color(1f, 0.45f, 0.35f, 1f);
    public Color unusedTextColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    public void ShowCost(Sprite icon, int amount, bool canPay)
    {
        gameObject.SetActive(true);

        if (resourceIcon != null && icon != null)
            resourceIcon.sprite = icon;

        if (amountText != null)
        {
            amountText.text = amount.ToString("000");
            amountText.color = canPay ? availableTextColor : missingTextColor;
        }
    }

    public void SetUnused()
    {
        gameObject.SetActive(true);

        if (amountText != null)
        {
            amountText.text = "--";
            amountText.color = unusedTextColor;
        }
    }
}
