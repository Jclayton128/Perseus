using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement_Translational : MonoBehaviour
{
    InputController _inputCon;
    EnergyHandler _hostEnergyHandler;
    Rigidbody2D _rb;
    [SerializeField] ParticleSystem[] _engineParticles = null;
    RadarProfileHandler _radarProfileHandler;

    [SerializeField] float _thrust = 300;
    [SerializeField] float _turnRate = 180;

    /// <summary>
    /// This much Profile is added to an actor's profile every second while thrusting.
    /// </summary>
    float _thrustProfileIncreaseRate = 15f;

    /// <summary>
    /// This is how much energy gets drained per second of thrusting. Normally zero;
    /// </summary>
    float _thrustEnergyCostRate = 0;
    public bool IsPlayer = true;
    //translate experiment
    [Header("Translational Movement")]
    [SerializeField] bool _isCommandedToTranslate;
    [SerializeField] Vector2 _commandedVector = Vector2.zero;
    [SerializeField] float maxAngleOffBoresightToDrive = 10f;
    [SerializeField] float angleOffCommandedVector;
    public bool ShouldAccelerate;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hostEnergyHandler = GetComponent<EnergyHandler>();
        _inputCon = FindObjectOfType<InputController>();
        _inputCon.DesiredTranslateChanged += HandleTranslateChange;
    }

    private void HandleTranslateChange(Vector2 translationVector)
    {
        if (translationVector.magnitude > Mathf.Epsilon) _isCommandedToTranslate = true;
        else _isCommandedToTranslate = false;
        _commandedVector = translationVector;
    }

    private void FixedUpdate()
    {

        UpdateTranslationBasedRotation();
        UpdateTranslationalMovement();
        UpdateAccelDecel();
    }

    private void UpdateTranslationBasedRotation()
    {
        angleOffCommandedVector = Vector3.SignedAngle(transform.up, _commandedVector, Vector3.forward);

        if (!_isCommandedToTranslate)
        {
            _rb.angularVelocity = 0;
            return;
        }
        if (angleOffCommandedVector > -0.1f)
        {
            _rb.angularVelocity = _turnRate;
        }
        if (angleOffCommandedVector < 0.1f)
        {
            _rb.angularVelocity = -_turnRate;
        }
    }

    private void UpdateTranslationalMovement()
    {

        if (!_isCommandedToTranslate || Mathf.Abs(angleOffCommandedVector) > maxAngleOffBoresightToDrive)
        {
            //_rb.velocity = Vector2.Lerp(_rb.velocity, Vector2.zero, Time.deltaTime * 3);
        }
        if (_isCommandedToTranslate)
        {
            //if (Mathf.Abs(angleOffCommandedVector) < maxAngleOffBoresightToDrive * 2)
            //{
            //    ShouldAccelerate = true;
            //    //_rb.AddForce(transform.up * (_thrust / 2f) * Time.fixedDeltaTime);
            //}
            if (Mathf.Abs(angleOffCommandedVector) < maxAngleOffBoresightToDrive)
            {
                HandleBeginAccelerating();
                //_rb.AddForce(transform.up * _thrust * Time.fixedDeltaTime);
            }
            else
            {
                HandleStopAccelerating();
            }
        }
    }
    private void UpdateAccelDecel()
    {
        if (ShouldAccelerate)
        {
            if (_hostEnergyHandler.CheckEnergy(_thrustEnergyCostRate))
            {
                _rb.AddForce(transform.up * (_thrust) * Time.fixedDeltaTime);
                _radarProfileHandler?.AddToCurrentRadarProfile(Time.fixedDeltaTime * _thrustProfileIncreaseRate);
                _hostEnergyHandler.SpendEnergy(_thrustEnergyCostRate * Time.fixedDeltaTime);
            }

        }

    }

    private void HandleBeginAccelerating()
    {
        ShouldAccelerate = true;
        foreach (var particle in _engineParticles)
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

}
