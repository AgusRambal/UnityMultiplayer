using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;

            foreach (LeaderboardEntityState entity in leaderboardEntities) 
            { 
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                { 
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            Player[] players = FindObjectsOfType<Player>();

            foreach (Player player in players)
            {
                HandlePlayerSpawned(player);
            }

            Player.OnPlayerSpawned += HandlePlayerSpawned;
            Player.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Insert:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.RemoveAt:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Clear:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Full:
                break;
            default:
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }

        if (IsServer)
        {
            Player.OnPlayerSpawned -= HandlePlayerSpawned;
            Player.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandlePlayerSpawned(Player player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            clientID = player.OwnerClientId,
            playerName = player.playerName.Value,
            coins = 0
        }) ;
    }

    private void HandlePlayerDespawned(Player player)
    {
        if (leaderboardEntities == null)
            return;

        foreach (LeaderboardEntityState entity in leaderboardEntities) 
        {
            if (entity.clientID != player.OwnerClientId)
            {
                continue;
            }

            leaderboardEntities.Remove(entity);
            break;
        }
    }
}
