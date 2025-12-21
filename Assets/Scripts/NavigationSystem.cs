using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationSystem : MonoBehaviour
{
    public FreeCameraController freeCameraController;
    public Dictionary<string, float> planetsDistance = new Dictionary<string, float>();
    void Awake()
    {
        planetsDistance.Add("Mercury", 0f);
        planetsDistance.Add("Venus", 0f);
        planetsDistance.Add("Earth", 0f);
        planetsDistance.Add("Mars", 0f);
        planetsDistance.Add("Jupiter", 0f);
        planetsDistance.Add("Saturn", 0f);
        planetsDistance.Add("Uranus", 0f);
        planetsDistance.Add("Neptune", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlanetInfo planet in DifficultySystem.Instance.planets)
        {
            float distance = Vector3.Distance(transform.position, planet.planetObject.transform.position);
            planetsDistance[planet.planetName] = distance;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            string nearestPlanet = findTheNearestPlanet();
            PlanetInfo targetPlanet = DifficultySystem.Instance.getPlanetByName(nearestPlanet);
            if (targetPlanet != null)
            {
                freeCameraController.FocusOnPlanet(targetPlanet.planetObject.transform, followPlanet: true);
            }
        }
       
        
    }

    string findTheNearestPlanet()
    {
        string nearestPlanet = "";
        float minDistance = float.MaxValue;

        foreach (var kvp in planetsDistance)
        {
            if (kvp.Value < minDistance)
            {
                minDistance = kvp.Value;
                nearestPlanet = kvp.Key;
            }
        }

        return nearestPlanet;
    }


}
