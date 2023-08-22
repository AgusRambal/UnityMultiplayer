using System.Collections;
using UnityEngine;

public class OnClickSFX : MonoBehaviour
{
    [SerializeField] private AudioClip SFX;
    [SerializeField] private AudioSource AudioSource;

    public bool playAtStart = false;

    private void Start()
    {
        if (playAtStart)
            EventManager.TriggerEvent(GenericEvents.PlaySound, new Hashtable() {
            {GameplayEventHashtableParams.AudioClip.ToString(), SFX},
            {GameplayEventHashtableParams.AudioSource.ToString(), AudioSource}
            });
    }

    //Button only
    public void PlaySoundFX()
    {
        EventManager.TriggerEvent(GenericEvents.PlaySound, new Hashtable() {
        {GameplayEventHashtableParams.AudioClip.ToString(), SFX},
        {GameplayEventHashtableParams.AudioSource.ToString(), AudioSource}
        });
    }
}
