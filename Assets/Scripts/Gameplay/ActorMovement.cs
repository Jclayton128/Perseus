using Sirenix.OdinInspector;
using System;
using UnityEngine;
using DG.Tweening;

public class ActorMovement : MonoBehaviour
{
    InputController _inputCon;
    HealthHandler _healthHandler;
    EnergyHandler _hostEnergyHandler;
    Rigidbody2D _rb;
    [SerializeField] ParticleSystem[] _engineParticles = null;
    [SerializeField] ParticleSystem[] _strafeParticles_exhaustingRight = null;
    [SerializeField] ParticleSystem[] _strafeParticles_exhaustingLeft = null;
    RadarProfileHandler _radarProfileHandler;
    MindsetHandler _mindsetHandler;

    //settings
    float _turningForce = 300f;

    //state
    public bool IsPlayer = false;

    [ShowIf("IsPlayer")]
    public bool IsMouseSteering = false;

    public bool ShouldAccelerate;
    public bool ShouldDecelerate;
    public bool ShouldTurnLeft;
    public bool ShouldTurnRight;

    [HideIf("IsPlayer")]
    [SerializeField] float _accelClosureDecision = 10.0f;
    public float TopSpeed_Chosen => _accelClosureDecision;

    [HideIf("IsPlayer")]
    [SerializeField] float _minAngleOffDesiredSteeringToAccel = 10.0f;

    [SerializeField] float _thrust;
    [SerializeField] float _turnRate;
    float _turnRateMemory;

    [Tooltip("Negative drag rate defaults to thrust/100")]
    [SerializeField] float _decelDragRate = -1f;
    [SerializeField] float _normalDragRate = 0f;
    float _manualDragCoeff = 0.0025f;

    /// <summary>
    /// This much Profile is added to an actor's profile every second while thrusting.
    /// </summary>
    [SerializeField] float _thrustProfileIncreaseRate = 15f;

    /// <summary>
    /// This is how much energy gets drained per second of thrusting. Normally zero;
    /// </summary>
    [SerializeField] float _thrustEnergyCostRate = 0;

    //state
    float _angleOffComputedSteering;
    Vector2 _desiredSteering;
    Vector2 _computedSteering;
    float _closure;
    Vector2 _closureDir;
    Vector2 _driftDir;
    [SerializeField] float _strafeCommanded = 0;
    Vector2 _closureVec;
    Vector2 _driftVec;
    float _speed_Current = 0;
    float _manualDragMagnitude = 0;
    Vector2 _manualDragVector = Vector2.zero;
    public float BoostMultiplier = 1;

