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
    [SerializeField] ParticleSystem _ionizationParticles = null;
    ParticleSystem.EmissionModule _ipem;

    public event Action<Vector2> ReceivingThreatVector = null;
    public event System.Action<DamagePack> ReceivingDamagePack = null;
    public event System.Action<DamagePack> ReceivingShieldDamage = null;
    public event System.Action<DamagePack> ReceivingHullDamage = null;

    #endregion

    //global settings
    float _minSecondsBetweenDamageFX = 0.125f;
    float _minDelayToBeginRechargingShields = 0.5f;
    float _ionizationGlory = 10f; // multiplied by Ion Factor for Ionization Particle emit rate

    #region Instance Settings
    //instance settings
    [FoldoutGroup("Starting Stats")]
    [Tooltip("Maximum (and starting) Hull Points")]
    [SerializeField] [Range(1, 100)] float _maxHullPoints = 10;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Emits hull damage FX when receiving hull damage. Should be FALSE for asteroids.")]
    [SerializeField] bool _emitsHullChunks = true;


    [FoldoutGroup("Starting Stats")]
    [Tooltip("Maxium (and starting) Shield Points")]
    [SerializeField] [Range(0, 100)] float _maxShieldPoints = 0;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Points of shielding healed per second.")]
    [SerializeField] [Range(0, 100)] float _shieldHealRate = 0;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Points of ionization healed per second. Max Ionization Amount is equal to total Hull Points.")]
    [SerializeField] [Range(0, 10)] float _ionHealRate = 0;

    float _damageInvulnerabilityDuration = 0.2f;
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
    float _timeToAllowShieldDamageFX = 0;
    float _timeToAllowShieldRegen = 0;
    float _timeToAllowHullDamageFX = 0;
    float _gatheredHullDamageForSingleParticleRelease = 0;
    float _timeToAllowDamageAgain = 0; //This is give the BlinkEngine a moment to work.

    
    /// <summary>
    /// If this is TRUE, when this HealthHandler hits zero, the game session ends. Good for the player,
    /// or someother kind of mission essential ship.
    /// </summary>
    bool _shouldEndGameSessionUponDeath = false;
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
            _UIController.UpdateIonizationBars(IonFactor, 1);
            _shouldEndGameSessionUponDeath = true;
        }

        _ionizationPointsAbsorbed = 0;
        IonFactor = 0;
        _ipem = _ionizationParticles.emission;
        _ipem.rateOverTime = IonFactor * _ionizationGlory;
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
        if (!_shouldEndGameSessionUponDeath)
        {
            Vector2 dir = UnityEngine.Random.onUnitSphere;
            Debug.Log($"should spawn {_scrapValue} scraps");
            _scrapController.SpawnScraps(_scrapValue, ((Vector2)transform.position), dir);
        }
        else
        {
            FindObjectOfType<GameController>().EndGameOnPlayerDeath();
        }

        Destroy(gameObject);
    }

    private void UpdateRechargeShield()
    {
        if (Time.time < _timeToAllowShieldRegen) return;

        ShieldPoints += _shieldHealRate * (1-IonFactor) * Time.deltaTime;
        ShieldPoints = Mathf.Clamp(ShieldPoints, 0, _maxShieldPoints);

        if (_movement.IsPlayer)
        {
            _UIController.UpdateShieldBar(ShieldPoints, _maxShieldPoints);
        }
    }

    private void UpdateIonization()
    {
        if (_ionizationPointsAbsorbed < 0) return;
        _ionizationPointsAbsorbed -= _ionHealRate * Time.deltaTime;
        _ionizationPointsAbsorbed = Mathf.Clamp(_ionizationPointsAbsorbed, 0, _maxHullPoints);
        IonFactor = (_ionizationPointsAbsorbed / _maxHullPoints);
        _ipem.rateOverTime = IonFactor * _ionizationGlory;
        if (_movement.IsPlayer)
        {
            _UIController.UpdateIonizationBars(IonFactor, 1);
        }
    }


    #endregion

    #region Receive Damage

    /// <summary>
    /// This grants a very short invulnerability to all DamagePacks. Intended for 
    /// use by the Blink Engine.
    /// </summary>
    public void ActivateDamageInvulnerability()
    {
        if (Time.time > _timeToAllowDamageAgain)
        {
            _timeToAllowDamageAgain = Time.time + _damageInvulnerabilityDuration;
        }

    }


    private void OnTriggerEnter2D(Collider2D weaponImpact)
    {
        Projectile pb;
        if (weaponImpact.TryGetComponent<Projectile>(out pb))
        {
            if (Time.time < _timeToAllowDamageAgain) return;
            Debug.Log("receiving damage pack");
            ReceivingDamagePack?.Invoke(pb.DamagePack);
            ReceivingThreatVector?.Invoke(pb.GetNormalizedVectorAtImpact());
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
        if (Time.time < _timeToAllowDamageAgain) return;
        ReceivingDamagePack?.Invoke(incomingDamage);
        ReceivingThreatVector?.Invoke(impactHeading);
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

        _timeToAllowShieldRegen = Time.time + _minDelayToBeginRechargingShields;

        if (Time.time >= _timeToAllowShieldDamageFX)
        {
            _particleController.RequestShieldDamageParticles(amount, impactPosition, impactHeading);
            _timeToAllowShieldDamageFX = Time.time + _minSecondsBetweenDamageFX;
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

        if (Time.time >= _timeToAllowHullDamageFX)
        {
            int amount = Mathf.RoundToInt(_gatheredHullDamageForSingleParticleRelease + 0.5f);
            _particleController.RequestHullDamageParticles(amount, impactPosition, impactHeading);
            _timeToAllowHullDamageFX = Time.time + _minSecondsBetweenDamageFX;
            _gatheredHullDamageForSingleParticleRelease = 0;
        }
        else
        {
            _gatheredHullDamageForSingleParticleRelease += normalDamage;
        }

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
