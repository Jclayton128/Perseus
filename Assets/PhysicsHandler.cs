using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhysicsHandler : MonoBehaviour
{
    public Action OnSpecsUpdate;


    //state
    public float Thrust = 10f;
    public float Mass = 2f;
    public float TurnRate = 50f;

    public SpecsPack GetUpdatedSpecsPack()
    {
        SpecsPack specs = new SpecsPack();
        specs.Thrust = Thrust;
        specs.Mass = Mass;
        specs.TurnRate = TurnRate;

        return specs;
    }

}

public struct SpecsPack
{
    public float Thrust;
    public float Mass;
    public float TurnRate;
}
