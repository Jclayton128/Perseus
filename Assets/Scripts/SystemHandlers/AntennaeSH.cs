using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaeSH : SystemHandler
{
    Radar _hostRadar;
    CameraController _cameraController;

    //settings
    [SerializeField] float _errorReduction_Upgrade = 7f;
    [SerializeField] float _radiusIncrease_Upgrade = 5f;
    [SerializeField] float _cameraFovToAdd_Upgrade = 5f;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _hostRadar = GetComponentInParent<Radar>();
        _hostRadar.ModifyArrivalError(-_errorReduction_Upgrade);
        _hostRadar.ModifyRadarRange(_radiusIncrease_Upgrade);
        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.ModifyCameraFOV(_cameraFovToAdd_Upgrade);
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _hostRadar.ModifyArrivalError(_errorReduction_Upgrade);
        _hostRadar.ModifyRadarRange(-_radiusIncrease_Upgrade);
        _cameraController.ModifyCameraFOV(-_cameraFovToAdd_Upgrade);
    }
    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementSystemUpgrade()
    {
        _hostRadar.ModifyArrivalError(-_errorReduction_Upgrade);
        _hostRadar.ModifyRadarRange(_radiusIncrease_Upgrade);
        _cameraController.ModifyCameraFOV(_cameraFovToAdd_Upgrade);
    }

    protected override void ImplementSystemDowngrade()
    {
        _hostRadar.ModifyArrivalError(_errorReduction_Upgrade);
        _hostRadar.ModifyRadarRange(-_radiusIncrease_Upgrade);
        _cameraController.ModifyCameraFOV(-_cameraFovToAdd_Upgrade);
    }
}
