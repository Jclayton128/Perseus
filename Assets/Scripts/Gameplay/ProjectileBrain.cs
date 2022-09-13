using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBrain : MonoBehaviour
{
    //Goal for this is to receive any target point/transform and manage the RigidBody to get there
    //Also manages lifetime of the weapon
    
    public enum PType //Each PType must unique to a single weapon, even if different PTypes look similar/same
    {
        PlayerBolt0, PlayerMissile1, PlayerScrapedo2, PlayerPopRocket3, PlayerCannon4,
        Player5, Player6, Player7, Player8, Player9,
        EnemyBolt10, Enemy11, Enemy12, Enemy13, Enemy14, Enemy15, Enemy16, Enemy17,
        Enemy18, Enemy19
    }
    
    public enum Behaviour
    {
        Bolt, //Basic bullet
        SmartMissile, //Assigned a target point, attempts to get there or to an acquired target
        DumbMissile, //Assigned a target point, attempts to get there
        AcceleratingBolt, //bullet that accelerates faster

    }

    public enum Allegiance
    {
        Player, //This weapon only reacts with enemies
        Neutral, //This weapon reacts to both the player and any/all enemies
        Enemy //This weapon reacts to just the player
    }

    public enum DeathBehaviour
    {
        Fizzle, //weapon fades away quickly at end of life
        Explode, //weapon explodes at end of life, causing AoE damage
        Spawn //The weapon creates something new at end of life.
    }

    //init
    PoolController _poolCon;
    Rigidbody2D _rb;

    //state
    float _lifetimeRemaining = 0;
    float _resilienceRemaining = 1; //Hits it can take from PD turret or number of penetrations allowed
    Behaviour _behaviour;
    public PType pType;
    Allegiance _allegiance;
    DeathBehaviour _deathBehaviour;
    Vector3 _targetPoint;
    Transform _targetTransform;
    float _genericParameter; // This is a float that can be used to inform specific things, like scrapedo accel rate

    public DamagePack DamagePack;
    public Vector2 ImpactHeading;

    public void Initialize(PoolController poolController)
    {
        _poolCon = poolController;
        _rb = GetComponent<Rigidbody2D>();  
    }

    public void SetupBrain(Behaviour behaviour, Allegiance allegiance,
        DeathBehaviour deathBehaviour, float lifetime, float resilience,
        DamagePack damagePack, Vector3 targetPoint)
    {
        _lifetimeRemaining = lifetime;
        DamagePack = damagePack;
        _behaviour = behaviour;
        _allegiance = allegiance;
        _deathBehaviour = deathBehaviour;
        _targetPoint = targetPoint;
    }

    private void Update()
    {
        _lifetimeRemaining -= Time.deltaTime;
        if (_lifetimeRemaining <= 0)
        {
            ExecuteLifetimeExpirationSequence();
        }
    }

    private void FixedUpdate()
    {
        ExecuteMovement();
    }

    private void ExecuteMovement()
    {
        switch(_behaviour)
        {
            case Behaviour.Bolt:
                //Bolts just maintain velocity
                return;

            case Behaviour.AcceleratingBolt:
                //Accelerate along same heading.
                return;

        }
    }

    private void ExecuteLifetimeExpirationSequence()
    {
        switch (_deathBehaviour)
        {
            case DeathBehaviour.Fizzle:
                _poolCon.ReturnDeadProjectile(this);
                return;

            case DeathBehaviour.Explode:

                return;

            case DeathBehaviour.Spawn:

                return;
        }
    }

    #region Public Impact Helpers

    public Vector2 GetNormalizedVectorAtImpact()
    {
        return _rb.velocity.normalized;
    }

    public void DecrementPenetrationOnImpact()
    {
        _resilienceRemaining--;
        if (_resilienceRemaining <= 0)
        {
            _poolCon.ReturnDeadProjectile(this);
        }
    }

    #endregion
}
