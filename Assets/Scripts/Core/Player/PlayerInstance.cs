using Cinemachine;
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInstance : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SpriteRenderer miniMapIcon;
    [SerializeField] private Sprite playerIcon;
    [SerializeField] private Color playerColor;
    [SerializeField] private AudioSource tankAudio;
    [SerializeField] private Texture2D corssHair;

    [field: SerializeField] public Health health { get; private set; }
    [field: SerializeField] public CoinWallet wallet { get; private set; }

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    public int level;
    public int kills;
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    [HideInInspector] public bool isPaused = false;

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
            virtualCamera.Priority = ownerPriority;
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
    }    
}
