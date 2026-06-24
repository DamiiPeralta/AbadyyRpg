using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class WorldMapResourceDrop
{
    public CaravanResourceType resourceType = CaravanResourceType.Wood;
    public int minAmount = 1;
    public int maxAmount = 2;
    public int weight = 1;
}

public static class CaravanResourceSearchUtility
{
    public const int HoursCost = 4;
    public const int CaravanStaminaCost = 3;

    public static bool TrySearchResources(out string message)
    {
        message = string.Empty;

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        CaravanState caravan = CaravanState.Instance;

        if (inventory == null)
        {
            message = "No existe InventoryRuntimeState.";
            return false;
        }

        if (caravan == null)
        {
            message = "No existe CaravanState para buscar recursos.";
            return false;
        }

        if (caravan.caravanStamina < CaravanStaminaCost)
        {
            message = $"No hay energia de viaje suficiente para buscar recursos. Requiere {CaravanStaminaCost}.";
            return false;
        }

        caravan.ChangeStamina(-CaravanStaminaCost);
        caravan.AdvanceHours(HoursCost);
        string rewardName = AddRandomBasicResource(inventory);

        message = $"La caravana busco recursos: -{CaravanStaminaCost} energia de viaje, +{HoursCost} h, +1 {rewardName}.";
        return true;
    }

    public static bool TrySearchResources(WorldMapNode node, out string message)
    {
        message = string.Empty;

        if (node == null)
        {
            message = "No hay nodo actual para buscar recursos.";
            return false;
        }

        if (!node.allowsResourceSearch || node.resourceSearchDrops == null || node.resourceSearchDrops.Count == 0)
        {
            message = "No hay recursos para buscar aqui.";
            return false;
        }

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        CaravanState caravan = CaravanState.Instance;

        if (inventory == null)
        {
            message = "No existe InventoryRuntimeState.";
            return false;
        }

        if (caravan == null)
        {
            message = "No existe CaravanState para buscar recursos.";
            return false;
        }

        int staminaCost = Mathf.Max(0, node.resourceSearchStaminaCost);
        int hourCost = Mathf.Max(0, node.resourceSearchHourCost);

        if (caravan.caravanStamina < staminaCost)
        {
            message = $"No hay energia de viaje suficiente para buscar recursos. Requiere {staminaCost}.";
            return false;
        }

        WorldMapResourceDrop drop = PickDrop(node.resourceSearchDrops);

        if (drop == null)
        {
            message = "No hay recursos validos para buscar aqui.";
            return false;
        }

        int amount = UnityEngine.Random.Range(Mathf.Max(1, drop.minAmount), Mathf.Max(Mathf.Max(1, drop.minAmount), drop.maxAmount) + 1);

        caravan.ChangeStamina(-staminaCost);

        if (hourCost > 0)
            caravan.AdvanceHours(hourCost);

        AddResource(inventory, drop.resourceType, amount);

        message = $"Conseguiste {GetResourceName(drop.resourceType)} x{amount}. Energia -{staminaCost}.";

        if (hourCost > 0)
            message += $" Tiempo +{hourCost} h.";

        return true;
    }

    public static string BuildPossibleResourcesText(WorldMapNode node)
    {
        if (node == null || !node.allowsResourceSearch || node.resourceSearchDrops == null || node.resourceSearchDrops.Count == 0)
            return string.Empty;

        List<string> names = new List<string>();

        foreach (WorldMapResourceDrop drop in node.resourceSearchDrops)
        {
            if (drop == null)
                continue;

            string name = GetResourceName(drop.resourceType);

            if (!names.Contains(name))
                names.Add(name);
        }

        return names.Count > 0 ? string.Join(", ", names) : string.Empty;
    }

    private static WorldMapResourceDrop PickDrop(List<WorldMapResourceDrop> drops)
    {
        int totalWeight = 0;

        foreach (WorldMapResourceDrop drop in drops)
        {
            if (drop != null)
                totalWeight += Mathf.Max(0, drop.weight);
        }

        if (totalWeight <= 0)
            return null;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (WorldMapResourceDrop drop in drops)
        {
            if (drop == null)
                continue;

            cursor += Mathf.Max(0, drop.weight);

            if (roll < cursor)
                return drop;
        }

        return null;
    }

    private static string AddRandomBasicResource(InventoryRuntimeState inventory)
    {
        int roll = UnityEngine.Random.Range(0, 7);

        switch (roll)
        {
            case 0:
                inventory.AddFood(1);
                return "comida";
            case 1:
                inventory.AddWood(1);
                return "madera";
            case 2:
                inventory.AddStone(1);
                return "piedra";
            case 3:
                inventory.AddIron(1);
                return "hierro";
            case 4:
                inventory.AddLeather(1);
                return "cuero";
            case 5:
                inventory.AddCrystals(1);
                return "cristal";
            default:
                inventory.AddFood(1);
                return "comida";
        }
    }

    private static void AddResource(InventoryRuntimeState inventory, CaravanResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case CaravanResourceType.Food:
                inventory.AddFood(amount);
                break;
            case CaravanResourceType.Wood:
                inventory.AddWood(amount);
                break;
            case CaravanResourceType.Stone:
                inventory.AddStone(amount);
                break;
            case CaravanResourceType.Iron:
                inventory.AddIron(amount);
                break;
            case CaravanResourceType.Leather:
                inventory.AddLeather(amount);
                break;
            case CaravanResourceType.Crystals:
                inventory.AddCrystals(amount);
                break;
        }
    }

    public static string GetResourceName(CaravanResourceType resourceType)
    {
        switch (resourceType)
        {
            case CaravanResourceType.Food:
                return "comida";
            case CaravanResourceType.Wood:
                return "madera";
            case CaravanResourceType.Stone:
                return "piedra";
            case CaravanResourceType.Iron:
                return "hierro";
            case CaravanResourceType.Leather:
                return "cuero";
            case CaravanResourceType.Crystals:
                return "cristales";
            default:
                return resourceType.ToString();
        }
    }
}
