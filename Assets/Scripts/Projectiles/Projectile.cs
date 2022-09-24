using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    //Goal for this is to receive any target point/transform and manage the RigidBody to get there
    //Also manages lifetime of the weapon
    
    public enum ProjectileType //Each PType must unique to a single weapon, even if different PTypes look similar/same
    {
        PlayerBolt0, PlayerSmartMissile1, PlayerScrapedo2, PlayerDumbMissile3, PlayerCannon4,
        Player5, Player6, Player7, Player8, Player9,
        EnemyBolt10, Enemy11, Enemy12, Enemy13, Enemy14, Enemy15, Enemy16, Enemy17,
        Enemy18, Enemy19
    }

    //init
    protected PoolController _poolCon;
    protected Rigidbody2D _rb;
    protected WeaponHandler _launchingWeaponHandler;

    //state
    float _lifetimeRemaining = 0;
    float _resilienceRemaining = 1; //Hits it can take from PD turret or number of penetrations allowed
    public ProjectileType PType;

    public DamagePack DamagePack;
    public Vector2 ImpactHeading;

    //This is called once per game session per pooled weapon object.
    public void Initialize(PoolController poolController)
    {
        _poolCon = poolController;
        _rb = GetComponent<Rigidbody2D>();  
    }

    /// <summary>
    /// This sets up a weapon type's general needs, and calls the specific weapon's setup method.
    /// </summary>
    /// <param name="allegiance"></param>
    /// <param name="launchingWeaponHandler"></param>
    public void SetupInstance(WeaponHandler launchingWeaponHandler)
    {
        _lifetimeRemaining = launchingWeaponHandler.GetLifetimeForProjectile();
        DamagePack = launchingWeaponHandler.GetDamagePackForProjectile();
        _launchingWeaponHandler = launchingWeaponHandler;

        ValidateWeaponHandlerMatchesWeaponType();

        SetupInstanceSpecifics();
    }

    private void ValidateWeaponHandlerMatchesWeaponType()
    {
        switch (PType)
        {
            case ProjectileType.PlayerBolt0:
                if (_launchingWeaponHandler.GetComponent<IBoltLauncher>() == null)
                {
                    Debug.LogError("Bolts must be launched by Bolt Lauchers!");
                }
                break;

            case ProjectileType.PlayerSmartMissile1:
                if (_launchingWeaponHandler.GetComponent<ISmartMissileLauncher>() == null)
                {
                    Debug.LogError("Smart Missiles must be launched by Smart MissileLauchers!");
                }
                break;

            default:
                Debug.LogError("Validation hasn't been set up for this Projectile Type!");
                break;



        }
    }



    /// <summary>
    /// These are the specific setup tasks that each weapon needs to operate properly.
    /// </summary>
    protected abstract void SetupInstanceSpecifics();


    private void Update()
    {
        _lifetimeRemaining -= Time.deltaTime;
        if (_lifetimeRemaining <= 0)
        {
            ExecuteLifetimeExpirationSequence();
        }
        ExecuteUpdateSpecifics();
    }

    /// <summary>
    /// This is where any weapon-specifics actions should occur every Update
    /// </summary>
    protected abstract void ExecuteUpdateSpecifics();

    private void FixedUpdate()
    {
        ExecuteMovement();
    }

    /// <summary>
    /// This is how this specific weapon executes its movement within FixedUpdate
    /// </summary>
    protected abstract void ExecuteMovement();

    /// <summary>
    /// This is called when a projectile's lifetime is exceeded. 
    /// Use ExecuteGenericExpiration_XXX() for generic methods.
    /// </summary>
    protected abstract void ExecuteLifetimeExpirationSequence();

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

    #region Generic Lifetime Expirations

    /// <summary>
    /// This Generic Expiration method just causes the projectile to disappear at life end
    /// </summary>
    protected void ExecuteGenericExpiration_Fizzle()
    {
        _poolCon.ReturnDeadProjectile(this);
    }

    protected void ExecuteGenericExpiration_Spawn()
    {
        Debug.LogError("Generic Expiration-Spawn not implemented yet");
    }

    protected void ExecuteGenericExpiration_Explode()
    {
        Debug.LogError("Generic Expiration-Explode not implemented yet");
    }

    #endregion



}
