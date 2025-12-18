using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class SpaceshipRB : MonoBehaviour
{
    public Rigidbody rb;
    public Transform model;

    [Header("Movement")]
    public float thrustForce = 30f;
    public float maxSpeed = 40f;
    public float boostMultiplier = 2f;
    public float reverseMultiplier = 0.5f; // Slower than normal forward speed
    public float dragAmount = 1f;

    [Header("Rotation")]
    public float turnSpeed = 100f;
    public float tiltAngle = 30f;
    public float tiltSpeed = 5f;

    [Header("Camera")]
    public CinemachineFreeLook freeLookCamera;
    public float movementThreshold = 2f; // Speed at which the camera locks


    bool isBoosted = false;
    float boostedThrustForce;

    float yawInput;   // A/D (Left/Right)
    float pitchInput; // Q/R (Down/Up)
    float currentYawTilt;
    float currentPitchTilt;

    

    // Store the original speeds so we can restore them
    float originalXSpeed;
    float originalYSpeed;

    void Start()
    {
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.drag = dragAmount;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalXSpeed = freeLookCamera.m_XAxis.m_MaxSpeed;
        originalYSpeed = freeLookCamera.m_YAxis.m_MaxSpeed;

        boostedThrustForce = thrustForce * boostMultiplier;
    }

    void Update()
    {
        handleCameraLock();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isBoosted = true;
   
        } else {
            isBoosted = false;
       
        }

        // 1. Get Inputs
        yawInput = Input.GetAxis("Horizontal"); // A/D

        // Manual Pitch Input using Q and R
        pitchInput = 0;
        if (Input.GetKey(KeyCode.E)) pitchInput = -1f; // Pitch Up
        if (Input.GetKey(KeyCode.Q)) pitchInput = 1f;  // Pitch Down
        if (Input.GetKey(KeyCode.R)) pitchInput = 0f;  // Neutral

        // 2. Handle Visual Banking & Pitching (The "Lean")
        float targetYawTilt = -yawInput * tiltAngle;
        float targetPitchTilt = pitchInput * (tiltAngle * 0.5f); // Half tilt for pitch looks better

        currentYawTilt = Mathf.Lerp(currentYawTilt, targetYawTilt, Time.deltaTime * tiltSpeed);
        currentPitchTilt = Mathf.Lerp(currentPitchTilt, targetPitchTilt, Time.deltaTime * tiltSpeed);

        // Apply to the model local rotation
        model.localRotation = Quaternion.Euler(currentPitchTilt, 0f, currentYawTilt);
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleThrust();
    }

    void HandleRotation()
    {
        // 3. Rotate the Rigidbody
        float yaw = yawInput * turnSpeed * Time.fixedDeltaTime;
        float pitch = pitchInput * turnSpeed * Time.fixedDeltaTime;

        // Create rotation for both Yaw (Y axis) and Pitch (X axis)
        Quaternion deltaRotation = Quaternion.Euler(pitch, yaw, 0f);
        if (yawInput == 0 && pitchInput == 0)
        {
            // Smoothly rotate the ship's X and Z back to 0 (Leveling the nose and wings)
            // We keep the current Y (Yaw) so the ship doesn't spin back to the North Pole
            Vector3 currentEuler = rb.rotation.eulerAngles;

            // We target 0 for Pitch (X) and 0 for Roll (Z)
            Quaternion leveledRot = Quaternion.Euler(0f, currentEuler.y, 0f);

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, leveledRot, Time.fixedDeltaTime * 2f));
        }
        else
        {
            // Apply normal movement
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void HandleThrust()
    {
        float currentMaxSpeed = isBoosted ? maxSpeed * boostMultiplier : maxSpeed;
        float currentThrust = isBoosted ? boostedThrustForce : thrustForce;

        // Forward Thrust (W)
        if (Input.GetKey(KeyCode.W))
        {
            SFXManager.Instance.StopEngineIdle();

            rb.AddForce(transform.forward * currentThrust, ForceMode.Acceleration);
            SFXManager.Instance.StartThrust();
        }
        else if (Input.GetKey(KeyCode.S)) // Backtracking / Reverse (S)
        {
            SFXManager.Instance.StopEngineIdle();

            // We apply force in the opposite direction of transform.forward
            rb.AddForce(-transform.forward * (thrustForce * reverseMultiplier), ForceMode.Acceleration);

            // Adjust max speed for reversing so you don't go 40mph backwards
            currentMaxSpeed = maxSpeed * reverseMultiplier;
            SFXManager.Instance.StartThrust();
        }
        else
        {
            SFXManager.Instance.StopThrust();
            SFXManager.Instance.StartEngineIdle();
        }

        // Clamp speed based on current state (Boosting vs Reversing vs Normal)
        if (rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }
    }

    void handleCameraLock()
    {
        if (rb.velocity.magnitude > movementThreshold)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 0f;
            freeLookCamera.m_YAxis.m_MaxSpeed = 0f;
        }
        else
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = originalXSpeed;
            freeLookCamera.m_YAxis.m_MaxSpeed = originalYSpeed;
        }
    }

 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            SFXManager.Instance.PlayCollision();
        }
        Debug.Log("Collided with " + collision.gameObject.name);
    }
}