using System.Collections.Generic;
using UnityEngine;

public class PartyRuntimeState : MonoBehaviour
{
    public static PartyRuntimeState Instance { get; private set; }

    [Header("Roster")]
    public int maxRosterMembers = 6;
    public int maxActiveMembers = 4;

    private List<Unit> runtimeRosterUnits = new List<Unit>();
    private List<Unit> activePartyUnits = new List<Unit>();
    private bool initialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("PartyRuntimeState duplicado destruido.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("PartyRuntimeState inicializado y persistente.");
    }

    public List<Unit> GetOrCreatePartyFromSetup(BattleSetup battleSetup)
    {
        if (!initialized)
        {
            Debug.Log("PartyRuntimeState: no había party, creando party nueva.");
            CreatePartyFromSetup(battleSetup);
        }
        else
        {
            Debug.Log("PartyRuntimeState: reutilizando party existente.");
        }

        return activePartyUnits;
    }

    public bool TryInitializePartyFromSetup(BattleSetup battleSetup)
    {
        if (HasParty())
            return false;

        CreatePartyFromSetup(battleSetup);
        return HasParty();
    }

    public void CreatePartyFromSetup(BattleSetup battleSetup)
    {
        runtimeRosterUnits.Clear();
        activePartyUnits.Clear();
        initialized = false;

        if (battleSetup == null)
        {
            Debug.LogError("PartyRuntimeState: battleSetup es null.");
            return;
        }

        if (battleSetup.allyPrefabs == null || battleSetup.allyPrefabs.Count == 0)
        {
            Debug.LogError("PartyRuntimeState: battleSetup no tiene allyPrefabs.");
            return;
        }

        foreach (GameObject prefab in battleSetup.allyPrefabs)
        {
            if (prefab == null)
                continue;

            UnitData unitData = prefab.GetComponent<UnitData>();

            if (unitData == null)
            {
                Debug.LogWarning($"PartyRuntimeState: el prefab {prefab.name} no tiene UnitData.");
                continue;
            }

            Unit unit = unitData.CreateUnit();
            runtimeRosterUnits.Add(unit);

            if (activePartyUnits.Count < Mathf.Max(1, maxActiveMembers))
                activePartyUnits.Add(unit);

            Debug.Log($"PartyRuntimeState: creada unidad persistente {unit.GetInfo()}");
        }

        initialized = true;

        Debug.Log($"PartyRuntimeState: party creada desde BattleSetup con {runtimeRosterUnits.Count} unidades.");
    }

    public void ResetParty()
    {
        initialized = false;
        runtimeRosterUnits.Clear();
        activePartyUnits.Clear();

        Debug.Log("PartyRuntimeState: party reseteada.");
    }

    public bool HasParty()
    {
        return initialized && activePartyUnits != null && activePartyUnits.Count > 0;
    }

    public List<Unit> GetCurrentParty()
    {
        return activePartyUnits;
    }

    public List<Unit> GetRoster()
    {
        return runtimeRosterUnits;
    }

    public List<Unit> GetReserveParty()
    {
        List<Unit> reserve = new List<Unit>();

        if (runtimeRosterUnits == null)
            return reserve;

        foreach (Unit unit in runtimeRosterUnits)
        {
            if (unit != null && !IsActiveMember(unit))
                reserve.Add(unit);
        }

        return reserve;
    }

    public bool IsActiveMember(Unit unit)
    {
        return unit != null && activePartyUnits != null && activePartyUnits.Contains(unit);
    }

    public bool CanAddMercenary()
    {
        int maxRoster = Mathf.Max(1, maxRosterMembers);
        return runtimeRosterUnits != null && runtimeRosterUnits.Count < maxRoster;
    }

    public bool AddMercenary(Unit unit, bool makeActive = false)
    {
        if (unit == null || runtimeRosterUnits == null)
            return false;

        if (runtimeRosterUnits.Contains(unit))
            return false;

        if (!CanAddMercenary())
            return false;

        runtimeRosterUnits.Add(unit);
        initialized = true;

        if (makeActive)
            TrySetActive(unit, true);

        return true;
    }

