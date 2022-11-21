using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoscaleHullSH : SystemHandler
{
    HealthHandler _healthHandler;
    LevelController _levelController;

    //settings
    [SerializeField] float _hullRegeneration_Install = 1.5f;
    [SerializeField] float _hullRegenerationAddition_Upgrade = 1.5f;

    //state
    float _hullRegenerationAmount = 0;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _healthHandler = GetComponentInParent<HealthHandler>();
        _levelController = FindObjectOfType<LevelController>();
        _levelController.WarpingIntoNewLevel += HandleWarpedToNewLevel;
        _hullRegenerationAmount = _hullRegeneration_Install;

    }

    private void HandleWarpedToNewLevel()
    {
        if (_healthHandler.HullPoints == _healthHandler.MaxHullPoints)
        {
            _healthHandler.AdjustHullMaximumAndCurrent(_hullRegenerationAmount / 2f);
        }
        else
        {
            _healthHandler.AdjustCurrentHullPoints(_hullRegenerationAmount);
        }

    }

    public override object GetUIStatus()
    {
        
        return null;
    }

    protected override void ImplementSystemDowngrade()
    {
        //shouldn't matter; doesn't passively affect any systems other than itself.
    }

    protected override void ImplementSystemUpgrade()
    {
        _hullRegenerationAmount += _hullRegenerationAddition_Upgrade;
    }

}
