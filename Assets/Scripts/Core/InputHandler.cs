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

    public Action OnUpKey;
    public Action<Vector2> OnDownKey;
    public Action<Vector2> OnLeftKey;
    public Action<Vector2> OnRightKey;
    public Action OnJumpKey;

    public Action OnUpKeyUp;
    public Action<Vector2> OnDownKeyUp;
    public Action<Vector2> OnLeftKeyUp;
    public Action<Vector2> OnRightKeyUp;
    public Action OnJumpKeyUp;

    public Action OnUpKeyDown;
    public Action<Vector2> OnDownKeyDown;
    public Action<Vector2> OnLeftKeyDown;
    public Action<Vector2> OnRightKeyDown;
    public Action OnJumpKeyDown;

    public Action<Vector2> OnAttackKeyDown;

    public Action OnEscapeKeyDown;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            OnUpKey?.Invoke();
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
        if (Input.GetKey(KeyCode.Space))
        {
            OnJumpKey?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            OnUpKeyUp?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            OnUpKeyUp?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            OnLeftKeyUp?.Invoke(Vector2.left);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            OnRightKeyUp?.Invoke(Vector2.right);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpKeyUp?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            OnUpKeyDown?.Invoke();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpKeyDown?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnAttackKeyDown?.Invoke(Input.mousePosition);
        }
    }
}
