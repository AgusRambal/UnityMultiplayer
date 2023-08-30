using System.Collections;
using UnityEngine;

public class PowerUpHandling : MonoBehaviour
{
    [SerializeField] private PlayerInstance poweredUpPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
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
