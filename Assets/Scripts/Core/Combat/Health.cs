using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int maxHelath { get; private set; } = 100;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    public NetworkObject dieVFX;

    private bool isDead;

    public Action<Health> onDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        currentHealth.Value = maxHelath;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(+healValue);

        if (currentHealth.Value >= maxHelath)
        {
            currentHealth.Value = maxHelath;
        }
    }

    private void ModifyHealth(int value)
    {
        if (isDead)
            return;

        int newHealth = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHelath);

        if (currentHealth.Value == 0)
        {
            onDie?.Invoke(this);

            DieVFX();

            isDead = true;
        }
    }

    public void DieVFX()
    {
        NetworkObject vfx = Instantiate(dieVFX, transform.position, Quaternion.identity);
        vfx.Spawn();
    }
}
