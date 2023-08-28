using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    private bool isMatchmaking;
    private bool isCancelling;

    void Start()
    {
        if (ClientSingleton.Instance == null)
            return;

        DOTween.Init();
        background.sprite = backgrounds[Random.Range(0, backgrounds.Count)];
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueTimerText.text = string.Empty;
        SetAnims();
    }

    private void SetAnims()
    {
        for (int i = 0; i < buttons.Count; i++) 
        {
            float time = Random.Range(.2f, .9f);

            buttons[i].transform.DOScale(1f, time);
            buttons[i].transform.DOScale(.75f, time);       
        }
    }

    //Buttons
    public async void StartHost()
    {
        await HostSingleton.Instance.gameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.gameManager.StartClientAsync(joinCodeField.text);
    }

    public void ExitGame()
    { 
        Application.Quit();
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
            findMatchButtonText.text = "Find Match";
            queueTimerStatus.text = string.Empty;

            return;
        }

        ClientSingleton.Instance.gameManager.MatchmakeAsync(OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueTimerStatus.text = "Searching...";
        isMatchmaking = true;
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
