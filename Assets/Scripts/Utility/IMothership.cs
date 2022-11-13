using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMothership
{

    public void AlertAllMinionsToTargetTransform
        (Vector3 targetPosition, Vector3 targetVelocity);

    public void RemoveDeadMinion(IMinionShip deadShip);

}
