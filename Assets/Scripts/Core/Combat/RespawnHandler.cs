using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private PlayerInstance playerPrefab;
    [SerializeField] private float keptCoinPercentage;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        PlayerInstance[] players = FindObjectsOfType<PlayerInstance>();

        foreach (PlayerInstance player in players)
        {
            HandlePlayerSpawned(player);
        }

        PlayerInstance.OnPlayerSpawned += HandlePlayerSpawned;
        PlayerInstance.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        PlayerInstance.OnPlayerSpawned -= HandlePlayerSpawned;
        PlayerInstance.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(PlayerInstance player)
    {
        player.health.onDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(PlayerInstance player)
    {
        player.health.onDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(PlayerInstance player)
    {
        int keptCoins = (int)(player.wallet.totalCoins.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID, int keptCoins)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.wallet.totalCoins.Value += keptCoins;
    }
}
