using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOrbiter : MonoBehaviour
{

    // The Transform of the Sun/Central Body (for the center of rotation)
    private Transform sunCenter;

    // The orbital speed calculated from the orbital period
    private float degreesPerYear;

    // The rotation axis, adjusted for orbital inclination
    private Vector3 rotationAxis;

    [Tooltip("The global multiplier applied to all orbital speeds.")]
    public float simulationSpeedMultiplier = 10f;

    private Transform lightTransform;

    private Transform anchorPoint;

    
    public void Setup(Transform center, float orbitalPeriod, float inclination, float simulationSpeed,Transform lightTransform, Transform anchorPoint)
    {
        sunCenter = center;
        simulationSpeedMultiplier = simulationSpeed;

        this.lightTransform = lightTransform;
        
        this.anchorPoint = anchorPoint;

        degreesPerYear = 360f / orbitalPeriod;

        // Calculate the tilted rotation axis (This handles the Orbit Inclination)
        Quaternion inclinationRotation = Quaternion.Euler(0, 0, inclination);
        rotationAxis = inclinationRotation * Vector3.up;
    }

    // 2. Continuous Movement Loop
    void Update()
    {
        if (sunCenter == null) return;

        Vector3 lightPosition = lightTransform.position;

        Vector3 inverseDirection = lightPosition - anchorPoint.position;

        lightTransform.rotation = Quaternion.LookRotation(inverseDirection);

        // Calculate the angle to rotate this frame:
        // (Base Speed) * (Time elapsed this frame) * (Simulation Speed Multiplier)
        float angle = degreesPerYear * Time.deltaTime * simulationSpeedMultiplier;

        // Apply the rotation around the Sun's center, along the correct tilted axis
        transform.RotateAround(sunCenter.position, rotationAxis, angle);
    }
}
