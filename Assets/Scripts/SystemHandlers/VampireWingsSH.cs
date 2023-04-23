using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireWingsSH : SystemHandler
{
    PlayerStateHandler _psh;
    HealthHandler _health;

    //settings
    [SerializeField] float _startingShieldGainPerScrap = 0.7f;

    //state
    float _shieldGainPerScrap;

    //upgrade
    [Header("Upgrade Parameters")]
    [SerializeField] float _shieldGainPerScrap_Upgrade = 0.7f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _psh = GetComponentInParent<PlayerStateHandler>();
        _health = GetComponentInParent<HealthHandler>();
        _psh.ScrapGained += HandleScrapGained;
        _shieldGainPerScrap = _startingShieldGainPerScrap;
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _psh.ScrapGained -= HandleScrapGained;
    }

    private void HandleScrapGained()
    {
        _health.HealCurrentShieldPoints(_shieldGainPerScrap);
    }
    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemUpgrade()
    {
        _shieldGainPerScrap += _shieldGainPerScrap_Upgrade;
    }
    protected override void ImplementSystemDowngrade()
    {
        _shieldGainPerScrap -= _shieldGainPerScrap_Upgrade;
    }
}
