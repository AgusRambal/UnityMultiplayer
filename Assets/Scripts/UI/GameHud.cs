using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour, IEventListener
{
    [Header("References")]
    [SerializeField] private Options options;

    [Header("Killing feed")]
    [SerializeField] private GameObject feed;
    [SerializeField] private GameObject feedParent;

    [Header("Other")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private Texture2D corssHair;

    private bool isPaused = false;

    public void Start()
    {
        OnEnableEventListenerSubscriptions();
        DOTween.Init();
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

    public void KillingFeed(Hashtable hashtable)
    {
        string player1 = (string)hashtable[GameplayEventHashtableParams.Killer.ToString()];
        string player2 = (string)hashtable[GameplayEventHashtableParams.Dead.ToString()];

        GameObject feedInstantiated = Instantiate(feed, feedParent.transform);
        feedInstantiated.GetComponent<TMP_Text>().text = $"{player1} killed {player2}";
        feedInstantiated.transform.DOScale(1f, .1f);
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

    private void OnDestroy()
    {
        CancelEventListenerSubscriptions();
    }

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.KillingFeed, KillingFeed);

    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.KillingFeed, KillingFeed);

    }
}
