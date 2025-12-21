using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 300f;
    public float lookSpeed = 3f;
    public float scrollSpeed = 500f;
    public float boostMultiplier = 5f;

    // Orbit settings
    public float orbitSpeed = 90f; // degrees per second
    public float orbitDistanceMultiplier = 2.5f;

    float rotationX = 0f;
    float rotationY = 0f;

    Coroutine focusRoutine;

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Learning Mode")
        {
            HandleLook();
            HandleMovement();
        }
    }

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

        float distanceFromCenter = transform.position.magnitude;
        float dynamicMultiplier = Mathf.Max(1f, distanceFromCenter * 0.01f);

        transform.position += move * speed * dynamicMultiplier * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * scrollSpeed * Time.deltaTime;
    }

    public void FocusOnPlanet(Transform planet, bool followPlanet = true)
    {
        if (focusRoutine != null)
            StopCoroutine(focusRoutine);

        // Only these planets orbit
        bool shouldOrbit = planet.name == "Jupiter" || planet.name == "Saturn" ||
                           planet.name == "Uranus" || planet.name == "Neptune";

        focusRoutine = StartCoroutine(FocusOn(planet, followPlanet, shouldOrbit));
    }

    IEnumerator FocusOn(Transform target, bool followPlanet, bool orbitPlanet)
    {
        // Initial direction & distance
        Vector3 direction = (transform.position - target.position).normalized;
        float baseDistance = target.localScale.x * orbitDistanceMultiplier;

        // Smooth initial move
        Vector3 targetPos = target.position + direction * baseDistance;
        float t = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(
                startRot,
                Quaternion.LookRotation(target.position - transform.position),
                t
            );
            yield return null;
        }

        while (followPlanet)
        {
            if (orbitPlanet)
            {
                // Calculate current offset from planet
                Vector3 offset = transform.position - target.position;

                // Keep distance proportional to planet size
                float desiredDistance = target.localScale.x * orbitDistanceMultiplier;
                offset = offset.normalized * desiredDistance;

                // Apply orbit around the planet
                transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

                // After rotation, adjust position to maintain consistent distance
                Vector3 newOffset = transform.position - target.position;
                newOffset = newOffset.normalized * desiredDistance;
                transform.position = target.position + newOffset;


                // Always look at the planet
                transform.LookAt(target.position);
            }
            else
            {
                // Non-orbit planets: follow at fixed offset
                Vector3 followPos = target.position + direction * baseDistance;
                transform.position = followPos;
                transform.LookAt(target.position);

            }

            yield return null;
        }

    }
}
