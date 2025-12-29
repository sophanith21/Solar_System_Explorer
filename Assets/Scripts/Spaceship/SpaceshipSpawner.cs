using UnityEngine;

public class SpaceshipSpawner : MonoBehaviour
{
    [Header("Spaceship")]
    public Transform spaceShipTransform;   
    public float spawnRadius = 5f;     

    public void SpawnSpaceshipNearPlanet(Mission mission)
    {
        if (mission == null || mission.startPlanet == null)
        {
            Debug.LogWarning("Mission or start planet not set!");
            return;
        }

        if (spaceShipTransform == null)
        {
            Debug.LogWarning("Spaceship prefab not assigned!");
            return;
        }

        // 1. Calculate the new position
        Vector3 randomOffset = Random.onUnitSphere * (spawnRadius + 10f); // Spawn slightly further out
        randomOffset.y = Mathf.Abs(randomOffset.y);
        Vector3 spawnPosition = mission.startPlanet.transform.position + randomOffset;

        // 2. Get the Rigidbody for Safe Teleportation
        Rigidbody rb = spaceShipTransform.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Disable interpolation to prevent the "streak" effect
            rb.interpolation = RigidbodyInterpolation.None;

            // Move the Rigidbody position directly
            rb.position = spawnPosition;

            // Set rotation to face the planet
            Vector3 directionToPlanet = mission.startPlanet.transform.position - spawnPosition;
            rb.rotation = Quaternion.LookRotation(directionToPlanet);

            // Reset velocity so the ship doesn't bring old momentum to the new planet
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Force Unity to update the transform immediately
            Physics.SyncTransforms();

            // Re-enable interpolation for smooth flying
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            // Fallback if no Rigidbody exists
            spaceShipTransform.position = spawnPosition;
            spaceShipTransform.LookAt(mission.startPlanet.transform);
        }

        Debug.Log($"Spaceship spawned near {mission.startPlanet.name} at {spawnPosition}");
    }
}
