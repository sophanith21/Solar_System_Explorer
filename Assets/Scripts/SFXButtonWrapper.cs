using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXButtonWrapper : MonoBehaviour
{
   public void play()
    {
        SFXManager.Instance.PlayUIClick();
    }
}
