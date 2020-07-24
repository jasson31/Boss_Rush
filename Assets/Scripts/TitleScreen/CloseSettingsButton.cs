using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseSettingsButton : MonoBehaviour
{
    public GameObject active1, active2;

    public void CloseSettings()
    {
        UnityEngine.Debug.Log("Close Settings!");
        active1.SetActive(true);
        active2.SetActive(false);
    }
}
