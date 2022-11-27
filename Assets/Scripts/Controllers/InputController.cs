using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;


public class InputController : MonoBehaviour
{
    PlayerInput _playerInput;

    public Action<Vector2> DesiredTranslateChanged;
    public Action AccelStarted;
    public Action AccelEnded;
    public Action DecelStarted;
    public Action DecelEnded;
    public Action<bool> TurnLeftChanged;
    public Action<bool> TurnRightChanged;
    public Action<float> StrafeCommanded;

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
    [SerializeField] float _lookDirChangeSpeed = 4.5f;
    [SerializeField] float _moveSensitivity = 0.2f;

    //state
    Vector2 _mousePos;
    private Vector2 _lookDir_Commanded = Vector2.one;
    private Vector2 _lookDir_Driven = Vector2.one;
    /// <summary>
    /// Normalize Vector2 of look direction, based on right stick or player-to-mouse dir
    /// </summary>
    public Vector2 LookDirection => _lookDir_Driven;
    float _lookAngle = 0;
    public float LookAngle => _lookAngle;
    //public bool IsTranslationalMovementMode = false;
    Vector2 _desiredTranslation = Vector2.zero;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        GetComponent<GameController>().PlayerSpawned += HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(GameObject newPlayer)
    {
        _playerTransform = newPlayer.transform;
    }

    private void Update()
    {
        //if (IsTranslationalMovementMode == true) UpdateKeyboardInput_Translation();
        //else UpdateKeyboardInput_Rotation();

        //UpdateMouseInput();
        //UpdateMouseScrollInput();
        //if (_playerTransform) UpdateLookDirection();
        ////UpdateMouseFiringInput();


        UpdateActualLookDirectionToCommandedDirection();
    }

    private void UpdateActualLookDirectionToCommandedDirection()
    {
        _lookDir_Driven = Vector3.RotateTowards(_lookDir_Driven, _lookDir_Commanded,
            _lookDirChangeSpeed * Time.unscaledDeltaTime,
            0);
        _lookDir_Driven = _lookDir_Driven.normalized;
        _lookAngle = Vector3.SignedAngle(Vector3.up, _lookDir_Driven, Vector3.forward);
        LookDirChanged?.Invoke(_lookDir_Driven, _lookAngle);
    }

    void OnFirePrimary(InputValue value)
    {
        LeftMouseChanged?.Invoke(value.isPressed);
    }

    void OnFireSecondary(InputValue value)
    {
        RightMouseChanged?.Invoke(value.isPressed);
        
    }

    void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        if (move.y > _moveSensitivity)
        {
            AccelStarted?.Invoke();
            DecelEnded?.Invoke();
        }
        if (move.y <= _moveSensitivity)
        {
            AccelEnded?.Invoke();
        }
        if (move.y < -_moveSensitivity)
        {
            DecelStarted?.Invoke();
        }

        if (Mathf.Abs(move.x) > _moveSensitivity)
        {
            StrafeCommanded?.Invoke(move.x);
            //TurnLeftChanged?.Invoke(true);
            //TurnRightChanged?.Invoke(false);
        }
        else
        {
            StrafeCommanded?.Invoke(0);
        }

    }

    void OnLook(InputValue value)
    {
        Vector2 prev = _lookDir_Commanded;
        Vector2 look = value.Get<Vector2>();

        if (_playerInput.currentControlScheme == ("Keyboard&Mouse") && _playerTransform)
        {
            ray = Camera.main.ScreenPointToRay(look);
            xy.Raycast(ray, out distance);
            _mousePos = ray.GetPoint(distance);
            _lookDir_Commanded = (_mousePos - (Vector2)_playerTransform.position).normalized;
        }
        else if (_playerInput.currentControlScheme == ("Gamepad"))
        {
            _lookDir_Commanded = look;
            //if (look.magnitude < Mathf.Epsilon)
            //{
            //    _lookDir_Commanded = prev;
            //}
            //else
            //{
            //    _lookDir_Commanded = look;
            //}
        }
        else
        {
            _lookDir_Commanded = Vector2.up;
        }
    }

    void OnScrollSecondary(InputValue value)
    {
        float scrollDir = value.Get<float>();

        if (scrollDir > Mathf.Epsilon)
        {
            ScrollWheelChanged?.Invoke(1);
            return;
        }
        if (scrollDir < -Mathf.Epsilon)
        {
            ScrollWheelChanged?.Invoke(-1);
            return;
        }
    }

    private void OnToggleUpgradeMenu(InputValue value)
    {
        UpgradeMenuToggled?.Invoke();
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



   

    //private void UpdateLookDirection()
    //{
    //    Vector2 prev = _lookDir_Commanded;
    //    Vector2 test = (_mousePos - (Vector2)_playerTransform.position).normalized;        
    //    if (Vector2.Dot(prev, test) < _lookDirChangeThreshold)
    //    {
    //        _lookDir_Commanded = test;
    //    }

    //    _lookDir_Driven = Vector3.RotateTowards(_lookDir_Driven, _lookDir_Commanded,
    //        _lookDirChangeSpeed * Time.unscaledDeltaTime,
    //        0);
    //    _lookDir_Driven = _lookDir_Driven.normalized;
    //    _lookAngle = Vector3.SignedAngle(Vector3.up, _lookDir_Driven, Vector3.forward);
    //    LookDirChanged?.Invoke(_lookDir_Driven, _lookAngle);
    //}

    public void OnDiageticClick()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    LeftMouseChanged?.Invoke(true);
        //}
        //if (Input.GetKeyDown(KeyCode.Mouse1))
        //{
        //    RightMouseChanged?.Invoke(true);
        //}
    }

    public void OnDiageticRelease()
    {
        //if (Input.GetKeyUp(KeyCode.Mouse0))
        //{
        //    LeftMouseChanged?.Invoke(false);
        //}
        //if (Input.GetKeyUp(KeyCode.Mouse1))
        //{
        //    RightMouseChanged?.Invoke(false);
        //}
    }


}
