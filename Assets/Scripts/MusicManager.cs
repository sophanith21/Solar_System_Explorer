using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip learningModeMusic;
    [SerializeField] private AudioClip explorationModeMusic;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float defaultMusicVolume = 0.7f;

    private AudioSource musicSource;
    private string currentSceneName;
    private bool isTransitioning = false;
    private float currentMusicVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        SetupAudioSource();
        SceneManager.sceneLoaded += OnSceneLoaded;
        currentMusicVolume = defaultMusicVolume;
    }

    private void SetupAudioSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        
        if (isTransitioning || sceneName == currentSceneName) return;
        
        currentSceneName = sceneName;
        
        if (sceneName == "Learning Mode" || sceneName == "Exploration Mode")
        {
            PlaySceneMusic(sceneName);
        }
        else
        {
            StopMusic();
        }
    }

    private void PlaySceneMusic(string sceneName)
    {
        AudioClip targetMusic = null;
        
        if (sceneName == "Learning Mode")
        {
            targetMusic = learningModeMusic;
        }
        else if (sceneName == "Exploration Mode")
        {
            targetMusic = explorationModeMusic;
        }
        
        if (targetMusic != null)
        {
            StartCoroutine(TransitionToMusic(targetMusic));
        }
    }

    private IEnumerator TransitionToMusic(AudioClip newClip)
    {
        isTransitioning = true;
        
        if (musicSource.isPlaying && musicSource.clip != newClip)
        {
            yield return StartCoroutine(FadeAudioSource(musicSource, musicSource.volume, 0f, fadeDuration * 0.5f));
            musicSource.Stop();
        }
        
        if (newClip != null && musicSource.clip != newClip)
        {
            musicSource.clip = newClip;
            musicSource.Play();
            yield return StartCoroutine(FadeAudioSource(musicSource, 0f, currentMusicVolume, fadeDuration * 0.5f));
        }
        
        isTransitioning = false;
    }

    private void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeAudioSource(musicSource, musicSource.volume, 0f, fadeDuration));
            StartCoroutine(StopAfterFade());
        }
    }

    private IEnumerator StopAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);
        musicSource.Stop();
    }

    private IEnumerator FadeAudioSource(AudioSource source, float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }
        
        source.volume = endVolume;
    }

    public void PlayLearningMusic()
    {
        StartCoroutine(TransitionToMusic(learningModeMusic));
    }
    
    public void PlayExplorationMusic()
    {
        StartCoroutine(TransitionToMusic(explorationModeMusic));
    }
    
    public void StopAllMusic()
    {
        StopMusic();
    }
    
    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        musicSource.volume = currentMusicVolume;
    }
    
    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}