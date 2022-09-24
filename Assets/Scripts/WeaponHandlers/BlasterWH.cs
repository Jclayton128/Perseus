using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWH : WeaponHandler, IBoltLauncher
{
    //settings
    [SerializeField] float _minModeToggle = 0.75f;
    [SerializeField] float _timeBetweenShots = 0.25f;
    [SerializeField] float _shotSpeed = 5f;

    [Header("Upgrade Settings")]
    [SerializeField] float _fireRateMultiplier_Upgrade = 0.8f;
    [SerializeField] float _energyCostSubtraction_Upgrade = 0.8f;

    //state
     bool _isFiring = false;
    float _timeOfNextShot = 0;
    float _timeToToggleModes = 0;

    protected override void InitializeWeaponSpecifics()
    {
        //_connectedWID?.UpdateUI("hey!");
    }

    protected override void ActivateInternal()
    {
        if (Time.time >= _timeToToggleModes)
        {
            _timeOfNextShot = Time.time + _minModeToggle;
            _timeToToggleModes = Time.time + _minModeToggle;
            _isFiring = true;
            _connectedWID?.UpdateUI("RDY");

            if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomActivationClip());
            else _hostAudioSource.PlayOneShot(GetRandomActivationClip());
        }
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        if (Time.time > _timeToToggleModes && _isFiring)
        {
            if (_isPlayer && !wasPausedDuringDeactivationAttempt)
            {
                _playerAudioSource.PlayGameplayClipForPlayer(GetRandomDeactivationClip());
            }
            _connectedWID?.UpdateUI("COOL");
            _timeToToggleModes = Time.time + _minModeToggle;

        }

        _isFiring = false;
    }

    private void Update()
    {
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

    private void Fire()
    {
        DamagePack dp = new DamagePack(_normalDamage, _shieldBonusDamage, _ionDamage, _knockBackAmount, _scrapBonus);
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);
        
        _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation);

        if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());

        _connectedWID?.UpdateUI("FIRE");
    }

    public Vector3 GetInitialBoltVelocity(Transform projectileTransform)
    {
        return (Vector3)_rb.velocity + (projectileTransform.transform.up * _shotSpeed);
    }

    protected override void ImplementWeaponUpgrade()
    {
        _activationCost *= _energyCostSubtraction_Upgrade;
        _timeBetweenShots *= _fireRateMultiplier_Upgrade;
    }

    public override object GetUIStatus()
    {
        return "INIT";
    }
}
