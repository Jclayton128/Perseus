using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    

    [SerializeField] ParticleSystem _hullDamageFXprefab = null;

    internal void RequestShieldDamageParticles(int particlesToMake, Vector3 position, Vector2 impactHeading)
    {
        Quaternion rot = Quaternion.LookRotation(impactHeading, Vector3.forward);
        ParticleSystem ps = Instantiate(_hullDamageFXprefab.gameObject, position, rot).GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule psem = ps.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0,particlesToMake);
        psem.SetBurst(0, burst);
        ps.Play();
    }

    [ContextMenu("Spawn Random hull particles")]
    private void DebugShieldDamage()
    {
        RequestShieldDamageParticles(10, Vector3.one, new Vector2(0, -1));
    }
}
