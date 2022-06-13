using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour
{
    InputController _inputCon;
    PhysicsHandler _physicsHandler;
    Rigidbody2D _rb;

    //settings
    float _turningForce = 300f;

    //state
    SpecsPack _currentSpecs;
    bool _isAccelerating;
    bool _isDecelerating;

    private void Awake()
    {
        _inputCon = FindObjectOfType<InputController>();
        _inputCon.OnAccelBegin += HandleBeginAccelerating;
        _inputCon.OnAccelEnd += HandleStopAccelerating;
        _inputCon.OnDecelBegin += HandleBeginDecelerating;
        _inputCon.OnDecelEnd += HandleStopDecelerating;

        _physicsHandler = GetComponent<PhysicsHandler>();
        _physicsHandler.OnSpecsUpdate += HandleUpdatedSpecs;
        HandleUpdatedSpecs();

        _rb = GetComponent<Rigidbody2D>();

        _inputCon.GetComponent<GameController>().RegisterPlayer(this.gameObject);

        
    }

    #region Flow
    private void FixedUpdate()
    {
        UpdateAccelDecel();
        UpdateMouseTurning();
    }   

    private void UpdateAccelDecel()
    {
        if (_isAccelerating)
        {
            _rb.AddForce(transform.up * (_currentSpecs.Thrust / _currentSpecs.Mass) * Time.fixedDeltaTime);
        }
    }

    private void UpdateMouseTurning()
    {
        Vector2 mouseDir = _inputCon.MousePos - transform.position;
        float angleToTarget = Vector3.SignedAngle(mouseDir, transform.up, transform.forward);
        float angleWithTurnDamper = Mathf.Clamp(angleToTarget, -10, 10);
        float currentTurnRate = Mathf.Clamp(-_currentSpecs.TurnRate * angleWithTurnDamper / 10, -_currentSpecs.TurnRate, _currentSpecs.TurnRate);
        if (angleToTarget > 0.02)
        {
            _rb.angularVelocity = Mathf.Lerp(_rb.angularVelocity, currentTurnRate, _turningForce * Time.deltaTime);
        }
        if (angleToTarget < -0.02)
        {
            _rb.angularVelocity = Mathf.Lerp(_rb.angularVelocity, currentTurnRate, _turningForce * Time.deltaTime);
        }

        //transform.rotation = Quaternion.LookRotation()
    }

    #endregion

    #region Event Handlers

    private void HandleBeginAccelerating()
    {
        _isAccelerating = true;
        _isDecelerating = false;
    }

    private void HandleStopAccelerating()
    {
        _isAccelerating = false;
    }

    private void HandleBeginDecelerating()
    {
        _isDecelerating = true;
        _isAccelerating = false;

        _rb.drag = _currentSpecs.Thrust/_currentSpecs.Mass/50f;
    }

    private void HandleStopDecelerating()
    {
        _isDecelerating = false;
        _rb.drag = 0;
    }


    private void HandleUpdatedSpecs()
    {
        _currentSpecs = _physicsHandler.GetUpdatedSpecsPack();
    }

    #endregion
}
