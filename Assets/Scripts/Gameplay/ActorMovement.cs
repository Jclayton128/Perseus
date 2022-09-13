using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement : MonoBehaviour
{
    InputController _inputCon;
    EnergyHandler _hostEnergyHandler;
    Rigidbody2D _rb;
    [SerializeField] ParticleSystem[] _engineParticles = null;
    RadarProfileHandler _radarProfileHandler;

    //settings
    float _turningForce = 300f;

    //state
    public bool IsPlayer = false;

    public bool ShouldAccelerate;
    public bool ShouldDecelerate;
    public Vector3 DesiredSteering;

    [SerializeField] float _thrust;
    [SerializeField] float _turnRate;

    /// <summary>
    /// This much Profile is added to an actor's profile every second while thrusting.
    /// </summary>
    float _thrustProfileIncreaseRate = 15f;

    /// <summary>
    /// This is how much energy gets drained per second of thrusting. Normally zero;
    /// </summary>
    float _thrustEnergyCostRate = 0; 


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hostEnergyHandler = GetComponent<EnergyHandler>();
        if (IsPlayer)
        {
            _inputCon = FindObjectOfType<InputController>();
            _inputCon.OnAccelBegin += HandleBeginAccelerating;
            _inputCon.OnAccelEnd += HandleStopAccelerating;
            _inputCon.OnDecelBegin += HandleBeginDecelerating;
            _inputCon.OnDecelEnd += HandleStopDecelerating;
            _radarProfileHandler = GetComponentInChildren<RadarProfileHandler>();
            
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
            if (_hostEnergyHandler.CheckEnergy(_thrustEnergyCostRate))
            {
                _rb.AddForce(transform.up * (_thrust) * Time.fixedDeltaTime);
                _radarProfileHandler.AddToCurrentRadarProfile(Time.fixedDeltaTime * _thrustProfileIncreaseRate);
                _hostEnergyHandler.SpendEnergy(_thrustEnergyCostRate * Time.fixedDeltaTime);
            }

        }
        if (ShouldDecelerate)
        {
            _rb.drag = _thrust / 100f;
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

    //public void ModifyMass(float amountToAdd)
    //{
    //    _mass += amountToAdd;
    //    _rb.mass = _mass;
    //}

    public void ModifyTurnRate(float amountToAdd)
    {
        _turnRate += amountToAdd;
    }

    public void SetThrustEnergyCost(float newThrustCost)
    {
        _thrustEnergyCostRate = newThrustCost;
    }

    public void ModifyThrustEnergyCost(float amountToAdd)
    {
        _thrustEnergyCostRate += amountToAdd;
    }

    public Color SwapParticleColor(Color newColor)
    {
        Color oldColor = Color.red;
        foreach (var ep in _engineParticles)
        {
            ParticleSystem.MainModule main = ep.main;
            oldColor = main.startColor.color;
            main.startColor = newColor;
        }
        return oldColor;
    }

    public void ModifyThrustProfileIncreaseRate(float amountToAdd)
    {
        _thrustProfileIncreaseRate += amountToAdd;
    }

    #endregion
}
