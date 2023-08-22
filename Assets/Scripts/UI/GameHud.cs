using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settings;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            settings.SetActive(false);
        }
    }

    public void QuitGame()
    { 
        if (NetworkManager.Singleton.IsHost) 
        {
            HostSingleton.Instance.gameManager.Shutdown();
        }

        ClientSingleton.Instance.gameManager.Disconnect();
    }
}
