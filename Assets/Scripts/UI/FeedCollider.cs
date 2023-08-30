using System.Collections;
using UnityEngine;

public class FeedCollider : MonoBehaviour
{
    [HideInInspector] public PlayerInstance player;
    [HideInInspector] public string playerOwner;

    private string enemy;
    private int bulletDamage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.TryGetComponent(out Health health))
        {
            if (health.currentHealth.Value - bulletDamage <= 0)
            {
                if (collision.attachedRigidbody.TryGetComponent(out PlayerInstance otherPlayer))
                {
                    enemy = otherPlayer.playerName.Value.ToString();
                }

                player.killingCounter++;

                EventManager.TriggerEvent(GenericEvents.KillingSpree, new Hashtable() {
                {GameplayEventHashtableParams.Player.ToString(), player},
                {GameplayEventHashtableParams.Killings.ToString(), player.killingCounter}
                });

                EventManager.TriggerEvent(GenericEvents.KillingFeed, new Hashtable() {
                {GameplayEventHashtableParams.Killer.ToString(), playerOwner},
                {GameplayEventHashtableParams.Dead.ToString(), enemy}
                });
            }
        }
    }
}
