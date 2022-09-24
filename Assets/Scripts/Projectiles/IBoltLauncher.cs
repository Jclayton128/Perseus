using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoltLauncher 
{
    public Vector3 GetInitialBoltVelocity(Transform boltTransform);
}
