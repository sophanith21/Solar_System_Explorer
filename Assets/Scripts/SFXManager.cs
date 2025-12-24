using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip uiClick;

    [Header("Spaceship Sounds")]
    [SerializeField] private AudioClip engineIdleSound;
    [SerializeField] private AudioClip thrustSound;
    [SerializeField] private AudioClip collisionSound;

    [Header("Game State Sounds")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private AudioSource uiSource;
    private AudioSource sfxSource;
    private AudioSource engineSource;
    private AudioSource thrustSource;

    private bool isThrusting = false;
    private float targetThrustVolume = 0f;
    private float currentThrustVolume = 0f;
    private const float thrustFadeSpeed = 3f;

    float thrustEnd = 0.6f;

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
        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("UI")[0];

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];

        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = engineIdleSound;
        engineSource.loop = true;
        engineSource.playOnAwake = false;
        engineSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];

        thrustSource = gameObject.AddComponent<AudioSource>();
        thrustSource.loop = true;
        thrustSource.playOnAwake = false;
        thrustSource.volume = 0f;
        thrustSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
    }

    private void Update()
    {
        if (Mathf.Abs(currentThrustVolume - targetThrustVolume) > 0.01f)
        {
            currentThrustVolume = Mathf.MoveTowards(currentThrustVolume, targetThrustVolume, 
                thrustFadeSpeed * Time.deltaTime);
            thrustSource.volume = currentThrustVolume;
            thrustSource.pitch = Mathf.Lerp(0.8f, 1.2f, currentThrustVolume);
            
            if (currentThrustVolume > 0.01f && thrustSound != null && !thrustSource.isPlaying)
            {
                if (thrustSource.clip == null)
                {
                    thrustSource.clip = thrustSound;
                }
                thrustSource.Play();
            }
            else if (currentThrustVolume <= 0.01f && thrustSource.isPlaying)
            {
                thrustSource.Stop();
                thrustSource.clip = null;
            }
        }
    }

    public void PlayUIClick()
    {
        if (uiClick != null)
            uiSource.PlayOneShot(uiClick);
    }

    public void StartEngineIdle()
    {
        if (!engineSource.isPlaying && engineIdleSound != null)
        {
            engineSource.volume = 0.5f;
            engineSource.Play();
        }
            
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

        if (!thrustSource.isPlaying)
        {
            thrustSource.clip = thrustSound;
            thrustSource.time = Mathf.Clamp(thrustSound.length * (power * 0.2f),0f,thrustEnd);
            thrustSource.Play();
        }
    }

    public void StopThrust()
    {
        isThrusting = false;
        targetThrustVolume = 0f;
    }

    public void PlayCollision()
    {
        if (collisionSound != null)
            sfxSource.PlayOneShot(collisionSound);
    }

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
    
    public bool IsThrusting
    {
        get { return isThrusting; }
    }
    
    public float CurrentThrustVolume
    {
        get { return currentThrustVolume; }
    }
}