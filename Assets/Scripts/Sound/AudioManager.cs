using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer audioMixer;

    public const string Music_key = "musicVolume";
    public const string sfx_key = "sfxVolume";

    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        LoadVolume();
    }

    private void LoadVolume()
    { 
        float musicVolume = PlayerPrefs.GetFloat(Music_key, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(sfx_key, 1f);

        audioMixer.SetFloat(SoundManager.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat(SoundManager.SFX_MUSIC, Mathf.Log10(sfxVolume) * 20);
    }
}
