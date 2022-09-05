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
    public Action<int> OnScroll;
    public Action<int> OnMouseDown;
    public Action<int> OnMouseUp;
    public Action OnUpgradeMenuToggled;

    Ray ray;
    float distance;
    Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));

    //settings


    //state
    public Vector3 MousePos { get; private set; }


    private void Update()
    {
        UpdateKeyboardInput();
        UpdateMouseInput();
        UpdateMouseScrollInput();
        UpdateMouseFiringInput();
    }

    private void UpdateMouseFiringInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnMouseDown?.Invoke(0);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnMouseUp?.Invoke(0);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnMouseDown?.Invoke(1);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            OnMouseUp?.Invoke(1);
        }
    }

    private void UpdateMouseScrollInput()
    {
        if (Input.mouseScrollDelta.y > 0.00f)
        {
            OnScroll?.Invoke(1);
            return;
        }
        if (Input.mouseScrollDelta.y < -0.00f)
        {
            OnScroll?.Invoke(-1);
            return;
        }
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
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OnUpgradeMenuToggled?.Invoke();
        }
    }

    private void UpdateMouseInput()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        xy.Raycast(ray, out distance);
        MousePos = ray.GetPoint(distance);

    }


}
