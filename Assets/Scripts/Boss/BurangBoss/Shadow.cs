using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.forward * 360 *Time.deltaTime);
    }
}
