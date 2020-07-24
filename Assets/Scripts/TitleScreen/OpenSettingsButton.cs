using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSettingsButton : MonoBehaviour
{

    public GameObject active1, active2;

    void Awake()
    {
        active2.SetActive(false);
    }

    public void OpenSettings()
    {
        UnityEngine.Debug.Log("Open Settings!");
        active1.SetActive(false);
        active2.SetActive(true);
    }
}