    /// <summary>
    /// This catches effects of both ionization and BoostMultiplier in single variable 
    /// that is referenced by both turning and accel
    /// </summary>
    float _performanceFactor = 1;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hostEnergyHandler = GetComponent<EnergyHandler>();
        _healthHandler = GetComponent<HealthHandler>();
        if (IsPlayer)
        {
            _inputCon = FindObjectOfType<InputController>();
            _inputCon.AccelStarted += HandleBeginAccelerating;
            _inputCon.AccelEnded += HandleStopAccelerating;
            _inputCon.DecelStarted += HandleBeginDecelerating;
            _inputCon.DecelEnded += HandleStopDecelerating;
            _inputCon.TurnLeftChanged += HandleTurningLeft;
            _inputCon.TurnRightChanged += HandleTurningRight;
            _inputCon.MKeySelected += HandleTurnModeToggle;
            _inputCon.StrafeCommanded += HandleStrafeCommanded;

            _radarProfileHandler = GetComponentInChildren<RadarProfileHandler>();
            
        }
        else
        {
            _mindsetHandler = GetComponent <MindsetHandler>();
        }
        _turnRateMemory = _turnRate;
    }


    #region Flow

    private void Update()
    {
        if (IsPlayer)
        {
            //UpdateSteering();
            //if (IsMouseSteering) ConverMouseIntoDesiredSteering();
            //else 
            _computedSteering = _inputCon.LookDirection;
        }
        else
        {
            //Debug.DrawLine(transform.position,
            //   _rb.velocity + (Vector2)transform.position,
            //   Color.green, 0.1f);

            _closureDir = _desiredSteering.normalized;
            _driftDir = Vector2.Perpendicular(_closureDir).normalized;

            _closureVec = Vector3.Project(_rb.velocity, _closureDir);
            _driftVec = Vector3.Project(_rb.velocity, _driftDir);

            Debug.DrawLine(transform.position,
                 _closureVec + (Vector2)transform.position,
                Color.yellow, 0.1f);

            Debug.DrawLine(transform.position,
                 _driftVec + (Vector2)transform.position,
                Color.blue, 0.1f);

            _desiredSteering =
                   (_mindsetHandler.TargetPosition - (Vector2)transform.position);

            if (!_mindsetHandler.ShouldLeadTargetPos)
            {
                _computedSteering = _desiredSteering - _driftVec;

                Debug.DrawLine((Vector2)transform.position,
                    (Vector2)transform.position + _computedSteering,
                    Color.gray, .1f);
            }
            else
            {
                _computedSteering = _desiredSteering +
                    _mindsetHandler.PlayerVelocity - _driftVec;// - _rb.velocity;

                Debug.DrawLine(transform.position,
                    (Vector2)transform.position + _desiredSteering,
                    Color.magenta, 0.1f);
            };

            _angleOffComputedSteering =
                Vector3.SignedAngle(transform.up, _computedSteering, Vector3.forward);
            UpdateAccelDecelDecision();
        }
    }


    //private void UpdateLookDirIntoDesiredSteering()
    //{
    //    _desiredSteering = (Vector2)transform.position + _inputCon.LookDirection;
    //}

    //private void UpdateSteering()
    //{
    //    if (ShouldTurnLeft)
    //    {
    //        _desiredSteering = Vector3.RotateTowards(_desiredSteering, -transform.right,
    //            _turnRate*Mathf.Deg2Rad * Time.deltaTime, 180f);
    //        return;
    //    }
    //    if (ShouldTurnRight)
    //    {
    //        _desiredSteering = Vector3.RotateTowards(_desiredSteering, transform.right,
    //            _turnRate * Mathf.Deg2Rad * Time.deltaTime, 180f);
    //        return;
    //    }

    //}


    private void UpdateAccelDecelDecision()
    {
        float dist = (_mindsetHandler.TargetPosition - (Vector2)transform.position).magnitude;
        _closure = _closureVec.magnitude;

        if (_hostEnergyHandler.CheckEnergy(_thrustEnergyCostRate) == false)
        {
            //_hostEnergyHandler.SpendEnergy(_thrustEnergyCostRate + 1f);
            HandleStopAccelerating();
            HandleBeginDecelerating();
            return;
        }

        if (dist < _mindsetHandler.StandoffRange || _closure > _accelClosureDecision)
        {
            HandleStopAccelerating();
            HandleBeginDecelerating();
        }
        else if (_angleOffComputedSteering < _minAngleOffDesiredSteeringToAccel
            && _closure < _accelClosureDecision)
        {
            HandleBeginAccelerating();
            HandleStopDecelerating();
        }
        else
        {
            HandleStopAccelerating();
            HandleStopDecelerating();
        }
    }


    private void FixedUpdate()
    {
        _performanceFactor = Mathf.Lerp(1, 0.5f, _healthHandler.IonFactor) * BoostMultiplier;
        UpdateAccelDecel();
        UpdateTurning();
        UpdateManualDrag();
        UpdateStrafe();
    }

    private void UpdateStrafe()
    {
        if (Mathf.Abs(_strafeCommanded) > Mathf.Epsilon)
        {
            _rb.AddForce((Vector2)transform.right *
                 _strafeCommanded * _performanceFactor * _thrust/2f *
                 Time.fixedDeltaTime);
        }

    }

    private void UpdateManualDrag()
    {
        _speed_Current = _rb.velocity.magnitude;
        _manualDragMagnitude = (_speed_Current * _speed_Current) * _manualDragCoeff;
        _manualDragVector = _manualDragMagnitude * -_rb.velocity.normalized;
        _rb.velocity += _manualDragVector;
    }

    private void UpdateAccelDecel()
    {
        if (ShouldAccelerate)
        {
            if (_hostEnergyHandler.CheckEnergy(_thrustEnergyCostRate))
            {             
                if (IsPlayer)
                {
                    _rb.AddForce(transform.up *
                        (_thrust * _performanceFactor)
                        * Time.fixedDeltaTime);
                    _radarProfileHandler.AddToCurrentRadarProfile(Time.fixedDeltaTime * _thrustProfileIncreaseRate);
                    _hostEnergyHandler.SpendEnergy(_thrustEnergyCostRate * Time.fixedDeltaTime);
                }
                else
                {
                    _rb.AddForce(transform.up *
                        (_thrust * _performanceFactor) *
                        Time.fixedDeltaTime);
                    _radarProfileHandler?.AddToCurrentRadarProfile(
                        Time.fixedDeltaTime * _thrustProfileIncreaseRate);
                    _hostEnergyHandler?.SpendEnergy(
                        _thrustEnergyCostRate * Time.fixedDeltaTime);
                }
            }

        }
        if (ShouldDecelerate)
        {
            _rb.drag = (_decelDragRate < 0) ? (_thrust / 100f) : _decelDragRate ;
        }
        if (!ShouldDecelerate)
        {
            _rb.drag = _normalDragRate;
        }
    }

    private void UpdateTurning()
    {
        _angleOffComputedSteering =
                Vector3.SignedAngle(_computedSteering, transform.up, transform.forward);
        float angleWithTurnDamper = Mathf.Clamp(_angleOffComputedSteering, -10f, 10f);
        float currentTurnRate = Mathf.Clamp(-_turnRate * angleWithTurnDamper / 10f,
            -_turnRate * _performanceFactor,
            _turnRate * _performanceFactor);
        if (_angleOffComputedSteering > 0.02)
        {
            _rb.angularVelocity = Mathf.Lerp(_rb.angularVelocity, currentTurnRate, _turningForce * Time.deltaTime);
        }
        if (_angleOffComputedSteering < -0.02)
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

    private void HandleTurningLeft(bool isStartingToTurnLeft)
    {
        ShouldTurnLeft = isStartingToTurnLeft;
        if (isStartingToTurnLeft) ShouldTurnRight = false;

    }

    private void HandleTurningRight(bool isStartingToTurnRight)
    {
        ShouldTurnRight = isStartingToTurnRight;
        if (isStartingToTurnRight) ShouldTurnLeft = false;
    }

    private void HandleTurnModeToggle()
    {
        IsMouseSteering = !IsMouseSteering;
    }

    private void HandleStrafeCommanded(float strafeCommanded)
    {
        _strafeCommanded = strafeCommanded;
        if (_strafeCommanded > Mathf.Epsilon)
        {
            foreach (var particle in _strafeParticles_exhaustingLeft)
            {
                particle.Play();
            }
            foreach (var particle in _strafeParticles_exhaustingRight)
            {
                particle.Stop();
            }
        }
        else if (_strafeCommanded < -Mathf.Epsilon)
        {
            foreach (var particle in _strafeParticles_exhaustingLeft)
            {
                particle.Stop();
            }
            foreach (var particle in _strafeParticles_exhaustingRight)
            {
                particle.Play();
            }
        }
        else
        {
            foreach (var particle in _strafeParticles_exhaustingLeft)
            {
                particle.Stop();
            }
            foreach (var particle in _strafeParticles_exhaustingRight)
            {
                particle.Stop();
            }
        }
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
        _turnRateMemory += amountToAdd;
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

    public void JumpToWarpGate(float signedAngle)
    {
        transform.rotation = Quaternion.Euler(0, 0, signedAngle);
        _turnRate = 0;
        DOTween.To(() => _turnRate, x => _turnRate = x, _turnRateMemory, 1f).SetEase(Ease.InExpo);
    }
    private void OnDestroy()
    {
        if (!_inputCon) _inputCon = FindObjectOfType<InputController>();

        if (_inputCon)
        {
            _inputCon.AccelStarted -= HandleBeginAccelerating;
            _inputCon.AccelEnded -= HandleStopAccelerating;
            _inputCon.DecelStarted -= HandleBeginDecelerating;
            _inputCon.DecelEnded -= HandleStopDecelerating;
            _inputCon.TurnLeftChanged -= HandleTurningLeft;
            _inputCon.TurnRightChanged -= HandleTurningRight;
            _inputCon.MKeySelected -= HandleTurnModeToggle;
            _inputCon.StrafeCommanded -= HandleStrafeCommanded;
        }

    }
}
