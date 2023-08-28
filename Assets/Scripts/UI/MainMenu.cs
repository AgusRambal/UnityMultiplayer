using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<Sprite> backgrounds = new List<Sprite>();
    [SerializeField] private Image background;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();
    [SerializeField] private GameObject lobbiesPanel;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_Text queueTimerText;
    [SerializeField] private TMP_Text queueTimerStatus;

    private float timeInQueue = 0;
    private bool isMatchmaking;
    private bool isCancelling;
    private bool isBusy;

    void Start()
    {
        if (ClientSingleton.Instance == null)
            return;

        DOTween.Init();
        background.sprite = backgrounds[UnityEngine.Random.Range(0, backgrounds.Count)];
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueTimerText.text = string.Empty;
        SetAnims();
    }

    private void SetAnims()
    {
        for (int i = 0; i < buttons.Count; i++) 
        {
            float time = UnityEngine.Random.Range(.2f, .9f);

            buttons[i].transform.DOScale(1f, time);
            buttons[i].transform.DOScale(.75f, time);       
        }
    }

    //Buttons
    public async void StartHost()
    {
        if (isBusy)
            return;

        isBusy = true;

        await HostSingleton.Instance.gameManager.StartHostAsync();

        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy)
            return;

        isBusy = true;

        await ClientSingleton.Instance.gameManager.StartClientAsync(joinCodeField.text);

        isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy)
            return;

        isBusy = true;

        try
        {
            Lobby joininLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joininLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.gameManager.StartClientAsync(joinCode);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isBusy = false;
    }

    public void ExitGame()
    { 
        Application.Quit();
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(timeInQueue);
            queueTimerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
    }

    public async void FindMatch()
    {
        if (isCancelling)
            return;

        if (isMatchmaking)
        {
            queueTimerStatus.text = "Cancelling...";
            isCancelling = true;
            await ClientSingleton.Instance.gameManager.CancelMatchmaking();
            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            findMatchButtonText.text = "Find Match";
            queueTimerStatus.text = string.Empty;
            queueTimerText.text = string.Empty;

            return;
        }

        if (isBusy)
            return;

        ClientSingleton.Instance.gameManager.MatchmakeAsync(OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueTimerStatus.text = "Searching...";
        timeInQueue = 0f;
        isMatchmaking = true;
        isBusy = true;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueTimerStatus.text = "Connecting...";
                break;    
        }
    }

    public void InteractWithLobbiesPanel(bool state)
    {
        if (state)
        {
            lobbiesPanel.transform.DOScale(1f, .35f);
        }

        else
        {
            lobbiesPanel.transform.DOScale(0f, .25f);
        }
    }
}
