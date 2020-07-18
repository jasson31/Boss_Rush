using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserKeySetting
{
    //TODO
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
}

public class InputHandler : SingletonBehaviour<InputHandler>
{
    private UserKeySetting setting;

    public Action<Vector2> OnUpKey;
    public Action<Vector2> OnDownKey;
    public Action<Vector2> OnLeftKey;
    public Action<Vector2> OnRightKey;

    public Action<Vector2> OnUpKeyDown;
    public Action<Vector2> OnDownKeyDown;
    public Action<Vector2> OnLeftKeyDown;
    public Action<Vector2> OnRightKeyDown;

    public Action<Vector2> OnAttackKeyDown;

    public Action OnEscapeKeyDown;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            OnUpKey?.Invoke(Vector2.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            OnDownKey?.Invoke(Vector2.down);
        }
        if (Input.GetKey(KeyCode.A))
        {
            OnLeftKey?.Invoke(Vector2.left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            OnRightKey?.Invoke(Vector2.right);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnUpKeyDown?.Invoke(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnDownKeyDown?.Invoke(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnLeftKeyDown?.Invoke(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnRightKeyDown?.Invoke(Vector2.right);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnAttackKeyDown?.Invoke(Input.mousePosition);
        }
    }
}
