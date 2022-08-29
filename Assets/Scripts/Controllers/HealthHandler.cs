using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class HealthHandler : MonoBehaviour
{
    //references
    ActorMovement _movement;
    ParticleController _particleController;
    ScrapController _scrapController;
    Rigidbody2D _rb;
    UI_Controller _UIController;
    AdjustableImageBar _shieldBar;
    AdjustableImageBar _hullBar;

    //global settings
    //[SerializeField] [Range(0,10)] float _particlesPerPointOfShieldDamage = 1f; //Amount of particles created per point of shield damage
    //[SerializeField] [Range(0, 10)] float _scrapsPerPointOfNormalDamage = 1f; //Amount of scrap peeled off per point of hull damage.

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
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float HullPoints { get; protected set; } = 1;

    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float ShieldPoints { get; protected set; } = 0;

    [Tooltip("Ion factor is the percentage of ionization")]
    private float _ionizationPointsAbsorbed = 0;
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float IonFactor = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<ActorMovement>();
        _particleController = FindObjectOfType<ParticleController>();
        _scrapController = _particleController.GetComponent<ScrapController>();
        _UIController = _particleController.GetComponent<UI_Controller>();
        _shieldBar = _UIController.GetShieldBar();
        _hullBar = _UIController.GetHullBar();

        HullPoints = _maxHullPoints;
        ShieldPoints = _maxShieldPoints;

        if (_movement.IsPlayer)
        {
            _hullBar.SetFactor(HullPoints / _maxHullPoints);
            _shieldBar.SetFactor(ShieldPoints / _maxShieldPoints);
        }

        _ionizationPointsAbsorbed = 0;
        IonFactor = 0;
    }

    #region Flow
    private void Update()
    {
        UpdateDeathCheck();
        UpdateIonization();
        UpdateRechargeShield();
    }
    private bool UpdateDeathCheck()
    {
        if (HullPoints <= 0)
        {
            Die();
            return true;
        }
        
        return false;
    }

    private void Die()
    {
        Debug.Log("Dead enemy!");
        Destroy(gameObject);
    }

    private void UpdateRechargeShield()
    {

        ShieldPoints += _shieldHealRate * (1-IonFactor) * Time.deltaTime;
        ShieldPoints = Mathf.Clamp(ShieldPoints, 0, _maxShieldPoints);

        if (_movement.IsPlayer)
        {
            _shieldBar.SetFactor(ShieldPoints / _maxShieldPoints);
        }
    }

    private void UpdateIonization()
    {
        if (_ionizationPointsAbsorbed <= 0) return;
        _ionizationPointsAbsorbed -= _ionHealRate * Time.deltaTime;
        _ionizationPointsAbsorbed = Mathf.Clamp(_ionizationPointsAbsorbed, 0, _maxHullPoints);
        IonFactor = (_ionizationPointsAbsorbed / _maxHullPoints);
    }


    #endregion

    #region Receive Damage

    private void OnTriggerEnter2D(Collider2D weaponImpact)
    {
        ProjectileBrain pb;
        if (weaponImpact.TryGetComponent<ProjectileBrain>(out pb))
        {
            ReceiveDamage(pb.DamagePack, weaponImpact.transform.position, pb.GetVectorAtImpact());
            pb.DecrementPenetrationOnImpact();
        }
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

    private void ReceiveShieldDamage(float shieldDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        ShieldPoints -= shieldDamage;
        float damageDone = shieldDamage + Mathf.Clamp(ShieldPoints, -999, 0);
        int amount = Mathf.FloorToInt(damageDone);
        _particleController.RequestShieldDamageParticles(amount, impactPosition, impactHeading);
        
        if (_movement.IsPlayer)
        {
            _shieldBar.SetFactor(ShieldPoints / _maxShieldPoints);
        }

    }

    private void ReceiveHullDamage(float normalDamage, float scrapBonus, Vector2 impactPosition, Vector2 impactHeading)
    {

        HullPoints -= normalDamage;
        float damageReceived;

        if (HullPoints < 0)
        {
            //A high-damage shot against something with little health left should not create lots of scrap
            damageReceived = normalDamage + scrapBonus + HullPoints; //Decrease damage received by negative Hull Points
        }
        else
        {
            damageReceived = normalDamage + scrapBonus;

        }

        if (_movement.IsPlayer)
        {
            _hullBar.SetFactor(HullPoints / _maxHullPoints);
        }

        if (!_movement.IsPlayer)
        {
            int scrapToMake = Mathf.RoundToInt((damageReceived));

            _scrapController.SpawnScraps(scrapToMake, ((Vector2)transform.position + impactPosition)/2f, impactHeading);
        }

    }

    private void ReceiveIonDamage(DamagePack incomingDamage)
    {       
        _ionizationPointsAbsorbed += incomingDamage.IonDamage;
        _ionizationPointsAbsorbed = Mathf.Clamp(_ionizationPointsAbsorbed, 0, _maxHullPoints);
    }

    #endregion


}
