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
    // 7, 9, 11 = Player, Enemy, and Neutral Ships
    int layerMask_PlayerNeutralEnemy = (1 << 7) | (1 << 9) | (1 << 11);

    //state
    Sprite _warningSprite;
    [SerializeField] float _playerRange = 0;
    public override void Initialize(PoolController poolController)
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
        _playerRange = 0;
        _sr.sprite = _safeSprite;
        GetComponent<HealthHandler>().Dying += Detonate;
    }

    protected override void ExecuteMovement()
    {
        // none
    }

    protected override void ExecuteUpdateSpecifics()
    {
        //none
    }

    protected override void ExecuteLifetimeExpirationSequence()
    {
        _particleController.RequestBlastParticles(1,transform.position);
        Debug.Log("expired");
        ExecuteGenericExpiration_Explode(0, 0);
    }

    private void Detonate()
    {
        _particleController.RequestBlastParticles(Mathf.RoundToInt(DamagePack.NormalDamage),
    transform.position);
        Debug.Log("Detonated");
        ExecuteGenericExpiration_Explode(_damageRange, layerMask_PlayerNeutralEnemy);
    }


    public void ReportPlayer(Vector2 playerPosition, Vector2 playerVelocity)
    {
        _playerRange = (playerPosition - (Vector2)transform.position).magnitude;
        _sr.sprite = GetSpriteBasedOnPlayerRange(_playerRange);
        if (_playerRange < _detonationRange)
        {
            Invoke(nameof(Detonate), _detonationDelay);
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
