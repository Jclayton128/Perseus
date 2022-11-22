using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoLauncherWH : WeaponHandler, IMissileLauncher
{
    //settings
    [Header("Base Missile Parameters")]
    [SerializeField] float _missileTurnRate = 40f;
    [SerializeField] int _startingResilience = 4;

    [Header("Upgrade Parameters")]
    [SerializeField] int _resilienceAddition_Upgrade = 2;
    [SerializeField] float _lifetimeAddition_Upgrade = 10f;

    float _maxSearchDistanceOnFire = 10f;

    int _legalTarget_layerMask;

    public int GetLegalTargetsLayerMask()
    {
        return _legalTarget_layerMask;
    }

    public float GetMissileScanRadius()
    {
        return 0;
    }

    public float GetSnakeAmount()
    {
        return 0;
    }

    public float GetSpeedSpec()
    {
        return _projectileSpeed;
    }

    public Vector3 GetTargetPosition()
    {
        return Vector3.zero;
    }

    public float GetTurnSpec()
    {
        return _missileTurnRate;
    }

    public override object GetUIStatus()
    {
        return null;
    }

    public override Vector3 GetInitialProjectileVelocity(Transform projectileTransform)
    {
        return (projectileTransform.transform.up * _projectileSpeed);
    }

    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost))
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            Fire();
        }
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        //none
    }

    private void Fire()
    {
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);
        pb.SetResilience(_startingResilience);

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    protected override void ImplementWeaponUpgrade()
    {
        _startingResilience += _resilienceAddition_Upgrade;
        _projectileLifetime += _lifetimeAddition_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        if (_isPlayer)
        {
            //Only locks on to actual enemy ships, and no neutrals (ie, asteroids)
            _legalTarget_layerMask = LayerLibrary.EnemyLayerMask;
        }
        else
        {
            // 7 is PlayerShip
            _legalTarget_layerMask = LayerLibrary.PlayerLayerMask;
        }
    }

    public Transform GetTargetTransform()
    {
        Transform t = CUR.FindNearestGameObjectOnLayer(
            (Vector2)transform.position + (_inputCon.LookDirection * 5f), _legalTarget_layerMask,
            _maxSearchDistanceOnFire)?.transform;
        return t;
    }
}
