using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTurretWH : WeaponHandler
{
    protected override void ActivateInternal()
    {
        throw new System.NotImplementedException();
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        throw new System.NotImplementedException();
    }

    public override object GetUIStatus()
    {
        return "archer";
    }

    protected override void ImplementWeaponUpgrade()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeWeaponSpecifics()
    {

    }
}
