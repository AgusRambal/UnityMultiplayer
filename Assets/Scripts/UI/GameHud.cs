using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHud : NetworkBehaviour, IEventListener
{
    [Header("References")]
    [SerializeField] private PlayerInstance[] players;
    [SerializeField] private Options options;
    [SerializeField] private Leaderboard leaderboard;

    [Header("Killing feed")]
    [SerializeField] private List<GameObject> killMessages = new List<GameObject>();
    [SerializeField] private GameObject killMessagesParent;
    [SerializeField] private GameObject feed;
    [SerializeField] private GameObject feedParent;
    [SerializeField] private NetworkObject levelUpVFX;

    [Header("LeaderBoard")]
    [SerializeField] private GameObject leaderboardObject;

    [Header("Other")]
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private Texture2D corssHair;
    [SerializeField] private TMP_Text totalCoins;

    [Header("HowToPlay")]
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private GameObject faded;

    [Header("Timer")]
    [SerializeField] private float roundTimeInMinutes;
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject winnerTab;
    [SerializeField] private GameObject waitingTab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip SFX;

    private float timer;
    private bool isPaused = false;
    private bool startTimer = false;

    public void Start()
    {
        OnEnableEventListenerSubscriptions();
        DOTween.Init();
        SetJoinCodeOnScreen();
        startTimer = true;
        timer = roundTimeInMinutes * 60;
        timerText.text = $"{roundTimeInMinutes}:00";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            options.OptionsHandler(false);
            SetAnims(isPaused);
            SetCursor();
        }

        SetWaitingText();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SingeltonGameManaher.instance.startGame.Value = true;
        }

        if (!SingeltonGameManaher.instance.startGame.Value)
            return;

        SetTimer();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            leaderboardObject.transform.DOScale(1f, .05f);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            leaderboardObject.transform.DOScale(0f, .05f);
        }
    }

    public void SetWaitingText()
    {
        if (IsServer)
        {
            waitingText.text = "Press key 'F1' to start game";
        }

        else 
        {
            waitingText.text = "Waiting for the host to start the game..";
        }
    }

    public void ShowCoins(Hashtable hashtable)
    {
        int coins = (int)hashtable[GameplayEventHashtableParams.Coins.ToString()];

        totalCoins.text = $"Total coins: {coins}";
    }

    public void KillingFeed(Hashtable hashtable)
    {
        string player1 = (string)hashtable[GameplayEventHashtableParams.Killer.ToString()];
        string player2 = (string)hashtable[GameplayEventHashtableParams.Dead.ToString()];

        SetFeedClientRpc(player1, player2);
    }

    [ClientRpc]
    public void SetFeedClientRpc(string player1, string player2)
    {
        GameObject feedInstantiated = Instantiate(feed, feedParent.transform);
        feedInstantiated.GetComponent<TMP_Text>().text = $"{player1} killed {player2}";
        feedInstantiated.transform.DOScale(1f, .1f);
    }

    public void KillingSpree(Hashtable hashtable)
    {
        string player = (string)hashtable[GameplayEventHashtableParams.Killer.ToString()];
        int killings = (int)hashtable[GameplayEventHashtableParams.Killings.ToString()];

        SetMessageClientRpc(player, killings);
    }

    [ClientRpc]
    public void SetMessageClientRpc(string player, int killings)
    {
        players = FindObjectsOfType<PlayerInstance>();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerName.Value == player)
            {
                if (players[i].kills.Value > 5)
                    return;

                if (players[i].kills.Value > 1)
                {
                    if (killMessagesParent.transform.childCount > 0)
                    {
                        for (int z = 0; z < killMessagesParent.transform.childCount; z++)
                        {
                            killMessagesParent.transform.GetChild(z).DOScale(0f, .2f);
                            Destroy(killMessagesParent.transform.GetChild(z).gameObject, .21f);
                        }
                    }

                    GameObject feedInstantiated = Instantiate(killMessages[killings], killMessagesParent.transform);
                    feedInstantiated.transform.DOScale(1f, .1f);

                    StartCoroutine(MessageDisappear(feedInstantiated));
                }
            }
        }

        players = null;
    }

    private IEnumerator MessageDisappear(GameObject feed)
    {
        yield return new WaitForSeconds(4);
        feed.transform.DOScale(0f, .2f);
        Destroy(feed, .21f);
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

    public void SetAnims(bool state)
    {
        if (state) 
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float time = Random.Range(.2f, .9f);

                buttons[i].transform.DOScale(1f, time);
                buttons[i].transform.DOScale(.75f, time);
            }
        }

        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float time = Random.Range(.2f, .9f);

                buttons[i].transform.DOScale(0, time);
            }
        }
    }

    public void HowToPlayButton(bool state)
    {
        if (state)
        {
            faded.SetActive(state);
            howToPlayScreen.transform.DOScale(1f, .25f);
        }

        else
        {
            faded.SetActive(state);
            howToPlayScreen.transform.DOScale(0f, .25f);
        }
    }

    private void SetTimer()
    {
        if (startTimer)
        {
            waitingTab.SetActive(false);

            timer -= Time.deltaTime;

            FormatText();

            if (timer <= 0)
            {
                startTimer = false;
                winnerTab.SetActive(true);
                winnerText.text = leaderboard.entityDisplays[0].playerName.ToString();

                EventManager.TriggerEvent(GenericEvents.PlaySound, new Hashtable() {
                {GameplayEventHashtableParams.AudioClip.ToString(), SFX},
                {GameplayEventHashtableParams.AudioSource.ToString(), audioSource}
                });
            }
        }
    }

    private void FormatText()
    {
        int minutes = (int)(timer / 60) % 60;
        int seconds = (int)(timer % 60);

        timerText.text = $"{minutes}:{seconds}";
    }

    private void OnDisable()
    {
        CancelEventListenerSubscriptions();
    }

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.KillingFeed, KillingFeed);
        EventManager.StartListening(GenericEvents.KillingSpree, KillingSpree);
        EventManager.StartListening(GenericEvents.ShowCoins, ShowCoins);
    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.KillingFeed, KillingFeed);
        EventManager.StopListening(GenericEvents.KillingSpree, KillingSpree);
        EventManager.StopListening(GenericEvents.ShowCoins, ShowCoins);
    }
}
