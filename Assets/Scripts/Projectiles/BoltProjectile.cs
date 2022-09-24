using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltProjectile : Projectile
{
    protected override void ExecuteLifetimeExpirationSequence()
    {
        ExecuteGenericExpiration_Fizzle();
    }

    protected override void ExecuteMovement()
    {
        // Bolt movement is purely handled by Unity physics
    }

    protected override void ExecuteUpdateSpecifics()
    {
        // Bolts don't update
    }

    protected override void SetupInstanceSpecifics()
    {
        _rb.velocity =
            _launchingWeaponHandler.GetComponent<IBoltLauncher>().GetInitialBoltVelocity(transform);
    }
}
