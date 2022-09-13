using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverCannonSH : WeaponHandler
{
    LevelController _levelController;

    //settings
    int _chargesPerUpgrade = 1;
    [SerializeField] float _shotLifetime = 10f;
    [SerializeField] float _shotSpeed = 10f;

    //state
    Vector2Int _chargeStatus = new Vector2Int(1,1);


    public override object GetUIStatus()
    {
        return _chargeStatus;
    }
    private void UpdateUI()
    {
        _connectedWID?.UpdateUI(_chargeStatus);
    }

    protected override void ActivateInternal()
    {
       if (_chargeStatus.x > 0)
        {
            Fire();
            _chargeStatus.x--;
            UpdateUI();
        }
    }

    private void Fire()
    {
        DamagePack dp = new DamagePack(_normalDamage, _shieldBonusDamage, _ionDamage, _knockBackAmount, _scrapBonus);
        ProjectileBrain pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupBrain(ProjectileBrain.Behaviour.Bolt, ProjectileBrain.Allegiance.Player,
            ProjectileBrain.DeathBehaviour.Fizzle, _shotLifetime, -1, dp, Vector3.zero); ;
        pb.GetComponent<Rigidbody2D>().velocity = (Vector3)_rb.velocity + pb.transform.up * _shotSpeed;

        _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation);

        if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {

    }

    protected override void ImplementWeaponUpgrade()
    {
        _chargeStatus.y += _chargesPerUpgrade;
        UpdateUI();
    }

    protected override void InitializeWeaponSpecifics()
    {
        _levelController = FindObjectOfType<LevelController>();
        _levelController.OnWarpIntoNewLevel += ReactToLevelWarp;
    }

    private void OnDestroy()
    {
        _levelController.OnWarpIntoNewLevel -= ReactToLevelWarp;
    }

    private void ReactToLevelWarp(Level throwawayParamForLevel)
    {
        _chargeStatus.x = _chargeStatus.y;
        UpdateUI();
    }
}
