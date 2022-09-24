using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{   
    [SerializeField] GameObject[] _projectilePrefabs = null;
    SystemWeaponLibrary _sysLib;

    //state
    Dictionary<Projectile.ProjectileType, Queue<Projectile>> _unusedPools = 
        new Dictionary<Projectile.ProjectileType, Queue<Projectile>>();
    Dictionary<Projectile.ProjectileType, List<Projectile>> _activePools =
        new Dictionary<Projectile.ProjectileType, List<Projectile>>();
    Dictionary<Projectile.ProjectileType, GameObject>_projectileMenu = 
        new Dictionary<Projectile.ProjectileType, GameObject>();

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
            Projectile.ProjectileType ptype =
                projectile.GetComponent<Projectile>().PType;

            if (!_projectileMenu.ContainsKey(ptype))
            {
                _projectileMenu.Add(ptype, projectile);

                Queue<Projectile> newQueue = new Queue<Projectile>();
                _unusedPools.Add(ptype, newQueue);

                List<Projectile> newList = new List<Projectile>();
                _activePools.Add(ptype, newList);

                //Debug.Log($"added {ptype} to menu");
            }
            else
            {
                Debug.Log($"Projectile Menu already contains a {ptype}");
            }
        }
    }

    public Projectile SpawnProjectile(Projectile.ProjectileType projectileType, Transform muzzle)
    {
        Debug.Log($"Asked to spawn a {projectileType}");
        Projectile pb;
        if (_unusedPools[projectileType].Count == 0)
        {
            pb = Instantiate(_projectileMenu[projectileType], Vector3.zero, Quaternion.identity)
                .GetComponent<Projectile>();
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

    public void ReturnDeadProjectile(Projectile deadProjectile)
    {
        _unusedPools[deadProjectile.PType].Enqueue(deadProjectile);
        _activePools[deadProjectile.PType].Remove(deadProjectile);
        deadProjectile.gameObject.SetActive(false);
    }

}
