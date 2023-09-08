using Unity.Netcode;
using UnityEngine;

public class SingeltonGameManaher : NetworkBehaviour
{
    public static SingeltonGameManaher instance;

    public NetworkVariable<bool> startGame = new NetworkVariable<bool>();

    private void Awake()
    {
        instance = this;
        startGame.Value = false;
    }
}
