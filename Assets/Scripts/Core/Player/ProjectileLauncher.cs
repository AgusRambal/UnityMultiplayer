using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject muzzleFlashLVL2;
    [SerializeField] private GameObject muzzleFlashLVL3;
    //[SerializeField] private GameObject muzzleFlashLVL3b;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private TankLevelHandling tankLevel;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;

    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;

    }

    private void Update()
    {
        if (tankLevel.lvl == 2)
        {
            muzzleFlash = muzzleFlashLVL2;
        }

        if (tankLevel.lvl == 3)
        {
            muzzleFlash = muzzleFlashLVL3;
        }

        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if (muzzleFlashTimer <= 0f)
            { 
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner)
            return;

        timer -= Time.deltaTime;

        if (!shouldFire)
            return;

        if (timer > 0)
            return;

        if (wallet.totalCoins.Value < costToFire)
            return;

        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner)
            return;

        SpawnDummyProjectile(spawnPos, direction);
    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (wallet.totalCoins.Value < costToFire)
            return;

        wallet.SpendCoins(costToFire);

        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRPC(spawnPos, direction);
    }
}
