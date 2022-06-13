using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemsLibrary : MonoBehaviour
{
    public enum SystemType { None, Engines, HeavyArmor, FasterShieldRegen, MoreShields}
    [SerializeField] SystemHandler[] _allSystems = null;

    //state
    Dictionary<SystemType, SystemHandler> _systems = new Dictionary<SystemType, SystemHandler>();

    private void Awake()
    {
        foreach (var system in _allSystems)
        {
            _systems.Add(system.SystemType, system);
        }
    }

    public GameObject GetSystem(SystemType systype)
    {
        if (_systems.ContainsKey(systype))
        {
            return _systems[systype].gameObject;
        }
        else
        {
            Debug.Log($"No system on file for type {systype}");
            return null;
        }
    }

}
