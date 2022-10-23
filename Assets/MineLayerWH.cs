using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLayerWH : WeaponHandler
{

    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost))
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            LayMine();
        }
    }

    private void LayMine()
    {
        Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);
        pb.SetupInstance(this);
        if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        
    }

    protected override void ImplementWeaponUpgrade()
    {
        
    }

    protected override void InitializeWeaponSpecifics()
    {
        
    }
}
