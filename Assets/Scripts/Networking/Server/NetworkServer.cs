using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private NetworkObject playerPrefab;
    public Action<GameData> OnUserJoined;
    public Action<GameData> OnUserLeft;
    public Action<string> OnClientLeft;
    private Dictionary<ulong, string> clientIDtoAuth = new Dictionary<ulong, string>();
    private Dictionary<string, GameData> authIDtoUserData = new Dictionary<string, GameData>();
        
    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
    {
        this.networkManager = networkManager;
        this.playerPrefab = playerPrefab;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if (clientIDtoAuth.TryGetValue(clientID, out string authID))
        {
            clientIDtoAuth.Remove(clientID);
            OnUserLeft?.Invoke(authIDtoUserData[authID]);
            authIDtoUserData.Remove(authID);
            OnClientLeft?.Invoke(authID);
        }
    }

    public GameData GetUserDataByClientID(ulong clientID)
    {
        if (clientIDtoAuth.TryGetValue(clientID, out string authID))
        {
            if (authIDtoUserData.TryGetValue(authID, out GameData data))
            {
                return data;
            }

            return null;
        }

        return null;
    }

    public bool OpenConnection(string ip, int port)
    {
        UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        return networkManager.StartServer();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload =  System.Text.Encoding.UTF8.GetString(request.Payload);
        GameData userData = JsonUtility.FromJson<GameData>(payload);

        clientIDtoAuth[request.ClientNetworkId] = userData.userAuthID;
        authIDtoUserData[userData.userAuthID] = userData;
        OnUserJoined?.Invoke(userData);

        _ = SpawnPlayerDelayed(request.ClientNetworkId);

        response.Approved = true;
        response.CreatePlayerObject = false;
    }

    private async Task SpawnPlayerDelayed(ulong clientID)
    { 
        await Task.Delay(1000);

        NetworkObject playerInstance = GameObject.Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.SpawnAsPlayerObject(clientID);
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= OnNetworkReady;

            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
    }
}
