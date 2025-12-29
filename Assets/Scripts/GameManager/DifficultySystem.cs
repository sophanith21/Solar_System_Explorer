using System.Collections.Generic;
using UnityEngine;

public enum DistanceCategory
{
    Easy,
    Moderate,
    Hard
}

[System.Serializable]
public class PlanetInfo
{
    public string planetName;
    public GameObject planetObject;
    public float auFromSun;
}

[System.Serializable]
public class Mission
{
    public string missionName;
    public DistanceCategory distanceCategory;
    public GameObject startPlanet;
    public GameObject destinationPlanet;
    public float distanceAU;
}

public class DifficultySystem : MonoBehaviour
{
    // ✅ Singleton
    public static DifficultySystem Instance;

    [Header("Distance Thresholds (AU)")]
    public float easyLimit = 1f;
    public float moderateLimit = 10f;

    [Header("Registered Planets (Runtime)")]
    public SolarSystemSpawner solarSystemSpawner;
    public List<PlanetInfo> planets = new();

    [Header("Mission Pools")]
    public List<Mission> easyMissions = new();
    public List<Mission> moderateMissions = new();
    public List<Mission> hardMissions = new();

    [Header("Mission Selector")]
    public MissionSelector missionSelector;

    void Awake()
    {
        Instance = this;
    }

    // ✅ Called by Planet scripts
    public void RegisterPlanet(GameObject planetObject, string name, float au)
    {
        if (!planets.Exists(p => p.planetObject == planetObject))
        {
            planets.Add(new PlanetInfo
            {
                planetName = name,
                planetObject = planetObject,
                auFromSun = au
            });
        }
    }

    // ✅ Call this AFTER all planets are spawned
    public void GenerateAllMissions()
    {
        easyMissions.Clear();
        moderateMissions.Clear();
        hardMissions.Clear();

        for (int i = 0; i < planets.Count; i++)
        {
            for (int j = 0; j < planets.Count; j++)
            {
                if (i == j) continue;

                PlanetInfo start = planets[i];
                PlanetInfo dest = planets[j];

                float distanceAU = Vector3.Distance(start.planetObject.transform.position, dest.planetObject.transform.position)/solarSystemSpawner.distanceScale;

                DistanceCategory category =
                    distanceAU <= easyLimit ? DistanceCategory.Easy :
                    distanceAU <= moderateLimit ? DistanceCategory.Moderate :
                    DistanceCategory.Hard;

                Mission mission = new Mission
                {
                    missionName = $"{start.planetName} → {dest.planetName}",
                    startPlanet = start.planetObject,
                    destinationPlanet = dest.planetObject,
                    distanceAU = distanceAU,
                    distanceCategory = category
                };

                switch (category)
                {
                    case DistanceCategory.Easy: easyMissions.Add(mission); break;
                    case DistanceCategory.Moderate: moderateMissions.Add(mission); break;
                    case DistanceCategory.Hard: hardMissions.Add(mission); break;
                }
            }
        }

        DebugLogMissions();

        if (missionSelector != null)
            missionSelector.SelectMission();
    }

    public PlanetInfo getPlanetByName(string name)
    {
        return planets.Find(p => p.planetName == name);
    }


    void DebugLogMissions()
    {
        Debug.Log($"Easy Missions: {easyMissions.Count}");
        Debug.Log($"Moderate Missions: {moderateMissions.Count}");
        Debug.Log($"Hard Missions: {hardMissions.Count}");
    }
}