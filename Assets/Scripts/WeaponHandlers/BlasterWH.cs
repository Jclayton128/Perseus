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

    bool _isFiring;
    float _timeOfNextShot;
    float _timeToToggleModes;

    protected override void InitializeWeaponSpecifics()
    {
        _connectedWID?.UpdateUI("hey!");
    }

    public override void Activate()
    {
        if (Time.time > _timeToToggleModes)
        {
            _timeOfNextShot = Time.time + _minModeToggle;
            _timeToToggleModes = Time.time + _minModeToggle;
            _isFiring = true;
            _connectedWID?.UpdateUI("warming");

            if (_isPlayer) _audioCon.PlayPlayerSound(GetRandomActivationClip());
        }


    }

    public override void Deactivate()
    {
        if (Time.time > _timeToToggleModes && _isFiring)
        {
            if (_isPlayer) _audioCon.PlayPlayerSound(GetRandomDeactivationClip());
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
                Deactivate();
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
        
        if (_isPlayer)
        {
            _audioCon.PlayPlayerSound(GetRandomFireClip());
        }
        else
        {
            _audioCon.PlayRemoteSound(GetRandomFireClip(), transform.position);
        }


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
