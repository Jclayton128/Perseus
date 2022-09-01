using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornShieldSH : SystemHandler
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

    public override void ImplementSystemUpgrade()
    {
        
    }
}
