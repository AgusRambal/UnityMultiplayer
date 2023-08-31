using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BountyCoin coinPrefab;
    [SerializeField] private Health health;
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip coinCollected; 
    [SerializeField] private TankLevelHandling tankLevelHandling; 

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private float coinRadius;
    private Collider2D[] coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;
        health.onDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        health.onDie -= HandleDie;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(totalCoins.Value * (bountyPercentage / 100));

        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue)
            return;

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);

            if (numColliders == 0)
                return spawnPoint;
        }
    }

    public void SpendCoins(int costToFire)
    {
        totalCoins.Value -= costToFire;

        EventManager.TriggerEvent(GenericEvents.ShowCoins, new Hashtable() {
        {GameplayEventHashtableParams.Coins.ToString(), totalCoins.Value}
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Coin coin))
            return;

        int coinValue = coin.Collect();

        if (IsOwner)
        {
            //Only the owner reproduce the coinCollcted sound
            EventManager.TriggerEvent(GenericEvents.PlaySound, new Hashtable() {
            {GameplayEventHashtableParams.AudioClip.ToString(), coinCollected},
            {GameplayEventHashtableParams.AudioSource.ToString(), audioSource}
            });
        }

        if (!IsServer)
            return;

        totalCoins.Value += coinValue;

        EventManager.TriggerEvent(GenericEvents.ShowCoins, new Hashtable() {
        {GameplayEventHashtableParams.Coins.ToString(), totalCoins.Value}
        });

        LevelUpCheck();
    }

    public void LevelUpCheck()
    {
        tankLevelHandling.LevelUpCheck(this);
    }
}
