using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public Action OnAccelBegin;
    public Action OnAccelEnd;
    public Action OnDecelBegin;
    public Action OnDecelEnd;
    public Action OnMousePositionMove;

    public Vector2 MousePos { get; private set; }

    private void Update()
    {
        UpdateKeyboardInput();
        UpdateMouseInput();
    }

    private void UpdateKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnAccelBegin?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            OnAccelEnd?.Invoke();   
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnDecelBegin?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            OnDecelEnd?.Invoke();
        }
    }

    private void UpdateMouseInput()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

}
