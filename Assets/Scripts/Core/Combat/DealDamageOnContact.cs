using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    [HideInInspector] public PlayerInstance playerShooted;

    private ulong ownerClientID;

    public void SetOwner(ulong ownerClientID)
    {
        this.ownerClientID = ownerClientID;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null)
            return;

        if (!collision.attachedRigidbody.TryGetComponent(out PlayerInstance playerEnemy))
            return;

        if (!collision.attachedRigidbody.TryGetComponent(out Health health))
            return;

        if (collision.attachedRigidbody.TryGetComponent(out NetworkObject netObj))
        {
            if (ownerClientID == netObj.OwnerClientId)
            {
                return;
            }
        }

        health.TakeDamage(damage);

        if (health.currentHealth.Value <= 0)
        {
            playerShooted.kills++;

            EventManager.TriggerEvent(GenericEvents.KillingSpree, new Hashtable() {
            {GameplayEventHashtableParams.Player.ToString(), playerShooted},
            {GameplayEventHashtableParams.Killings.ToString(), playerShooted.kills}
            });

            EventManager.TriggerEvent(GenericEvents.KillingFeed, new Hashtable() {
            {GameplayEventHashtableParams.Killer.ToString(), playerShooted.playerName.Value.ToString()},
            {GameplayEventHashtableParams.Dead.ToString(), playerEnemy.playerName.Value.ToString() }
            });
        }
    }
}
