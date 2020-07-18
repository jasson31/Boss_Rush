using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        InputHandler.inst.OnLeftKey += Move;
        InputHandler.inst.OnRightKey += Move;
    }

    private void OnDisable()
    {
        InputHandler.inst.OnLeftKey -= Move;
        InputHandler.inst.OnRightKey -= Move;
    }

    private void Move(Vector2 direction)
    {
        Vector3 moveDirection = direction;
        transform.position += moveDirection * 2 * Time.deltaTime;
    }
}
