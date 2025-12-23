using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    // ---------------- UI ----------------
    [Header("UI Sounds")]
    [SerializeField] private AudioClip uiClick;

    // ---------------- Spaceship ----------------
    [Header("Spaceship Sounds")]
    [SerializeField] private AudioClip engineIdleSound;
    [SerializeField] private AudioClip thrustSound;
    [SerializeField] private AudioClip collisionSound;

    // ---------------- Game State ----------------
    [Header("Game State Sounds")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    // ---------------- Audio Sources ----------------
    private AudioSource uiSource;
    private AudioSource sfxSource;
    private AudioSource engineSource;
    private AudioSource thrustSource;

    private void Awake()
    {
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
        // UI sounds
        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.outputAudioMixerGroup =
            audioMixer.FindMatchingGroups("UI")[0];

        // One-shot SFX
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup =
            audioMixer.FindMatchingGroups("SFX")[0];

        // Engine idle loop
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = engineIdleSound;
        engineSource.loop = true;
        engineSource.playOnAwake = false;
        engineSource.outputAudioMixerGroup =
            audioMixer.FindMatchingGroups("SFX")[0];

        // Thrust loop
        thrustSource = gameObject.AddComponent<AudioSource>();
        thrustSource.clip = thrustSound;
        thrustSource.loop = true;
        thrustSource.playOnAwake = false;
        thrustSource.outputAudioMixerGroup =
            audioMixer.FindMatchingGroups("SFX")[0];
    }

    // ================= UI =================
    public void PlayUIClick()
    {
        if (uiClick != null)
            uiSource.PlayOneShot(uiClick);
    }

    // ================= Spaceship =================
    public void StartEngineIdle()
    {
        if (!engineSource.isPlaying && engineIdleSound != null)
            engineSource.Play();
    }

    public void StopEngineIdle()
    {
        if (engineSource.isPlaying)
            engineSource.Stop();
    }

    public void StartThrust(float power = 1f)
    {
        if (thrustSound == null) return;

        thrustSource.volume = Mathf.Clamp01(power);
        thrustSource.pitch = Mathf.Lerp(0.9f, 1.2f, power);

        if (!thrustSource.isPlaying)
            thrustSource.Play();
    }

    public void StopThrust()
    {
        if (thrustSource.isPlaying)
            thrustSource.Stop();
    }

    public void PlayCollision()
    {
        if (collisionSound != null)
            sfxSource.PlayOneShot(collisionSound);
        Debug.Log("Playing collision sound");
    }

    // ================= Game State =================
    public void PlayWin()
    {
        if (winSound != null)
            sfxSource.PlayOneShot(winSound);
    }

    public void PlayLose()
    {
        if (loseSound != null)
            sfxSource.PlayOneShot(loseSound);
    }

    // ================= Volume Control =================
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetUIVolume(float value)
    {
        audioMixer.SetFloat("UIVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("UIVolume", value);
    }

}
