using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    //settings
    [Tooltip("This multiplies the number of particles to make when an FX is spawned. Higher is more.")]
    [SerializeField] float _shieldGloryFactor = 1;
    

    [SerializeField] ParticleSystem _shieldDamageFXprefab = null;

    internal void RequestShieldDamageParticles(int particlesToMake, Vector3 position, Vector2 impactHeading)
    {
        Quaternion rot = Quaternion.LookRotation(impactHeading, Vector3.forward);
        ParticleSystem ps = Instantiate(_shieldDamageFXprefab.gameObject, position, rot).GetComponent<ParticleSystem>();
        int count = Mathf.RoundToInt(particlesToMake * _shieldGloryFactor);
        ps.Emit(count);
    }

}
