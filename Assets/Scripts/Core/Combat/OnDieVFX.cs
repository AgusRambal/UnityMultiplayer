using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnDieVFX : NetworkBehaviour, IEventListener
{
    [SerializeField] private GameObject dieVFX;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        OnEnableEventListenerSubscriptions();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        CancelEventListenerSubscriptions();
    }

    public void DieVFX(Hashtable hashtable)
    {
        Vector2 pos = (Vector2)hashtable[GameplayEventHashtableParams.DieVFXpos.ToString()];

        Instantiate(dieVFX, pos, Quaternion.identity);
    }

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.DieVFX, DieVFX);
    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.DieVFX, DieVFX);
    }
}
