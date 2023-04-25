using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    //Goal for this is to receive any target point/transform and manage the RigidBody to get there
    //Also manages lifetime of the weapon
    
    /// <summary>
    /// The ProjectileType define what the project looks like, as well as its physics layer.
    /// Its actual behavior is governed by which Projectile script is assigned to it.
    /// </summary>
    public enum ProjectileType //Each PType must unique to a single weapon, even if different PTypes look similar/same
    {
        PlayerBolt0, PlayerMissile1, PlayerScrapedo2, PlayerRocket3, PlayerCannon4,
        PlayerTorpedo5, PlayerJavelin6, PlayerShieldBlast7, PlayerWarpBlast8, Player9,
        EnemyBolt10, EnemyMissile11, EnemyMine12, EnemyRocket13, EnemyJavelin14, Enemy15, Enemy16, Enemy17,
        Enemy18, Enemy19
    }

    //init
    protected ProjectilePoolController _poolCon;
    protected Rigidbody2D _rb;
    protected WeaponHandler _launchingWeaponHandler;
    protected AudioSource _auso;

    //settings
    [Tooltip("Only deliver damage when the projectile is about to be removed, ie a rocket" +
        "when it is detonating. Direct hits will reduce penetration but not deliver any damage.")]
    [SerializeField] protected bool _deliversDamageOnlyAtExpiration = false;
    public bool DeliversDamageOnlyAtExpiration => _deliversDamageOnlyAtExpiration;
    [SerializeField] AudioClip[] _detonateSounds = null;


    //state
    float _lifetimeRemaining = 0;
    float _resilienceRemaining = 1; //Hits it can take from PD turret or number of penetrations allowed
    public ProjectileType PType;

    public DamagePack DamagePack;
    public Vector2 ImpactHeading;

    //This is called once per game session per pooled weapon object.
    public virtual void Initialize(ProjectilePoolController poolController)
    {
        _poolCon = poolController;
        _rb = GetComponent<Rigidbody2D>();
        _auso = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This sets up a projectile's lifetime and DamagePack, and resets its resilience to 1.
    /// It then executes any weapon-specific methods.
    /// </summary>
    /// <param name="allegiance"></param>
    /// <param name="launchingWeaponHandler"></param>
    public void SetupInstance(WeaponHandler launchingWeaponHandler)
    {
        _lifetimeRemaining = launchingWeaponHandler.GetLifetimeForProjectile();
        DamagePack = launchingWeaponHandler.GetDamagePackForProjectile();
        _launchingWeaponHandler = launchingWeaponHandler;
        _resilienceRemaining = 1;

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
                    Debug.Log("Bolts must be launched by IBoltLaunchers!");
                }
                break;

            case ProjectileType.PlayerMissile1:
                if (_launchingWeaponHandler.GetComponent<IMissileLauncher>() == null)
                {
                    Debug.Log("Smart Missiles must be launched by IMissileLaunchers!");
                }
                break;

            case ProjectileType.PlayerRocket3:
                if (_launchingWeaponHandler.GetComponent<IMissileLauncher>() == null)
                {
                    Debug.Log("Rockets must be launched by IMissileLaunchers!");
                }
                break;

            case ProjectileType.PlayerTorpedo5:
                if (_launchingWeaponHandler.GetComponent<IMissileLauncher>() == null)
                {
                    Debug.Log("Torpedos must be launched by IMissileLaunchers!");
                }
                break;

            case ProjectileType.EnemyBolt10: break;
            case ProjectileType.EnemyMine12: break;

            case ProjectileType.EnemyMissile11:
                if (_launchingWeaponHandler.GetComponent<IMissileLauncher>() == null)
                {
                    Debug.Log("Smart Missiles must be launched by IMissileLaunchers!");
                }
                break;

            default:
                Debug.Log("Validation hasn't been set up for this Projectile Type!");
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

    /// <summary>
    /// Optional method that allows resilience to be greater than 1 to allow for multiple penetrations
    /// </summary>
    /// <param name="resilience"></param>
    public void SetResilience(int resilience)
    {
        //Resilience of zero means it insta-destructs
        resilience = Mathf.Clamp(resilience, 1, 99);
        _resilienceRemaining = resilience;
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
            ExecuteLifetimeExpirationSequence();
            //_poolCon.ReturnDeadProjectile(this);
        }
    }

    #endregion

    #region Generic Lifetime Expirations

    /// <summary>
    /// This Generic Expiration method just causes the projectile to disappear at life end
    /// </summary>
    protected void ExecuteGenericExpiration_Fizzle()
    {
        //Play a fizzle sound?
        _poolCon.ReturnDeadProjectile(this);
    }

    protected void ExecuteGenericExpiration_Spawn()
    {
        Debug.LogError("Generic Expiration-Spawn not implemented yet");

        _poolCon.ReturnDeadProjectile(this);
    }

    /// <summary>
    /// Handles the damage dealing and projectile disposal for an explosion.
    /// Does not handle any desired particle FX.
    /// </summary>
    /// <param name="maxDamageRange"></param>
    protected void ExecuteGenericExpiration_Explode(float maxDamageRange,
        int vulnerableLayerMask)
    {
        if (_detonateSounds.Length > 0)
        {
            AudioSource.PlayClipAtPoint(
                (AudioClip)CUR.GetRandomFromCollection(_detonateSounds), transform.position);
            //_auso.PlayOneShot();
        } 

        if (maxDamageRange <=0 )
        {
            _poolCon.ReturnDeadProjectile(this);
            return;
        }

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, maxDamageRange,
            vulnerableLayerMask);

        foreach (Collider2D hit in hits)
        {
            HealthHandler hh = hit.transform.GetComponent<HealthHandler>();
            Vector2 point = hit.ClosestPoint(transform.position);
            Vector2 dir = (hit.transform.position - transform.position);
            DamagePack.FadeDamage(
                (maxDamageRange - dir.magnitude)/maxDamageRange);
            hh?.ReceiveNonProjectileDamage(DamagePack, point, dir.normalized);
        }
        _poolCon.ReturnDeadProjectile(this);
    }

    #endregion



}
