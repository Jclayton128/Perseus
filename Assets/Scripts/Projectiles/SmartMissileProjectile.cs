using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissileProjectile : Projectile
{
    protected override void ExecuteLifetimeExpirationSequence()
    {
        ExecuteGenericExpiration_Fizzle();
    }

    protected override void ExecuteMovement()
    {
        // Steer to minimize theta
    }

    protected override void ExecuteUpdateSpecifics()
    {
        // Set theta to desired steer direction toward desired point
    }

    protected override void SetupInstanceSpecifics()
    {
        _rb.velocity =
            _launchingWeaponHandler.GetInitialProjectileVelocity(transform);
    }
}
