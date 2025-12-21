using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WinLoseUI : MonoBehaviour
{
    [Header("Win/Lose Screen")]
    public GameObject winScreen;
    public GameObject loseScreen;

    void Update()
    {
        // simulate screen switch of win lose
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchScreen();
        }

    }


    public void SwitchScreen()
    {
        winScreen.SetActive(!winScreen.activeSelf);
        loseScreen.SetActive(!loseScreen.activeSelf);
    }


}
