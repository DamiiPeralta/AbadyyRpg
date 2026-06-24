using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class CaravanFirePanelUI : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text stateText;
    public TMP_Text messageText;
    public CaravanFireActionButtonUI partialRestButton;
    public CaravanFireActionButtonUI physicalRepairButton;
    public CaravanFireActionButtonUI magicalRepairButton;
    public CaravanFireActionButtonUI sleepButton;
    public CaravanFireActionButtonUI gatherResourcesButton;

    [Header("Resource Icons")]
    public Sprite foodIcon;
    public Sprite woodIcon;
    public Sprite ironIcon;
    public Sprite leatherIcon;
    public Sprite crystalsIcon;

    [Header("Recovery")]
    [Range(0f, 1f)]
    public float partialRecoveryPercent = 0.50f;
    [Range(0f, 1f)]
    public float armorRepairPercent = 0.50f;

    [Header("Costs")]
    public int partialRestFoodPerLivingMember = 1;
    public int partialRestWoodCost = 0;
    public int partialRestCaravanStaminaCost = 0;
    public int partialRestHoursCost = 2;
    public int sleepFoodPerLivingMember = 1;
    public int sleepWoodCost = 2;
    public int physicalArmorIronCost = 0;
    public int physicalArmorLeatherCost = 1;
    public int physicalArmorRepairHoursCost = 2;
    public int magicalArmorCrystalsCost = 0;
    public int magicalArmorLeatherCost = 0;
    public int magicalArmorRepairHoursCost = 2;

    private void Awake()
    {
        AutoBind();
        HideGatherResourcesButton();
        BindButtons();
        GameSfxPlayer.GetOrCreate();
        ScreenFadeUI.GetOrCreate();
    }

    private void OnEnable()
    {
        PartyRuntimeState.PartyChanged -= HandleRuntimeChanged;
        PartyRuntimeState.PartyChanged += HandleRuntimeChanged;
        Refresh();
    }

    private void OnDisable()
    {
        PartyRuntimeState.PartyChanged -= HandleRuntimeChanged;
    }

    public void Refresh()
    {
        RefreshStateText();
        RefreshActionButtons();
    }

    public void ExecutePartialRest()
    {
        ExecuteAction(CaravanFireActionType.PartialRest);
    }

    public void ExecutePhysicalArmorRepair()
    {
        ExecuteAction(CaravanFireActionType.RepairPhysicalArmor);
    }

    public void ExecuteMagicalArmorRepair()
    {
        ExecuteAction(CaravanFireActionType.PartialRest);
    }

    public void ExecuteSleep()
    {
        ExecuteAction(CaravanFireActionType.Sleep);
    }

    private void BindButtons()
    {
        partialRestButton?.Bind(
            CaravanFireActionType.PartialRest,
            "Descanso parcial",
            $"Recupera {GetPercentText(partialRecoveryPercent)} HP y energia. Consume comida y tiempo.",
            ExecuteAction);

        physicalRepairButton?.Bind(
            CaravanFireActionType.RepairPhysicalArmor,
            "Reparar armaduras",
            $"Repara {GetPercentText(armorRepairPercent)} de armadura fisica y magica. Consume cuero y tiempo.",
            ExecuteAction);

        magicalRepairButton?.Bind(
            CaravanFireActionType.PartialRest,
            "Descanso parcial",
            $"Recupera {GetPercentText(partialRecoveryPercent)} HP y energia. Consume comida y tiempo.",
            ExecuteAction);

        sleepButton?.Bind(
            CaravanFireActionType.Sleep,
            "Dormir",
            "Recupera compania y energia de viaje. Pasa el dia.",
            ExecuteAction);

        HideGatherResourcesButton();
    }

    private void RefreshActionButtons()
    {
        RefreshButton(partialRestButton, CaravanFireActionType.PartialRest);
        RefreshButton(physicalRepairButton, CaravanFireActionType.RepairPhysicalArmor);
        RefreshButton(magicalRepairButton, CaravanFireActionType.PartialRest);
        RefreshButton(sleepButton, CaravanFireActionType.Sleep);
        HideGatherResourcesButton();
    }

    private void RefreshButton(CaravanFireActionButtonUI button, CaravanFireActionType actionType)
    {
        if (button == null)
            return;

        CaravanFireResourceCost[] costs = GetCosts(actionType);
        bool[] canPayCosts = GetCanPayCosts(costs);
        Sprite[] icons = GetCostIcons(costs);
        CaravanFireResourceCost[] displayCosts = GetDisplayCosts(costs);
        bool canUse = CanPayAll(canPayCosts) && CanPayActionCosts(actionType) && HasRequiredTargets(actionType);

        button.SetCosts(displayCosts, icons, canPayCosts);
        button.SetAvailable(canUse);
    }

    private void ExecuteAction(CaravanFireActionType actionType)
    {
        if (!ValidateRuntime())
            return;

        if (!HasRequiredTargets(actionType))
        {
            SetMessage(GetNoTargetMessage(actionType));
            Refresh();
            return;
        }

        CaravanFireResourceCost[] costs = GetCosts(actionType);

        if (!CanPayAll(GetCanPayCosts(costs)) || !CanPayActionCosts(actionType))
        {
            SetMessage(GetMissingResourcesMessage(actionType));
            Refresh();
            return;
        }

        if (actionType == CaravanFireActionType.Sleep)
        {
            ScreenFadeUI.GetOrCreate().PlaySleepTransition(() =>
            {
                SpendCosts(costs);
                ApplySleep();
                Refresh();
            });
            return;
        }

        SpendCosts(costs);

        switch (actionType)
        {
            case CaravanFireActionType.GatherResources:
                ApplyGatherResources();
                break;

            case CaravanFireActionType.PartialRest:
                ApplyPartialRest();
                break;

            case CaravanFireActionType.RepairPhysicalArmor:
                ApplyPhysicalArmorRepair();
                break;

            case CaravanFireActionType.RepairMagicalArmor:
                ApplyMagicalArmorRepair();
                break;

            case CaravanFireActionType.Sleep:
                ApplySleep();
                break;
        }

        Refresh();
    }

    private void ApplyPartialRest()
    {
        foreach (Unit unit in GetLivingRosterUnits())
        {
            unit.Heal(Mathf.CeilToInt(unit.maxHP * partialRecoveryPercent));
            unit.RestoreStamina(Mathf.CeilToInt(unit.maxStamina * partialRecoveryPercent));
        }

        SpendActionCosts(CaravanFireActionType.PartialRest);
        SetMessage($"Descanso parcial realizado: {GetPercentText(partialRecoveryPercent)} HP y energia. +{partialRestHoursCost} h.");
    }

    private void ApplyGatherResources()
    {
        if (CaravanResourceSearchUtility.TrySearchResources(out string message))
            SetMessage(message);
        else
            SetMessage(message);
    }

    private void ApplyPhysicalArmorRepair()
    {
        int restoredPhysical = 0;
        int restoredMagical = 0;

        foreach (Unit unit in GetLivingRosterUnits())
        {
            restoredPhysical += unit.RestorePhysicalArmor(Mathf.CeilToInt(unit.maxPhysicalArmor * armorRepairPercent));
            restoredMagical += unit.RestoreMagicalArmor(Mathf.CeilToInt(unit.maxMagicalArmor * armorRepairPercent));
        }

        SpendActionCosts(CaravanFireActionType.RepairPhysicalArmor);
        SetMessage($"Armaduras reparadas: +{restoredPhysical} fisica, +{restoredMagical} magica. +{physicalArmorRepairHoursCost} h.");
    }

    private void ApplyMagicalArmorRepair()
    {
        int restored = 0;

        foreach (Unit unit in GetLivingRosterUnits())
            restored += unit.RestoreMagicalArmor(Mathf.CeilToInt(unit.maxMagicalArmor * armorRepairPercent));

        SpendActionCosts(CaravanFireActionType.RepairMagicalArmor);
        SetMessage($"Armadura magica reparada: +{restored}. +{magicalArmorRepairHoursCost} h.");
    }

    private void ApplySleep()
    {
        foreach (Unit unit in GetLivingRosterUnits())
        {
            unit.FullHeal();
            unit.currentStamina = unit.maxStamina;
            unit.currentMana = unit.maxMana;
        }

        if (CaravanState.Instance != null)
        {
            CaravanState.Instance.RestoreStamina();
            CaravanState.Instance.AdvanceDay();
            SetMessage($"La compania durmio hasta el dia {CaravanState.Instance.day}.");
        }
        else
        {
            SetMessage("La compania durmio.");
        }
    }

    private bool HasRequiredTargets(CaravanFireActionType actionType)
    {
        List<Unit> units = GetLivingRosterUnits();

        if (units.Count == 0)
            return false;

        if (actionType == CaravanFireActionType.Sleep)
            return true;

        if (actionType == CaravanFireActionType.GatherResources)
        {
            CaravanState caravan = CaravanState.Instance;
            return caravan != null && caravan.caravanStamina >= CaravanResourceSearchUtility.CaravanStaminaCost;
        }

        foreach (Unit unit in units)
        {
            if (actionType == CaravanFireActionType.PartialRest &&
                (unit.currentHP < unit.maxHP || unit.currentStamina < unit.maxStamina))
                return true;

            if (actionType == CaravanFireActionType.RepairPhysicalArmor &&
                (unit.currentPhysicalArmor < unit.maxPhysicalArmor || unit.currentMagicalArmor < unit.maxMagicalArmor))
                return true;

            if (actionType == CaravanFireActionType.RepairMagicalArmor && unit.currentMagicalArmor < unit.maxMagicalArmor)
                return true;
        }

        return false;
    }

    private CaravanFireResourceCost[] GetCosts(CaravanFireActionType actionType)
    {
        switch (actionType)
        {
            case CaravanFireActionType.PartialRest:
                return new[]
                {
                    CreateCost(CaravanResourceType.Food, partialRestFoodPerLivingMember, true)
                };

            case CaravanFireActionType.RepairPhysicalArmor:
                return new[]
                {
                    CreateCost(CaravanResourceType.Leather, physicalArmorLeatherCost, false)
                };

            case CaravanFireActionType.RepairMagicalArmor:
                return new[]
                {
                    CreateCost(CaravanResourceType.Crystals, magicalArmorCrystalsCost, false),
                    CreateCost(CaravanResourceType.Leather, magicalArmorLeatherCost, false)
                };

            case CaravanFireActionType.Sleep:
                return new[]
                {
                    CreateCost(CaravanResourceType.Food, sleepFoodPerLivingMember, true),
                    CreateCost(CaravanResourceType.Wood, sleepWoodCost, false)
                };

            case CaravanFireActionType.GatherResources:
                return new CaravanFireResourceCost[0];
        }

        return new CaravanFireResourceCost[0];
    }

    private CaravanFireResourceCost CreateCost(CaravanResourceType resourceType, int amount, bool perLivingRosterMember)
    {
        return new CaravanFireResourceCost
        {
            resourceType = resourceType,
            amount = Mathf.Max(0, amount),
            perLivingRosterMember = perLivingRosterMember
        };
    }

    private bool[] GetCanPayCosts(CaravanFireResourceCost[] costs)
    {
        bool[] canPay = new bool[costs != null ? costs.Length : 0];
        int livingMembers = GetLivingRosterUnits().Count;

        for (int i = 0; i < canPay.Length; i++)
        {
            int required = costs[i].GetTotalCost(livingMembers);
            canPay[i] = GetResourceAmount(costs[i].resourceType) >= required;
        }

        return canPay;
    }

    private Sprite[] GetCostIcons(CaravanFireResourceCost[] costs)
    {
        Sprite[] icons = new Sprite[costs != null ? costs.Length : 0];

        for (int i = 0; i < icons.Length; i++)
            icons[i] = GetResourceIcon(costs[i].resourceType);

        return icons;
    }

    private CaravanFireResourceCost[] GetDisplayCosts(CaravanFireResourceCost[] costs)
    {
        CaravanFireResourceCost[] displayCosts = new CaravanFireResourceCost[costs != null ? costs.Length : 0];
        int livingMembers = GetLivingRosterUnits().Count;

        for (int i = 0; i < displayCosts.Length; i++)
        {
            displayCosts[i] = CreateCost(costs[i].resourceType, costs[i].GetTotalCost(livingMembers), false);
        }

        return displayCosts;
    }

    private bool CanPayAll(bool[] canPayCosts)
    {
        if (canPayCosts == null)
            return true;

        foreach (bool canPay in canPayCosts)
        {
            if (!canPay)
                return false;
        }

        return true;
    }

    private bool CanPayActionCosts(CaravanFireActionType actionType)
    {
        int staminaCost = GetCaravanStaminaCost(actionType);
        int hoursCost = GetHoursCost(actionType);

        if (staminaCost <= 0 && hoursCost <= 0)
            return true;

        CaravanState caravan = CaravanState.Instance;

        if (caravan == null)
            return false;

        return staminaCost <= 0 || caravan.caravanStamina >= staminaCost;
    }

    private void SpendActionCosts(CaravanFireActionType actionType)
    {
        CaravanState caravan = CaravanState.Instance;

        if (caravan == null)
            return;

        int staminaCost = GetCaravanStaminaCost(actionType);
        int hoursCost = GetHoursCost(actionType);

        if (staminaCost > 0)
            caravan.ChangeStamina(-staminaCost);

        if (hoursCost > 0)
            caravan.AdvanceHours(hoursCost);
    }

    private int GetCaravanStaminaCost(CaravanFireActionType actionType)
    {
        return actionType == CaravanFireActionType.PartialRest ? Mathf.Max(0, partialRestCaravanStaminaCost) : 0;
    }

    private int GetHoursCost(CaravanFireActionType actionType)
    {
        switch (actionType)
        {
            case CaravanFireActionType.PartialRest:
                return Mathf.Max(0, partialRestHoursCost);
            case CaravanFireActionType.RepairPhysicalArmor:
                return Mathf.Max(0, physicalArmorRepairHoursCost);
            case CaravanFireActionType.RepairMagicalArmor:
                return Mathf.Max(0, magicalArmorRepairHoursCost);
            case CaravanFireActionType.GatherResources:
                return Mathf.Max(0, CaravanResourceSearchUtility.HoursCost);
        }

        return 0;
    }

    private void SpendCosts(CaravanFireResourceCost[] costs)
    {
        if (InventoryRuntimeState.Instance == null || costs == null)
            return;

        int livingMembers = GetLivingRosterUnits().Count;

        foreach (CaravanFireResourceCost cost in costs)
        {
            if (cost == null)
                continue;

            SpendResource(cost.resourceType, cost.GetTotalCost(livingMembers));
        }
    }

    private int GetResourceAmount(CaravanResourceType resourceType)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null)
            return 0;

        switch (resourceType)
        {
            case CaravanResourceType.Food:
                return inventory.food;
            case CaravanResourceType.Wood:
                return inventory.wood;
            case CaravanResourceType.Stone:
                return inventory.stone;
            case CaravanResourceType.Iron:
                return inventory.iron;
            case CaravanResourceType.Leather:
                return inventory.leather;
            case CaravanResourceType.Crystals:
                return inventory.crystals;
        }

        return 0;
    }

    private bool SpendResource(CaravanResourceType resourceType, int amount)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null)
            return false;

        switch (resourceType)
        {
            case CaravanResourceType.Food:
                return inventory.SpendFood(amount);
            case CaravanResourceType.Wood:
                return inventory.SpendWood(amount);
            case CaravanResourceType.Stone:
                return inventory.SpendStone(amount);
            case CaravanResourceType.Iron:
                return inventory.SpendIron(amount);
            case CaravanResourceType.Leather:
                return inventory.SpendLeather(amount);
            case CaravanResourceType.Crystals:
                return inventory.SpendCrystals(amount);
        }

        return false;
    }

    private Sprite GetResourceIcon(CaravanResourceType resourceType)
    {
        switch (resourceType)
        {
            case CaravanResourceType.Food:
                return foodIcon;
            case CaravanResourceType.Wood:
                return woodIcon;
            case CaravanResourceType.Stone:
                return null;
            case CaravanResourceType.Iron:
                return ironIcon;
            case CaravanResourceType.Leather:
                return leatherIcon;
            case CaravanResourceType.Crystals:
                return crystalsIcon;
        }

        return null;
    }

    private void RefreshStateText()
    {
        if (stateText == null)
            return;

        CaravanState caravan = CaravanState.Instance;
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        PartyRuntimeState party = PartyRuntimeState.Instance;

        int totalRoster = party != null && party.GetRoster() != null ? party.GetRoster().Count : 0;
        int active = party != null && party.GetCurrentParty() != null ? party.GetCurrentParty().Count : 0;
        int reserve = party != null ? party.GetReserveParty().Count : 0;
        int wounded = CountRosterUnits(unit => unit != null && unit.isAlive && unit.currentHP < unit.maxHP);
        int fallen = CountRosterUnits(unit => unit != null && !unit.isAlive);
        int maxActive = party != null ? party.maxActiveMembers : 4;

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Estado de la caravana");
        builder.AppendLine();
        builder.AppendLine($"Dia {(caravan != null ? caravan.day : 0)}");
        builder.AppendLine($"Moral {(caravan != null ? caravan.morale : 0)}/100");
        builder.AppendLine($"Energia de viaje {(caravan != null ? caravan.caravanStamina : 0)}/{(caravan != null ? caravan.maxCaravanStamina : 0)}");
        builder.AppendLine($"Carga {(caravan != null ? caravan.currentCarry : 0)}/{(caravan != null ? caravan.maxCarry : 0)}");
        builder.AppendLine();
        builder.AppendLine("Compania");
        builder.AppendLine();
        builder.AppendLine($"Mercenarios activos {active}/{maxActive}");
        builder.AppendLine($"Reserva {reserve}");
        builder.AppendLine($"Heridos {wounded}");
        builder.AppendLine($"Caidos {fallen}");
        builder.AppendLine();
        builder.AppendLine("Suministros clave");
        builder.AppendLine();
        builder.AppendLine($"Comida {(inventory != null ? inventory.food : 0)}");
        builder.AppendLine($"Madera {(inventory != null ? inventory.wood : 0)}");
        builder.AppendLine($"Piedra {(inventory != null ? inventory.stone : 0)}");
        builder.AppendLine($"Hierro {(inventory != null ? inventory.iron : 0)}");
        builder.AppendLine($"Cuero {(inventory != null ? inventory.leather : 0)}");
        builder.AppendLine($"Cristales {(inventory != null ? inventory.crystals : 0)}");

        stateText.text = builder.ToString();
    }

    private int CountRosterUnits(System.Func<Unit, bool> predicate)
    {
        int count = 0;
        List<Unit> roster = PartyRuntimeState.Instance != null ? PartyRuntimeState.Instance.GetRoster() : null;

        if (roster == null)
            return 0;

        foreach (Unit unit in roster)
        {
            if (predicate(unit))
                count++;
        }

        return count;
    }

    private List<Unit> GetLivingRosterUnits()
    {
        List<Unit> livingUnits = new List<Unit>();
        List<Unit> roster = PartyRuntimeState.Instance != null ? PartyRuntimeState.Instance.GetRoster() : null;

        if (roster == null)
            return livingUnits;

        foreach (Unit unit in roster)
        {
            if (unit != null && unit.isAlive)
                livingUnits.Add(unit);
        }

        return livingUnits;
    }

    private string GetNoTargetMessage(CaravanFireActionType actionType)
    {
        switch (actionType)
        {
            case CaravanFireActionType.PartialRest:
                return "No hay nada que recuperar en la compania.";
            case CaravanFireActionType.RepairPhysicalArmor:
                return "Las armaduras ya estan completas.";
            case CaravanFireActionType.RepairMagicalArmor:
                return "La armadura magica ya esta completa.";
            case CaravanFireActionType.Sleep:
                return "No hay mercenarios vivos para dormir.";
            case CaravanFireActionType.GatherResources:
                return $"No hay energia de viaje suficiente para buscar recursos. Requiere {CaravanResourceSearchUtility.CaravanStaminaCost}.";
        }

        return "No se puede realizar la accion.";
    }

    private string GetMissingResourcesMessage(CaravanFireActionType actionType)
    {
        switch (actionType)
        {
            case CaravanFireActionType.PartialRest:
                return "No se puede descansar: falta comida.";
            case CaravanFireActionType.RepairPhysicalArmor:
                return "No se puede reparar armaduras: falta cuero o estado de caravana.";
            case CaravanFireActionType.RepairMagicalArmor:
                return "No se puede reparar armadura magica: faltan cristales, cuero o estado de caravana.";
            case CaravanFireActionType.Sleep:
                return "No se puede dormir: faltan comida o madera.";
            case CaravanFireActionType.GatherResources:
                return "No se puede buscar recursos: falta estado de caravana o energia de viaje.";
        }

        return "Faltan recursos.";
    }

    private bool ValidateRuntime()
    {
        if (InventoryRuntimeState.Instance == null)
        {
            SetMessage("No existe InventoryRuntimeState.");
            return false;
        }

        if (PartyRuntimeState.Instance == null || PartyRuntimeState.Instance.GetRoster() == null || PartyRuntimeState.Instance.GetRoster().Count == 0)
        {
            SetMessage("No hay roster para usar la hoguera.");
            return false;
        }

        return true;
    }

    private string GetPercentText(float percent)
    {
        return $"{Mathf.RoundToInt(percent * 100f)}%";
    }

    private void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;

        if (!string.IsNullOrWhiteSpace(message))
            Debug.Log(message);
    }

    private void HandleRuntimeChanged()
    {
        if (isActiveAndEnabled)
            Refresh();
    }

    private void AutoBind()
    {
        if (partialRestButton == null)
            partialRestButton = FindChild<CaravanFireActionButtonUI>("partialRestButton");

        if (physicalRepairButton == null)
            physicalRepairButton = FindChild<CaravanFireActionButtonUI>("physicalRepairButton");

        if (magicalRepairButton == null)
            magicalRepairButton = FindChild<CaravanFireActionButtonUI>("magicalRepairButton");

        if (sleepButton == null)
            sleepButton = FindChild<CaravanFireActionButtonUI>("sleepButton");

        if (gatherResourcesButton == null)
            gatherResourcesButton = FindChild<CaravanFireActionButtonUI>("gatherResourcesButton");

        if (stateText == null)
            stateText = FindChild<TMP_Text>("TextState");

        if (messageText == null)
            messageText = FindChild<TMP_Text>("Text_Message");
    }

    private void HideGatherResourcesButton()
    {
        if (gatherResourcesButton != null)
            gatherResourcesButton.gameObject.SetActive(false);
    }

    private T FindChild<T>(string childName) where T : Component
    {
        Transform child = FindChildTransform(childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindChildTransform(string childName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child;
        }

        return null;
    }
}
