using TMPro;
using UnityEngine;

public class WorldMapHudUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statusText;

    public void Refresh(WorldMapNode currentNode)
    {
        if (statusText == null)
            return;

        CaravanState caravan = CaravanState.Instance;
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        int day = caravan != null ? caravan.day : 1;
        int hour = caravan != null ? caravan.hour : 8;
        int stamina = caravan != null ? caravan.caravanStamina : 0;
        int maxStamina = caravan != null ? caravan.maxCaravanStamina : 0;
        int gold = inventory != null ? inventory.gold : 0;
        string location = currentNode != null ? currentNode.nodeName : "Sin ubicacion";

        statusText.text = $"Dia {day} | Hora {hour:00} | Stamina {stamina}/{maxStamina} | Oro {gold} | {location}";
    }
}
