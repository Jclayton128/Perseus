using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversionCoreSH : SystemHandler
{
    HealthHandler _playerHealth;
    EnergyHandler _playerEnergy;

    [SerializeField] float _shieldConversionFactor_current = 1;

    [SerializeField] float _shieldConversionMultiplier_upgrade = 1.5f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _playerEnergy = GetComponentInParent<EnergyHandler>();
        _playerHealth = GetComponentInParent<HealthHandler>();
        _playerHealth.ReceivedShieldDamage += ConvertShieldDamageIntoEnergyGain;
    }

    private void ConvertShieldDamageIntoEnergyGain(float shieldDamage)
    {
        _playerEnergy.SpendEnergy(-1 * shieldDamage * _shieldConversionFactor_current);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
    }

   
    public override object GetUIStatus()
    {
        return null;
    }

    

    protected override void ImplementSystemUpgrade()
    {
        _shieldConversionFactor_current *= _shieldConversionMultiplier_upgrade;
    }
    protected override void ImplementSystemDowngrade()
    {

    }
}
