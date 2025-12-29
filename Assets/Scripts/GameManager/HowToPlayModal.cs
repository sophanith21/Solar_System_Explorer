using UnityEngine;

public class HowToPlayModal : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject modalPanel;    // Drag your modal Panel here
    public GameObject darkOverlay;   // Optional: dim background

    void Start()
    {
        // Ensure modal is hidden at start
        modalPanel.SetActive(false);
        if (darkOverlay != null)
            darkOverlay.SetActive(false);
    }

    // Call this to open the modal
    public void OpenModal()
    {
        modalPanel.SetActive(true);
        if (darkOverlay != null)
            darkOverlay.SetActive(true);

        // Optional: pause game while modal is open
        // Time.timeScale = 0f;
    }

    // Call this to close the modal
    public void CloseModal()
    {
        modalPanel.SetActive(false);
        if (darkOverlay != null)
            darkOverlay.SetActive(false);

        // Optional: resume game if paused
        // Time.timeScale = 1f;
    }
}
