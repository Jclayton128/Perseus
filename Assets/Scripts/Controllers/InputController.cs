using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public Action<Vector2> DesiredTranslateChanged;
    public Action AccelStarted;
    public Action AccelEnded;
    public Action DecelStarted;
    public Action DecelEnded;
    public Action<bool> TurnLeftChanged;
    public Action<bool> TurnRightChanged;

    public Action<Vector2, float> LookDirChanged;
    //public Action MousePositionMoved;
    public Action<int> ScrollWheelChanged;
    /// <summary>
    /// Invoked when Mouse0/LMB is changed. True: down. False: Up.
    /// </summary>
    public Action<bool> LeftMouseChanged;
    /// <summary>
    /// Invoked when Mouse1/RMB is changed. True: down. False: Up.
    /// </summary>
    public Action<bool> RightMouseChanged;
    public Action UpgradeMenuToggled;
    public Action ScanDecremented;
    public Action ScanIncremented;
    public Action MKeySelected; // currently used to toggle between mouse and keyboard turning

    Transform _playerTransform;

    Ray ray;
    float distance;
    Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));

    //settings
    [SerializeField] float _mousePosSensitivity = 0.2f;
    [SerializeField] float _lookDirChangeThreshold = 0.95f;
    [SerializeField] float _lookDirChangeSpeed = 1f;

    //state
    public Vector3 _mousePos;
    [SerializeField] private Vector2 _lookDir_Commanded = Vector2.one;
    

    private Vector2 _lookDir_Driven = Vector2.one;
    /// <summary>
    /// Normalize Vector2 of look direction, based on right stick or player-to-mouse dir
    /// </summary>
    public Vector2 LookDirection => _lookDir_Driven;
    float _lookAngle = 0;
    public float LookAngle => _lookAngle;
    public bool IsTranslationalMovementMode = false;
    Vector2 _desiredTranslation = Vector2.zero;

    private void Awake()
    {
        GetComponent<GameController>().PlayerSpawned += HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject newPlayer)
    {
        _playerTransform = newPlayer.transform;
    }

    private void Update()
    {
        if (IsTranslationalMovementMode == true) UpdateKeyboardInput_Translation();
        else UpdateKeyboardInput_Rotation();

        UpdateMouseInput();
        UpdateMouseScrollInput();
        if (_playerTransform) UpdateLookDirection();
        //UpdateMouseFiringInput();
    }

    private void UpdateMouseFiringInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LeftMouseChanged?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LeftMouseChanged?.Invoke(false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RightMouseChanged?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            RightMouseChanged?.Invoke(false);
        }
    }

    private void UpdateMouseScrollInput()
    {
        if (Input.mouseScrollDelta.y > 0.00f)
        {
            ScrollWheelChanged?.Invoke(1);
            return;
        }
        if (Input.mouseScrollDelta.y < -0.00f)
        {
            ScrollWheelChanged?.Invoke(-1);
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


        DesiredTranslateChanged?.Invoke(_desiredTranslation.normalized);
        
    }


    private void UpdateKeyboardInput_Rotation()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            AccelStarted?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            AccelEnded?.Invoke();   
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DecelStarted?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            DecelEnded?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.A)) TurnLeftChanged?.Invoke(true);
        if (Input.GetKeyUp(KeyCode.A)) TurnLeftChanged?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.D)) TurnRightChanged?.Invoke(true);
        if (Input.GetKeyUp(KeyCode.D)) TurnRightChanged?.Invoke(false);

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            UpgradeMenuToggled?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.Q)) ScanDecremented?.Invoke();
        if (Input.GetKeyDown(KeyCode.E)) ScanIncremented?.Invoke();

        if (Input.GetKeyDown(KeyCode.M)) MKeySelected?.Invoke();
    }

    private void UpdateMouseInput()
    {
        Vector3 prev = _mousePos;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        xy.Raycast(ray, out distance);
        _mousePos = ray.GetPoint(distance);

        if ((_mousePos - prev).magnitude > _mousePosSensitivity)
        {
            //MousePositionMoved?.Invoke();
        }
    }

    private void UpdateLookDirection()
    {
        Vector2 prev = _lookDir_Commanded;

        Vector2 test = (_mousePos - (Vector3)_playerTransform.position).normalized;
        
        if (Vector2.Dot(prev, test) < _lookDirChangeThreshold)
        {
            _lookDir_Commanded = test;
        }

        _lookDir_Driven = Vector3.RotateTowards(_lookDir_Driven, _lookDir_Commanded,
            _lookDirChangeSpeed * Time.unscaledDeltaTime,
            0);
        _lookDir_Driven = _lookDir_Driven.normalized;
        _lookAngle = Vector3.SignedAngle(Vector3.up, _lookDir_Driven, Vector3.forward);
        LookDirChanged?.Invoke(_lookDir_Driven, _lookAngle);


    }

    public void OnDiageticClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LeftMouseChanged?.Invoke(true);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RightMouseChanged?.Invoke(true);
        }
    }

    public void OnDiageticRelease()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LeftMouseChanged?.Invoke(false);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            RightMouseChanged?.Invoke(false);
        }
    }


}
