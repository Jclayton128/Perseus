using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement : MonoBehaviour
{
    InputController _inputCon;
    Rigidbody2D _rb;
    [SerializeField] ParticleSystem[] _engineParticles = null;

    //settings
    float _turningForce = 300f;

    //state
    public bool IsPlayer = false;

    public bool ShouldAccelerate;
    public bool ShouldDecelerate;
    public Vector3 DesiredSteering;

    [SerializeField] float _thrust;
    [SerializeField] float _mass;
    [SerializeField] float _turnRate;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (IsPlayer)
        {
            _inputCon = FindObjectOfType<InputController>();
            _inputCon.OnAccelBegin += HandleBeginAccelerating;
            _inputCon.OnAccelEnd += HandleStopAccelerating;
            _inputCon.OnDecelBegin += HandleBeginDecelerating;
            _inputCon.OnDecelEnd += HandleStopDecelerating;
        }
        
    }

    #region Flow

    private void Update()
    {
        if (IsPlayer)
        {
            ConverMouseIntoDesiredSteering();
        }
    }

    private void ConverMouseIntoDesiredSteering()
    {
        DesiredSteering = _inputCon.MousePos - transform.position;
    }

    private void FixedUpdate()
    {
        UpdateAccelDecel();
        UpdateMouseTurning();

    }

    private void UpdateAccelDecel()
    {
        if (ShouldAccelerate)
        {
            _rb.AddForce(transform.up * (_thrust) * Time.fixedDeltaTime);
        }
        if (ShouldDecelerate)
        {
            _rb.drag = _thrust / _mass / 50f;
        }
        if (!ShouldDecelerate)
        {
            _rb.drag = 0;
        }
    }

    private void UpdateMouseTurning()
    {
       
        float angleToTarget = Vector3.SignedAngle(DesiredSteering, transform.up, transform.forward);
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
        ShouldAccelerate = true;
        foreach(var particle in _engineParticles)
        {
            particle.Play();
        }
    }

    private void HandleStopAccelerating()
    {
        ShouldAccelerate = false;
        foreach (var particle in _engineParticles)
        {
            particle.Stop();
        }
    }

    private void HandleBeginDecelerating()
    {
        ShouldDecelerate = true;
    }

    private void HandleStopDecelerating()
    {
        ShouldDecelerate = false;
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
