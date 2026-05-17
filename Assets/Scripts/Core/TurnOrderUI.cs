using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    [Header("Referencias")]
    public BattleManager battleManager;

    [Header("Slots UI - 20 imágenes")]
    public List<Image> turnIcons = new List<Image>();

    [Header("Visual")]
    public Color activeColor = Color.white;
    public Color usedColor = new Color(0.35f, 0.35f, 0.35f, 0.65f);
    public Color emptyColor = new Color(1f, 1f, 1f, 0f);

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (battleManager == null || turnIcons == null)
            return;

        List<Unit> roundOrder = battleManager.GetCurrentRoundOrder();
        int currentTurnIndex = battleManager.GetCurrentTurnIndex();

        for (int i = 0; i < turnIcons.Count; i++)
        {
            Image iconImage = turnIcons[i];

            if (iconImage == null)
                continue;

            if (roundOrder == null || i >= roundOrder.Count || roundOrder[i] == null)
            {
                iconImage.sprite = null;
                iconImage.color = emptyColor;
                iconImage.enabled = false;
                continue;
            }

            Unit unit = roundOrder[i];

            Sprite icon = null;

            if (unit.unitData != null)
                icon = unit.unitData.icon;

            iconImage.sprite = icon;
            iconImage.enabled = icon != null;

            if (i < currentTurnIndex)
                iconImage.color = usedColor;
            else
                iconImage.color = activeColor;
        }
    }
}