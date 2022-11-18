using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    //Shield settings
    [Tooltip("This multiplies the number of particles to make when an FX is spawned. Higher is more.")]
    [SerializeField] float _shieldGloryFactor = 1;
    List<ParticleSystem> _activeShieldParticles = new List<ParticleSystem>();
    Queue<ParticleSystem> _pooledShieldParticles = new Queue<ParticleSystem>();
    [SerializeField] ParticleSystem _shieldDamageFXprefab = null;

    //Hull settings
    [Tooltip("This multiplies the number of particles to make when an FX is spawned. Higher is more.")]
    [SerializeField] float _hullGloryFactor = 1;
    List<ParticleSystem> _activeHullParticles = new List<ParticleSystem>();
    Queue<ParticleSystem> _pooledHullParticles = new Queue<ParticleSystem>();
    [SerializeField] ParticleSystem _hullDamageFXprefab = null;

    //Hull settings
    [Tooltip("This multiplies the number of particles to make when an FX is spawned. Higher is more.")]
    [SerializeField] float _blastGloryFactor = 1;
    List<ParticleSystem> _activeBlastParticles = new List<ParticleSystem>();
    Queue<ParticleSystem> _pooledBlastParticles = new Queue<ParticleSystem>();
    [SerializeField] ParticleSystem _blastDamageFXprefab = null;

    private void Awake()
    {
        GetComponent<LevelController>().WarpingOutFromOldLevel += ReturnAllParticles;
    }


    /// <summary>
    /// Spawns a short-lived, high-speed burst of shield energy particles right at the point of collision 
    /// in an opposite heading of the impact.
    /// </summary>
    /// <param name="particlesToMake"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="impactHeading"></param>
    internal void RequestShieldDamageParticles(int particlesToMake, 
        Vector3 spawnPosition, Vector2 impactHeading)
    {    
        if (particlesToMake <= 0) return;

        ParticleSystem ps;
        Quaternion rot = Quaternion.LookRotation(impactHeading, Vector3.forward);

        if (_pooledShieldParticles.Count == 0)
        {            
            ps = Instantiate(_shieldDamageFXprefab.gameObject, spawnPosition, rot).GetComponent<ParticleSystem>();
            ps.GetComponent<ParticleSystemHandler>().Initalize(this);
        }
        else
        {
            ps = _pooledShieldParticles.Dequeue();
            ps.gameObject.SetActive(true);
            ps.transform.position = spawnPosition;
            ps.transform.rotation = rot;
        }
        _activeShieldParticles.Add(ps);
        int count = Mathf.RoundToInt(particlesToMake * _shieldGloryFactor);
        ps.Emit(count);

    }

    public void ReturnAllParticles()
    {
        if (_activeShieldParticles.Count > 0)
        {
            for (int i = _activeShieldParticles.Count-1; i > 0; i--)
            {
                ReturnParticle(_activeShieldParticles[i]);
            }
        }

        if (_activeHullParticles.Count > 0)
        {
            for (int i = _activeHullParticles.Count-1; i > 0; i--)
            {
                ReturnParticle(_activeHullParticles[i]);
            }
        }

        if (_activeBlastParticles.Count > 0)
        {
            for (int i = _activeBlastParticles.Count-1; i > 0; i--)
            {
                ReturnParticle(_activeBlastParticles[i]);
            }
        }
    }

    public void ReturnParticle(ParticleSystem completedParticleSystem)
    {
        completedParticleSystem.gameObject.SetActive(false);
        switch (completedParticleSystem.GetComponent<ParticleSystemHandler>().ThisParticleType)
        {
            case ParticleSystemHandler.ParticleType.Shield:
                _activeShieldParticles.Remove(completedParticleSystem);
                _pooledShieldParticles.Enqueue(completedParticleSystem);
                break;

            case ParticleSystemHandler.ParticleType.Hull:
                _activeHullParticles.Remove(completedParticleSystem);
                _pooledHullParticles.Enqueue(completedParticleSystem);
                break;

            case ParticleSystemHandler.ParticleType.Blast:
                _activeBlastParticles.Remove(completedParticleSystem);
                _pooledBlastParticles.Enqueue(completedParticleSystem);
                break;

        }

    }

    /// <summary>
    /// Fires a longer-lived, low-speed scattering of hull-like bits set back from the impact point
    /// to make it seem like it is coming more from the ship itself.
    /// </summary>
    /// <param name="particlesToMake"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="impactHeading"></param>
    public void RequestHullDamageParticles(int particlesToMake, 
        Vector3 spawnPosition, Vector3 impactHeading)
    {
        if (particlesToMake <= 0) return;

        ParticleSystem ps;
        Quaternion rot = Quaternion.LookRotation(impactHeading, Vector3.forward);

        Vector3 modSpawnPos = spawnPosition + (impactHeading.normalized * 0.3f);

        if (_pooledHullParticles.Count == 0)
        {
            ps = Instantiate(_hullDamageFXprefab.gameObject, modSpawnPos, rot).GetComponent<ParticleSystem>();
            ps.GetComponent<ParticleSystemHandler>().Initalize(this);
        }
        else
        {
            ps = _pooledHullParticles.Dequeue();
            ps.gameObject.SetActive(true);
            ps.transform.position = modSpawnPos;
            ps.transform.rotation = rot;
        }
        _activeHullParticles.Add(ps);
        int count = Mathf.RoundToInt(particlesToMake * _hullGloryFactor);
        //Debug.Log($"spawning {particlesToMake} hull FX");
        ps.Emit(count);
    }

    public void RequestBlastParticles(int particlesToMake, float blastRange,
        Vector3 spawnPosition)
    {
        if (particlesToMake <= 0) return;

        ParticleSystem ps;


        if (_pooledBlastParticles.Count == 0)
        {
            ps = Instantiate(_blastDamageFXprefab.gameObject, spawnPosition, 
                Quaternion.identity).GetComponent<ParticleSystem>();
            ps.GetComponent<ParticleSystemHandler>().Initalize(this);
        }
        else
        {
            ps = _pooledBlastParticles.Dequeue();
            ps.gameObject.SetActive(true);
            ps.transform.position = spawnPosition;
        }
        ParticleSystem.MainModule psm = ps.main;
        psm.startLifetime = blastRange / psm.startSpeed.constantMin;
        _activeBlastParticles.Add(ps);
        int count = Mathf.RoundToInt(particlesToMake * _blastGloryFactor);
        //Debug.Log($"spawning {particlesToMake} hull FX");
        ps.Emit(count);
    }

}
