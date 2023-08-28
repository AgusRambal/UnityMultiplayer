using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using System.Text;
using Unity.Services.Authentication;

public class ClientGameManager : IDisposable
{
    private JoinAllocation allocation;
    private NetworkClient networkClient;
    private MatchplayMatchmaker matchmaker;
    private GameData gameData;

    private const string menuSceneName = "MainMenu";

    public async Task<bool> InitAsync()
    { 
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            gameData = new GameData

            {
                userName = PlayerPrefs.GetString(NameSelector.playerNameKey, "Missing Name"),
                userAuthID = AuthenticationService.Instance.PlayerId
            };

            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public void StartClient(string ip, int port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        ConnectClient();
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls"); //The other one was udp
        transport.SetRelayServerData(relayServerData);

        ConnectClient();
    }

    private void ConnectClient()
    {
        string payload = JsonUtility.ToJson(gameData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
    {
        if (matchmaker.IsMatchmaking)
            return;

        MatchmakerPollingResult matchResult = await GetMatchAsync();
        onMatchmakeResponse?.Invoke(matchResult);
    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(gameData);

        if (matchmakingResult.result == MatchmakerPollingResult.Success) 
        {
            StartClient(matchmakingResult.ip, matchmakingResult.port);
        }

        return matchmakingResult.result;
    }

    public async Task CancelMatchmaking()
    {
        await matchmaker.CancelMatchmaking();
    }

    public void Disconnect()
    {
        networkClient.Disconnect();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }

}
