using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeionizerSH : SystemHandler
{
    HealthHandler _healthHandler;

    //settings
    [SerializeField] float _ionHealRateAddition_Install = 1f;
    [SerializeField] float _ionHealRateAddition_Upgrade = 1f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _healthHandler = GetComponentInParent<HealthHandler>();
        _healthHandler.AdjustIonHealRate(_ionHealRateAddition_Install);
        _connectedID.UpdateUI(_healthHandler.IonHealRate.ToString("0.0"));
        
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _healthHandler.AdjustIonHealRate(-_ionHealRateAddition_Install);
        _connectedID.UpdateUI(_healthHandler.IonHealRate.ToString("0.0"));
    }

    public override object GetUIStatus()
    {
        return "ion";
    }

    protected override void ImplementSystemDowngrade()
    {
        _healthHandler.AdjustIonHealRate(-_ionHealRateAddition_Upgrade);
    }

    protected override void ImplementSystemUpgrade()
    {
        _healthHandler.AdjustIonHealRate(_ionHealRateAddition_Upgrade);
        _connectedID.UpdateUI(_healthHandler.IonHealRate.ToString("0.0"));
    }
}
