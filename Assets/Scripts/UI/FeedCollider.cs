using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FeedCollider : MonoBehaviour
{
    [HideInInspector] public PlayerInstance playerShooted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (!collision.attachedRigidbody)
            return;

        if (!collision.attachedRigidbody.TryGetComponent(out Health health))
            return;

        if (!collision.attachedRigidbody.TryGetComponent(out PlayerInstance playerEnemy))
            return;

        if (health.currentHealth.Value - 20 <= 0)
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
        }   */  
    }
}
