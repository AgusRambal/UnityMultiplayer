using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    public Action<string> OnClientLeft;
    private Dictionary<ulong, string> clientIDtoAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIDtoUserData = new Dictionary<string, UserData>();
        
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

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
            authIDtoUserData.Remove(authID);
            OnClientLeft?.Invoke(authID);
        }
    }

    public UserData GetUserDataByClientID(ulong clientID)
    {
        if (clientIDtoAuth.TryGetValue(clientID, out string authID))
        {
            if (authIDtoUserData.TryGetValue(authID, out UserData data))
            {
                return data;
            }

            return null;
        }

        return null;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload =  System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIDtoAuth[request.ClientNetworkId] = userData.userAuthID;
        authIDtoUserData[userData.userAuthID] = userData;

        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
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
