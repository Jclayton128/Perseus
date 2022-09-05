using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireWingsSH : SystemHandler
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

    }
    protected override void ImplementSystemDowngrade()
    {

    }
}
