using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// This script is for controlling the player camera in learning mode
public class FreeCameraController : MonoBehaviour
{
    [Header("Pause Settings")]
    public GameObject pauseMenu;
    [Header("Movement Settings")]
    public float moveSpeed = 300f;
    public float lookSpeed = 3f;
    public float scrollSpeed = 500f;
    public float boostMultiplier = 5f;

    [Header("Focus Settings")]
    public float orbitSpeed = 45f; // Degrees per second
    public float orbitDistanceMultiplier = 3.0f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isFocusing = false; // Prevents manual input from fighting the orbit
    private Coroutine focusRoutine;

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Learning Mode")
        {
            // Only allow manual control if NOT currently auto-orbiting
            if (!isFocusing)
            {
                HandleLook();
                HandleMovement();
            }
            else
            {
                if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
                {
                    StopFocus();
                }
            }

            if (Input.GetKeyDown(KeyCode.M)) TogglePause();
        }
    }

    public void TogglePause() => pauseMenu.SetActive(!pauseMenu.activeSelf);

    void HandleLook()
    {
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);
            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * v + transform.right * h;
        if (Input.GetKey(KeyCode.Q)) move += Vector3.down * 2f;
        if (Input.GetKey(KeyCode.E)) move += Vector3.up * 2f;

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= boostMultiplier;

        float dynamicMultiplier = Mathf.Max(1f, transform.position.magnitude * 0.01f);
        transform.position += move * speed * dynamicMultiplier * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * scrollSpeed * Time.deltaTime;
    }

    public void FocusOnPlanet(Transform planet, bool followPlanet = true)
    {
        StopFocus();
        isFocusing = true;
        focusRoutine = StartCoroutine(FocusOn(planet, followPlanet));
    }

    public void StopFocus()
    {
        if (focusRoutine != null) StopCoroutine(focusRoutine);
        isFocusing = false;

        Vector3 rot = transform.eulerAngles;
        rotationX = rot.y;
        rotationY = rot.x;
    }

    IEnumerator FocusOn(Transform target, bool followPlanet)
    {
        float distance = target.localScale.x * orbitDistanceMultiplier;

        // Find the starting angle based on current camera position relative to target
        Vector3 dirFromTarget = (transform.position - target.position).normalized;
        float currentAngleRad = Mathf.Atan2(dirFromTarget.x, dirFromTarget.z);

        // 1. Smooth Transition Phase
        float t = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f; // Transition speed

            // Calculate point on orbit circle using currentAngleRad
            Vector3 orbitPathPos = target.position + new Vector3(Mathf.Sin(currentAngleRad), 0, Mathf.Cos(currentAngleRad)) * distance;

            transform.position = Vector3.Lerp(startPos, orbitPathPos, t);

            Vector3 lookDir = target.position - transform.position;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(startRot, Quaternion.LookRotation(lookDir), t);

            yield return null;
        }

        // 2. Continuous Orbit Loop
        while (followPlanet)
        {
            // Update the angle over time 
            currentAngleRad += (orbitSpeed * Mathf.Deg2Rad) * Time.deltaTime;

            // Position = Target + [Sin(angle), 0, Cos(angle)] * Distance
            Vector3 offset = new Vector3(
                Mathf.Sin(currentAngleRad) * distance,
                0, // Orbiting on the horizontal plane
                Mathf.Cos(currentAngleRad) * distance
            );

            transform.position = target.position + offset;
            transform.LookAt(target.position);

            yield return null;
        }

        isFocusing = false;
    }
}