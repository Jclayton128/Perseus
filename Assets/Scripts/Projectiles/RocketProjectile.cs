using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile
{

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


    protected override void SetupInstanceSpecifics()
    {
        _rb.velocity =
            _launchingWeaponHandler.GetInitialProjectileVelocity(transform);

        IMissileLauncher sml = _launchingWeaponHandler.GetComponent<IMissileLauncher>();

        _targetPosition = sml.GetTargetPosition();
        //_thrust = sml.GetThrustSpec();
        _speed = sml.GetSpeedSpec();
        _turnRate = sml.GetTurnSpec();
        _snakeAmount = sml.GetSnakeAmount();
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
        ExecuteGenericExpiration_Fizzle();
    }
}
