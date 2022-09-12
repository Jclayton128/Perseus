using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterburnerSH : SystemHandler
{
    ActorMovement _hostActorMovement;

    //settings
    [SerializeField] float _thrustEnergyCostRate_Initial = 4f;
    [SerializeField] float _thrustEnergyCostRate_Upgrade = 0.5f;
    [SerializeField] float _thrustIncrease_Initial = 100f;
    [SerializeField] float _thrustIncrease_Upgrade = 60f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _hostActorMovement = GetComponentInParent<ActorMovement>();
        _hostActorMovement.SetThrustEnergyCost(_thrustEnergyCostRate_Initial);
        _hostActorMovement.ModifyThrust(_thrustIncrease_Initial);
        
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _hostActorMovement.SetThrustEnergyCost(0);
        _hostActorMovement.ModifyThrust(-_thrustIncrease_Initial);
    }


    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemUpgrade()
    {
        _hostActorMovement.ModifyThrust(_thrustIncrease_Upgrade);
        _hostActorMovement.ModifyThrustEnergyCost(-_thrustEnergyCostRate_Upgrade);
    }
    protected override void ImplementSystemDowngrade()
    {
        _hostActorMovement.ModifyThrust(-_thrustIncrease_Upgrade);
        _hostActorMovement.ModifyThrustEnergyCost(_thrustEnergyCostRate_Upgrade);
    }
}
