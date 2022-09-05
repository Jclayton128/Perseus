using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeHullSH : SystemHandler
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
        return null;
    }

    protected override void ImplementSystemUpgrade()
    {
        throw new System.NotImplementedException();
    }
    protected override void ImplementSystemDowngrade()
    {

    }
}
