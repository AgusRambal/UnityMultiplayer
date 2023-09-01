using System.Collections;
using UnityEngine;

public class PowerUpHandling : MonoBehaviour
{
    [SerializeField] private PlayerInstance poweredUpPlayer;

    [SerializeField] private bool isHealth = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHealth)
        {
            if (collision.attachedRigidbody.TryGetComponent(out Health health))
            {
                health.RestoreHealth(50);
            }
        }

        else
        {
            if (collision.attachedRigidbody.TryGetComponent(out PlayerInstance player))
            {
                EventManager.TriggerEvent(GenericEvents.HandlePowerUp, new Hashtable() {
                {GameplayEventHashtableParams.Player.ToString(), player},
                {GameplayEventHashtableParams.NewPlayer.ToString(), poweredUpPlayer}
                });
            }
        }
    }
}
