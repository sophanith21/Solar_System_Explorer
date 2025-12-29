using UnityEngine;
public enum GameDifficulty
{
    Easy,
    Moderate,
    Hard
}
public class MissionSelector : MonoBehaviour
{
    
    [Header("References")]
    public DifficultySystem difficultySystem;
    public MissionUI missionUI;
     // drag GameManager here

    [Header("Player Selection")]
    public GameDifficulty selectedDifficulty;

    [Header("Selected Mission (Read Only)")]
    public Mission currentMission;

    [Header("Spaceship Spawner")]
    public SpaceshipSpawner spaceshipSpawner;   

    bool missionSelected = false;
    bool isSpaceshipSpawned = false;
    public void Awake()
    {
      selectedDifficulty = DifficultyStorage.SelectedDifficulty;  
    }

    public void Update()
    {
        if (missionSelected)
        {
            missionUI.UpdateMissionText(currentMission);
        }

        if (isSpaceshipSpawned)
        {
            spaceshipSpawner.SpawnSpaceshipNearPlanet(currentMission);
            isSpaceshipSpawned = false;
        }
    }
    public void SelectMission()
    {
        if (difficultySystem == null)
        {
            Debug.LogError("DifficultySystem reference missing!");
            return;
        }

        switch (selectedDifficulty)
        {
            case GameDifficulty.Easy:
                currentMission = GetRandomMission(difficultySystem.easyMissions);
                break;

            case GameDifficulty.Moderate:
                currentMission = GetRandomMission(difficultySystem.moderateMissions);
                break;

            case GameDifficulty.Hard:
                currentMission = GetRandomMission(difficultySystem.hardMissions);
                break;
        }

        if (currentMission != null)
        {
            Debug.Log($"Selected {selectedDifficulty} mission: {currentMission.missionName}");
            if (missionUI != null)
            {
                missionSelected = true;
            }
        }
     
        if (spaceshipSpawner != null)
        {
            isSpaceshipSpawned = true;
        }
        
    }

    Mission GetRandomMission(System.Collections.Generic.List<Mission> missions)
    {
        if (missions == null || missions.Count == 0)
        {
            Debug.LogWarning("No missions available for this difficulty!");
            return null;
        }

        int index = Random.Range(0, missions.Count);
        return missions[index];
    }

    

}