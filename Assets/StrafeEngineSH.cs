using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeEngineSH : SystemHandler
{
    //settings
    [SerializeField] Color _newStrafeColor = Color.yellow;
    [Header("Upgrades")]
    [SerializeField] float _strafeMultiplier_Upgrade = 1.25f;


    //state
    ActorMovement _hostActorMovement;
    float _oldStrafeThrust;
    Color _oldStrafeColor;


    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _hostActorMovement = GetComponentInParent<ActorMovement>();
        _oldStrafeThrust = _hostActorMovement.StrafeThrust;
        float currentStrafeThrust = _hostActorMovement.StrafeThrust;
        _hostActorMovement.ModifyThrust(currentStrafeThrust * _strafeMultiplier_Upgrade);
        _oldStrafeColor = _hostActorMovement.SwapStrafeParticleColor(_newStrafeColor);

    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _hostActorMovement.SetStrafe(_oldStrafeThrust);
        _hostActorMovement.SwapStrafeParticleColor(_oldStrafeColor);
    }



    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemDowngrade()
    {
        
    }

    protected override void ImplementSystemUpgrade()
    {
        float currentStrafeThrust = _hostActorMovement.StrafeThrust;
        _hostActorMovement.ModifyStrafe(currentStrafeThrust * _strafeMultiplier_Upgrade);
    }
}
