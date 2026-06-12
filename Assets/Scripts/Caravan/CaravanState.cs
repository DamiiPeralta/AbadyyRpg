using UnityEngine;

public class CaravanState : MonoBehaviour
{
    public static CaravanState Instance { get; private set; }

    [Header("Time")]
    public int day = 1;
    public int hour = 8;

    [Header("Morale")]
    public int morale = 70;

    [Header("Stamina")]
    public int caravanStamina = 100;
    public int maxCaravanStamina = 100;

    [Header("Carry Capacity")]
    public int currentCarry = 10;
    public int maxCarry = 30;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AdvanceDay()
    {
        day++;
        hour = 8;
    }

    public void AdvanceHours(int hours)
    {
        if (hours <= 0)
            return;

        hour += hours;

        while (hour >= 24)
        {
            hour -= 24;
            day++;
        }
    }

    public void RestoreStamina()
    {
        caravanStamina = maxCaravanStamina;
    }

    public void ChangeMorale(int amount)
    {
        morale += amount;
        morale = Mathf.Clamp(morale, 0, 100);
    }

    public void ChangeStamina(int amount)
    {
        caravanStamina += amount;
        caravanStamina = Mathf.Clamp(caravanStamina, 0, maxCaravanStamina);
    }

    public void SetCarry(int amount)
    {
        currentCarry = Mathf.Clamp(amount, 0, maxCarry);
    }
}
