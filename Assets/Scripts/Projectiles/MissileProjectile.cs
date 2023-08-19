using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileProjectile : Projectile, IProximityFuzed
{
    SpriteRenderer _sr;
    ParticleController _particleController;

    //settings
    [SerializeField] Sprite _lockedOnSprite;

    //Full turn authority is reduced inside this cone
    private const float _turnDampingCoefficient = 10f;

    float _snakeAmount;
    float _turnRate;
    float _speed;

    float _timeBetweenTargetScans = 0.02f;
    float _scanOriginOffset_near = 4.0f; //full radius
    float _scanOriginOffset_far = 10f; //4x radius
    float _scanRadius;
    int _legalTarget_LayerMask;
    float _closeEnough = 0.3f;

    float _boresightTolerance = 0.02f;

    //state
    Sprite _startingSprite;
    Vector3 _targetPosition;
    Transform _targetTransform;
    float _angleToTarget;
    float _timeForNextTargetScan;
    bool _hasHitTargetPosition = false;

    public override void Initialize(ProjectilePoolController poolController)
    {
        base.Initialize(poolController);
        _sr = GetComponent<SpriteRenderer>();
        _startingSprite = _sr.sprite;
        _particleController = FindObjectOfType<ParticleController>();
    }

    protected override void SetupInstanceSpecifics()
    {
        _sr.sprite = _startingSprite;

        _rb.velocity =
            _launchingWeaponHandler.GetInitialProjectileVelocity(transform);

        IMissileLauncher sml = _launchingWeaponHandler.GetComponent<IMissileLauncher>();

        if (sml != null)
        {
            _targetPosition = sml.GetTargetPosition();
            //_thrust = sml.GetThrustSpec();
            _speed = sml.GetSpeedSpec();
            _turnRate = sml.GetTurnSpec();
            _scanRadius = sml.GetMissileScanRadius();
            _snakeAmount = sml.GetSnakeAmount();
            _legalTarget_LayerMask = sml.GetLegalTargetsLayerMask();
        }
        else
        {
            //magic numbers here
            _targetPosition = transform.position + (transform.forward * 30f);
            _turnRate = 200f;
            _snakeAmount = 20f;
            _scanRadius = 0.3f;
            _legalTarget_LayerMask = LayerLibrary.EnemyLayerMask;
            _speed = 3f;
        }

        _targetTransform = null; // in case this is a pool object that otherwise retains old target
        _hasHitTargetPosition = false;


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
                
                //Debug.Log("close enough!");
            }

        }
        
        if (!_hasHitTargetPosition)
        {
            UpdateSteering();
        }
    }

    private void UpdateScanTargetTransform()
    {
        //Debug.Log($"Scanning for {_legalTarget_LayerMask}. PLM: {LayerLibrary.PlayerLayerMask}" );
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
        _particleController.
            RequestBlastParticles(Mathf.RoundToInt(DamagePack.NormalDamage),
            DamagePack.NormalDamage,
            transform.position);
        ExecuteGenericExpiration_Explode(DamagePack.NormalDamage, LayerLibrary.PlayerEnemyNeutralLayerMask);
    }

    public void DetonateViaProximityFuze()
    {
        ExecuteLifetimeExpirationSequence();
    }
}
