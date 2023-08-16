using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    public void SpendCoins(int costToFire)
    {
        totalCoins.Value -= costToFire;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Coin coin))
            return;

        int coinValue = coin.Collect();

        if (!IsServer)
            return;

        totalCoins.Value += coinValue;
    }
}
