using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Componente para poner en el prefab UI (panel) que contiene Slider + textos + icono + estados
public class HealthBarUI : MonoBehaviour
{
    [Header("Referencias UI (arrastrar en prefab)")]
    public Image iconImage;

    public Slider healthSlider;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;

    [Header("Armaduras (opcional)")]
    public Slider physicalArmorSlider;
    public Slider magicalArmorSlider;
    public TextMeshProUGUI physicalArmorText;
    public TextMeshProUGUI magicalArmorText;

    [Header("Status Icons (opcional)")]
    public StatusIconUI statusIconUI;

    // Aplica los componentes al UnitHealthBar de la unidad y retorna si fue exitoso
    public bool ApplyTo(UnitHealthBar target, Unit unit)
    {
        if (target == null || unit == null)
            return false;

        target.unit = unit;

        target.healthSlider = healthSlider;
        target.nameText = nameText;
        target.healthText = healthText;

        target.physicalArmorSlider = physicalArmorSlider;
        target.magicalArmorSlider = magicalArmorSlider;
        target.physicalArmorText = physicalArmorText;
        target.magicalArmorText = magicalArmorText;

        if (iconImage != null)
        {
            Sprite icon = null;

            if (unit.unitData != null)
                icon = unit.unitData.icon;

            if (icon == null && unit.unitView != null)
            {
                SpriteRenderer spriteRenderer = unit.unitView.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    icon = spriteRenderer.sprite;
            }

            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        target.InitializeHealthBar();

        if (statusIconUI != null)
        {
            if (unit.unitView != null)
            {
                unit.unitView.statusIconUI = statusIconUI;
                statusIconUI.Refresh(unit.activeEffects);
            }
            else
            {
                Debug.LogWarning($"HealthBarUI: {unit.unitName} no tiene UnitView asignado para conectar StatusIconUI.");
            }
        }

        return true;
    }
}
