using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class HealthHandler : MonoBehaviour
{
    #region References
    //references
    ActorMovement _movement;
    ParticleController _particleController;
    ScrapController _scrapController;
    Rigidbody2D _rb;
    UI_Controller _UIController;

    public event System.Action<DamagePack> ReceivingDamagePack = null;
    public event System.Action<DamagePack> ReceivingShieldDamage = null;
    public event System.Action<DamagePack> ReceivingHullDamage = null;

    #endregion

    //global settings
    float _minSecondsBetweenShieldDamageFX = 0.125f;
    float _minSecondsToBeginRechargingShields = 0.5f;

    #region Instance Settings
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

    #endregion;

    #region State
    //state
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float HullPoints { get; protected set; } = 1;

    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float ShieldPoints { get; protected set; } = 0;

    [Tooltip("Ion factor is the percentage of ionization")]
    private float _ionizationPointsAbsorbed = 0;
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float IonFactor = 0;

    int _scrapValue;
    float _secondsSinceLastShieldDamageFX = 0;
    float _secondsSinceLastShieldDamage = 0;
    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<ActorMovement>();
        _particleController = FindObjectOfType<ParticleController>();
        _scrapController = _particleController.GetComponent<ScrapController>();
        _UIController = _particleController.GetComponent<UI_Controller>();

        HullPoints = _maxHullPoints;
        ShieldPoints = _maxShieldPoints;
        _scrapValue = Mathf.RoundToInt(_maxHullPoints);

        if (_movement.IsPlayer)
        {
            _UIController.UpdateShieldBar(ShieldPoints, _maxShieldPoints);
            _UIController.UpdateHullBar(HullPoints, _maxHullPoints);
            _UIController.UpdateShieldRegenTMP(_shieldHealRate.ToString("F1"), Color.white);
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
        UpdateShieldDamageCounters();
    }

    private void UpdateShieldDamageCounters()
    {
        _secondsSinceLastShieldDamageFX += Time.deltaTime;
        _secondsSinceLastShieldDamage += Time.deltaTime;
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
        if (!_movement.IsPlayer)
        {
            Vector2 dir = UnityEngine.Random.onUnitSphere;
            Debug.Log($"should spawn {_scrapValue} scraps");
            _scrapController.SpawnScraps(_scrapValue, ((Vector2)transform.position), dir);
        }

        Destroy(gameObject);
    }

    private void UpdateRechargeShield()
    {
        if (_secondsSinceLastShieldDamage < _minSecondsToBeginRechargingShields) return;

        ShieldPoints += _shieldHealRate * (1-IonFactor) * Time.deltaTime;
        ShieldPoints = Mathf.Clamp(ShieldPoints, 0, _maxShieldPoints);

        if (_movement.IsPlayer)
        {
            _UIController.UpdateShieldBar(ShieldPoints, _maxShieldPoints);
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
        Projectile pb;
        if (weaponImpact.TryGetComponent<Projectile>(out pb))
        {
            ReceivingDamagePack?.Invoke(pb.DamagePack);
            ReceiveDamage(pb.DamagePack, weaponImpact.transform.position, pb.GetNormalizedVectorAtImpact());
            pb.DecrementPenetrationOnImpact();
        }
    }

    /// <summary>
    /// This is used to force a Health Handler to take damage, such as from a beam weapon that doesn't
    /// otherwise have a collider.
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <param name="impactPosition"></param>
    /// <param name="impactHeading"></param>
    public void ReceiveNonColliderDamage(DamagePack incomingDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        ReceiveDamage(incomingDamage, impactPosition, impactHeading);
    }

    private void ReceiveDamage(DamagePack incomingDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        //receive damage
        //receive ionization; update ionization factor.
        //request scraps

        if (incomingDamage.NormalDamage > 0 || incomingDamage.ShieldBonusDamage > 0)
        {
            float carryoverHullDamage = incomingDamage.NormalDamage - 
                    Mathf.Clamp(ShieldPoints - incomingDamage.ShieldBonusDamage, 0 , 999);

            ReceivingShieldDamage?.Invoke(incomingDamage);
            float shieldDamage = incomingDamage.NormalDamage + incomingDamage.ShieldBonusDamage;
            ReceiveShieldDamage(shieldDamage, impactPosition, impactHeading);
            incomingDamage.NormalDamage = carryoverHullDamage;
            if (incomingDamage.NormalDamage > 0)
            {
                ReceivingHullDamage?.Invoke(incomingDamage);
                ReceiveHullDamage(incomingDamage.NormalDamage, incomingDamage.ScrapBonus, impactPosition, impactHeading);
            }
        }

        if (incomingDamage.IonDamage > 0)
        {
            ReceiveIonDamage(incomingDamage);
        }

        if (Mathf.Abs(incomingDamage.KnockbackAmount) > Mathf.Epsilon)
        {
            ReceiveKnockback(incomingDamage.KnockbackAmount, impactHeading);
        }

    }
    private void ReceiveShieldDamage(float shieldDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        if (shieldDamage <= 0) return;
        ShieldPoints -= shieldDamage;
        float damageDone = shieldDamage + Mathf.Clamp(ShieldPoints, -999, 0);
        int amount = Mathf.RoundToInt(damageDone+0.5f) ;

        _secondsSinceLastShieldDamage = 0;

        if (_secondsSinceLastShieldDamageFX >= _minSecondsBetweenShieldDamageFX)
        {
            _particleController.RequestShieldDamageParticles(amount, impactPosition, impactHeading);
            _secondsSinceLastShieldDamageFX = 0;
        }
        
        if (_movement.IsPlayer)
        {
            _UIController.UpdateShieldBar(ShieldPoints, _maxShieldPoints);
        }

    }

    private void ReceiveHullDamage(float normalDamage, float scrapBonus, Vector2 impactPosition, Vector2 impactHeading)
    {

        HullPoints -= normalDamage;
        _scrapValue += Mathf.RoundToInt(scrapBonus);
        
        if (_movement.IsPlayer)
        {
            _UIController.UpdateHullBar(HullPoints, _maxHullPoints);
        }
    }

    private void ReceiveIonDamage(DamagePack incomingDamage)
    {       
        _ionizationPointsAbsorbed += incomingDamage.IonDamage;
        _ionizationPointsAbsorbed = Mathf.Clamp(_ionizationPointsAbsorbed, 0, _maxHullPoints);
    }

    private void ReceiveKnockback(float knockbackAmount, Vector2 impactHeading)
    {
        if (knockbackAmount > 0)
        {
            _rb.AddForce(knockbackAmount * impactHeading, ForceMode2D.Impulse);
        }
    }

    #endregion

    #region System Modifications

    /// <summary>
    /// Adds to the per-second shield regeneration/heal rate. Clamped to range of 0-99 (can't go negative).
    /// </summary>
    /// <param name="shieldHealRateAddition"></param>
    public void AdjustShieldHealRate(float shieldHealRateAddition)
    {
        _shieldHealRate += shieldHealRateAddition;
        _shieldHealRate = Mathf.Clamp(_shieldHealRate, 0, 99);
        _UIController.UpdateShieldRegenTMP(_shieldHealRate.ToString("F1"), Color.white);
    }

    /// <summary>
    /// Adds to the maximum shield amount. Clamped to range of 0-999 (can't go negative);
    /// </summary>
    /// <param name="shieldMaxAddition"></param>
    public void AdjustShieldMaximum(float shieldMaxAddition)
    {
        _maxShieldPoints += shieldMaxAddition;
        _maxShieldPoints = Mathf.Clamp(_maxShieldPoints, 0, 999);
        _UIController?.UpdateShieldBar(ShieldPoints, _maxShieldPoints);
    }

    /// <summary>
    /// Increase both the maximum hull level and current hull level by this much.
    /// </summary>
    /// <param name="hullAddition"></param>
    public void AdjustHullMaximumAndCurrent(float hullAddition)
    {
        _maxHullPoints += hullAddition;
        HullPoints += hullAddition;
        _UIController?.UpdateHullBar(HullPoints, _maxHullPoints);
    }

    #endregion

    #region Public Gets

    public float GetShieldHealRate()
    {
        return _shieldHealRate;
    }

    #endregion


}
