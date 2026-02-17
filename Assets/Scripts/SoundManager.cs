using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [Header("Music")]
    [SerializeField] private AudioSource MusicSouce;
    [SerializeField] private AudioClip[] backgroundMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource SFXSource;

    public static SoundManager instance;

    private readonly string musicVolumeString = "MusicVolume";
    private readonly string SFXVolumeString = "SFXVolume";
    [HideInInspector] public float defaultMusicVolume = 0.2f;
    [HideInInspector] public float defaultSFXVolume = 0.3f;

    [Header("Events")]
    [SerializeField] private UnityEvent onConfigured = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayRandomMusic(backgroundMusic);
        }

        SetDefaultMusicVolume();
        SetDefaultSFXVolume();
        onConfigured.Invoke();
    }

    public void SaveVolumeSettings(float musicVolume, float SFXVolume)
    {
        PlayerPrefs.SetFloat(musicVolumeString, musicVolume);
        PlayerPrefs.SetFloat(SFXVolumeString, SFXVolume);
    }

    private void SetDefaultSFXVolume()
    {
        if (PlayerPrefs.HasKey(SFXVolumeString))
        {
            defaultSFXVolume = PlayerPrefs.GetFloat(SFXVolumeString);
            SetSFXVolume(defaultSFXVolume);
        }
        else
        {
            SetSFXVolume(defaultSFXVolume);
        }
    }

    private void SetDefaultMusicVolume()
    {
        if (PlayerPrefs.HasKey(musicVolumeString))
        {
            defaultMusicVolume = PlayerPrefs.GetFloat(musicVolumeString);
            SetMusicVolume(defaultMusicVolume);
        }
        else
        {
            SetMusicVolume(defaultMusicVolume);
        }
    }

    public void PlayMusic(ref AudioClip clip)
    {
        MusicSouce.clip = clip;
        MusicSouce.Play();
    }

    public void PlayRandomMusic(AudioClip[] clips)
    {
        ref AudioClip clip = ref GetRandomClip(clips);
        PlayMusic(ref clip);
    }

    private float ToMixerValue(float value)
    {
        return Mathf.Log10(value) * 20.0f;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", ToMixerValue(volume));
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", ToMixerValue(volume));
    }

    public void PlayRandomSFX(AudioClip[] clips)
    {
        ref AudioClip clip = ref GetRandomClip(clips);
        SFXSource.PlayOneShot(clip);
    }

    public void PlaySFX(ref AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    private ref AudioClip GetRandomClip(AudioClip[] clips)
    {
        return ref clips[Random.Range(0, clips.Length)];
    }
}
