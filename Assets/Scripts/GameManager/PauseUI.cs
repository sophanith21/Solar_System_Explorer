using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("Pause Settings")]
    public GameObject pauseMenu;

    Coroutine focusRoutine;

    void Update()
    {
        if (pauseMenu == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pauseMenu != null)
        {
            bool isActive = pauseMenu.activeSelf;
            pauseMenu.SetActive(!isActive);

            // Optional: Freezes time when the menu is open
            Time.timeScale = isActive ? 1f : 0f;
        }
    }
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
