using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSence : MonoBehaviour
{
     
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
