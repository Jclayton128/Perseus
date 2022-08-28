using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWH : WeaponHandler
{
    //settings
    [SerializeField] float _spoolupTime = 0.75f;
    [SerializeField] float _timeBetweenShots = 0.125f;
    [SerializeField] float _shotLifetime = 2f;
    [SerializeField] float _shotSpeed = 5f;

    //state

    bool _isFiring;
    float _timeOfNextShot;
    float _minTimeForDespoolSound;

    public override void Activate()
    {
        _timeOfNextShot = Time.time + _spoolupTime;
        _minTimeForDespoolSound = Time.time + _spoolupTime;
        _isFiring = true;

        if (_isPlayer) _audioCon.PlayPlayerSound(GetRandomActivationClip());

    }

    public override void Deactivate()
    {
        if (Time.time > _minTimeForDespoolSound && _isFiring)
        {
            if (_isPlayer) _audioCon.PlayPlayerSound(GetRandomDeactivationClip());
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
        _audioCon.PlayPlayerSound(GetRandomFireClip());
    }

}
