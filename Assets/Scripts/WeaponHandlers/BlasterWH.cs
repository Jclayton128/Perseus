using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWH : WeaponHandler, IBoltLauncher
{
    //settings
    [Tooltip("Time required to go from slowest to best firing rate")]
    [SerializeField] float _spoolUpTime = 6f;
    [SerializeField] float _BestTimeBetweenShots = 0.25f;
    [Tooltip("Lowest value applied to the BestTimeBetweenShots")]
    [SerializeField] float _fireRateModifier_Initial = 0.33f;

    [Header("Upgrade Settings")]
    [SerializeField] float _timeBetweenShotsMultiplier_Upgrade = 0.8f;
    [SerializeField] float _spoolUpTimeMultiplier_Upgrade = 0.8f;
    Color _fireRateBarColor_Low = Color.red;
    Color _fireRateBarColor_High = Color.green;

    //state
    float _fireRateModifier_Current;
    Color _fireRateBarColor_Current;
    bool _isFiring = false;
    float _timeOfNextShot = 0;

    protected override void InitializeWeaponSpecifics()
    {
        //_connectedWID?.UpdateUI("hey!");
        _fireRateModifier_Current = _fireRateModifier_Initial;
    }

    protected override void ActivateInternal()
    {
        _isFiring = true;
        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomActivationClip());
        else _hostAudioSource.PlayOneShot(GetRandomActivationClip());
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        if (_isFiring && _isPlayer && !wasPausedDuringDeactivationAttempt)
        {
            _playerAudioSource.PlayClipAtPlayer(GetRandomDeactivationClip());
        }

        _isFiring = false;
    }

    private void Update()
    {
        UpdateFireRateModifier();

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
            _timeOfNextShot = Time.time + (_BestTimeBetweenShots / _fireRateModifier_Current);
        }
    }

    private void UpdateFireRateModifier()
    {
        if (_isFiring)
        {
            _fireRateModifier_Current += Time.deltaTime / _spoolUpTime;
        }
        else
        {
            _fireRateModifier_Current -= Time.deltaTime;
        }
        _fireRateModifier_Current = Mathf.Clamp(_fireRateModifier_Current,
            _fireRateModifier_Initial, 1);

        _fireRateBarColor_Current = Color.Lerp(_fireRateBarColor_Low, _fireRateBarColor_High,
            _fireRateModifier_Current);
        _connectedWID?.UpdateUI(_fireRateModifier_Current, _fireRateBarColor_Current);
    }

    private void Fire()
    {
        DamagePack dp = new DamagePack(_normalDamage, _shieldBonusDamage, _ionDamage, _knockBackAmount, _scrapBonus);
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);
        
        _hostRadarProfileHandler?.AddToCurrentRadarProfile(_profileIncreaseOnActivation);

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    protected override void ImplementWeaponUpgrade()
    {
        _spoolUpTime *= _spoolUpTimeMultiplier_Upgrade;
        _BestTimeBetweenShots *= _timeBetweenShotsMultiplier_Upgrade;
    }

    public override object GetUIStatus()
    {
        return _fireRateModifier_Current;
    }
}
