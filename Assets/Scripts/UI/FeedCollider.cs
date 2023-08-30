using System.Collections;
using UnityEngine;

public class FeedCollider : MonoBehaviour
{
    public int bulletDamage = 20;
    public string playerOwner;
    public string enemy;

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

                EventManager.TriggerEvent(GenericEvents.KillingFeed, new Hashtable() {
                {GameplayEventHashtableParams.Killer.ToString(), playerOwner},
                {GameplayEventHashtableParams.Dead.ToString(), enemy}
                });
            }
        }
    }
}
