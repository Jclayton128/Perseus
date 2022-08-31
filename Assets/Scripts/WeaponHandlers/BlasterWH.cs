using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWH : WeaponHandler
{
    //settings
    [SerializeField] float _minModeToggle = 0.75f;
    [SerializeField] float _timeBetweenShots = 0.125f;
    [SerializeField] float _shotLifetime = 2f;
    [SerializeField] float _shotSpeed = 5f;

    //state
    [Header("State")]
    [SerializeField] bool _isFiring = false;
    [SerializeField] float _timeOfNextShot = 0;
    [SerializeField] float _timeToToggleModes = 0;

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
            _connectedWID?.UpdateUI("warming");

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
            _connectedWID?.UpdateUI("cooling");
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
        ProjectileBrain pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupBrain(ProjectileBrain.Behaviour.Bolt, ProjectileBrain.Allegiance.Player,
            ProjectileBrain.DeathBehaviour.Fizzle, _shotLifetime, -1, dp, Vector3.zero);
        pb.GetComponent<Rigidbody2D>().velocity = pb.transform.up * _shotSpeed;

        if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());

        _connectedWID?.UpdateUI("firing");
    }

    protected override void ImplementWeaponUpgrade()
    {

    }

    public override object GetUIStatus()
    {
        return "blaster";
    }
}
