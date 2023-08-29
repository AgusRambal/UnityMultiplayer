using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private NetworkObject playerPrefab;

    private ApplicationData appData;
    private const string gameplayScene = "Gameplay";

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            Application.targetFrameRate = 240;

            appData = new ApplicationData();
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }

        else 
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated) 
            {
                clientSingleton.gameManager.GoToMenu();             
            }
        }
    }

    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(gameplayScene);

        while (!asyncOperation.isDone) 
        {
            yield return null;
        }

        Task createrServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createrServerTask.IsCompleted);

        Task startServerTask = serverSingleton.gameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}
