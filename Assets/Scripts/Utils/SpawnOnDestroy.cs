using System.Collections;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private AudioClip impact;

    private void OnDestroy()
    {
        EventManager.TriggerEvent(GenericEvents.PlayGameplaySound, new Hashtable() {
        {GameplayEventHashtableParams.AudioClip.ToString(), impact}
        });

        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
