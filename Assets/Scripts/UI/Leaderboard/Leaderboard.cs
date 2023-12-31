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
    public List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

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
            PlayerInstance[] players = FindObjectsOfType<PlayerInstance>();

            foreach (PlayerInstance player in players)
            {
                HandlePlayerSpawned(player);
            }

            PlayerInstance.OnPlayerSpawned += HandlePlayerSpawned;
            PlayerInstance.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        if (!gameObject.scene.isLoaded)
            return;

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:

                if (!entityDisplays.Any(x => x.clientID == changeEvent.Value.clientID))
                {
                    LeaderboardEntityDisplay leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder);
                    leaderboardEntity.Initialize(changeEvent.Value.clientID, changeEvent.Value.playerName, changeEvent.Value.points, changeEvent.Value.deaths, changeEvent.Value.totalKills);
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
                    displayToUpdate.UpdateKills(changeEvent.Value.points, changeEvent.Value.deaths, changeEvent.Value.totalKills);
                }

                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Clear:
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Full:
                break;
            default:
                break;
        }

        entityDisplays.Sort((x, y) => y.points.CompareTo(x.points));

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
            PlayerInstance.OnPlayerSpawned -= HandlePlayerSpawned;
            PlayerInstance.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandlePlayerSpawned(PlayerInstance player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            clientID = player.OwnerClientId,
            playerName = player.playerName.Value,
            points = 0,
            deaths = 0,
        }) ;

        player.points.OnValueChanged += (oldPoints, newPoints) => HandlePointsChange(player.OwnerClientId, newPoints);
        player.myDeaths.OnValueChanged += (oldDeaths, newDeaths) => HandleDeathsChange(player.OwnerClientId, newDeaths);
        player.totalKills.OnValueChanged += (oldTotalKills, newTotalKills) => HandleTotalKillsChange(player.OwnerClientId, newTotalKills);
    }

    private void HandlePlayerDespawned(PlayerInstance player)
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

        player.points.OnValueChanged -= (oldPoints, newPoints) => HandlePointsChange(player.OwnerClientId, newPoints);
        player.myDeaths.OnValueChanged -= (oldDeaths, newDeaths) => HandleDeathsChange(player.OwnerClientId, newDeaths);
        player.totalKills.OnValueChanged -= (oldTotalKills, newTotalKills) => HandleTotalKillsChange(player.OwnerClientId, newTotalKills);
    }

    private void HandlePointsChange(ulong clientID, int newPoints)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].clientID != clientID)
                continue;

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                clientID = leaderboardEntities[i].clientID,
                playerName = leaderboardEntities[i].playerName.Value,
                points = newPoints,
                deaths = leaderboardEntities[i].deaths,
                totalKills = leaderboardEntities[i].totalKills,
            };

            return;
        }
    }

    private void HandleDeathsChange(ulong clientID, int newDeaths)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].clientID != clientID)
                continue;

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                clientID = leaderboardEntities[i].clientID,
                playerName = leaderboardEntities[i].playerName.Value,
                points = leaderboardEntities[i].points,
                deaths = newDeaths,
                totalKills = leaderboardEntities[i].totalKills,
            };

            return;
        }
    }

    private void HandleTotalKillsChange(ulong clientID, int newTotalKills)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].clientID != clientID)
                continue;

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                clientID = leaderboardEntities[i].clientID,
                playerName = leaderboardEntities[i].playerName.Value,
                points = leaderboardEntities[i].points,
                deaths = leaderboardEntities[i].deaths,
                totalKills = newTotalKills,
            };

            return;
        }
    }
}
