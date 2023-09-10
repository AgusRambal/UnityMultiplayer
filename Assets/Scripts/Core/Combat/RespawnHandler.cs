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
        int keptPoints = 0;

        if (player.points.Value < 1)
        {
            keptPoints = player.points.Value;
        }

        else
        {
            keptPoints = player.points.Value - 1;
        }

        int playerDeaths = player.myDeaths.Value + 1;
        int totalKills = player.totalKills.Value;

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins, keptPoints, playerDeaths, totalKills));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID, int keptCoins, int keptPoints, int playerDeaths, int totalKills)
    {
        yield return null;

        PlayerInstance playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.wallet.totalCoins.Value += keptCoins;
        playerInstance.points.Value += keptPoints;
        playerInstance.myDeaths.Value = playerDeaths;
        playerInstance.totalKills.Value += totalKills;
    }
}
