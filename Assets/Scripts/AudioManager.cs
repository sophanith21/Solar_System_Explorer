using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string MUSIC_VOL = "MusicVolume";
    private const string SFX_VOL = "SFXVolume";
    private const string UI_VOL = "UIVolume";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetMusicVolume(GetVolume(MUSIC_VOL));
        SetSFXVolume(GetVolume(SFX_VOL));
        SetUIVolume(GetVolume(UI_VOL));
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MUSIC_VOL, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MUSIC_VOL, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFX_VOL, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_VOL, value);
    }

    public void SetUIVolume(float value)
    {
        audioMixer.SetFloat(UI_VOL, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(UI_VOL, value);
    }

    public float GetVolume(string parameter)
    {
        return PlayerPrefs.GetFloat(parameter, 1f);
    }
}