using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoProjectile : Projectile
{
    SpriteRenderer _sr;
    IMissileLauncher _torpedoLauncher;

    //settings
    [SerializeField] Sprite _lockedOnSprite;

    //Full turn authority is reduced inside this cone
    private const float _turnDampingCoefficient = 10f;

    float _turnRate;
    float _speed;
    float _timeBetweenTargetScans = 0.1f;
    float _scanOriginOffset_near = 4.0f; //full radius
    float _scanOriginOffset_far = 10f; //4x radius
    float _scanRadius;
    int _legalTarget_LayerMask;
    float _closeEnough = 0.3f;

    float _boresightTolerance = 0.02f;

    //state
    Sprite _startingSprite;
    [SerializeField] Transform _targetTransform;
    float _angleToTarget;
    float _timeForNextTargetScan;


    public override void Initialize(ProjectilePoolController poolController)
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

        _torpedoLauncher = _launchingWeaponHandler.GetComponent<IMissileLauncher>();

        _targetTransform = _torpedoLauncher.GetTargetTransform();
        _speed = _torpedoLauncher.GetSpeedSpec();
        _turnRate = _torpedoLauncher.GetTurnSpec();
        _scanRadius = _torpedoLauncher.GetMissileScanRadius();

        _targetTransform = null; // in case this is a pool object that otherwise retains old target

        _legalTarget_LayerMask = _torpedoLauncher.GetLegalTargetsLayerMask();
    }

    protected override void ExecuteUpdateSpecifics()
    {
        if (!_targetTransform && Time.time >= _timeForNextTargetScan)
        {
            _targetTransform = _torpedoLauncher.GetTargetTransform();
            if (_targetTransform)
            {
                _sr.sprite = _lockedOnSprite;
            }
            else
            {
                _sr.sprite = _startingSprite;

            }
            _timeForNextTargetScan = Time.time + _timeBetweenTargetScans;
        }

        UpdateSteering();
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
                _scanRadius * 3f, _legalTarget_LayerMask);

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
        _sr.sprite = _lockedOnSprite;
    }

    private void UpdateSteering()
    {
        if (_targetTransform)
        {
            _angleToTarget = Vector3.SignedAngle((_targetTransform.position - transform.position),
    transform.up, transform.forward);
        }
        else
        {
            _angleToTarget = 0;
        }

    }

    protected override void ExecuteMovement()
    {
        float angleWithTurnDamper = Mathf.Clamp(_angleToTarget, -_turnDampingCoefficient, _turnDampingCoefficient);
        float currentTurnRate = Mathf.Clamp(-_turnRate * angleWithTurnDamper / _turnDampingCoefficient, -_turnRate, _turnRate);
                
        if (angleWithTurnDamper > _boresightTolerance || angleWithTurnDamper < -_boresightTolerance)
        {
            //Target outside of acceptable boresight
            _rb.angularVelocity = currentTurnRate;
        }

        _rb.velocity = _speed * transform.up;
    }

    protected override void ExecuteLifetimeExpirationSequence()
    {
        ExecuteGenericExpiration_Fizzle();
    }
}
