using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour, IEventListener
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject muzzleFlashLVL1;
    [SerializeField] private GameObject muzzleFlashLVL2;
    [SerializeField] private GameObject muzzleFlashLVL3;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private PlayerInstance player;

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

        OnEnableEventListenerSubscriptions();
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        CancelEventListenerSubscriptions();
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void Update()
    {
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

        if (player.isPaused)
            return;

        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;
    }

    public void SetMuzzleFlash(Hashtable hashtable)
    {
        if (player.level == 1)
        {
            muzzleFlash = muzzleFlashLVL1;
        }

        if (player.level == 2)
        {
            muzzleFlash = muzzleFlashLVL2;
        }

        if (player.level == 3)
        {
            muzzleFlash = muzzleFlashLVL3;
        }
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        //Shot Sound
        EventManager.TriggerEvent(GenericEvents.PlaySound, new Hashtable() {
        {GameplayEventHashtableParams.AudioClip.ToString(), shotSound},
        {GameplayEventHashtableParams.AudioSource.ToString(), audioSource}
        });

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

    public void OnEnableEventListenerSubscriptions()
    {
        EventManager.StartListening(GenericEvents.SetMuzzleFlash, SetMuzzleFlash);
    }

    public void CancelEventListenerSubscriptions()
    {
        EventManager.StopListening(GenericEvents.SetMuzzleFlash, SetMuzzleFlash);
    }
}
