using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LevelUpHandler : NetworkBehaviour, IEventListener
{
    [SerializeField] private NetworkObject levelUpVFX;

    private Vector2 posToSpawn;
    private Quaternion rotation;

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

        posToSpawn = position;
        rotation = player.transform.GetChild(0).transform.rotation;
        int keptCoins = player.wallet.totalCoins.Value;
        int level = player.level;

        Destroy(player.gameObject);
        StartCoroutine(LevelUp(player.OwnerClientId, keptCoins, lvlTo, level));
    }

    private IEnumerator LevelUp(ulong ownerClientID, int keptCoins, PlayerInstance lvlTo, int level)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(lvlTo, posToSpawn, Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.transform.GetChild(0).transform.rotation = rotation;
        playerInstance.wallet.totalCoins.Value = keptCoins;
        playerInstance.level = level;

        LevelUpVFX();
        EventManager.TriggerEvent(GenericEvents.SetMuzzleFlash);
    }

    public void LevelUpVFX()
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
