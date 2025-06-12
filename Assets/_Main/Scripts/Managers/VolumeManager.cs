using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    void Start()
    {
        float savedBGM = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Apply to sliders
        if (bgmSlider != null)
        {
            bgmSlider.value = savedBGM;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // Apply initial values to SoundManager
        SoundManager.Instance?.SetBGMVolume(savedBGM);
        SoundManager.Instance?.SetSFXVolume(savedSFX);
    }

    private void OnBGMVolumeChanged(float volume)
    {
        SoundManager.Instance?.SetBGMVolume(volume);
        PlayerPrefs.SetFloat(BGM_KEY, volume);
        PlayerPrefs.Save();
    }

    private void OnSFXVolumeChanged(float volume)
    {
        SoundManager.Instance?.SetSFXVolume(volume);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }
}
