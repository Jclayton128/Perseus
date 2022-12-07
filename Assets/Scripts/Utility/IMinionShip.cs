using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMinionShip
{
    public void InitializeWithAssignedMothership(IMothership mothership, Transform mothershipTransform, 
        Vector2 initialVelocity);

    public void AssignTarget(Vector3 targetPosition, Vector3 targetVelocity);

    public void KillMinionUponMothershipDeath();

}
