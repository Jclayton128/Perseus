using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineProjectile : Projectile, IPlayerSeeking
{
    SpriteRenderer _sr;
    ParticleController _particleController;
    [SerializeField] Sprite _safeSprite = null;

    //settings
    float _drag = 2f;
    [SerializeField] float _maxDetectionRange = 10f;
    [SerializeField] float _warningRange = 5f;
    [SerializeField] float _detonationRange = 3f;
    [SerializeField] float _damageRange = 2.5f;
    [SerializeField] float _detonationDelay = 0.5f;


    //state
    bool _isDetonating = false;
    [SerializeField] float _timeToDetonate = Mathf.Infinity;
    Sprite _warningSprite;
    public float PlayerRange { get; private set; } = Mathf.Infinity;
    public override void Initialize(ProjectilePoolController poolController)
    {
        base.Initialize(poolController);
        _sr = GetComponent<SpriteRenderer>();
        _warningSprite = _sr.sprite;
        _particleController = poolController.GetComponent<ParticleController>();    
        GetComponentInChildren<DetectionHandler>().ModifyDetectorRange(_maxDetectionRange);
        _rb.drag = _drag;
    }

    protected override void SetupInstanceSpecifics()
    {
        _isDetonating = false;
        PlayerRange = _maxDetectionRange;
        _sr.sprite = _safeSprite;
        _timeToDetonate = Mathf.Infinity;
        GetComponent<HealthHandler>().ResetCurrentHullAndShieldLevels();
        GetComponent<HealthHandler>().Dying += BeginDetonationSequence;
    }

    protected override void ExecuteMovement()
    {
        // none
    }

    protected override void ExecuteUpdateSpecifics()
    {
        if (Time.time >= _timeToDetonate)
        {
            Detonate(); 
        }
    }

    protected override void ExecuteLifetimeExpirationSequence()
    {
        _particleController.RequestBlastParticles(1, 2f,transform.position);
        ExecuteGenericExpiration_Explode(0, 0);
    }

    private void BeginDetonationSequence()
    {
        if (_isDetonating) return;
        _isDetonating = true;
        _timeToDetonate = Time.time + _detonationDelay;
    }

    private void Detonate()
    {
        _particleController.
            RequestBlastParticles(Mathf.RoundToInt(DamagePack.NormalDamage),
            _damageRange,
            transform.position);
        ExecuteGenericExpiration_Explode(_damageRange, 
            LayerLibrary.PlayerEnemyNeutralLayerMask);
    }


    public void ReportPlayer(Vector2 playerPosition, Vector2 playerVelocity)
    {
        PlayerRange = (playerPosition - (Vector2)transform.position).magnitude;
        _sr.sprite = GetSpriteBasedOnPlayerRange(PlayerRange);
        if (!_isDetonating && PlayerRange < _detonationRange)
        {
            BeginDetonationSequence();
            //Invoke(nameof(Detonate), _detonationDelay);
        }
    }

    private Sprite GetSpriteBasedOnPlayerRange(float playerRange)
    {
        if (!_warningSprite) return _sr.sprite;

        if (playerRange < _warningRange)
        {
            return _warningSprite;
        }
        else
        {
            return _safeSprite;
        }
    }
}
