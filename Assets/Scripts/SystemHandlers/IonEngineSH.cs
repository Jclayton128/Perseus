using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonEngineSH : SystemHandler
{
    ActorMovement _hostActorMovement;

    //settings
    [SerializeField] float _decreaseInThrustProfileRate_Upgrade = 2f;
    Color _oldEngineColor;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _hostActorMovement = GetComponentInParent<ActorMovement>();
        _hostActorMovement.ModifyThrustProfileIncreaseRate(-_decreaseInThrustProfileRate_Upgrade);
        _oldEngineColor = _hostActorMovement.SwapParticleColor(Color.blue);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _hostActorMovement.ModifyThrustProfileIncreaseRate(_decreaseInThrustProfileRate_Upgrade);
        _hostActorMovement.SwapParticleColor(_oldEngineColor);
    }

    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemUpgrade()
    {
        _hostActorMovement.ModifyThrustProfileIncreaseRate(-_decreaseInThrustProfileRate_Upgrade);
    }
    protected override void ImplementSystemDowngrade()
    {
        _hostActorMovement.ModifyThrustProfileIncreaseRate(_decreaseInThrustProfileRate_Upgrade);
    }
}
