using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DespawnNetworkObject : NetworkBehaviour
{
    [SerializeField] private NetworkObject thisObj;

    private void Start()
    {
        StartCoroutine(DestroyAfterStart());
    }

    public IEnumerator DestroyAfterStart()
    {
        yield return new WaitForSeconds(4);
        thisObj.Despawn();
    }
}
