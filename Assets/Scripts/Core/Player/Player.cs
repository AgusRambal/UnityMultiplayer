using Cinemachine;
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SpriteRenderer miniMapIcon;
    [SerializeField] private Sprite playerIcon;
    [SerializeField] private Color playerColor;
    [field: SerializeField] public Health health { get; private set; }
    [field: SerializeField] public CoinWallet wallet { get; private set; }

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer) 
        {
            UserData userData = HostSingleton.Instance.gameManager.networkServer.GetUserDataByClientID(OwnerClientId);

            playerName.Value = userData.userName;
            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
            miniMapIcon.sprite = playerIcon;
            miniMapIcon.color = playerColor;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
