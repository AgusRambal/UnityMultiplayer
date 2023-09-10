using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private PlayerInstance playerPrefab;

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
        int keptCoins = 0;
        int keptKills = 0;

        if (player.totalKills.Value < 1)
        {
            keptKills = player.totalKills.Value;
        }

        else
        {
            keptKills = player.totalKills.Value - 1;
        }

        int playerDeaths = player.myDeaths.Value + 1;

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins, keptKills, playerDeaths));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID, int keptCoins, int keptKills, int playerDeaths)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.wallet.totalCoins.Value += keptCoins;
        playerInstance.totalKills.Value += keptKills;
        playerInstance.myDeaths.Value = playerDeaths;

    }
}
