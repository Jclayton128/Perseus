using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class HealthHandler : MonoBehaviour
{
    AudioSource _audioSourceAsEnemy;
    public Action<float, float> ShieldPointChanged;
    public Action<float, float> HullPointsChanged;
    public Action<float, float> IonFactorChanged;
    public Action<string, Color> ShieldRegenChanged;
    public Action Dying;

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
    public event Action<float> ReceivedShieldDamage = null;
    public event System.Action<DamagePack> ReceivingHullDamage = null;
    public event Action<float> ReceivedHullDamage = null;

    #endregion

    //global settings
    float _minSecondsBetweenDamageFX = 0.125f;
    float _minDelayToBeginRechargingShields = 0.5f;
    float _ionizationGlory = 10f; // multiplied by Ion Factor for Ionization Particle emit rate

    #region Instance Settings
    //instance settings
    [FoldoutGroup("Starting Stats")]
    [Tooltip("Maximum (and starting) Hull Points")]
    [SerializeField][Range(.1f, 100)] float _maxHullPoints = 10;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Emits hull damage FX when receiving hull damage. Should be FALSE for asteroids.")]
    [SerializeField] bool _emitsHullChunks = true;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Is a ship (ie, releases scrap when killed). Should be FALSE for asteroids, mines, etc.")]
    [SerializeField] bool _isShip = true;


    [FoldoutGroup("Starting Stats")]
    [Tooltip("Maxium (and starting) Shield Points")]
    [SerializeField][Range(0, 100)] float _maxShieldPoints = 0;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Points of shielding healed per second.")]
    [SerializeField][Range(0, 100)] float _shieldHealRate = 0;

    [FoldoutGroup("Starting Stats")]
    [Tooltip("Points of ionization healed per second. Max Ionization Amount is equal to total Hull Points.")]
    [SerializeField][Range(0, 10)] float _ionHealRate = 0;
    public float IonHealRate => _ionHealRate;

    float _damageInvulnerabilityDuration = 0.2f;

    [SerializeField] AudioClip[] _deathSounds = null;
    #endregion;

    #region State
    //state
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float HullPoints { get; protected set; } = 1;

    public float MaxHullPoints => _maxHullPoints;
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float ShieldPoints { get; protected set; } = 0;

    [Tooltip("Ion factor is the percentage of ionization")]
    private float _ionizationPointsAbsorbed = 0;
    //[BoxGroup("Current Stats")]
    [ShowInInspector] public float IonFactor = 0;
    Color _shieldRegenColor;
    bool _isPlayer = false;
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
        _rb = GetComponentInParent<Rigidbody2D>();
        _movement = GetComponent<ActorMovement>();
        _particleController = FindObjectOfType<ParticleController>();
        _scrapController = _particleController.GetComponent<ScrapController>();
        _UIController = _particleController.GetComponent<UI_Controller>();

        ResetCurrentHullAndShieldLevels();
        _scrapValue = Mathf.RoundToInt(_maxHullPoints);

        if (!_isShip) return;
        if (!_movement || _movement.IsPlayer) _shouldEndGameSessionUponDeath = true;

        _ionizationPointsAbsorbed = 0;
        IonFactor = 0;
        if (_ionizationParticles)
        {
            _ipem = _ionizationParticles.emission;
            _ipem.rateOverTime = IonFactor * _ionizationGlory;
        }

        if (!GetComponent<ActorMovement>().IsPlayer)
        {
            _audioSourceAsEnemy = GetComponent<AudioSource>();
            _isPlayer = false;
        }
        else
        {
            _isPlayer = true;
        }


        ShieldPointChanged?.Invoke(ShieldPoints, _maxShieldPoints);
        HullPointsChanged?.Invoke(HullPoints, _maxHullPoints);
        IonFactorChanged?.Invoke(IonFactor, 1);
        ShieldRegenChanged?.Invoke(_shieldHealRate.ToString("F1"), Color.white);
    }

    public void ResetCurrentHullAndShieldLevels()
    {
        HullPoints = _maxHullPoints;
        ShieldPoints = _maxShieldPoints;
        HullPointsChanged?.Invoke(HullPoints, _maxHullPoints);
        ShieldPointChanged?.Invoke(ShieldPoints, _maxShieldPoints);
    }

    internal void ResetShieldsToMax()
    {
        ShieldPoints = _maxShieldPoints;
        ShieldPointChanged?.Invoke(ShieldPoints, _maxShieldPoints);
    }

    #region Flow
    private void Update()
    {
        UpdateDeathCheck();
        if (_isShip)
        {
            UpdateIonization();
            UpdateRechargeShield();
        }
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
        Dying?.Invoke();

        if (!_shouldEndGameSessionUponDeath)
        {
            if (_isShip && !_isPlayer)
            {
                PlayRandomDeathSound();
                SpawnScrapUponDeath();
            }
        }
        else
        {
            FindObjectOfType<GameController>().EndGameOnPlayerDeath();
        }

        if (_isShip) Destroy(gameObject);
    }

    private void PlayRandomDeathSound()
    {
        Debug.Log("should play death sound");
        if (_deathSounds.Length == 0 || !_audioSourceAsEnemy) return;
        int rand = UnityEngine.Random.Range(0, _deathSounds.Length);
        _audioSourceAsEnemy.PlayOneShot(_deathSounds[rand]);
    }

    private void SpawnScrapUponDeath()
    {
        Vector2 dir = UnityEngine.Random.onUnitSphere;
        Debug.Log($"should spawn {_scrapValue} scraps");
        _scrapController.SpawnScraps(_scrapValue, ((Vector2)transform.position), dir);
    }

    private void UpdateRechargeShield()
    {
        if (Time.time < _timeToAllowShieldRegen) return;

        ShieldPoints += _shieldHealRate * (1-IonFactor) * Time.deltaTime;
        ShieldPoints = Mathf.Clamp(ShieldPoints, 0, _maxShieldPoints);
        ShieldPointChanged?.Invoke(ShieldPoints, _maxShieldPoints);

    }

    private void UpdateIonization()
    {
        if (_ionizationPointsAbsorbed < 0) return;
        _ionizationPointsAbsorbed -= _ionHealRate * Time.deltaTime;
        _ionizationPointsAbsorbed = Mathf.Clamp(_ionizationPointsAbsorbed, 0, _maxHullPoints);
        IonFactor = (_ionizationPointsAbsorbed / _maxHullPoints);
        _ipem.rateOverTime = IonFactor * _ionizationGlory;

        IonFactorChanged?.Invoke(IonFactor, 1);
        _shieldRegenColor = Color.Lerp(Color.white, Color.green, IonFactor);
        ShieldRegenChanged?.Invoke((_shieldHealRate * (1 - IonFactor)).ToString("F1"),
            _shieldRegenColor);
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
            ReceivingThreatVector?.Invoke(pb.GetNormalizedVectorAtImpact());
            if (Time.time < _timeToAllowDamageAgain) return;
            ReceivingDamagePack?.Invoke(pb.DamagePack);
            if (pb.DeliversDamageOnlyAtExpiration == false)
            {
                ReceiveDamage(pb.DamagePack, weaponImpact.transform.position, pb.GetNormalizedVectorAtImpact());
            }            
            pb.DecrementPenetrationOnImpact();
        }
    }

    /// <summary>
    /// This is used to force a Health Handler to take damage, such as from a beam weapon that doesn't
    /// otherwise have a projectile.
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <param name="impactPosition"></param>
    /// <param name="impactHeading"></param>
    public void ReceiveNonProjectileDamage(DamagePack incomingDamage, Vector2 impactPosition, Vector2 impactHeading)
    {
        DamagePack localDamage = new DamagePack(incomingDamage);
        if (Time.time < _timeToAllowDamageAgain) return;
        ReceivingDamagePack?.Invoke(localDamage);
        ReceivingThreatVector?.Invoke(impactHeading);
        ReceiveDamage(localDamage, impactPosition, impactHeading);
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
            ReceivedShieldDamage?.Invoke(shieldDamage);
            incomingDamage.NormalDamage = carryoverHullDamage;
            if (incomingDamage.NormalDamage > 0)
            {
                ReceivingHullDamage?.Invoke(incomingDamage);
                ReceiveHullDamage(incomingDamage.NormalDamage, incomingDamage.ScrapBonus, impactPosition, impactHeading);
                ReceivedHullDamage?.Invoke(incomingDamage.NormalDamage);
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

        ShieldPointChanged?.Invoke(ShieldPoints, _maxShieldPoints);
    }

    private void ReceiveHullDamage(float normalDamage, float scrapBonus, Vector2 impactPosition, Vector2 impactHeading)
    {
        HullPoints -= normalDamage;
        _scrapValue += Mathf.RoundToInt(scrapBonus);

        if (_emitsHullChunks)
        {
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
        }

        HullPointsChanged?.Invoke(HullPoints, _maxHullPoints);
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
        ShieldRegenChanged?.Invoke(_shieldHealRate.ToString("F1"), Color.white);

    }

    /// <summary>
    /// Adds to the maximum shield amount. Clamped to range of 0-999 (can't go negative);
    /// </summary>
    /// <param name="shieldMaxAddition"></param>
    public void AdjustShieldMaximum(float shieldMaxAddition)
    {
        _maxShieldPoints += shieldMaxAddition;
        _maxShieldPoints = Mathf.Clamp(_maxShieldPoints, 0, 999);
        ShieldPoints = Mathf.Clamp(ShieldPoints, 0, _maxShieldPoints);
        ShieldPointChanged?.Invoke(ShieldPoints, _maxShieldPoints);
    }

    /// <summary>
    /// Increase both the maximum hull level and current hull level by this much.
    /// </summary>
    /// <param name="hullAddition"></param>
    public void AdjustHullMaximumAndCurrent(float hullAddition)
    {
        _maxHullPoints += hullAddition;
        HullPoints += hullAddition;
        HullPointsChanged?.Invoke(HullPoints, _maxHullPoints);
    }

    /// <summary>
    /// Set the maximum hull level and current hull level to this specific amount. Not the same
    /// as AdjustHullMaximum!
    /// </summary>
    /// <param name="hullAddition"></param>
    public void SetHullMaximumAndCurrent(float newMaxHull)
    {
        _maxHullPoints = newMaxHull;
        HullPoints = _maxHullPoints;
        HullPointsChanged?.Invoke(HullPoints, _maxHullPoints);
    }

    internal void AdjustCurrentHullPoints(float amountToAdd)
    {
        HullPoints += amountToAdd;
        HullPoints = Mathf.Clamp(HullPoints, 0, _maxHullPoints);
        HullPointsChanged?.Invoke(HullPoints, _maxHullPoints);
    }

    public void AdjustIonHealRate(float amountToAdd)
    {
        _ionHealRate += amountToAdd;
    }



    #endregion

    #region Public Gets

    public float GetShieldHealRate()
    {
        return _shieldHealRate;
    }

    #endregion


}
