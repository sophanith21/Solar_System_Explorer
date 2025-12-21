using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class spaceshipUI : MonoBehaviour
{
    [Header("Health UI")]
    public TextMeshProUGUI healthText;
    public Image healthBar;

    [Header("Screen Effects")]
    public Image screenOverlay;     // Fullscreen UI Image
    public Camera mainCamera;

    [Header("Settings")]
    float health, maxHealth = 100f;
    float lerpSpeed;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.15f;
    public float overlayFadeSpeed = 5f;

    Vector3 cameraOriginalPos;

    [Header("Damage FX")]
    public Image colorOverlay;
    public Image topBar;
    public Image bottomBar;

    bool isDead = false;


    void Start()
    {
        health = maxHealth;
        cameraOriginalPos = mainCamera.transform.localPosition;
        screenOverlay.color = new Color(1, 1, 1, 0); // transparent

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TogglePause();
        }

       
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None:CursorLockMode.Locked;
        


        HealthBar();
        healthText.text = Mathf.RoundToInt(health).ToString();

        lerpSpeed = 3f * Time.deltaTime;

        
        FadeOverlay();
    }

    void HealthBar()
    {
        float targetHealth = health / maxHealth;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetHealth, lerpSpeed);
    }

    void changeColor()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, health / maxHealth);

        healthBar.color = healthColor;
    }

    public void Damage(float damage)
    {
        if (isDead) return;

        health = Mathf.Clamp(health - damage, 0, maxHealth);

        if (health <= 0)
        {
            isDead = true;
            StartCoroutine(GameOverTransition());
        }
        else
        {
            FlashOverlay(Color.red);
            StartCoroutine(CameraShake());
        }
    }

    public void Heal(float heal)
    {

        health = Mathf.Clamp(health + heal, 0, maxHealth);
        FlashOverlay(Color.green);
    }

    void FlashOverlay(Color color)
    {
        color.a = 0.4f; // flash intensity
        screenOverlay.color = color;
    }

    void FadeOverlay()
    {
        Color c = screenOverlay.color;
        c.a = Mathf.Lerp(c.a, 0, overlayFadeSpeed * Time.deltaTime);
        screenOverlay.color = c;
    }


    IEnumerator CameraShake()
    {
        float time = 0;

        while (time < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;
            mainCamera.transform.localPosition = cameraOriginalPos + randomOffset;

            time += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = cameraOriginalPos;
    }


    IEnumerator GameOverTransition()
    {
        yield return new WaitForSeconds(0.5f); // wait 0.5 seconds
        Debug.Log("Executed after 0.5 seconds");
        // Stop time
        Time.timeScale = 0f;

        float t = 0f;



        Vector2 topStart = new Vector2(0, 1080);
        Vector2 topEnd = new Vector2(0, 540);

        Vector2 bottomStart = new Vector2(0, -1080);
        Vector2 bottomEnd = new Vector2(0, -540);

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;

            // Fade overlay
            colorOverlay.color = new Color(1, 1, 1, t);

            // Slide bars
            topBar.rectTransform.anchoredPosition =
                Vector2.Lerp(topStart, topEnd, t);

            bottomBar.rectTransform.anchoredPosition =
                Vector2.Lerp(bottomStart, bottomEnd, t);

            yield return null;
        }
        SceneManagement.Instance.LoadScene("Losing Scene");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    [Header("Pause")]

    public GameObject pausePanel;
    // public MonoBehaviour? playerController; // drag your movement script here
    private bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        // playerController.enabled = !isPaused;
    }

    [Header("Boost")]

    public GameObject boostPanel;
    private bool isBoost = false;

    public void ToggleBoost(bool? value = null)
    {
        bool newState = value ?? !isBoost;

        
        if (newState != isBoost)
        {
            isBoost = newState;
            boostPanel.SetActive(isBoost);
            Debug.Log("UI Updated only when needed!");
        }
    }

}
