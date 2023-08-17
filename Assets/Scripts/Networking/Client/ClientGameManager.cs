using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string menuSceneName = "MainMenu";

    public async Task<bool> InitAsync()
    { 
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
