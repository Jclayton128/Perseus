using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerTurretWH : WeaponHandler, IBoltLauncher
{
    [SerializeField] Transform _turretMuzzle = null;

    //settings
    [SerializeField] float _minModeToggle = 0.01f;
    [SerializeField] float _timeBetweenShots = 0.5f;

    [Header("Upgrade Settings")]
    [SerializeField] float _shotSpeedIncrease_Upgrade = 1.0f;
    [SerializeField] float _ionizationIncrease_Upgrade = 0.5f;

    //state
    TurretSteerer _turretSteerer;
    bool _isFiring = false;
    float _timeOfNextShot = 0;
    float _timeToToggleModes = 0;

    protected override void ActivateInternal()
    {
        if (!_isFiring && Time.time >= _timeToToggleModes)
        {
            _timeOfNextShot = Time.time + _minModeToggle;
            _timeToToggleModes = Time.time + _minModeToggle;
            _isFiring = true;

            //if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomActivationClip());
            //else _hostAudioSource.PlayOneShot(GetRandomActivationClip());
        }
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        if (Time.time > _timeToToggleModes && _isFiring)
        {
            if (_isPlayer && !wasPausedDuringDeactivationAttempt)
            {
                //_playerAudioSource.PlayGameplayClipForPlayer(GetRandomDeactivationClip());
            }
            _timeToToggleModes = Time.time + _minModeToggle;

        }

        _isFiring = false;
    }

    private void Update()
    {
        UpdateFacing();

        if (_isFiring && Time.time >= _timeOfNextShot)
        {
            if (_hostEnergyHandler.CheckEnergy(_activationCost))
            {
                _hostEnergyHandler.SpendEnergy(_activationCost);
                Fire();
            }
            else
            {
                //TODO audio sound of insufficient energy to fire
                DeactivateInternal(false);
            }
            _timeOfNextShot = Time.time + _timeBetweenShots;
        }
    }

    private void UpdateFacing()
    {
        if (_isPlayer)
        {
            _turretSteerer.SetLookAngle(_inputCon.LookAngle);
        }
    }

    private void Fire()
    {
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _turretMuzzle);
        pb.SetupInstance(this);

        _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation);

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }


    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementWeaponUpgrade()
    {
        _projectileSpeed += _shotSpeedIncrease_Upgrade;
        _ionDamage += _ionizationIncrease_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        _turretSteerer = GetComponentInChildren<TurretSteerer>();
    }
}
