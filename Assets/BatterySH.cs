using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySH : SystemHandler
{
    EnergyHandler _energyHandler;

    [SerializeField] float _maxEnergyAddition = 10f;

    [Header("Upgrades")]
    [SerializeField] float _maxEnergyAddition_Upgrade = 10f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _energyHandler = GetComponentInParent<EnergyHandler>();
        _energyHandler.ModifyMaxEnergyLevel(_maxEnergyAddition);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _energyHandler.ModifyMaxEnergyLevel(-_maxEnergyAddition);
    }

    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemDowngrade()
    {
        _energyHandler.ModifyMaxEnergyLevel(-_maxEnergyAddition_Upgrade);
    }

    protected override void ImplementSystemUpgrade()
    {
        _energyHandler.ModifyMaxEnergyLevel(_maxEnergyAddition_Upgrade);
    }
}
