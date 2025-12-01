using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
public class PlanetData
{
    public string name;
    public GameObject prefab;
    [Tooltip("Distance from Sun in Astronomical Units (AU)")]
    public float distanceFromSunAU;
    [Tooltip("Diameter relative to Earth (Earth = 1)")]
    public float relativeSize;
    [Tooltip("Tilt of the orbit in degrees")]
    public float orbitInclination;
    [Tooltip("Speed of orbit (Earth years per revolution)")]
    public float orbitalPeriod;
    [Tooltip("Planetary rotaton simulation speed")]
    public float simulationSpeed = 10f;
    [Tooltip("Layer Index")]
    public int layerIndex;
}

public class SolarSystemSpawner : MonoBehaviour
{
    [Header("Global Settings")]
    [Tooltip("How many Unity Units equal 1 Astronomical Unit (Distance from Sun to Earth)")]
    public float distanceScale = 100f; // 1 AU = 100 Unity Meters

    [Tooltip("Multiplier for the size of the planets. CAUTION: True scale is 1, but planets will be invisible dots.")]
    public float planetSizeMultiplier = 1f;

    [Tooltip("Change how fast planets move around the sun")]
    public float simulationSpeed = 10f;

    [Tooltip("The point at the center of the solar system")]
    public Transform anchorPoint;

    [Header("Central Body (Sun)")]
    [Tooltip("Prefab for the Sun (central star)")]
    public GameObject sunPrefab;
    [Tooltip("Diameter relative to Earth (Earth = 1). Sun is ~109.")]
    public float sunRelativeSize = 109f; // Real-world value (109.2x Earth's diameter)

    [Header("Planetary Data")]
    public List<PlanetData> planets = new List<PlanetData>();

    private GameObject sun;

    private void Start()
    {
        SpawnSun(); 
        SpawnPlanets();
    }

    void SpawnSun()
    {
        if (sunPrefab == null)
        {
            Debug.LogWarning("Sun Prefab not assigned. The central body will not be spawned.");
            return;
        }

        // Instantiate the Sun at the spawner's location (the center of the system)
        GameObject sun = Instantiate(sunPrefab, transform.position, Quaternion.identity);
        sun.name = "Sun";
        sun.transform.SetParent(this.transform);

        // Apply scale, using the same planet size multiplier for consistency
        float finalSize = sunRelativeSize * planetSizeMultiplier;
        sun.transform.localScale = Vector3.one * finalSize;
    }

    void SpawnPlanets()
    {
        if (planets.Count == 0)
        {
            Debug.LogWarning("No planet data entered!");
            return;
        }


        foreach (PlanetData planet in planets)
        {
            if (planet.prefab == null) continue;

            

            // 1. Calculate Position
            // We use a random angle to scatter them around the sun so they aren't in a straight line
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // Calculate X and Z based on angle and distance (Polar to Cartesian coordinates)
            float currentDistance = planet.distanceFromSunAU * distanceScale;

            float x = Mathf.Cos(randomAngle) * currentDistance;
            float z = Mathf.Sin(randomAngle) * currentDistance;

            // Calculate Y based on Inclination (simplified)
            // Tan(inclination) = y / distance
            float y = Mathf.Tan(planet.orbitInclination * Mathf.Deg2Rad) * currentDistance;

            Vector3 spawnPos = new Vector3(x, y, z) + transform.position;


            // 2. Instantiate
            GameObject newPlanet = Instantiate(planet.prefab, spawnPos, Quaternion.identity);
            newPlanet.name = planet.name;
            newPlanet.transform.SetParent(this.transform);


            // 3. Apply Scale
            // Default Earth diameter in Unity is usually assumed to be 1 unit for reference
            float finalSize = planet.relativeSize * planetSizeMultiplier;
            newPlanet.transform.localScale = Vector3.one * finalSize;

            GameObject lightGameObject = new GameObject(planet.name + "CompensatorLight");
            Light compensatorLight = lightGameObject.AddComponent<Light>();
            compensatorLight.type = UnityEngine.LightType.Directional;
            compensatorLight.intensity = 2f; 
            compensatorLight.shadows = LightShadows.Hard; 
           
            compensatorLight.useColorTemperature = true;     
            compensatorLight.colorTemperature = 5000f;      
            compensatorLight.color = Color.white;



            lightGameObject.transform.LookAt(anchorPoint);

            Vector3 midpoint = (spawnPos + anchorPoint.position) / 2;

            Vector3 direction = (spawnPos - anchorPoint.position);


            lightGameObject.transform.position = midpoint + (direction * 0.35f);

            lightGameObject.transform.SetParent(newPlanet.transform);


            // Set Culling Mask to ONLY affect the planet's layer
            compensatorLight.cullingMask = 1 << planet.layerIndex;


            


            newPlanet.AddComponent<PlanetOrbiter>().Setup(this.transform,planet.orbitalPeriod,planet.orbitInclination,simulationSpeed,lightGameObject.transform,anchorPoint);


            // Ensure all child objects are on the correct layer too
            SetLayerRecursively(newPlanet.transform, planet.layerIndex);
            
            
        }
    }



    private void SetLayerRecursively(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;

        // Iterate through all children and call the function again (recursion)
        foreach (Transform child in parent)
        {
            SetLayerRecursively(child, layer);
        }
    }

    // Visualization to help you see the orbits in the Editor before playing
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        foreach (PlanetData planet in planets)
        {
            float radius = planet.distanceFromSunAU * distanceScale;
            DrawOrbitGizmo(radius);
        }
    }

    void DrawOrbitGizmo(float radius)
    {  
        int segments = 360;
        float angleStep = 360f / segments;
        Vector3 prevPoint = transform.position + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments + 1; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 nextPoint = transform.position + new Vector3(x, 0, z);

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    // HELPER: Context menu to auto-fill real world data
    [ContextMenu("Fill Real Solar System Data")]
    void FillRealData()
    {
        // Set the Sun's relative size
        sunRelativeSize = 109f;

        planets.Clear();
        // Distances in AU, Sizes relative to Earth
        AddPlanet("Mercury", 0.39f, 0.38f, 7.0f, 0.24f,6);
        AddPlanet("Venus", 0.72f, 0.95f, 3.4f, 0.62f,7);
        AddPlanet("Earth", 1.00f, 1.00f, 0.0f, 1.00f,8);
        AddPlanet("Mars", 1.52f, 0.53f, 1.9f, 1.88f,9);
        AddPlanet("Jupiter", 5.20f, 11.21f, 1.3f, 11.86f,10);
        AddPlanet("Saturn", 9.58f, 9.45f, 2.5f, 29.45f,11);
        AddPlanet("Uranus", 19.22f, 4.01f, 0.8f, 84.02f,12);
        AddPlanet("Neptune", 30.05f, 3.88f, 1.8f, 164.8f,13);
        Debug.Log("Data Filled! Please assign Prefabs manually.");
    }

    void AddPlanet(string n, float dist, float size, float inc, float period,int layerIndex)
    {
        PlanetData p = new PlanetData();
        p.name = n;
        p.distanceFromSunAU = dist;
        p.relativeSize = size;
        p.orbitInclination = inc;
        p.orbitalPeriod = period;
        p.layerIndex = layerIndex;
        planets.Add(p);
    }
}