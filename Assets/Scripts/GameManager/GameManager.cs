using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Winning Distance")]
    public float winningDistance = 50f;
    [Header("References")]
    public NavigationSystem navigationSystem;
    public MissionSelector missionSelector;

    float currentDistance = 0f;

    void Start()
    {
        winningDistance += missionSelector.currentMission.destinationPlanet.transform.localScale.x * 0.5f;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        currentDistance = navigationSystem.planetsDistance[missionSelector.currentMission.destinationPlanet.name];
        if (currentDistance < winningDistance)
        {
            
            SceneManagement.Instance.LoadScene("Winning Scene");
            SFXManager.Instance.PlayWin();
        }
    }
}
