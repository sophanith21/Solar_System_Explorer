using UnityEngine;
using TMPro;

public class MissionUI : MonoBehaviour
{
    [Header("Optional: Assign manually or leave empty for auto-find")]
    public TMP_Text missionText;
    public NavigationSystem nav;
    public SolarSystemSpawner solarSystemSpawner;

    void Awake()
    {
        // Auto-find TMP_Text in children if not assigned manually
        if (missionText == null)
        {
            missionText = GetComponentInChildren<TMP_Text>();

            if (missionText == null)
            {
                Debug.LogWarning("Mission Text TMP component not found! Please add one in children.");
            }
        }
    }

    // ✅ This is the method MissionSelector will call
    public void UpdateMissionText(Mission mission)
    {
        if (missionText != null && mission != null)
        {
            missionText.text = $"Mission: {mission.missionName}\n" +
                               $"Planets' Distance: {mission.distanceAU} AU\n" +
                               $"Spaceship's Distance: {nav.planetsDistance[mission.destinationPlanet.name]/solarSystemSpawner.distanceScale} AU\n" +
                               $"Difficulty: {mission.distanceCategory}";
        }
    }
}