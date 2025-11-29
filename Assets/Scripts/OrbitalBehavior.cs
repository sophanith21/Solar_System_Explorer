using UnityEngine;

public class SolarSystemRotate : MonoBehaviour
{
    public float rotationSpeed = 5f;

    void Update()
    {
        transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
    }
}
