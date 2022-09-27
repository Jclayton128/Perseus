using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMissileLauncher
{
    public Vector3 GetTargetPosition();

    public Transform GetTargetTransform();

    public float GetSpeedSpec();

    public float GetTurnSpec();

    public float GetSnakeAmount();

    public int GetLegalTargetsLayerMask();

    public float GetMissileScanRadius();

}
