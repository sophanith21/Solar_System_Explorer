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

        // Load saved values
        musicSlider.value = AudioManager.Instance.GetVolume("MusicVolume");
        sfxSlider.value = AudioManager.Instance.GetVolume("SFXVolume");
        uiSlider.value = AudioManager.Instance.GetVolume("UIVolume");

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
