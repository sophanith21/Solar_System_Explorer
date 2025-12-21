using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PlanetButtonUI
{
    public Button button;
    public string planetName;
}
// The button in sidebar public string planetName; // One button corresponding to one planet in the solar system }

public class PlanetSidebarController : MonoBehaviour
{
    public FreeCameraController cameraController;
    public PlanetButtonUI[] planetButtons;
    private Transform solarSystemRoot;
    public SolarSystemSpawner spawner;
    public PlanetFactsUI planetFactsUI;

    // Add a dictionary to store facts for each planet
    private Dictionary<string, string> interestingFacts = new Dictionary<string, string>()
    {
        {"Mercury", "Most extreme temperature swings: −173°C to 427°C."},
        {"Venus", "A day on Venus is longer than its year."},
        {"Earth", "The only planet known to support life."},
        {"Mars", "Home to the tallest volcano in the solar system: Olympus Mons."},
        {"Jupiter", "Has a giant storm, the Great Red Spot, larger than Earth."},
        {"Saturn", "Its rings are wide but extremely thin."},
        {"Uranus", "Rotates on its side, making seasons extreme."},
        {"Neptune", "Has the strongest winds in the solar system: over 2,000 km/h."}
    };

    void Start()
    {
        solarSystemRoot = spawner.transform;

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
                        // Get fact from dictionary
                        string interestingFact = interestingFacts.ContainsKey(nameCopy) ? interestingFacts[nameCopy] : "No fact available";

                        string facts = $"Name: {data.name}\n" +
                                    $"Distance from Sun: {data.distanceFromSunAU} AU\n" +
                                    $"Diameter (relative to Earth): {data.relativeSize}\n" +
                                    $"Orbit Inclination: {data.orbitInclination}°\n" +
                                    $"Orbital Period: {data.orbitalPeriod} Earth years\n" +
                                    $"Rotation Speed: {data.simulationSpeed}\n" +
                                    $"Interesting Fact: {interestingFact}";

                        planetFactsUI.ShowFacts(facts);
                    }
                }
            });
        }
    }
}
