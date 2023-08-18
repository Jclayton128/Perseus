using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Cockpit2SH : SystemHandler
{

    //settings
    [SerializeField] WeaponHandler _weaponHandler = null;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        LevelController lc = FindObjectOfType<LevelController>();
        lc.WarpedIntoNewLevel += ActivateBowWave;
        _weaponHandler.Initialize(null, true, null);
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
        
    }
    protected override void ImplementSystemDowngrade()
    {

    }

    private void ActivateBowWave(Level throwaway)
    {
        _weaponHandler.Activate();
    }

}
