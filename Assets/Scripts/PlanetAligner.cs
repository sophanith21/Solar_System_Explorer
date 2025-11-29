using UnityEngine;

public class PlanetAligner : MonoBehaviour
{
    public Transform sun;          // Center
    public Transform[] planets;    // Planets in order
    public float radius = 10f;     // Distance from sun

    void Start()
    {
        int N = planets.Length;
        float angleStep = 360f / N;

        for (int i = 0; i < N; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            planets[i].position = sun.position + new Vector3(x, 0, z);
        }
    }
}
