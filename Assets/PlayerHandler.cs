using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    InputController _inputCon;
    Rigidbody2D _rb;
    PlayerSystemHandler _playerSystemHandler;

    //settings
    float _turningForce = 300f;

    //state
    bool _isAccelerating;
    bool _isDecelerating;
    [SerializeField] float _thrust;
    [SerializeField] float _mass;
    [SerializeField] float _turnRate;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerSystemHandler = GetComponent<PlayerSystemHandler>();

        _inputCon = FindObjectOfType<InputController>();
        _inputCon.OnAccelBegin += HandleBeginAccelerating;
        _inputCon.OnAccelEnd += HandleStopAccelerating;
        _inputCon.OnDecelBegin += HandleBeginDecelerating;
        _inputCon.OnDecelEnd += HandleStopDecelerating;

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
            _rb.AddForce(transform.up * (_thrust) * Time.fixedDeltaTime);
        }
    }

    private void UpdateMouseTurning()
    {
        Vector2 mouseDir = _inputCon.MousePos - transform.position;
        float angleToTarget = Vector3.SignedAngle(mouseDir, transform.up, transform.forward);
        float angleWithTurnDamper = Mathf.Clamp(angleToTarget, -10, 10);
        float currentTurnRate = Mathf.Clamp(-_turnRate * angleWithTurnDamper / 10, -_turnRate, _turnRate);
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

        _rb.drag = _thrust/_mass/50f;
    }

    private void HandleStopDecelerating()
    {
        _isDecelerating = false;
        _rb.drag = 0;
    }

    #endregion

    #region Modify Specs
    public void ModifyThrust(float amountToAdd)
    {
        _thrust += amountToAdd;
    }

    public void ModifyMass(float amountToAdd)
    {
        _mass += amountToAdd;
        _rb.mass = _mass;
    }

    public void ModifyTurnRate(float amountToAdd)
    {
        _turnRate += amountToAdd;
    }

    #endregion
}