    public bool TrySetActive(Unit unit, bool active)
    {
        if (unit == null || runtimeRosterUnits == null || activePartyUnits == null)
            return false;

        if (!runtimeRosterUnits.Contains(unit))
            return false;

        if (active)
        {
            if (activePartyUnits.Contains(unit))
                return true;

            if (activePartyUnits.Count >= Mathf.Max(1, maxActiveMembers))
                return false;

            activePartyUnits.Add(unit);
            return true;
        }

        if (!activePartyUnits.Contains(unit))
            return true;

        if (activePartyUnits.Count <= 1)
            return false;

        activePartyUnits.Remove(unit);
        return true;
    }

    public bool ToggleActive(Unit unit)
    {
        return TrySetActive(unit, !IsActiveMember(unit));
    }

    public bool SetActivePartyOrdered(List<Unit> units)
    {
        if (units == null || units.Count == 0)
            return false;

        if (runtimeRosterUnits == null || activePartyUnits == null)
            return false;

        int maxActive = Mathf.Max(1, maxActiveMembers);

        if (units.Count > maxActive)
            return false;

        List<Unit> orderedUnits = new List<Unit>();

        foreach (Unit unit in units)
        {
            if (unit == null)
                return false;

            if (!runtimeRosterUnits.Contains(unit))
                return false;

            if (orderedUnits.Contains(unit))
                return false;

            orderedUnits.Add(unit);
        }

        activePartyUnits.Clear();
        activePartyUnits.AddRange(orderedUnits);
        initialized = true;
        return true;
    }

    public bool DismissUnit(Unit unit)
    {
        if (unit == null || runtimeRosterUnits == null)
            return false;

        if (!runtimeRosterUnits.Contains(unit))
            return false;

        if (IsActiveMember(unit) && activePartyUnits.Count <= 1)
            return false;

        activePartyUnits.Remove(unit);
        runtimeRosterUnits.Remove(unit);

        if (runtimeRosterUnits.Count == 0)
            initialized = false;

        return true;
    }

    public void ClearSceneViews()
    {
        if (runtimeRosterUnits == null)
            return;

        foreach (Unit unit in runtimeRosterUnits)
        {
            if (unit != null)
                unit.unitView = null;
        }
    }

    public int GetLivingMembersCount()
    {
        if (activePartyUnits == null)
            return 0;

        int count = 0;

        foreach (Unit unit in activePartyUnits)
        {
            if (unit == null)
                continue;

            if (unit.isAlive)
                count++;
        }

        return count;
    }

    public void HealAllLivingPercent(float percent)
    {
        if (activePartyUnits == null || activePartyUnits.Count == 0)
        {
            Debug.LogWarning("PartyRuntimeState: no hay party para curar.");
            return;
        }

        foreach (Unit unit in activePartyUnits)
        {
            if (unit == null)
                continue;

            if (!unit.isAlive)
                continue;

            int healAmount = Mathf.CeilToInt(unit.maxHP * percent);

            unit.Heal(healAmount);

            Debug.Log($"PartyRuntimeState: {unit.unitName} curado por {healAmount}. HP actual: {unit.currentHP}/{unit.maxHP}");
        }
    }

    public void RestoreAllLivingStamina()
    {
        if (activePartyUnits == null || activePartyUnits.Count == 0)
        {
            Debug.LogWarning("PartyRuntimeState: no hay party para restaurar stamina.");
            return;
        }

        foreach (Unit unit in activePartyUnits)
        {
            if (unit == null || !unit.isAlive)
                continue;

            unit.currentStamina = unit.maxStamina;
        }
    }

    public void RestoreAllLivingMana()
    {
        if (activePartyUnits == null || activePartyUnits.Count == 0)
        {
            Debug.LogWarning("PartyRuntimeState: no hay party para restaurar mana.");
            return;
        }

        foreach (Unit unit in activePartyUnits)
        {
            if (unit == null || !unit.isAlive)
                continue;

            unit.currentMana = unit.maxMana;
        }
    }

    public void LogPartyState()
    {
        if (runtimeRosterUnits == null || runtimeRosterUnits.Count == 0)
        {
            Debug.Log("PartyRuntimeState: no hay party runtime.");
            return;
        }

        Debug.Log("=== PARTY RUNTIME STATE ===");

        foreach (Unit unit in runtimeRosterUnits)
        {
            if (unit != null)
                Debug.Log(unit.GetInfo());
        }
    }
}
