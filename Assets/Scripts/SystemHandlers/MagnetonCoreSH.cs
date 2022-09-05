using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetonCoreSH : SystemHandler
{
    HealthHandler _playerHealth;

    [SerializeField] float _shieldMaxAddition = 20f;
    float _originalShieldRegenRate;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);

        _playerHealth = GetComponentInParent<HealthHandler>();

        _originalShieldRegenRate = _playerHealth.GetShieldHealRate();

        _playerHealth.AdjustShieldMaximum(_shieldMaxAddition);
        _playerHealth.AdjustShieldHealRate(-99f);

        
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _playerHealth.AdjustShieldMaximum(-_shieldMaxAddition);
        _playerHealth.AdjustShieldHealRate(_originalShieldRegenRate);
    }
    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemDowngrade()
    {
        _playerHealth.AdjustShieldMaximum(-_shieldMaxAddition);
    }

    protected override void ImplementSystemUpgrade()
    {
        _playerHealth.AdjustShieldMaximum(_shieldMaxAddition);
    }
}
