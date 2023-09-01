using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LevelUpHandler : NetworkBehaviour, IEventListener
{
    [SerializeField] private NetworkObject levelUpVFX;

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
        PlayerInstance player = (PlayerInstance)hashtable[GameplayEventHashtableParams.Player.ToString()];
        PlayerInstance lvlTo = (PlayerInstance)hashtable[GameplayEventHashtableParams.PlayerLVL.ToString()];
        Vector2 position = (Vector2)hashtable[GameplayEventHashtableParams.PlayerPos.ToString()];

        PlayerInstance newPlayer = lvlTo;
        Vector2 posToSpawn = position;
        Quaternion rotation = player.transform.GetChild(0).transform.rotation;
        int keptCoins = player.wallet.totalCoins.Value;
        int keptKills = player.totalKills.Value;  
        int playerKills = player.kills.Value;

        Destroy(player.gameObject);
        StartCoroutine(LevelUp(player.OwnerClientId, newPlayer, posToSpawn, rotation, keptCoins, playerKills, keptKills));
    }

    private IEnumerator LevelUp(ulong ownerClientID, PlayerInstance newPlayer, Vector2 posToSpawn, Quaternion rotation, int keptCoins, int playerKills, int keptKills)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(newPlayer, posToSpawn, Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.transform.GetChild(0).transform.rotation = rotation;
        playerInstance.wallet.totalCoins.Value = keptCoins;
        playerInstance.totalKills.Value = keptKills;
        playerInstance.kills.Value = playerKills;


        LevelUpVFX(posToSpawn);
    }

    public void LevelUpVFX(Vector2 posToSpawn)
    {
        NetworkObject vfx = Instantiate(levelUpVFX, posToSpawn, Quaternion.identity);
        vfx.Spawn();
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
