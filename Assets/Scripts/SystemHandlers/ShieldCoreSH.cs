using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCoreSH : SystemHandler
{
    HealthHandler _playerHealth;

    [SerializeField] float _shieldHealRateAdditionPerLevel = 1.3f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _playerHealth = GetComponentInParent<HealthHandler>();
        _playerHealth.AdjustShieldHealRate(_shieldHealRateAdditionPerLevel);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _playerHealth.AdjustShieldHealRate(-_shieldHealRateAdditionPerLevel);
    }

   
    public override object GetUIStatus()
    {
        return null;
    }

    

    protected override void ImplementSystemUpgrade()
    {
        _playerHealth.AdjustShieldHealRate(_shieldHealRateAdditionPerLevel);
    }
    protected override void ImplementSystemDowngrade()
    {
        _playerHealth.AdjustShieldHealRate(-_shieldHealRateAdditionPerLevel);
    }
}
