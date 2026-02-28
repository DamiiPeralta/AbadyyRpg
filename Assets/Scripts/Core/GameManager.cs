using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;

    [SerializeField] private GameObject[] playerHealthBarPrefabs = new GameObject[3];
    [SerializeField] private GameObject[] enemyHealthBarPrefabs = new GameObject[8];
    public BattleTest BattleTest;

    public GameObject[] GetPlayerHealthBars() => playerHealthBarPrefabs;
    public GameObject[] GetEnemyHealthBars() => enemyHealthBarPrefabs;

    public void Start()
    {
        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
    }
    public void SetHealthBars(List<Unit> playerUnitsList, List<Unit> enemyUnitsList)
    {
        playerUnits = playerUnitsList;
        enemyUnits = enemyUnitsList;

        if (playerHealthBarPrefabs == null || playerHealthBarPrefabs.Length == 0)
        {
            Debug.LogWarning("playerHealthBarPrefabs no está asignado en el inspector");
            return;
        }

        if (enemyHealthBarPrefabs == null || enemyHealthBarPrefabs.Length == 0)
        {
            Debug.LogWarning("enemyHealthBarPrefabs no está asignado en el inspector");
            return;
        }

        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (i < playerHealthBarPrefabs.Length && playerHealthBarPrefabs[i] != null)
            {
                SetupHealthBar(playerHealthBarPrefabs[i], playerUnits[i], "Player");
            }
        }

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (i < enemyHealthBarPrefabs.Length && enemyHealthBarPrefabs[i] != null)
            {
                SetupHealthBar(enemyHealthBarPrefabs[i], enemyUnits[i], "Enemy");
            }
        }
    }

    private void SetupHealthBar(GameObject healthBarPrefab, Unit unit, string type)
    {
        // Buscar el UnitHealthBar en la unidad (en unitView)
        UnitHealthBar healthBar = unit.unitView.GetComponent<UnitHealthBar>();
        if (healthBar == null)
        {
            Debug.LogWarning($"Unit {unit.unitName} ({type}) no tiene componente UnitHealthBar en unitView");
            return;
        }

        // Si el prefab tiene HealthBarUI, usarlo (más claro para el diseñador)
        HealthBarUI ui = healthBarPrefab.GetComponent<HealthBarUI>();
        if (ui != null)
        {
            bool ok = ui.ApplyTo(healthBar, unit);
            if (ok)
            {
                Debug.Log($"HealthBar {type} (HealthBarUI) configurado para {unit.unitName}");
                return;
            }
            else
            {
                Debug.LogWarning($"HealthBarUI no pudo aplicarse para {unit.unitName}");
            }
        }

        // Fallback: buscar componentes manualmente en el prefab
        Slider slider = healthBarPrefab.GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogWarning($"HealthBar prefab {type} no tiene Slider");
            return;
        }

        TextMeshProUGUI[] allTexts = healthBarPrefab.GetComponentsInChildren<TextMeshProUGUI>();
        if (allTexts.Length < 2)
        {
            Debug.LogWarning($"HealthBar prefab {type} no tiene al menos 2 TextMeshProUGUI. Encontrados: {allTexts.Length}");
            return;
        }

        TextMeshProUGUI nameText = allTexts[0];
        TextMeshProUGUI healthText = allTexts[1];

        healthBar.healthSlider = slider;
        healthBar.nameText = nameText;
        healthBar.healthText = healthText;

        healthBar.InitializeHealthBar();

        Debug.Log($"HealthBar {type} configurado para {unit.unitName} (fallback)");
    }

    
}
