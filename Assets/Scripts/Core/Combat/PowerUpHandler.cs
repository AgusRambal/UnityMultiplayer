using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PowerUpHandler : NetworkBehaviour, IEventListener
{
    [SerializeField] private NetworkObject levelUpVFX;

    private PlayerInstance newPlayer;
    private Vector2 posToSpawn;
    private Quaternion rotation;
    private int killingCounter;
    private int keptCoins;
    private int level;
     
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

        posToSpawn = player.transform.position;
        rotation = player.transform.GetChild(0).transform.rotation;
        killingCounter = player.killingCounter;
        keptCoins = player.wallet.totalCoins.Value;
        level = player.level;
        this.newPlayer = newPlayer;

        Destroy(player.gameObject);

        StartCoroutine(LevelUp(player.OwnerClientId));
    }

    private IEnumerator LevelUp(ulong ownerClientID)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(newPlayer, posToSpawn, Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.transform.GetChild(0).transform.rotation = rotation;
        playerInstance.wallet.totalCoins.Value = keptCoins;
        playerInstance.level = level;
        playerInstance.killingCounter = killingCounter;

        LevelUpVFX();
    }

    public void LevelUpVFX()
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
