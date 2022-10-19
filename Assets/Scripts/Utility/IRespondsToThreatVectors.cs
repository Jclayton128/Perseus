using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRespondsToThreatVectors
{
    public Vector2 ThreatVector { get; }

    public void SetThreatVector(Vector2 threatVector);

}
