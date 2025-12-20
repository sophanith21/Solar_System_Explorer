using UnityEngine;
using TMPro;

public class PlanetFactsUI : MonoBehaviour
{
    public GameObject panel;    
    public TMP_Text factsText;

    public void ShowFacts(string facts)
    {
        factsText.text = facts;
        panel.SetActive(true);
    }

    public void HideFacts()
    {
        panel.SetActive(false);
    }
}
