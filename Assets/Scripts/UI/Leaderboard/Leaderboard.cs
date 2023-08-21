using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [SerializeField] private int entitiesToDisplay = 8;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

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

                if (!entityDisplays.Any(x => x.clientID == changeEvent.Value.clientID))
                {
                    LeaderboardEntityDisplay leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialize(changeEvent.Value.clientID, changeEvent.Value.playerName, changeEvent.Value.coins);
                    entityDisplays.Add(leaderboardEntity);
                }

                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Insert:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:

                LeaderboardEntityDisplay displayToRemove = entityDisplays.FirstOrDefault(x => x.clientID == changeEvent.Value.clientID);

                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }

                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.RemoveAt:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:

                LeaderboardEntityDisplay displayToUpdate = entityDisplays.FirstOrDefault(x => x.clientID == changeEvent.Value.clientID);

                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.coins);
                }

                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Clear:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Full:
                break;
            default:
                break;
        }

        entityDisplays.Sort((x, y) => y.coins.CompareTo(x.coins));

        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            bool shouldShow = i <= entitiesToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);
        }

        LeaderboardEntityDisplay entityDisplay = entityDisplays.FirstOrDefault(x => x.clientID == NetworkManager.Singleton.LocalClientId);

        if (entityDisplay != null)
        {
            if (entityDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                entityDisplay.gameObject.SetActive(true);
            }
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

        player.wallet.totalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);
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

        player.wallet.totalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);

    }

    private void HandleCoinsChange(ulong clientID, int newCoins)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].clientID != clientID)           
                continue;

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                clientID = leaderboardEntities[i].clientID,
                playerName = leaderboardEntities[i].playerName.Value,
                coins = newCoins
            };

            return;
        }  
    }
}
