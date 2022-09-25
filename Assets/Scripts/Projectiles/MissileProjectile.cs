using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileProjectile : Projectile
{
    SpriteRenderer _sr;

    //settings
    [SerializeField] Sprite _lockedOnSprite;
    //Full turn authority is reduced inside this cone
    private const float _turnDampingCoefficient = 10f;

    [Tooltip("This determines how snaky a missile is; ie, how much overshoot it has in degrees")]
    float _snakeAmount;
    float _turnRate;
    float _thrust;
    float _speed;

    float _timeBetweenTargetScans = 0.1f;
    float _scanOriginOffset_near = 4.0f; //full radius
    float _scanOriginOffset_far = 10f; //4x radius
    float _scanRadius;
    int _legalTarget_LayerMask;
    [SerializeField] float _closeEnough = 0.5f;

    float _boresightTolerance = 0.02f;

    //state
    Sprite _startingSprite;
    [SerializeField] Vector3 _targetPosition;
    [SerializeField] Transform _targetTransform;
    [SerializeField] float _angleToTarget;
    float _timeForNextTargetScan;
    [SerializeField] bool _hasHitTargetPosition = false;

    public override void Initialize(PoolController poolController)
    {
        base.Initialize(poolController);
        _sr = GetComponent<SpriteRenderer>();
        _startingSprite = _sr.sprite;
    }

    protected override void SetupInstanceSpecifics()
    {
        _sr.sprite = _startingSprite;

        _rb.velocity =
            _launchingWeaponHandler.GetInitialProjectileVelocity(transform);

        IMissileLauncher sml = _launchingWeaponHandler.GetComponent<IMissileLauncher>();

        _targetPosition = sml.GetTargetPosition();
        //_thrust = sml.GetThrustSpec();
        _speed = sml.GetSpeedSpec();
        _turnRate = sml.GetTurnSpec();
        _scanRadius = sml.GetMissileScanRadius();
        _snakeAmount = sml.GetSnakeAmount();
        _targetTransform = null; // in case this is a pool object that otherwise retains old target
        _hasHitTargetPosition = false;
        _legalTarget_LayerMask = sml.GetLegalTargetsLayerMask();

        if (_snakeAmount > Mathf.Epsilon)
        {
            _rb.angularVelocity = _turnRate;
        }
    }

    protected override void ExecuteUpdateSpecifics()
    {
        if (!_targetTransform && Time.time >= _timeForNextTargetScan)
        {
            UpdateScanTargetTransform();
            _timeForNextTargetScan = Time.time + _timeBetweenTargetScans;

            if ((_targetPosition - transform.position).sqrMagnitude <= _closeEnough)
            {
                _angleToTarget = 0;
                _hasHitTargetPosition = true;
                
                Debug.Log("close enough!");
            }

        }
        
        if (!_hasHitTargetPosition)
        {
            UpdateSteering();
        }
    }

    private void UpdateScanTargetTransform()
    {
        //look for target transform 
        Collider2D coll = Physics2D.OverlapCircle(
            transform.position + (transform.up * _scanOriginOffset_near * _scanRadius),
            _scanRadius, _legalTarget_LayerMask);

        Vector3 pos_n = transform.position + (transform.up * _scanOriginOffset_near * _scanRadius);
        Debug.DrawLine(pos_n, pos_n + Vector3.up * _scanRadius, Color.yellow, _timeBetweenTargetScans);
        Debug.DrawLine(pos_n, pos_n - Vector3.up * _scanRadius, Color.yellow, _timeBetweenTargetScans);
        Debug.DrawLine(pos_n, pos_n + Vector3.right * _scanRadius, Color.yellow, _timeBetweenTargetScans);
        Debug.DrawLine(pos_n, pos_n - Vector3.right * _scanRadius, Color.yellow, _timeBetweenTargetScans);

        if (coll)
        {
            LockOnTransform(coll);
        }
        else
        {
            //far scan is 3x the radius of near scan.
            coll = Physics2D.OverlapCircle(
                transform.position + (transform.up * _scanOriginOffset_far * _scanRadius),
                _scanRadius*3f, _legalTarget_LayerMask);

            Vector3 pos_f = transform.position + (transform.up * _scanOriginOffset_far * _scanRadius);
            Debug.DrawLine(pos_f, pos_f + Vector3.up * _scanRadius * 4f, Color.red, _timeBetweenTargetScans);
            Debug.DrawLine(pos_f, pos_f - Vector3.up * _scanRadius * 4f, Color.red, _timeBetweenTargetScans);
            Debug.DrawLine(pos_f, pos_f + Vector3.right * _scanRadius * 4f, Color.red, _timeBetweenTargetScans);
            Debug.DrawLine(pos_f, pos_f - Vector3.right * _scanRadius * 4f, Color.red, _timeBetweenTargetScans);
        }
        if (coll)
        {
            LockOnTransform(coll);
        }
    }

    private void LockOnTransform(Collider2D coll)
    {
        _targetTransform = coll.transform;
        _hasHitTargetPosition = false;
        _sr.sprite = _lockedOnSprite;
    }

    private void UpdateSteering()
    {
        if (_targetTransform)
        {
            _targetPosition = _targetTransform.position;
            _hasHitTargetPosition = false;
        }

        _angleToTarget = Vector3.SignedAngle((_targetPosition - transform.position),
            transform.up, transform.forward);
    }

    protected override void ExecuteMovement()
    {
        float angleWithTurnDamper = Mathf.Clamp(_angleToTarget, -_turnDampingCoefficient, _turnDampingCoefficient);
        float currentTurnRate = Mathf.Clamp(-_turnRate * angleWithTurnDamper / _turnDampingCoefficient, -_turnRate, _turnRate);

        if (Mathf.Abs(_angleToTarget) > _snakeAmount)
        {
            if (angleWithTurnDamper > _boresightTolerance || angleWithTurnDamper < -_boresightTolerance)
            {
                //Target outside of acceptable boresight
                _rb.angularVelocity = currentTurnRate;
            }
        }

        if (_hasHitTargetPosition)
        {
            _rb.angularVelocity = 0;
        }

        _rb.velocity = _speed * transform.up;
    }

    protected override void ExecuteLifetimeExpirationSequence()
    {
        ExecuteGenericExpiration_Fizzle();
    }

}
