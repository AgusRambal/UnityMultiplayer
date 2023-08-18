using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager gameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<ClientSingleton>();

            if (instance == null)
                return null;

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        gameManager = new ClientGameManager();

        return await gameManager.InitAsync();
    }

    private void OnDestroy()
    {
        gameManager?.Dispose();
    }
}
