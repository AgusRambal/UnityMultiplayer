using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using Unity.Netcode;

public class ServerSingleton : MonoBehaviour
{
    private static ServerSingleton instance;

    public ServerGameManager gameManager { get; private set; }

    public static ServerSingleton Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<ServerSingleton>();

            if (instance == null)
                return null;

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        gameManager = new ServerGameManager(ApplicationData.IP(), ApplicationData.Port(), ApplicationData.QPort(), NetworkManager.Singleton);
    }

    private void OnDestroy()
    {
        gameManager?.Dispose();
    }
}
