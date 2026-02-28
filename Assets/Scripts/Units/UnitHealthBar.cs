using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Script que maneja la visualización de HP con un slider
public class UnitHealthBar : MonoBehaviour
{
    public Unit unit;
    public Slider healthSlider;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    
    public Slider physicalArmorSlider;
    public Slider magicalArmorSlider;
    public TextMeshProUGUI physicalArmorText;
    public TextMeshProUGUI magicalArmorText;

    private void Start()
    {
        // Start ya no hace nada, la inicialización se hace desde GameManager
    }

    public void InitializeHealthBar()
    {
        Debug.Log($"InitializeHealthBar - nameText: {(nameText != null ? "OK" : "NULL")}, healthSlider: {(healthSlider != null ? "OK" : "NULL")}, healthText: {(healthText != null ? "OK" : "NULL")}, physicalArmorSlider: {(physicalArmorSlider != null ? "OK" : "NULL")}, magicalArmorSlider: {(magicalArmorSlider != null ? "OK" : "NULL")}, physicalArmorText: {(physicalArmorText != null ? "OK" : "NULL")}, magicalArmorText: {(magicalArmorText != null ? "OK" : "NULL")}, unit: {(unit != null ? "OK" : "NULL")}");

        if (unit == null)
            return;

        // Configurar el slider de HP
        if (healthSlider != null)
        {
            healthSlider.maxValue = unit.maxHP;
            healthSlider.value = unit.currentHP;
        }
        
        // Configurar sliders de armadura
        if (physicalArmorSlider != null)
        {
            physicalArmorSlider.maxValue = unit.maxPhysicalArmor;
            physicalArmorSlider.value = unit.currentPhysicalArmor;
            if (physicalArmorText != null)
                physicalArmorText.text = $"{unit.currentPhysicalArmor}/{unit.maxPhysicalArmor}";
        }
        
        if (magicalArmorSlider != null)
        {
            magicalArmorSlider.maxValue = unit.maxMagicalArmor;
            magicalArmorSlider.value = unit.currentMagicalArmor;
            if (magicalArmorText != null)
                magicalArmorText.text = $"{unit.currentMagicalArmor}/{unit.maxMagicalArmor}";
        }

        // Asignar el nombre
        if (nameText != null)
        {
            nameText.text = unit.unitName;
        }

        UpdateHealthBar();
    }

    private void Update()
    {
        if (unit != null)
        {
            UpdateHealthBar();
        }
    }

    public void UpdateHealthBar()
    {
        if (unit == null)
            return;

        if (healthSlider != null)
        {
            healthSlider.value = unit.currentHP;
        }

        if (healthText != null)
        {
            healthText.text = $"{unit.currentHP}/{unit.maxHP}";
        }

        if (physicalArmorSlider != null)
        {
            physicalArmorSlider.value = unit.currentPhysicalArmor;
        }

        if (physicalArmorText != null)
        {
            physicalArmorText.text = $"{unit.currentPhysicalArmor}/{unit.maxPhysicalArmor}";
        }

        if (magicalArmorSlider != null)
        {
            magicalArmorSlider.value = unit.currentMagicalArmor;
        }

        if (magicalArmorText != null)
        {
            magicalArmorText.text = $"{unit.currentMagicalArmor}/{unit.maxMagicalArmor}";
        }

        // Cambiar color del slider de HP según el porcentaje
        if (healthSlider != null)
        {
            Image fillImage = healthSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float hpPercent = (float)unit.currentHP / unit.maxHP;
                
                if (hpPercent > 0.5f)
                    fillImage.color = Color.green;
                else if (hpPercent > 0.25f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }
        
        // Fijar color constante para armaduras: física gris, mágica azul
        if (physicalArmorSlider != null && physicalArmorSlider.fillRect != null)
        {
            Image physFill = physicalArmorSlider.fillRect.GetComponent<Image>();
            if (physFill != null)
            {
                physFill.color = Color.gray;
            }
        }

        if (magicalArmorSlider != null && magicalArmorSlider.fillRect != null)
        {
            Image magFill = magicalArmorSlider.fillRect.GetComponent<Image>();
            if (magFill != null)
            {
                magFill.color = Color.blue;
            }
        }
    }
}
