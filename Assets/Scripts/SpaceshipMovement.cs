using UnityEngine;

public class SpaceshipRB : MonoBehaviour
{
    public Rigidbody rb;
    public Transform model;   // spaceship visual model

    [Header("Movement")]
    public float thrustForce = 20f;
    public float maxSpeed = 30f;

    [Header("Turn + Tilt")]
    public float turnSpeed = 60f;   // Y-axis turn speed
    public float tiltAngle = 25f;   // Z-axis visual tilt
    public float tiltSpeed = 5f;    // speed of tilt animation

    float currentTilt = 0f;
    float yawRotation = 0f;

    void Update()
    {
        HandleTiltAndYaw();
    }

    void FixedUpdate()
    {
        HandleThrust();
    }

    // --- FORWARD FORCE ---
    void HandleThrust()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // Accelerate forward
            rb.AddForce(transform.forward * thrustForce, ForceMode.Acceleration);
        }

        // Limit max speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * thrustForce, ForceMode.Acceleration);
        }
    }

    // --- COMBINED YAW ROTATION AND VISUAL TILT ---
    void HandleTiltAndYaw()
    {
        float turnInput = 0f;
        if (Input.GetKey(KeyCode.A)) turnInput = -1f;
        if (Input.GetKey(KeyCode.D)) turnInput = 1f;

        // --- PERSISTENT YAW ROTATION ---
        // Only change yaw if pressing A/D
        yawRotation += turnInput * turnSpeed * Time.deltaTime;

        // Apply yaw rotation to Rigidbody
        Quaternion yawRot = Quaternion.Euler(0f, yawRotation, 0f);
        rb.MoveRotation(yawRot);

        // --- TEMPORARY Z-AXIS TILT ---
        float targetTilt = -turnInput * tiltAngle; // negative for A, positive for D
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Apply tilt on Z-axis, yaw stays as Rigidbody rotation
        model.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }


}
