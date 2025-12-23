using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    private void Start()
    {
        // Set slider ranges
        SetSliderDefaults(musicSlider);
        SetSliderDefaults(sfxSlider);
        SetSliderDefaults(uiSlider);

        // Load saved values using AudioManager's constants
        musicSlider.value = AudioManager.Instance.GetVolume(AudioManager.MUSIC_VOL);
        sfxSlider.value = AudioManager.Instance.GetVolume(AudioManager.SFX_VOL);
        uiSlider.value = AudioManager.Instance.GetVolume(AudioManager.UI_VOL);

        // Register listeners
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        uiSlider.onValueChanged.AddListener(AudioManager.Instance.SetUIVolume);
    }

    private void SetSliderDefaults(Slider slider)
    {
        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }
}