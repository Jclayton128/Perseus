using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWH : WeaponHandler
{
    //settings
    [SerializeField] float _timeBetweenShots = 0.25f;
    [SerializeField] float _shotLifetime = 2f;
    [SerializeField] float _shotSpeed = 5f;

    //state
    bool _isFiring;
    float _timeOfNextShot;

    public override void Activate()
    {
        _timeOfNextShot = Time.time + _timeBetweenShots;
        _isFiring = true;
    }

    public override void Deactivate()
    {
        _isFiring = false;
    }

    private void Update()
    {
        if (_isFiring && Time.time >= _timeOfNextShot)
        {             
            Fire();
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
    }

}
