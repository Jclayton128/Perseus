using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamscoopReactorSH : SystemHandler
{
    Rigidbody2D _hostRB;
    EnergyHandler _hostEnergyHandler;

    [Header("Upgrades")]
    [SerializeField] float _energyVelocityRateAddition_Upgrade = 1f;

    //state
    float _currentEnergyVelocityRate = 1f;
    float _oldEnergyRegenRate;

    public override object GetUIStatus()
    {
        return null;
    }

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _hostEnergyHandler = GetComponentInParent<EnergyHandler>();
        _hostRB = GetComponentInParent<Rigidbody2D>();
        _oldEnergyRegenRate = _hostEnergyHandler.EnergyGainRate;
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _hostEnergyHandler.SetEnergyRegenRate(_oldEnergyRegenRate);
    }

    protected override void ImplementSystemDowngrade()
    {

    }

    protected override void ImplementSystemUpgrade()
    {
        _currentEnergyVelocityRate += _energyVelocityRateAddition_Upgrade;   
    }

    private void Update()
    {
        float mag = _hostRB.velocity.magnitude;
        _hostEnergyHandler.SetEnergyRegenRate(mag * _currentEnergyVelocityRate);
    }
}
