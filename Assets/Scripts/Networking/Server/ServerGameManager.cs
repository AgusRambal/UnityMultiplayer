using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;

public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int queryPort;
    private NetworkServer server;
    private MultiplayAllocationService multiplayAllocationService;
    private const string gameplayScene = "Gameplay";

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager) 
    { 
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        server = new NetworkServer(manager);
        multiplayAllocationService = new MultiplayAllocationService();
    }

    public async Task StartGameServerAsync()
    {
        await multiplayAllocationService.BeginServerCheck();

        if (!server.OpenConnection(serverIP, serverPort))
        {
            Debug.LogError("Network server did not start as expected");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(gameplayScene, LoadSceneMode.Single);
    }

    public void Dispose()
    {
        multiplayAllocationService?.Dispose();
        server?.Dispose();
    }
}
