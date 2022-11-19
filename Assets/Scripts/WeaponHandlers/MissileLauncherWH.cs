using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherWH : WeaponHandler, IMissileLauncher
{
    //settings
    [Header("Base Missile Parameters")]
    [SerializeField] float _missileTurnRate = 180f;
    [Tooltip("Amount of angular overshoot allowed before correcting. Higher = curvier profile")]
    [SerializeField] float _missileSnakeAmount = 0f;
    [Tooltip("Radius of search area. Offset = Radius * 1.5")]
    [SerializeField] float _missileScanRadius = 0.25f;

    [Header("Upgrade Parameters")]
    [SerializeField] float _missileDamageAddition_Upgrade = 0;
    [SerializeField] float _missileSpeedAddition_Upgrade = 20f;
    [SerializeField] float _missileTurnRateAddition_Upgrade = 30f;

    int _legalTarget_layerMask;   

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
        

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementWeaponUpgrade()
    {
        _normalDamage += _missileDamageAddition_Upgrade;
        _projectileSpeed += _missileSpeedAddition_Upgrade;
        _missileTurnRateAddition_Upgrade += _missileTurnRateAddition_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        // 9 is EnemyShip
        // 11 is NeutralShip
        if (_isPlayer)
        {
            _legalTarget_layerMask = LayerLibrary.EnemyNeutralLayerMask;
        }
        else
        {
            // 7 is PlayerShip
            _legalTarget_layerMask = LayerLibrary.PlayerLayerMask;
        }

    }

    public int GetLegalTargetsLayerMask()
    {
        return _legalTarget_layerMask;
    }

    public Vector3 GetTargetPosition()
    {
        if (_isPlayer)
        {
            return _inputCon._mousePos;
        }
        else
        {
            return _mindsetHandler.PlayerPosition;
        }

    }

    public Transform GetTargetTransform()
    {
        return null;
    }

    public float GetSpeedSpec()
    {
        return _projectileSpeed;
    }

    public float GetTurnSpec()
    {
        return _missileTurnRate;
    }

    public float GetSnakeAmount()
    {
        return _missileSnakeAmount;
    }

    public float GetMissileScanRadius()
    {
        return _missileScanRadius;
    }
}
