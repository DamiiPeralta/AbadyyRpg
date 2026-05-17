using UnityEngine;

public class BattleSceneStarter : MonoBehaviour
{
    public BattleManager battleManager;

    private void Start()
    {
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();

        if (battleManager == null)
        {
            Debug.LogError("BattleSceneStarter: no se encontró BattleManager.");
            return;
        }

        Debug.Log("BattleSceneStarter: iniciando batalla desde BattleManager.StartBattleFromSetup().");

        battleManager.StartBattleFromSetup();
        battleManager.AutomateBattle();
    }
}