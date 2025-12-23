using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSence : MonoBehaviour
{
    public String nextScene;

    public void LoadNextScene()
    {
        SceneManagement.Instance.LoadScene(nextScene);
    }
    public void loadLastScene()
    {
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
}
