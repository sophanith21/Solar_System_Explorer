using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlanetButtonUI
{
    public Button button;       // The button in sidebar
    public string planetName;   // One button corresponding to one planet in the solar system
}

public class PlanetSidebarController : MonoBehaviour
{
    public FreeCameraController cameraController;
    public PlanetButtonUI[] planetButtons;       
    private Transform solarSystemRoot;
    public SolarSystemSpawner spawner;           
    public PlanetFactsUI planetFactsUI;                     

    void Start()
    {
        solarSystemRoot = GameObject.FindObjectOfType<SolarSystemSpawner>().transform;

        foreach (var pb in planetButtons)
        {
            string nameCopy = pb.planetName;  
            pb.button.onClick.AddListener(() =>
            {
                Transform planet = solarSystemRoot.Find(nameCopy);
                if (planet != null)
                {
                    cameraController.FocusOnPlanet(planet);
                    // Get the PlanetData for this planet
                    PlanetData data = spawner.planets.Find(p => p.name == nameCopy);
                    if (data != null)
                    {
                        string facts = $"Name: {data.name}\n" +
                                    $"Distance from Sun: {data.distanceFromSunAU} AU\n" +
                                    $"Diameter (relative to Earth): {data.relativeSize}\n" +
                                    $"Orbit Inclination: {data.orbitInclination}°\n" +
                                    $"Orbital Period: {data.orbitalPeriod} Earth years\n" +
                                    $"Rotation Speed: {data.simulationSpeed}\n" +
                                    $"Layer Index: {data.layerIndex}";

                        planetFactsUI.ShowFacts(facts);
                    }

                }

            });
        }
    }
}
