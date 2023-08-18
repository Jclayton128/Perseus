using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile, IProximityFuzed
{
    ParticleController _particleController;

    //settings
    //Full turn authority is reduced inside this cone
    private const float _turnDampingCoefficient = 10f;
    float _snakeAmount;
    float _turnRate;
    float _speed;
    float _closeEnough = 0.3f;
    float _boresightTolerance = 0.02f;

    //state
    Vector3 _targetPosition;
    float _angleToTarget;
    bool _hasHitTargetPosition = false;

    public override void Initialize(ProjectilePoolController poolController)
    {
        base.Initialize(poolController);
        _particleController = poolController.GetComponent<ParticleController>();
    }

    protected override void SetupInstanceSpecifics()
    {
        _rb.velocity =
            _launchingWeaponHandler.GetInitialProjectileVelocity(transform);

        IMissileLauncher sml = _launchingWeaponHandler.GetComponent<IMissileLauncher>();
        
        if (sml != null)
        {
            _targetPosition = sml.GetTargetPosition();
            _turnRate = sml.GetTurnSpec();
            _snakeAmount = sml.GetSnakeAmount();
        } 
        else
        {
            //magic numbers here
            _targetPosition = transform.position + (transform.forward * 30f);
            _turnRate = 90f;
            _snakeAmount = 20f;
        }

        //_thrust = sml.GetThrustSpec();
        _speed = _rb.velocity.magnitude;

        _hasHitTargetPosition = false;

        if (_snakeAmount > Mathf.Epsilon)
        {
            _rb.angularVelocity = _turnRate;
        }
    }

    protected override void ExecuteUpdateSpecifics()
    {
        if (!_hasHitTargetPosition)
        {
            UpdateSteering();
        }
    }    

    private void UpdateSteering()
    {
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
        ExecuteGenericExpiration_Explode(DamagePack.NormalDamage,
            LayerLibrary.PlayerEnemyNeutralLayerMask);
    }

    public void DetonateViaProximityFuze()
    {
        ExecuteLifetimeExpirationSequence();
    }
}
