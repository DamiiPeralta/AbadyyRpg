using System.Text;

public static class EventOptionCostUtility
{
    public static bool CanPay(WorldMapEventOption option)
    {
        if (option == null)
            return false;

        if (!MeetsFlagRequirements(option))
            return false;

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        CaravanState caravan = CaravanState.Instance;

        if (HasInventoryCosts(option) && inventory == null)
            return false;

        if (inventory != null)
        {
            if (inventory.gold < option.goldCost) return false;
            if (inventory.food < option.foodCost) return false;
            if (inventory.wood < option.woodCost) return false;
            if (inventory.stone < option.stoneCost) return false;
            if (inventory.iron < option.ironCost) return false;
            if (inventory.leather < option.leatherCost) return false;
            if (inventory.crystals < option.crystalsCost) return false;

            if (option.itemCosts != null)
            {
                foreach (EventOptionItemCost itemCost in option.itemCosts)
                {
                    if (itemCost == null || itemCost.amount <= 0)
                        continue;

                    if (!inventory.HasItem(itemCost.GetItemId(), itemCost.amount))
                        return false;
                }
            }
        }

        if (HasCaravanCosts(option) && caravan == null)
            return false;

        if (caravan != null && caravan.caravanStamina < option.caravanStaminaCost)
            return false;

        return true;
    }

    public static bool Pay(WorldMapEventOption option)
    {
        if (!CanPay(option))
            return false;

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;
        CaravanState caravan = CaravanState.Instance;

        if (inventory != null)
        {
            inventory.SpendGold(option.goldCost);
            inventory.SpendFood(option.foodCost);
            inventory.SpendWood(option.woodCost);
            inventory.SpendStone(option.stoneCost);
            inventory.SpendIron(option.ironCost);
            inventory.SpendLeather(option.leatherCost);
            inventory.SpendCrystals(option.crystalsCost);

            if (option.itemCosts != null)
            {
                foreach (EventOptionItemCost itemCost in option.itemCosts)
                {
                    if (itemCost == null || itemCost.amount <= 0)
                        continue;

                    inventory.RemoveItem(itemCost.GetItemId(), itemCost.amount);
                }
            }
        }

        if (caravan != null)
        {
            caravan.ChangeStamina(-option.caravanStaminaCost);
            caravan.AdvanceHours(option.hoursCost);
        }

        return true;
    }

    public static string BuildCostText(WorldMapEventOption option)
    {
        if (option == null)
            return string.Empty;

        StringBuilder builder = new StringBuilder();

        AppendCost(builder, option.goldCost, "oro");
        AppendCost(builder, option.foodCost, "comida");
        AppendCost(builder, option.woodCost, "madera");
        AppendCost(builder, option.stoneCost, "piedra");
        AppendCost(builder, option.ironCost, "hierro");
        AppendCost(builder, option.leatherCost, "cuero");
        AppendCost(builder, option.crystalsCost, "cristales");
        AppendCost(builder, option.caravanStaminaCost, "stamina caravana");
        AppendCost(builder, option.hoursCost, "h");
        AppendFlags(builder, option.requiredFlags, "requiere");
        AppendFlags(builder, option.blockedByFlags, "bloqueado por");

        if (option.itemCosts != null)
        {
            foreach (EventOptionItemCost itemCost in option.itemCosts)
            {
                if (itemCost == null || itemCost.amount <= 0)
                    continue;

                ItemBase item = itemCost.GetItem();
                string itemName = item != null && !string.IsNullOrWhiteSpace(item.itemName)
                    ? item.itemName
                    : itemCost.GetItemId();

                AppendCost(builder, itemCost.amount, itemName);
            }
        }

        return builder.Length > 0 ? builder.ToString() : "Sin costo";
    }

    private static bool HasInventoryCosts(WorldMapEventOption option)
    {
        if (option.goldCost > 0 || option.foodCost > 0 || option.woodCost > 0 || option.stoneCost > 0 ||
            option.ironCost > 0 || option.leatherCost > 0 || option.crystalsCost > 0)
            return true;

        if (option.itemCosts == null)
            return false;

        foreach (EventOptionItemCost itemCost in option.itemCosts)
        {
            if (itemCost != null && itemCost.amount > 0)
                return true;
        }

        return false;
    }

    private static bool HasCaravanCosts(WorldMapEventOption option)
    {
        return option.caravanStaminaCost > 0 || option.hoursCost > 0;
    }

    private static bool MeetsFlagRequirements(WorldMapEventOption option)
    {
        GameRunState runState = GameRunState.Instance;

        if (option.requiredFlags != null)
        {
            foreach (string flag in option.requiredFlags)
            {
                if (string.IsNullOrWhiteSpace(flag))
                    continue;

                if (runState == null || !runState.HasFlag(flag))
                    return false;
            }
        }

        if (option.blockedByFlags != null && runState != null)
        {
            foreach (string flag in option.blockedByFlags)
            {
                if (!string.IsNullOrWhiteSpace(flag) && runState.HasFlag(flag))
                    return false;
            }
        }

        return true;
    }

    private static void AppendCost(StringBuilder builder, int amount, string label)
    {
        if (amount <= 0)
            return;

        if (builder.Length > 0)
            builder.Append(", ");

        builder.Append(amount);
        builder.Append(" ");
        builder.Append(label);
    }

    private static void AppendFlags(StringBuilder builder, System.Collections.Generic.List<string> flags, string label)
    {
        if (flags == null || flags.Count == 0)
            return;

        foreach (string flag in flags)
        {
            if (string.IsNullOrWhiteSpace(flag))
                continue;

            if (builder.Length > 0)
                builder.Append(", ");

            builder.Append(label);
            builder.Append(" ");
            builder.Append(flag);
        }
    }
}
