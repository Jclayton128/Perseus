using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemHandler : MonoBehaviour
{
    [SerializeField] Sprite _icon = null;
    public SystemsLibrary.SystemType SystemType;
    public Vector2 LocalPosition;
    public float MassToAdd;
    public float ThrustToAdd;
    public float TurnRateToAdd;

    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }

    public void IntegrateSystem(PlayerMovementHandler ph)
    {
        ph.ModifyMass(MassToAdd);
        ph.ModifyThrust(ThrustToAdd);
        ph.ModifyTurnRate(TurnRateToAdd);

    }
}
