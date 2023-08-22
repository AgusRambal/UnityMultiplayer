using System.Collections;
using UnityEngine;

public class OnClickSFX : MonoBehaviour
{
    [SerializeField] private AudioClip SFX;

    public void PlaySoundFX()
    {
        EventManager.TriggerEvent(GenericEvents.PlayUISFXSound, new Hashtable() {
        {GameplayEventHashtableParams.AudioClip.ToString(), SFX}
        });
    }
}
