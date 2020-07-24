using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Incline : MonoBehaviour
{

    public GameObject text;
    public GameObject backText;

    Rigidbody2D rgbd;

    AudioSource audio;

    private int TitleTextValue = 0;

    void Awake()
    {
        rgbd = text.GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    public void Tilt()
    {
        if (TitleTextValue == 0)
        {
            rgbd.WakeUp();
            rgbd.bodyType = RigidbodyType2D.Dynamic;

            TitleTextValue++;

            audio.Play();
        }
        else
        {
            rgbd.bodyType = RigidbodyType2D.Static;

            text.transform.rotation = Quaternion.Euler(0,0,0);
            text.transform.position = backText.transform.position;

            TitleTextValue--;

            audio.Stop();
        }
    }
}
