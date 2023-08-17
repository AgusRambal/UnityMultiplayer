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

public class HostGameManager
{
    private Allocation allocation;
    private string joinCode;
    private string lobbyID;

    private const int maxConnections = 20;
    private const string gameplayScene = "Gameplay";

    public async Task StartHostAsync()
    {
		try
		{
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
        }

        catch (Exception e)
		{

            Debug.Log(e);
            return;
		}

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }

        catch (Exception e)
        {

            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls"); //The other one was udp
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                { 
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member, 
                        value: joinCode
                        )
                }
            };
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("MyLobby", maxConnections, lobbyOptions);

            lobbyID = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15f));
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplayScene, LoadSceneMode.Single);
    }

    private IEnumerator HeartbeatLobby(float waitTime)
    { 
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTime);
        while (true) 
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return delay;
        }
    }
}
