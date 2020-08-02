using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonBehaviour<CameraController>
{
    private float z;
    private void Start()
    {
        z = transform.position.z;
    }

    public void ShakeCamera(float scale, float duration)
    {
        StartCoroutine(ShakeCameraRoutine(scale, duration));
    }

    private IEnumerator ShakeCameraRoutine(float scale, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            transform.localPosition = scale * ((duration - t) / duration) * Random.insideUnitCircle;
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
            yield return null;
        }
        transform.localPosition = new Vector3(0, 0, z);
    }

}
