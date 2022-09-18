using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{   
    [SerializeField] GameObject[] _projectilePrefabs = null;
    SystemWeaponLibrary _sysLib;

    //state
    Dictionary<ProjectileBrain.PType, Queue<ProjectileBrain>> _unusedPools = 
        new Dictionary<ProjectileBrain.PType, Queue<ProjectileBrain>>();
    Dictionary<ProjectileBrain.PType, List<ProjectileBrain>> _activePools =
        new Dictionary<ProjectileBrain.PType, List<ProjectileBrain>>();
    Dictionary<ProjectileBrain.PType, GameObject>_projectileMenu = 
        new Dictionary<ProjectileBrain.PType, GameObject>();

    private void Awake()
    {
        _sysLib = FindObjectOfType<SystemWeaponLibrary>();
    }

    private void Start()
    {
        PrepareProjectileMenu();
    }

    private void PrepareProjectileMenu()
    {
        foreach (var projectile in _projectilePrefabs)
        {
            ProjectileBrain.PType ptype =
                projectile.GetComponent<ProjectileBrain>().pType;

            if (!_projectileMenu.ContainsKey(ptype))
            {
                _projectileMenu.Add(ptype, projectile);

                Queue<ProjectileBrain> newQueue = new Queue<ProjectileBrain>();
                _unusedPools.Add(ptype, newQueue);

                List<ProjectileBrain> newList = new List<ProjectileBrain>();
                _activePools.Add(ptype, newList);

                //Debug.Log($"added {ptype} to menu");
            }
            else
            {
                Debug.Log($"Projectile Menu already contains a {ptype}");
            }
        }
    }

    public ProjectileBrain SpawnProjectile(ProjectileBrain.PType projectileType, Transform muzzle)
    {
        Debug.Log($"Asked to spawn a {projectileType}");
        ProjectileBrain pb;
        if (_unusedPools[projectileType].Count == 0)
        {
            pb = Instantiate(_projectileMenu[projectileType], Vector3.zero, Quaternion.identity)
                .GetComponent<ProjectileBrain>();
            pb.Initialize(this);
            
        }
        else
        {
            pb = _unusedPools[projectileType].Dequeue();
            pb.gameObject.SetActive(true);
        }

        _activePools[projectileType].Add(pb);
        pb.transform.position = muzzle.position;
        pb.transform.rotation = muzzle.rotation;
        
        return pb;

    }

    public void ReturnDeadProjectile(ProjectileBrain deadProjectile)
    {
        _unusedPools[deadProjectile.pType].Enqueue(deadProjectile);
        _activePools[deadProjectile.pType].Remove(deadProjectile);
        deadProjectile.gameObject.SetActive(false);
    }

}
