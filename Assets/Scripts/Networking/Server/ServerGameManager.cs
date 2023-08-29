using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;

public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int queryPort;
    public NetworkServer server { get; private set; }
    private MultiplayAllocationService multiplayAllocationService;
    private MatchplayBackfiller backfiller;

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager, NetworkObject playerPrefab) 
    { 
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        server = new NetworkServer(manager, playerPrefab);
        multiplayAllocationService = new MultiplayAllocationService();
    }

    public async Task StartGameServerAsync()
    {
        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakingPayload = await GetMatchmakerPayload();

            if (matchmakingPayload != null)
            {
                await StartBackfill(matchmakingPayload);
                server.OnUserJoined += UserJoined;
                server.OnUserLeft += UserLeft;
            }

            else
            {
                Debug.LogWarning("Matchmaking payload timed out");
            }
        }

        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        if (!server.OpenConnection(serverIP, serverPort))
        {
            Debug.LogError("Network server did not start as expected");
            return;
        }
    }

    private async Task StartBackfill(MatchmakingResults payload)
    {
        backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", payload.QueueName, payload.MatchProperties, 20);

        if (backfiller.NeedsPlayers())
        {
            await backfiller.BeginBackfilling();
        }
    }

    private void UserJoined(GameData user)
    {
        backfiller.AddPlayerToMatch(user);
        multiplayAllocationService.AddPlayer();

        if (!backfiller.NeedsPlayers() && backfiller.IsBackfilling) 
        {
            _ = backfiller.StopBackfill();
        }
    }

    private void UserLeft(GameData user) 
    {
        int playersCount = backfiller.RemovePlayerFromMatch(user.userAuthID);
        multiplayAllocationService.RemovePlayer();

        if(playersCount <= 0)
        {
            CloseServer();
            return;
        }

        if (backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
        {
            _ = backfiller.BeginBackfilling();
        }
    }

    private async void CloseServer()
    {
        await backfiller.StopBackfill();
        Dispose();
        Application.Quit();
    }

    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    public void Dispose()
    {
        server.OnUserJoined -= UserJoined;
        server.OnUserLeft -= UserLeft;

        backfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        server?.Dispose();
    }
}
