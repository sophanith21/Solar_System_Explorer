using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class SpaceshipRB : MonoBehaviour
{
    public Rigidbody rb;
    public Transform model;

    [Header("UI Settings")]
    public spaceshipUI spaceshipUI;

    [Header("Movement")]
    public float thrustForce = 30f;
    public float maxSpeed = 40f;
    public float boostMultiplier = 2f;
    public float reverseMultiplier = 0.5f; // Slower than normal forward speed
    public float dragAmount = 1f;
    public float distanceToTeleport = 50f;

    [Header("Rotation")]
    public float turnSpeed = 100f;
    public float tiltAngle = 30f;
    public float tiltSpeed = 5f;

    [Header("Camera")]
    public CinemachineFreeLook freeLookCamera;
    public float movementThreshold = 2f; // Speed at which the camera locks

    [Header("Particle")]
    public ParticleSystem portal;
    public Transform portalTransform;

    [Header("Portal Scaling")]
    public float distanceToMove = 4f; // How much the portal moves forward when activated
    public float growthSpeed = 2f; // How fast it grows
    public Vector3 initialScale = new Vector3(1, 1, 1);
    public Vector3 maxScale = new Vector3(40, 40, 40);

    Vector3 initialPortalLocalPosition;
    bool isPortalActive = false;


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
        initialPortalLocalPosition = portalTransform.localPosition;
        portal.Stop(); 
    }

    void Update()
    {
        handleCameraLock();
        if (Input.GetKey(KeyCode.LeftShift) && !isPortalActive )
        {
            isBoosted = true;
   
        } else {
            isBoosted = false;
       
        }

        if (isBoosted)
        {
            spaceshipUI.ToggleBoost(true);
        }
        else 
        { 
            spaceshipUI.ToggleBoost(false);
        }

        if (Input.GetKey(KeyCode.F))
        {
           
            if (!isPortalActive)
            {
                portal.Play();
                isPortalActive = true;
            }
            portalTransform.localPosition = new Vector3(portalTransform.localPosition.x, portalTransform.localPosition.y, portalTransform.localPosition.z - distanceToMove*Time.deltaTime);
            portalTransform.localScale = Vector3.Lerp(portalTransform.localScale, new Vector3(40, 40, 40), Time.deltaTime * growthSpeed);
        }
        else
        {
            isPortalActive = false;
            portal.Stop();
            portalTransform.localPosition = initialPortalLocalPosition;
            portalTransform.localScale = Vector3.Lerp(portalTransform.localScale, initialScale, Time.deltaTime * growthSpeed);
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

    public void teleportSpaceship()
    {
        // 1. Tell the Rigidbody we are about to move it manually
        Vector3 newPosition = rb.position + (transform.forward * distanceToTeleport);
        rb.position = newPosition;
        

        // 2. IMPORTANT: Manually sync the transform to the physics engine
        // This prevents the 'snapping back' or interpolation glitches
        rb.interpolation = RigidbodyInterpolation.None;

        // 3. Optional: Clear the velocity so you don't keep your old speed 
        // at the new location (unless you want to keep the momentum)

        // 4. Turn interpolation back on for smooth movement after teleport
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            SFXManager.Instance.PlayCollision();
            spaceshipUI.Damage(rb.velocity.magnitude * 0.2f);
        }
        Debug.Log("Collided with " + collision.gameObject.name);
    }
}