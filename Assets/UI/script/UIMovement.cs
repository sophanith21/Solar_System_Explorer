using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMovement : MonoBehaviour
{
    [Header("Spin Direcrtion")]
    public Vector3 rotationSpeed = new Vector3(0, 100f, 0); // degrees per second

    [Header("Up/Down Movement")]
    public float floatHeight = 1.5f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    public bool isSpaceship = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isSpaceship)
        {
            // Float up & down
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = startPos + Vector3.up * yOffset;
        }
        else
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

    }
}
