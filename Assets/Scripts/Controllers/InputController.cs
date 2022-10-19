using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public Action<Vector2> OnDesiredTranslateChange;
    public Action OnAccelBegin;
    public Action OnAccelEnd;
    public Action OnDecelBegin;
    public Action OnDecelEnd;
    public Action<bool> OnTurnLeft;
    public Action<bool> OnTurnRight;
    public Action OnMousePositionMove;
    public Action<int> OnScroll;
    public Action<int> OnMouseDown;
    public Action<int> OnMouseUp;
    public Action OnUpgradeMenuToggled;
    public Action OnScanDecrement;
    public Action OnScanIncrement;
    public Action OnMSelect;

    Ray ray;
    float distance;
    Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));

    //settings
    [SerializeField] float _mousePosSensitivity = 0.1f;

    //state
    public Vector3 MousePos { get; private set; }
    public bool IsTranslationalMovementMode = false;
    Vector2 _desiredTranslation = Vector2.zero;



    private void Update()
    {
        if (IsTranslationalMovementMode == true) UpdateKeyboardInput_Translation();
        else UpdateKeyboardInput_Rotation();

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

    private void UpdateKeyboardInput_Translation()
    {
        _desiredTranslation = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            _desiredTranslation.y += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _desiredTranslation.y -= 1f;
        }
        _desiredTranslation.y =
            Mathf.Clamp(_desiredTranslation.y ,- 1f, 1f);

        if (Input.GetKey(KeyCode.A))
        {
            _desiredTranslation.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _desiredTranslation.x += 1f;
        }
        _desiredTranslation.x =
            Mathf.Clamp(_desiredTranslation.x ,- 1f, 1f);


        OnDesiredTranslateChange?.Invoke(_desiredTranslation.normalized);
        
    }


    private void UpdateKeyboardInput_Rotation()
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
        
        if (Input.GetKeyDown(KeyCode.A)) OnTurnLeft?.Invoke(true);
        if (Input.GetKeyUp(KeyCode.A)) OnTurnLeft?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.D)) OnTurnRight?.Invoke(true);
        if (Input.GetKeyUp(KeyCode.D)) OnTurnRight?.Invoke(false);

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OnUpgradeMenuToggled?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.Q)) OnScanDecrement?.Invoke();
        if (Input.GetKeyDown(KeyCode.E)) OnScanIncrement?.Invoke();

        if (Input.GetKeyDown(KeyCode.M)) OnMSelect?.Invoke();
    }

    private void UpdateMouseInput()
    {
        Vector3 prev = MousePos;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        xy.Raycast(ray, out distance);
        MousePos = ray.GetPoint(distance);

        if ((MousePos - prev).magnitude > _mousePosSensitivity)
        {
            OnMousePositionMove?.Invoke();
        }

    }


}
