using System.Collections.Generic;
using UnityEngine;

public class AsteroidZoneSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject asteriodPrefab;
    public float spawnDistanceAhead = 15f;
    public float spawnWidth = 10f;
    public float spawnHeight = 8f;
    public float minSpawnDepth = 5f;
    public float maximumSpawnDepth = 30f;
    public float spawnInterval = 10f;
    public int asteroidsPerInterval = 4;

    float currentTime = 0f;
    List<GameObject> asteroidGameObjects = new List<GameObject>();
    bool isFar = true; // Must initialized with TRUE because the first asteroid batch must be spawned

    public Vector3 GetRandomSpawnPosition()
    {
        // 1. Calculate the base anchor point in front of the player
        //    (Player Position + Forward Vector * Distance)
        Vector3 anchorPosition = player.position + player.forward * spawnDistanceAhead + player.forward * Random.Range(minSpawnDepth,maximumSpawnDepth);

        // 2. Generate random offsets (from -HalfSize to +HalfSize)
        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float randomY = Random.Range(-spawnHeight / 2f, spawnHeight / 2f);

        // 3. Transform the offsets into World Space relative to the player's rotation
        //    This ensures the random X/Y spread is relative to where the player is looking.
        Vector3 offset = player.right * randomX + player.up * randomY;

        // 4. Final Spawn Position
        Vector3 finalSpawnPosition = anchorPosition + offset;

        return finalSpawnPosition;
    }

    private void Update()
    {
        // Example spawning logic (for demonstration purposes)
        if (currentTime >= spawnInterval && isFar)
        {
            for (int i = 0; i < asteroidsPerInterval; i++)
            {
                // Only spawns the asteroids once, then reuse it using the pooling manager
                if (asteroidGameObjects.Count < asteroidsPerInterval)
                {
                    Debug.Log(" spawning asteroid...");
                    Vector3 spawnPosition = GetRandomSpawnPosition();
                    asteriodPrefab.layer = player.gameObject.layer;
                    GameObject asteroid = Instantiate(asteriodPrefab, spawnPosition, Quaternion.identity);
                    asteroid.tag = "Asteroid";
                    asteroidGameObjects.Add(asteroid);

                }else
                {
                    Debug.Log(" Reusing asteroid...");
                    GameObject asteroidToRecycle = asteroidGameObjects[i];
                    asteroidToRecycle.transform.position = GetRandomSpawnPosition();
                    asteroidToRecycle.SetActive(true);
                }
                
            }
            currentTime = 0f;
            
        }

        currentTime += Time.deltaTime;

        if (asteroidGameObjects.Count > 0)
        {
            float distanceToPlayer = Vector3.Distance(asteroidGameObjects[0].transform.position, player.position);
            if (distanceToPlayer > spawnDistanceAhead + maximumSpawnDepth)
            {
               
                isFar = true;
                
            }
            else
            {
                isFar = false;
            }
            
        }
    
    }

    // Pooling manager:
    void RecycleAsteroid(GameObject asteroidToRecycle) {
        asteroidToRecycle.transform.position = GetRandomSpawnPosition();
        asteroidToRecycle.SetActive(true);
    }

    
}