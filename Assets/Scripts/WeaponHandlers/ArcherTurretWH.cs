using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTurretWH : WeaponHandler, IBoltLauncher
{
    //settings
    float _maxCharge = 10f;
    float _chargeRate = 2f;
    bool _isCharging;
    ParticleSystem _particleSystem;
    ParticleSystem.EmissionModule _psem;

    //state
    public float _chargeLevel = 0;
    Color _chargeColor;

    private void Update()
    {
        if (_isCharging)
        {
            _chargeLevel += _chargeRate * Time.deltaTime;
            _chargeLevel = Mathf.Clamp(_chargeLevel, 0, _maxCharge);

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
        _chargeColor = Color.Lerp(Color.red, Color.green, _chargeLevel / _maxCharge);
        _connectedWID?.UpdateUI(_chargeLevel / _maxCharge, _chargeColor);
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
        _chargeLevel = 0;
        UpdateUI();
    }

    private void Fire()
    {        
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);

        _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation);

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());

    }

    public override DamagePack GetDamagePackForProjectile()
    {
        DamagePack dp =
            new DamagePack(_chargeLevel * 3f, _shieldBonusDamage, _ionDamage, _knockBackAmount, _scrapBonus);
        return dp;
    }

    public override float GetLifetimeForProjectile()
    {
        return _chargeLevel * 2f;
    }

    public override object GetUIStatus()
    {
        return _chargeLevel;
    }

    protected override void ImplementWeaponUpgrade()
    {
        
    }

    protected override void InitializeWeaponSpecifics()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _psem = _particleSystem.emission;
    }
}
