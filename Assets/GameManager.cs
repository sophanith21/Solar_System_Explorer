using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Winning Distance")]
    public float winningDistance = 100f;
    [Header("References")]
    public NavigationSystem navigationSystem;
    public MissionSelector missionSelector;

    float currentDistance = 0f;
    

    // Update is called once per frame
    void Update()
    {
        currentDistance = navigationSystem.planetsDistance[missionSelector.currentMission.destinationPlanet.name];
        if (currentDistance < 100)
        {
            SceneManagement.Instance.LoadScene("Winning Scene");
        }
    }
}
