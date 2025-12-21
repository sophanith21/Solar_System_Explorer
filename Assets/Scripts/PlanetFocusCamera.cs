using UnityEngine;
using Cinemachine;

public class PlanetFocusCamera : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineVirtualCamera vCam;

    [Header("Camera Settings")]
    public float distanceFromPlanet = 10f;

    private void Awake()
    {
        // Auto-assign if not set
        if (vCam == null)
            vCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Call this when a mission starts
    public void FocusOnPlanet(Transform planet)
    {
        if (planet == null)
        {
            Debug.LogWarning("Planet is null!");
            return;
        }

        // Tell Cinemachine what to look at
        vCam.LookAt = planet;

        // Place the virtual camera in front of the planet
        Vector3 direction = (transform.position - planet.position).normalized;

        // Fallback direction if camera is exactly at planet position
        if (direction == Vector3.zero)
            direction = Vector3.back;

        transform.position = planet.position + direction * distanceFromPlanet;
    }
}
