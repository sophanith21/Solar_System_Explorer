using UnityEngine;

public class AsteroidZoneSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject asteriodPrefab;
    public float spawnDistanceAhead = 15f;
    public float spawnWidth = 10f;
    public float spawnHeight = 8f;


    public Vector3 GetRandomSpawnPosition()
    {
        // 1. Calculate the base anchor point in front of the player
        //    (Player Position + Forward Vector * Distance)
        Vector3 anchorPosition = player.position + player.forward * spawnDistanceAhead;

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
        if (Input.GetKeyDown(KeyCode.Space) )// Press Space to spawn an asteroid
        {
            Debug.Log("Space key pressed, spawning asteroid...");
            Vector3 spawnPosition = GetRandomSpawnPosition();
            asteriodPrefab.layer = player.gameObject.layer; 
            Instantiate(asteriodPrefab, spawnPosition, Quaternion.identity);
        }
    
    }

    // Example call in the pooling manager:
    void RecycleAsteroid(GameObject asteroidToRecycle) {
        asteroidToRecycle.transform.position = GetRandomSpawnPosition();
        asteroidToRecycle.SetActive(true);
    }
}