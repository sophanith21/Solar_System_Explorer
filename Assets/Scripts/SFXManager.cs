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

    // Thrust control variables
    private bool isThrusting = false;
    private float targetThrustVolume = 0f;
    private float currentThrustVolume = 0f;
    private const float thrustFadeSpeed = 3f; // How fast volume changes (higher = faster)

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

        // Thrust source - DON'T assign clip in Start, we'll handle it dynamically
        thrustSource = gameObject.AddComponent<AudioSource>();
        thrustSource.loop = true;
        thrustSource.playOnAwake = false;
        thrustSource.volume = 0f; // Start silent
        thrustSource.outputAudioMixerGroup =
            audioMixer.FindMatchingGroups("SFX")[0];
    }

    private void Update()
    {
        // Smoothly fade thrust volume
        if (Mathf.Abs(currentThrustVolume - targetThrustVolume) > 0.01f)
        {
            currentThrustVolume = Mathf.MoveTowards(currentThrustVolume, targetThrustVolume, 
                thrustFadeSpeed * Time.deltaTime);
            thrustSource.volume = currentThrustVolume;
            
            // Adjust pitch based on volume
            thrustSource.pitch = Mathf.Lerp(0.8f, 1.2f, currentThrustVolume);
            
            // Start playing if we have volume and clip
            if (currentThrustVolume > 0.01f && thrustSound != null && !thrustSource.isPlaying)
            {
                if (thrustSource.clip == null)
                {
                    thrustSource.clip = thrustSound;
                }
                thrustSource.Play();
            }
            // Stop if volume is (almost) zero
            else if (currentThrustVolume <= 0.01f && thrustSource.isPlaying)
            {
                thrustSource.Stop();
                thrustSource.clip = null; // Reset clip to avoid playing ascending part again
            }
        }
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
        
        isThrusting = true;
        targetThrustVolume = Mathf.Clamp01(power);
        
        // If this is the first time thrusting, set up the clip
        if (thrustSource.clip == null)
        {
            // For ascending sounds, we need a different approach
            // Let's assume the sound has 1 second of ascending, then continuous
            // We'll start playing from 1 second in
            thrustSource.clip = thrustSound;
            thrustSource.time = Mathf.Min(1f, thrustSound.length * 0.1f); // Skip first 10% or 1 second
        }
    }

    public void StopThrust()
    {
        isThrusting = false;
        targetThrustVolume = 0f;
    }

    // Alternative: Simple one-shot thrust (good for ascending sounds)
    public void PlayThrustOneShot(float volume = 1f)
    {
        if (thrustSound != null)
        {
            sfxSource.PlayOneShot(thrustSound, volume);
        }
    }

    // Alternative: Continuous thrust with manual control
    public void SetThrustLevel(float level)
    {
        level = Mathf.Clamp01(level);
        targetThrustVolume = level;
        
        if (level > 0.01f && thrustSound != null)
        {
            // Ensure we're playing from a stable part of the sound
            if (thrustSource.clip == null)
            {
                thrustSource.clip = thrustSound;
                // Start from 25% into the sound to skip ascending part
                thrustSource.time = thrustSound.length * 0.25f;
            }
            
            if (!thrustSource.isPlaying)
            {
                thrustSource.Play();
            }
        }
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
    
    // Public property to check if thrust is playing
    public bool IsThrusting
    {
        get { return isThrusting; }
    }
    
    // Get current thrust volume (0-1)
    public float CurrentThrustVolume
    {
        get { return currentThrustVolume; }
    }
}