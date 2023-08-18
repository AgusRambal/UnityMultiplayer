using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager networkManager;
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
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload =  System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIDtoAuth[request.ClientNetworkId] = userData.userAuthID;
        authIDtoUserData[userData.userAuthID] = userData;

        response.Approved = true;
        response.CreatePlayerObject = true;
    }
}
