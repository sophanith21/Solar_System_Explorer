using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public String nextScene;

    public void LoadNextScene()
    {
        PlayClickSound();
        StopGameplayAudio();
        SceneManagement.Instance.LoadScene(nextScene);
    }
    
    public void LoadLastScene()
    {
        PlayClickSound();
        StopGameplayAudio();
        SceneManagement.Instance.GoBack();
    }
    public void LoadEasyScene(string sceneName)
    {
        DifficultyStorage.SelectedDifficulty = GameDifficulty.Easy;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadModerateScene(string sceneName)
    {
        DifficultyStorage.SelectedDifficulty = GameDifficulty.Moderate;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadHardScene(string sceneName)
    {
        DifficultyStorage.SelectedDifficulty = GameDifficulty.Hard;
        SceneManager.LoadScene(sceneName);
    }
    
    private void PlayClickSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayUIClick();
        }
    }
    
    private void StopGameplayAudio()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.StopEngineIdle();
            SFXManager.Instance.StopThrust();
        }
    }
}