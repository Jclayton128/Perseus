using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Health : MonoBehaviour
{
    //references
    ActorMovement _movement;
    ParticleController _particleController;
    ScrapController _scrapController;
    Rigidbody2D _rb;

    //global settings
    [SerializeField] [Range(0,10)] float _particlesPerPointOfShieldDamage = 1f; //Amount of particles created per point of shield damage
    [SerializeField] [Range(0, 10)] float _scrapsPerPointOfNormalDamage = 1f; //Amount of scrap peeled off per point of hull damage.

    //instance settings
    [FoldoutGroup("Starting Stats")]
    [Tooltip("Maximum (and starting) Hull Points")]
    [SerializeField] [Range(1, 100)] float _maxHullPoints = 10;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Maxium (and starting) Shield Points")]
    [SerializeField] [Range(0, 100)] float _maxShieldPoints = 0;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Points of shielding healed per second.")]
    [SerializeField] [Range(0, 100)] float _shieldHealRate = 0;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Points of ionization healed per second. Max Ionization Amount is equal to total Hull Points.")]
    [SerializeField] [Range(0, 10)] float _ionHealRate = 0;

    //state
    [ShowInInspector] public float HullPoints { get; protected set; } = 1;
    [ShowInInspector] public float ShieldPoints { get; protected set; } = 0;

    [Tooltip("Ion factor is the percentage of ionization")]
    private float _ionizationPointsAbsorbed = 0;
    [ShowInInspector] public float IonFactor = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<ActorMovement>();
        _particleController = FindObjectOfType<ParticleController>();

        HullPoints = _maxHullPoints;
        ShieldPoints = _maxShieldPoints;
        _ionizationPointsAbsorbed = 0;
        IonFactor = 0;
    }

    private void Update()
    {
        //Check for death
        //Recharge Shield
        //Reduce Ionization
        //if player, push visualization to UI elements
    }

    private void ReceiveDamage(DamagePack incomingDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        //receive damage
        //receive ionization; update ionization factor.
        //request scraps

        if (incomingDamage.NormalDamage > 0 || incomingDamage.ShieldBonusDamage > 0)
        {
            float shieldDamage = incomingDamage.NormalDamage + incomingDamage.ShieldBonusDamage;
            ReceiveShieldDamage(shieldDamage, impactPosition, impactHeading);
            float hullDamage = incomingDamage.NormalDamage - ShieldPoints;
            if (hullDamage > 0)
            {
                ReceiveHullDamage(hullDamage, incomingDamage.ScrapBonus, impactPosition, impactHeading);
            }
        }

        if (incomingDamage.IonDamage > 0)
        {
            ReceiveIonDamage(incomingDamage);
        }

    }

    private void ReceiveHullDamage(float normalDamage, float scrapBonus, Vector2 impactPosition, Vector2 impactHeading)
    {
        HullPoints -= normalDamage;

        if (!_movement.IsPlayer)
        {
            int scrapToMake = Mathf.RoundToInt((normalDamage + scrapBonus) * _scrapsPerPointOfNormalDamage);
            _scrapController.SpawnScraps(scrapToMake, impactPosition, impactHeading);
        }

    }

    private void ReceiveShieldDamage(float shieldDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        ShieldPoints -= shieldDamage;
        int amount = Mathf.RoundToInt(shieldDamage * _particlesPerPointOfShieldDamage);
        _particleController.RequestShieldDamageParticles(amount, transform.position, impactHeading);
    }

    private void ReceiveIonDamage(DamagePack incomingDamage)
    {       
        _ionizationPointsAbsorbed += incomingDamage.IonDamage;
        _ionizationPointsAbsorbed = Mathf.Clamp(_ionizationPointsAbsorbed, 0, _maxHullPoints);
    }

    private bool CheckForDeath()
    {
        if (HullPoints <= 0)
        {
            Die();
            return true;
        }
        else return false;
    }

    private void Die()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D impactingWeapon)
    {
        // Reduce penetration on the impactingWeapon. Destroy it if necessary
        // Receive Damage on this
    }
}
