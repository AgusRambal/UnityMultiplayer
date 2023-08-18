using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        Player[] players = FindObjectsOfType<Player>();

        foreach (Player player in players)
        {
            HandlePlayerSpawned(player);
        }

        Player.OnPlayerSpawned += HandlePlayerSpawned;
        Player.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        Player.OnPlayerSpawned -= HandlePlayerSpawned;
        Player.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(Player player)
    {
        player.health.onDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(Player player)
    {
        player.health.onDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(Player player)
    { 
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID)
    {
        yield return null;

        NetworkObject playerInstanfe = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstanfe.SpawnAsPlayerObject(ownerClientID);
    }
}
