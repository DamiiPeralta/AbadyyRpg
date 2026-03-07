using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Componente para poner en el prefab UI (panel) que contiene Slider + textos
public class HealthBarUI : MonoBehaviour
{
    [Header("Referencias UI (arrastrar en prefab)")]
    public Slider healthSlider;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    
    [Header("Armaduras (opcional)")]
    public Slider physicalArmorSlider;
    public Slider magicalArmorSlider;
    public TextMeshProUGUI physicalArmorText;
    public TextMeshProUGUI magicalArmorText;

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

        target.InitializeHealthBar();
        return true;
    }
}
