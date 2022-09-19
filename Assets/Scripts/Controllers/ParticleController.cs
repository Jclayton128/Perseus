using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    //settings
    [Tooltip("This multiplies the number of particles to make when an FX is spawned. Higher is more.")]
    [SerializeField] float _shieldGloryFactor = 1;

    List<ParticleSystem> _activeParticles = new List<ParticleSystem>();
    Queue<ParticleSystem> _pooledParticles = new Queue<ParticleSystem>();

    [SerializeField] ParticleSystem _shieldDamageFXprefab = null;

    internal void RequestShieldDamageParticles(int particlesToMake, 
        Vector3 spawnPosition, Vector2 impactHeading)
    {    
        if (particlesToMake <= 0) return;

        ParticleSystem ps;
        Quaternion rot = Quaternion.LookRotation(impactHeading, Vector3.forward);

        if (_pooledParticles.Count == 0)
        {            
            ps = Instantiate(_shieldDamageFXprefab.gameObject, spawnPosition, rot).GetComponent<ParticleSystem>();
            ps.GetComponent<ParticleSystemHandler>().Initalize(this);
        }
        else
        {
            ps = _pooledParticles.Dequeue();
            ps.gameObject.SetActive(true);
            ps.transform.position = spawnPosition;
            ps.transform.rotation = rot;
        }
        _activeParticles.Add(ps);
        int count = Mathf.RoundToInt(particlesToMake * _shieldGloryFactor);
        ps.Emit(count);

    }

    public void ReturnParticleSystem(ParticleSystem completedParticleSystem)
    {
        completedParticleSystem.gameObject.SetActive(false);
        _activeParticles.Remove(completedParticleSystem);
        _pooledParticles.Enqueue(completedParticleSystem);
    }

}
