using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthHullSH : SystemHandler
{
    RadarProfileHandler _rph;

    [Header("Upgrade")]
    [SerializeField] float _maxProfileMultiplier_Upgrade = 0.8f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _rph = transform.root.GetComponentInChildren<RadarProfileHandler>();
        _rph.ModifyProfileMaximum(_maxProfileMultiplier_Upgrade);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _rph.ResetProfileMaximum();
    }


    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemUpgrade()
    {
        _rph.ModifyProfileMaximum(_maxProfileMultiplier_Upgrade);
    }
    protected override void ImplementSystemDowngrade()
    {

    }
}
