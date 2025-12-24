using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class SpaceshipRB : MonoBehaviour
{
    public Rigidbody rb;
    public Transform model;

    [Header("UI Settings")]
    public spaceshipUI spaceshipUI;

    [Header("Particle Effects")]
    public ParticleSystem thrusterEffect_left;
    public ParticleSystem thrusterEffect_right;
    public ParticleSystem boostedEffect_left;
    public ParticleSystem boostedEffect_right;

    [Header("Movement")]
    public float thrustForce = 30f;
    public float maxSpeed = 40f;
    public float boostMultiplier = 2f;
    public float reverseMultiplier = 0.5f;
    public float dragAmount = 1f;
    public float distanceToTeleport = 50f;

    [Header("Rotation")]
    public float turnSpeed = 100f;
    public float tiltAngle = 30f;
    public float tiltSpeed = 5f;

    [Header("Camera")]
    public CinemachineFreeLook freeLookCamera;
    public float movementThreshold = 2f;

    [Header("Particle")]
    public ParticleSystem portal;
    public Transform portalTransform;

    [Header("Portal Scaling")]
    public float distanceToMove = 4f;
    public float growthSpeed = 2f;
    public Vector3 initialScale = new Vector3(1, 1, 1);
    public Vector3 maxScale = new Vector3(40, 40, 40);

    Vector3 initialPortalLocalPosition;
    bool isPortalActive = false;

    public bool hasTeleported = false;
    bool isBoosted = false;
    float boostedThrustForce;

    float yawInput;
    float pitchInput;
    float currentYawTilt;
    float currentPitchTilt;

    float originalXSpeed;
    float originalYSpeed;

    void Start()
    {
        // Configure physics for space flight
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.drag = dragAmount;

        // Mouse behavior
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store camera settings to restore them when ship stops
        originalXSpeed = freeLookCamera.m_XAxis.m_MaxSpeed;
        originalYSpeed = freeLookCamera.m_YAxis.m_MaxSpeed;

        boostedThrustForce = thrustForce * boostMultiplier;
        initialPortalLocalPosition = portalTransform.localPosition;
        portal.Stop();
    }

    void Update()
    {
        handleCameraLock();

        // Determine boost state based on Shift key and Portal status
        isBoosted = Input.GetKey(KeyCode.LeftShift) && !isPortalActive;
        spaceshipUI.ToggleBoost(isBoosted);

        // Portal activation logic with gated .Play() to prevent stuttering
        if (Input.GetKey(KeyCode.F) && spaceshipUI.Warp(isPortalActive))
        {
            if (!isPortalActive)
            {
                portal.Play();
                isPortalActive = true;
            }
            portalTransform.localPosition -= new Vector3(0, 0, distanceToMove * Time.deltaTime);
            portalTransform.localScale = Vector3.Lerp(portalTransform.localScale, maxScale, Time.deltaTime * growthSpeed);
        }
        else
        {
            if (isPortalActive)
            {
                portal.Stop();
                isPortalActive = false;
            }
            portalTransform.localPosition = initialPortalLocalPosition;
            portalTransform.localScale = Vector3.Lerp(portalTransform.localScale, initialScale, Time.deltaTime * growthSpeed);
            hasTeleported = false;
        }

        if (Input.GetKey(KeyCode.C))
        {
            spaceshipUI.Heal(10 * Time.deltaTime);
        }

        // Gather flight inputs
        yawInput = Input.GetAxis("Horizontal");
        pitchInput = 0;
        if (Input.GetKey(KeyCode.E)) pitchInput = -1f;
        if (Input.GetKey(KeyCode.Q)) pitchInput = 1f;

        // Visual banking: Tilts the visual model without affecting physics rotation
        float targetYawTilt = -yawInput * tiltAngle;
        float targetPitchTilt = pitchInput * (tiltAngle * 0.5f);

        currentYawTilt = Mathf.Lerp(currentYawTilt, targetYawTilt, Time.deltaTime * tiltSpeed);
        currentPitchTilt = Mathf.Lerp(currentPitchTilt, targetPitchTilt, Time.deltaTime * tiltSpeed);

        model.localRotation = Quaternion.Euler(currentPitchTilt, 0f, currentYawTilt);
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleThrust();
    }

    void HandleRotation()
    {
        float yaw = yawInput * turnSpeed * Time.fixedDeltaTime;
        float pitch = pitchInput * turnSpeed * Time.fixedDeltaTime;

        if (yawInput == 0 && pitchInput == 0)
        {
            // Auto-leveling: Smoothly returns X and Z rotation to 0 when no input is given
            Vector3 currentEuler = rb.rotation.eulerAngles;
            Quaternion leveledRot = Quaternion.Euler(0f, currentEuler.y, 0f);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, leveledRot, Time.fixedDeltaTime * 2f));
        }
        else
        {
            // Apply 3D rotation based on player input
            Quaternion deltaRotation = Quaternion.Euler(pitch, yaw, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void HandleThrust()
    {
        float currentMaxSpeed = isBoosted ? maxSpeed * boostMultiplier : maxSpeed;
        float currentThrust = isBoosted ? boostedThrustForce : thrustForce;

        if (Input.GetKey(KeyCode.W))
        {
            // SFXManager.Instance.StopEngineIdle();
            SFXManager.Instance.StartThrust(rb.velocity.magnitude);
            rb.AddForce(transform.forward * currentThrust, ForceMode.Acceleration);

            // Gated Particle Logic: Only calls Play/Stop when switching states to prevent flickering
            if (isBoosted)
            {
                if (thrusterEffect_left.isPlaying) { thrusterEffect_left.Stop(); thrusterEffect_right.Stop(); }
                if (!boostedEffect_left.isPlaying) { boostedEffect_left.Play(); boostedEffect_right.Play(); }
            }
            else
            {
                if (boostedEffect_left.isPlaying) { boostedEffect_left.Stop(); boostedEffect_right.Stop(); }
                if (!thrusterEffect_left.isPlaying) { thrusterEffect_left.Play(); thrusterEffect_right.Play(); }
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // SFXManager.Instance.StopEngineIdle();
            SFXManager.Instance.StartThrust(rb.velocity.magnitude);
            rb.AddForce(-transform.forward * (thrustForce * reverseMultiplier), ForceMode.Acceleration);
            currentMaxSpeed = maxSpeed * reverseMultiplier;
            StopAllThrusters();
        }
        else
        {
            // Neutral state: Return to idle sounds and kill all exhaust particles
            SFXManager.Instance.StopThrust();
            // SFXManager.Instance.StartEngineIdle();
            StopAllThrusters();
        }

        // Hard cap for velocity based on current state (boost/normal/reverse)
        if (rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }
    }

    void StopAllThrusters()
    {
        if (thrusterEffect_left.isPlaying) { thrusterEffect_left.Stop(); thrusterEffect_right.Stop(); }
        if (boostedEffect_left.isPlaying) { boostedEffect_left.Stop(); boostedEffect_right.Stop(); }
    }

    void handleCameraLock()
    {
        // Locks the FreeLook camera orbit when the ship is moving fast to keep focus forward
        bool shouldLock = rb.velocity.magnitude > movementThreshold;
        freeLookCamera.m_XAxis.m_MaxSpeed = shouldLock ? 0f : originalXSpeed;
        freeLookCamera.m_YAxis.m_MaxSpeed = shouldLock ? 0f : originalYSpeed;
    }

    public void teleportSpaceship()
    {
        // Temporarily disable interpolation to prevent the camera from "stretching" during teleport
        rb.interpolation = RigidbodyInterpolation.None;
        rb.position += transform.forward * distanceToTeleport;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        hasTeleported = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            SFXManager.Instance.PlayCollision();
            // Damage scale based on how fast the ship was moving at impact
            spaceshipUI.Damage(rb.velocity.magnitude * 0.2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Logic for "ghost" colliders like pick-ups
        if (other.CompareTag("EnergyOrb"))
        {
            spaceshipUI.UpdateEnergy(20f);
            other.gameObject.SetActive(false);
        }
    }
}