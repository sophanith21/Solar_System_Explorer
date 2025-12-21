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
}
