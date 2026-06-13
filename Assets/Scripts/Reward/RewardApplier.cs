using System.Text;
using UnityEngine;

public static class RewardApplier
{
    public static void ApplyReward(RewardData reward)
    {
        if (reward == null || reward.IsEmpty())
        {
            return;
        }

        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (inventory == null)
        {
            Debug.LogError("No existe InventoryRuntimeState en la escena.");
            return;
        }

        inventory.AddGold(reward.gold);
        inventory.AddFood(reward.food);
        inventory.AddWood(reward.wood);
        inventory.AddIron(reward.iron);
        inventory.AddLeather(reward.leather);

        if (reward.experience > 0 && PartyRuntimeState.Instance != null)
            PartyRuntimeState.Instance.AddExperienceToActiveParty(reward.experience);

        if (reward.items != null)
        {
            foreach (RewardItemEntry entry in reward.items)
            {
                if (entry == null)
                    continue;

                if (string.IsNullOrWhiteSpace(entry.itemId))
                    continue;

                if (entry.amount <= 0)
                    continue;

                inventory.AddItem(entry.itemId, entry.amount);
            }
        }

        Debug.Log(BuildRewardSummary(reward));
    }

    public static string BuildRewardSummary(RewardData reward)
    {
        if (reward == null || reward.IsEmpty())
        {
            return "Recompensa vacía.";
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("Recompensa obtenida: ");

        bool hasPrevious = false;

        AddPart(sb, ref hasPrevious, reward.gold, "oro");
        AddPart(sb, ref hasPrevious, reward.food, "comida");
        AddPart(sb, ref hasPrevious, reward.wood, "madera");
        AddPart(sb, ref hasPrevious, reward.iron, "hierro");
        AddPart(sb, ref hasPrevious, reward.leather, "cuero");
        AddPart(sb, ref hasPrevious, reward.experience, "XP");

        if (reward.items != null)
        {
            foreach (RewardItemEntry entry in reward.items)
            {
                if (entry == null)
                    continue;

                if (string.IsNullOrWhiteSpace(entry.itemId))
                    continue;

                if (entry.amount <= 0)
                    continue;

                if (hasPrevious)
                    sb.Append(", ");

                sb.Append("+");
                sb.Append(entry.amount);
                sb.Append(" ");
                sb.Append(entry.itemId);

                hasPrevious = true;
            }
        }

        sb.Append(".");
        return sb.ToString();
    }

    private static void AddPart(StringBuilder sb, ref bool hasPrevious, int amount, string label)
    {
        if (amount <= 0)
            return;

        if (hasPrevious)
            sb.Append(", ");

        sb.Append("+");
        sb.Append(amount);
        sb.Append(" ");
        sb.Append(label);

        hasPrevious = true;
    }
}
