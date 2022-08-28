using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWH : WeaponHandler
{
    public override void Activate()
    {
        throw new System.NotImplementedException();
    }

    public override void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    public override object GetUIStatus()
    {        
        return  0.2f;
    }

    protected override void ImplementWeaponUpgrade()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeWeaponSpecifics()
    {
        
    }
}
