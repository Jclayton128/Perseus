using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    public enum ProjectileType { None, LaserBolt, Shuriken,
        HomingMissile, Torpedo, PopRocket, Harpoon}

    [SerializeField] GameObject _weaponPrefab = null;


    //state
    Queue<GameObject> _unusedPool = new Queue<GameObject>();
    List<GameObject> _activePool = new List<GameObject>();

    public ProjectileHandler SpawnProjectile(ProjectileType weaponType, Transform muzzle)
    {
        GameObject go;
        if (_unusedPool.Count == 0)
        {
            go = Instantiate(_weaponPrefab, Vector3.zero, Quaternion.identity) ;
            
        }
        else
        {
            go = _unusedPool.Dequeue();
        }
        _activePool.Add(go);
        go.transform.position = muzzle.position;
        go.transform.rotation = muzzle.rotation;
        
        return go.GetComponent<ProjectileHandler>();

    }

    private void TransmogrifyProjectile(ProjectileType projType)
    {

    }
}
