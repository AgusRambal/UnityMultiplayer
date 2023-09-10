using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInstance : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer miniMapIcon;
    [SerializeField] private Sprite playerIcon;
    [SerializeField] private Color playerColor;
    [SerializeField] private AudioSource tankAudio;
    [SerializeField] private Texture2D corssHair;

    [field: SerializeField] public Health health { get; private set; }
    [field: SerializeField] public CoinWallet wallet { get; private set; }

    [Header("Settings")]
    public int level;
    public int damage;
    public int coins;

    public NetworkVariable<int> totalKills = new NetworkVariable<int>();
    public NetworkVariable<int> kills = new NetworkVariable<int>();
    public NetworkVariable<int> myDeaths = new NetworkVariable<int>();

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    [HideInInspector] public bool isPaused = false;

    [Header("CameraSettings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    private Camera cam;
    private Vector3 velocity;

    public static event Action<PlayerInstance> OnPlayerSpawned;
    public static event Action<PlayerInstance> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GameData userData = null;

            if (IsHost)
            {
                userData = HostSingleton.Instance.gameManager.networkServer.GetUserDataByClientID(OwnerClientId);
            }

            else
            {
                userData = ServerSingleton.Instance.gameManager.server.GetUserDataByClientID(OwnerClientId);
            }

            playerName.Value = userData.userName;
            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            Cursor.SetCursor(corssHair, new Vector2(corssHair.width / 2, corssHair.height / 2), CursorMode.Auto);
            cam = Camera.main;
            miniMapIcon.sprite = playerIcon;
            miniMapIcon.color = playerColor;
            tankAudio.Play();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        coins = wallet.totalCoins.Value;

        if (IsOwner)
        {
            EventManager.TriggerEvent(GenericEvents.ShowCoins, new Hashtable() {
            {GameplayEventHashtableParams.Coins.ToString(), coins}
            });
        }
    }

    private void FixedUpdate()
    {
        CameraFollow();
    }

    private void CameraFollow()
    {
        if (IsOwner)
        {
            Vector3 movePosition = transform.position + offset;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, movePosition, ref velocity, damping);
        }
    }
}
