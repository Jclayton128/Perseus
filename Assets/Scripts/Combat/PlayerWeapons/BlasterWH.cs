using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWH : WeaponHandler
{
    //settings
    [SerializeField] float _timeBetweenShots = 0.5f;
    [SerializeField] float _shotLifetime = 2f;
    [SerializeField] float _shotSpeed = 5f;

    //state
    public bool _isFiring;
    public float _timeUntilNextShot;

    public override void Activate()
    {
        _timeUntilNextShot = _timeBetweenShots;
        Debug.Log($"start fire. parent: {gameObject.transform.parent}");
        _isFiring = true;
    }

    public override void Deactivate()
    {
        Debug.Log("stop fire");
        _isFiring = false;
    }

    private void Update()
    {
        if (_isFiring)
        {
            _timeUntilNextShot -= Time.deltaTime;
            if (_timeUntilNextShot <= 0)
            {
                Fire();
                _timeUntilNextShot = _timeBetweenShots;
            }
        }       
    }

    private void Fire()
    {
        Debug.Log("pew");
        DamagePack dp = new DamagePack(1,0,0,0,0);
        ProjectileBrain pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupBrain(ProjectileBrain.Behaviour.Bolt, ProjectileBrain.Allegiance.Player,
            ProjectileBrain.DeathBehaviour.Fizzle, _shotLifetime, -1, dp, Vector3.zero);
        pb.GetComponent<Rigidbody2D>().velocity = Vector3.one;


    }

}
