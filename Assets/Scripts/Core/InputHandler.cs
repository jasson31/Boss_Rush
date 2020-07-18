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
    public Action OnUpKeyDown;

    public Action<Vector2> OnAttackKeyDown;

    public Action OnEscapeKeyDown;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnUpKeyDown?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnAttackKeyDown?.Invoke(Input.mousePosition);
        }
    }
}
