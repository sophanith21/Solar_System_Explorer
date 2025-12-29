using UnityEngine;
using UnityEngine.UI;

public class SFXControls : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    private void Start()
    {
        // Slider setup
        SetupSlider(musicSlider, PlayerPrefs.GetFloat("MusicVolume", 1f));
        SetupSlider(sfxSlider, PlayerPrefs.GetFloat("SFXVolume", 1f));
        SetupSlider(uiSlider, PlayerPrefs.GetFloat("UIVolume", 1f));

        // Hook UI → THIS script (not SFXManager directly)
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        uiSlider.onValueChanged.AddListener(OnUIChanged);

        // Apply immediately
        OnMusicChanged(musicSlider.value);
        OnSFXChanged(sfxSlider.value);
        OnUIChanged(uiSlider.value);
    }

    private void SetupSlider(Slider slider, float value)
    {
        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.value = value;
    }

    // ===== Slider callbacks =====

    public void OnMusicChanged(float value)
    {
        if (SFXManager.Instance != null)
            SFXManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXChanged(float value)
    {
        if (SFXManager.Instance != null)
            SFXManager.Instance.SetSFXVolume(value);
    }

    public void OnUIChanged(float value)
    {
        if (SFXManager.Instance != null)
            SFXManager.Instance.SetUIVolume(value);
    }

    private void OnDestroy()
    {
        // Check if sliders still exist before removing listeners
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);

        if (uiSlider != null)
            uiSlider.onValueChanged.RemoveListener(OnUIChanged);
    }
}
