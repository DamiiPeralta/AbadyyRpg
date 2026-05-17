using UnityEngine;

public class PartyInitializer : MonoBehaviour
{
    [Header("Setup inicial de party")]
    public BattleSetup startingBattleSetup;

    private void Start()
    {
        if (startingBattleSetup == null)
        {
            Debug.LogError("PartyInitializer: no hay startingBattleSetup asignado.");
            return;
        }

        if (PartyRuntimeState.Instance == null)
        {
            Debug.LogError("PartyInitializer: no existe PartyRuntimeState.Instance.");
            return;
        }

        if (PartyRuntimeState.Instance.HasParty())
        {
            Debug.Log("PartyInitializer: la party ya existe.");
            return;
        }

        if (PartyRuntimeState.Instance.TryInitializePartyFromSetup(startingBattleSetup))
            Debug.Log("PartyInitializer: party inicial creada.");
        else
            Debug.LogError("PartyInitializer: no se pudo crear la party inicial.");
    }
}
