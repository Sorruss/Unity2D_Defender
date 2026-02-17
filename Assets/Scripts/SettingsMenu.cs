using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    public void SetSliders()
    {
        musicSlider.value = SoundManager.instance.defaultMusicVolume;
        SFXSlider.value = SoundManager.instance.defaultSFXVolume;
    }

    public void SaveVolumeSettings()
    {
        SoundManager.instance.SaveVolumeSettings(musicSlider.value, SFXSlider.value);
    }

    public void SetMusicVolume()
    {
        SoundManager.instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSFXVolume()
    {
        SoundManager.instance.SetSFXVolume(SFXSlider.value);
    }
}
