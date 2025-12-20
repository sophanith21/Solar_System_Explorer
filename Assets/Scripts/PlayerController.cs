using UnityEngine;
using System.Collections;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 300f;
    public float lookSpeed = 3f;
    public float scrollSpeed = 500f;
    public float boostMultiplier = 5f;


    float rotationX = 0f;
    float rotationY = 0f;

    Coroutine focusRoutine;

    void Update()
    {
        HandleLook();
        HandleMovement();
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

        // Base speed
        float speed = moveSpeed;

        // 🚀 Speed boost (SHIFT)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= boostMultiplier;
        }

        // 🌌 Distance-based speed (solar scale)
        float distanceFromCenter = transform.position.magnitude;
        float dynamicMultiplier = Mathf.Max(1f, distanceFromCenter * 0.01f);

        transform.position += move * speed * dynamicMultiplier * Time.deltaTime;

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * scrollSpeed * Time.deltaTime;
    }


    public void FocusOnPlanet(Transform planet, bool followPlanet = true)
    {
        if (focusRoutine != null)
            StopCoroutine(focusRoutine);

        focusRoutine = StartCoroutine(FocusOn(planet, followPlanet));
    }


    IEnumerator FocusOn(Transform target, bool followPlanet = true)
    {
        Vector3 direction = (transform.position - target.position).normalized;
        float size = target.localScale.x;
        float distance = size * 2.5f;

        // Smoothly move to initial position first
        Vector3 targetPos = target.position + direction * distance;
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

        // After reaching, follow continuously
        while (followPlanet)
        {
            transform.position = target.position + direction * distance;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
            yield return null;
        }
    }


}
