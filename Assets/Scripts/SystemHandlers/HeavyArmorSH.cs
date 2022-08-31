using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyArmorSH : SystemHandler
{
    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
    }


    public override object GetUIStatus()
    {
        throw new System.NotImplementedException();
    }

    public override void ImplementSystemUpgrade()
    {
        throw new System.NotImplementedException();
    }
}
