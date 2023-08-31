using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PowerUpHandler : NetworkBehaviour, IEventListener
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

    private void HandlePowerUp(Hashtable hashtable)
    {
        PlayerInstance player = (PlayerInstance)hashtable[GameplayEventHashtableParams.Player.ToString()];
        PlayerInstance newPlayer = (PlayerInstance)hashtable[GameplayEventHashtableParams.NewPlayer.ToString()];

        PlayerInstance newPlayerInstance = newPlayer;
        Vector2 posToSpawn = player.transform.position;
        Quaternion rotation = player.transform.GetChild(0).transform.rotation;
        int keptCoins = player.wallet.totalCoins.Value;
        int keptKills = 0;

        if (player.totalKills.Value < 1)
        {
            keptKills = player.totalKills.Value;
        }

        else
        {
            keptKills = player.totalKills.Value - 1;
        }

        int playerKills = player.kills;

        Destroy(player.gameObject);

        StartCoroutine(LevelUp(player.OwnerClientId, newPlayerInstance, posToSpawn, rotation, keptCoins, playerKills, keptKills));
    }

    private IEnumerator LevelUp(ulong ownerClientID, PlayerInstance newPlayer, Vector2 posToSpawn, Quaternion rotation, int keptCoins, int playerKills, int keptKills)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(newPlayer, posToSpawn, Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.transform.GetChild(0).transform.rotation = rotation;
        playerInstance.wallet.totalCoins.Value = keptCoins;
        playerInstance.totalKills.Value = keptKills;
        playerInstance.kills = playerKills;

        LevelUpVFX(posToSpawn);
    }

    public void LevelUpVFX(Vector2 posToSpawn)
    {
        NetworkObject vfx = Instantiate(levelUpVFX, posToSpawn, Quaternion.identity);
        vfx.Spawn();
    }

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.HandlePowerUp, HandlePowerUp);
    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.HandlePowerUp, HandlePowerUp);
    }
}
