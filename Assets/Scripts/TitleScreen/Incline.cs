using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Incline : MonoBehaviour
{

    public GameObject text;

    private int TitleTextValue = 0;

    public void Tilt()
    {
        if (TitleTextValue == 0)
        {
            text.transform.Rotate(new Vector3(0, 0, 15));
            text.transform.position += Vector3.down * 10;
            TitleTextValue++;
        }
        else
        {
            text.transform.Rotate(new Vector3(0, 0, -15));
            text.transform.position += Vector3.up * 10;
            TitleTextValue--;
        }
    }
}
