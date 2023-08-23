using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LevelUpHandler : NetworkBehaviour, IEventListener
{
    private Vector2 posToSpawn;

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

    private void HandlePlayerLevel(Hashtable hashtable)
    {
        Player player = (Player)hashtable[GameplayEventHashtableParams.Player.ToString()];
        Player lvlTo = (Player)hashtable[GameplayEventHashtableParams.PlayerLVL.ToString()];
        Vector2 position = (Vector2)hashtable[GameplayEventHashtableParams.PlayerPos.ToString()];

        posToSpawn = position;
        int keptCoins = player.wallet.totalCoins.Value;
        int level = player.level;

        StartCoroutine(Level(player.OwnerClientId, keptCoins, lvlTo, level));
        Destroy(player.gameObject);
    }

    private IEnumerator Level(ulong ownerClientID, int keptCoins, Player lvlTo, int level)
    {
        yield return null;

        Player playerInstance = Instantiate(lvlTo, posToSpawn, Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.wallet.totalCoins.Value = keptCoins;
        playerInstance.level = level;
    }

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.HandlePlayerLevel, HandlePlayerLevel);
    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.HandlePlayerLevel, HandlePlayerLevel);
    }
}
