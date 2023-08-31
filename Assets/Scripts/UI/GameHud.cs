using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour, IEventListener
{
    [Header("References")]
    [SerializeField] private Options options;

    [Header("Killing feed")]
    [SerializeField] private NetworkObject feed;
    [SerializeField] private GameObject feedParent;
    [SerializeField] private List<NetworkObject> killMessages = new List<NetworkObject>();
    [SerializeField] private GameObject killMessagesParent;

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

        Debug.Log("asd");

        NetworkObject feedInstantiated = Instantiate(feed, feedParent.transform);
        feedInstantiated.Spawn();
        feedInstantiated.GetComponent<TMP_Text>().text = $"{player1} killed {player2}";
        feedInstantiated.transform.DOScale(1f, .1f);
    }

    public void KillingSpree(Hashtable hashtable)
    {
        PlayerInstance player = (PlayerInstance)hashtable[GameplayEventHashtableParams.Player.ToString()];
        int killings = (int)hashtable[GameplayEventHashtableParams.Killings.ToString()];

        Debug.Log("dsa");


        if (player.kills > 5)
            return;

        if (player.kills >= 2)
        {
            if (killMessagesParent.transform.childCount > 0)
            {
                for (int z = 0; z < killMessagesParent.transform.childCount; z++)
                {
                    killMessagesParent.transform.GetChild(z).DOScale(0f, .2f);
                    Destroy(killMessagesParent.transform.GetChild(z).gameObject, .21f);
                }
            }

            NetworkObject feedInstantiated = Instantiate(killMessages[killings], killMessagesParent.transform);
            feedInstantiated.Spawn();
            feedInstantiated.transform.DOScale(1f, .1f);

            StartCoroutine(MessageDisappear(feedInstantiated));
        }
    }

    private IEnumerator MessageDisappear(NetworkObject feed)
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

    private void OnDisable()
    {
        CancelEventListenerSubscriptions();
    }

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.KillingFeed, KillingFeed);
        EventManager.StartListening(GenericEvents.KillingSpree, KillingSpree);
    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.KillingFeed, KillingFeed);
        EventManager.StopListening(GenericEvents.KillingSpree, KillingSpree);
    }
}
