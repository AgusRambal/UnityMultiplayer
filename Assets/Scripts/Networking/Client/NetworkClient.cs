using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient
{
    private NetworkManager networkManager;

    private const string menuSceneName = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if (clientID != 0 && clientID != networkManager.LocalClientId)
            return;

        if(SceneManager.GetActiveScene().name != menuSceneName)
        {
            SceneManager.LoadScene(menuSceneName);
        }

        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }
}
