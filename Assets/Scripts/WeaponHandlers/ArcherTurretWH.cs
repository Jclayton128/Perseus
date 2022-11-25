using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTurretWH : WeaponHandler, IBoltLauncher
{
    //settings
    float _chargeRate = .3f;
    bool _isCharging;
    ParticleSystem _particleSystem;
    ParticleSystem.EmissionModule _psem;
    [SerializeField] float _chargeRateAddition_Upgrade = .1f;
    [SerializeField] float _weaponSpeedMultiplier_Upgrade = 1.2f;

    //state
    float _chargeFactor = 0;
    Color _chargeColor;

    private void Update()
    {
        if (_isCharging)
        {
            _chargeFactor += _chargeRate * Time.deltaTime;
            _chargeFactor = Mathf.Clamp(_chargeFactor, 0, 1);

            _hostEnergyHandler.SpendEnergy(_activationCost * Time.deltaTime);

            if (!_hostEnergyHandler.CheckEnergy(_activationCost * Time.deltaTime))
            {

                DeactivateInternal(false);
            }

            UpdateUI();
        }

    }
    private void UpdateUI()
    {
        _chargeColor = Color.Lerp(Color.red, Color.green, _chargeFactor );
        _connectedWID?.UpdateUI(_chargeFactor, _chargeColor);
    }

    protected override void ActivateInternal()
    {
        _isCharging = true;
        //todo emit cool charge particles
        _psem.rateOverTime = 3f;
        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomActivationClip());
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        
        if (!wasPausedDuringDeactivationAttempt)
        {

            Fire();
        }        
        
        _isCharging = false;
        _psem.rateOverTime = 0f;
        _chargeFactor = 0;
        UpdateUI();
    }

    private void Fire()
    {        
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);

        _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation*_chargeFactor);

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());

    }

    public override DamagePack GetDamagePackForProjectile()
    {
        DamagePack dp =
            new DamagePack(_chargeFactor * _normalDamage,
            _chargeFactor * _shieldBonusDamage,
            _chargeFactor * _ionDamage,
            _chargeFactor * _knockBackAmount,
            _chargeFactor * _scrapBonus);
        return dp;
    }

    public override float GetLifetimeForProjectile()
    {
        return _projectileLifetime;
    }

    public override object GetUIStatus()
    {
        return _chargeFactor;
    }

    protected override void ImplementWeaponUpgrade()
    {
        _projectileSpeed *= _weaponSpeedMultiplier_Upgrade;
        _chargeRate += _chargeRateAddition_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _psem = _particleSystem.emission;
    }
}
