using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SystemsLibrary : MonoBehaviour
{
    public enum SystemType { None, Engines, HeavyArmor, FasterShieldRegen, MoreShields}
    [SerializeField] SystemHandler[] _allSystems = null;
    [SerializeField] GameObject _cratePrefab = null;
    GameController _gameCon;

    //state
    Dictionary<SystemType, SystemHandler> _systems = new Dictionary<SystemType, SystemHandler>();

    private void Awake()
    {
        _gameCon = FindObjectOfType<GameController>();  
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

    public void SpawnRandomSystemCrate()
    {
        int rand = UnityEngine.Random.Range(0, _allSystems.Length);

        GameObject go = Instantiate(_cratePrefab);
        go.GetComponent<SystemCrateHandler>().SystemChunk = _allSystems[rand].gameObject;
        go.GetComponent<SystemCrateHandler>().Initialize();

        Vector3 offset = (UnityEngine.Random.insideUnitCircle * 2.0f);
        go.transform.position = _gameCon.GetPlayerGO().transform.position + offset;
    }

}
