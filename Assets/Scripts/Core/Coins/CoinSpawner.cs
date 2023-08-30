using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;
    [SerializeField] private List<RespawningCoin> powerUpPrefab = new List<RespawningCoin>();
    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int maxpowerUps = 5;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private int powerUpValue = 15;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;

    private float coinRadius;
    private List<float> powerUpRadius = new List<float>();
    private Collider2D[] coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < powerUpPrefab.Count; i++)
        {
            powerUpRadius.Add(powerUpPrefab[i].GetComponent<CircleCollider2D>().radius);
        }

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }

        for (int i = 0; i < maxpowerUps; i++)
        {
            SpawnPowerUp();
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
        coinInstance.SetValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void SpawnPowerUp()
    {
        RespawningCoin coinInstance = Instantiate(powerUpPrefab[Random.Range(0, powerUpPrefab.Count)], GetSpawnPoint(), Quaternion.identity);
        coinInstance.SetValue(powerUpValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandlePowerUpCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private void HandlePowerUpCollected(RespawningCoin powerUp)
    {
        powerUp.transform.position = GetSpawnPoint();
        powerUp.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);

            Vector2 spawnPoint = new Vector2(x, y);
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);

            if (numColliders == 0)
                return spawnPoint;
        }
    }
}
