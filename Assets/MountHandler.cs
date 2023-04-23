using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MountHandler : SerializedMonoBehaviour
{
    [SerializeField] List<Transform> _totalWeaponMounts = new List<Transform>();
    [SerializeField]
    Dictionary<SystemWeaponLibrary.SystemLocation, SystemMountDescription>
        _totalSystemMounts = new Dictionary<SystemWeaponLibrary.SystemLocation, SystemMountDescription>();

    //state
    Queue<int> _availableWeaponMounts = new Queue<int>();

    private void Awake()
    {
        for (int i = 0; i < _totalWeaponMounts.Count; i++)
        {
            _availableWeaponMounts.Enqueue(i);
        }
    }

    public Transform GetWeaponMountTransform(int mountIndex)
    {
        return _totalWeaponMounts[mountIndex];
    } 

    public SystemMountDescription GetSystemMountDescription(SystemWeaponLibrary.SystemLocation location)
    {
        return _totalSystemMounts[location];
    }

    public int RequisitionWeaponMountIndex()
    {
        if (_availableWeaponMounts.Count == 0)
        {
            Debug.LogWarning("No mounts remaining");
            return -1;
        }

        int index = _availableWeaponMounts.Dequeue();

        Debug.Log($"assigning weapon mount index {index}. {_availableWeaponMounts.Count} remaining");
        return index;
    }

    public void ReturnWeaponMount(int unneededIndex)
    {
        Debug.Log($"returning weapon mount index {unneededIndex}. {_availableWeaponMounts.Count} remaining");
        _availableWeaponMounts.Enqueue(unneededIndex);
    }
}
