using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    [SerializeField] private Options options;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private Texture2D corssHair;

    private bool isPaused = false;

    public void Start()
    {
        SetJoinCodeOnScreen();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            options.OptionsHandler(false);
            SetCursor();
        }
    }

    public void SetCursor()
    {
        if (isPaused) 
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        else 
        {
            Cursor.SetCursor(corssHair, new Vector2(corssHair.width / 2, corssHair.height / 2), CursorMode.Auto);
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

    public void SetJoinCodeOnScreen()
    {
        codeText.text = $"Game code: {HostSingleton.Instance.gameManager.joinCode}";
    }
}
