using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpGateWH : WeaponHandler
{
    ActorMovement _actorMovement;
    [SerializeField] GameObject _spotPrefab = null;
    [SerializeField] GameObject _blinkInParticleFXPrefab = null;

    //settings
    [SerializeField] float _spotExtendRate = 1f;
    [SerializeField] float _spotExtension_Min = 3.0f;
    [SerializeField] float _spotExtension_Max = 10f;
    [SerializeField] int _novaCount = 10;
    [SerializeField] float _spotExtendRateAddition_Upgrade = 0.5f;
    [SerializeField] float _activationCostMultiplier_Upgrade = 0.8f;

    //state
    bool _isExtending = false;
    GameObject _spot;
    float _spotExtension;

    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost))
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            _isExtending = true;
            _spotExtension = _spotExtension_Min;
            if (!_spot)
            {
                _spot = Instantiate(_spotPrefab,
                    (Vector2)transform.position + (_inputCon.LookDirection * _spotExtension),
                    Quaternion.identity);
            }
        }
    }

    private void Update()
    {
        if (_isExtending)
        {
            _spotExtension += Time.deltaTime * _spotExtendRate;
            _spotExtension = Mathf.Clamp(_spotExtension, _spotExtension_Min, _spotExtension_Max);
            _spot.transform.position =
                (Vector2)transform.position + (_inputCon.LookDirection * _spotExtension);
        }
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        JumpToSpot();
        Destroy(_spot);
        _spotExtension = _spotExtension_Min;
        _isExtending = false;
    }

    private void JumpToSpot()
    {
        if (!_spot) return;
        transform.root.position = _spot.transform.position;

        Instantiate(_blinkInParticleFXPrefab, _spot.transform.position, Quaternion.identity);

        GameObject target = CUR.FindNearestGameObjectOnLayer(_spot.transform,
            LayerLibrary.EnemyLayerMask, 4f);
        if (target)
        {
            _actorMovement.SetDesiredSteering(target.transform.position - transform.position);
            transform.root.rotation = target.transform.rotation;
        }

        float spreadSubdivided = 360f / _novaCount;
        for (int i = 0; i < _novaCount; i++)
        {
            float rand = UnityEngine.Random.Range(0.9f, 1.1f);
            //_projectileLifetime = rand * (_inputCon._mousePos - transform.position).magnitude / _projectileSpeed;

            Quaternion sector = Quaternion.Euler(0, 0,
                (i * spreadSubdivided) - (360f / 2f) + _muzzle.eulerAngles.z);
            Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);

            pb.transform.rotation = sector;

            pb.SetupInstance(this);

        }



    }

    protected override void ImplementWeaponUpgrade()
    {
        _activationCost *= _activationCostMultiplier_Upgrade;
        _spotExtendRate += _spotExtendRateAddition_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        _actorMovement = GetComponentInParent<ActorMovement>();
    }
}
