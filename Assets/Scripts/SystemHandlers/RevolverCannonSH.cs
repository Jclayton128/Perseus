using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverCannonSH : WeaponHandler
{
    LevelController _levelController;

    //settings
    int _chargesPerUpgrade = 1;
    [SerializeField] int _resilience = 6;


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
        else
        {
            if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomDeactivationClip());
        }
    }

    private void Fire()
    {
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);
        pb.SetResilience(_resilience);
        _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation);

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
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
        _levelController.WarpedIntoNewLevel += ReactToLevelWarp;
    }

    private void OnDestroy()
    {
        _levelController.WarpedIntoNewLevel -= ReactToLevelWarp;
    }

    private void ReactToLevelWarp(Level throwawayParamForLevel)
    {
        _chargeStatus.x = _chargeStatus.y;
        UpdateUI();
    }


}
