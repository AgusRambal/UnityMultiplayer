using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource fxSource;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public const string MIXER_MUSIC = "MusicVolume";
    public const string SFX_MUSIC = "SFXVolume";

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.Music_key, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.sfx_key, 1f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.Music_key, musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.sfx_key, sfxSlider.value);
    }

    public void PlaySound(AudioClip clip)
    {
        fxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
    } 

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat(SFX_MUSIC, Mathf.Log10(value) * 20);
    }
}
