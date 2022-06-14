using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SystemsLibrary : MonoBehaviour
{
    public enum SystemType { None, Engines, HeavyArmor, FasterShieldRegen, MoreShields}

    public enum WeaponType {PrimaryBlaster, SecondaryBlaster, TertiaryBlaster,
        ArcherTurret, MarkerTurret
    }
    [SerializeField] SystemHandler[] _allSystems = null;
    [SerializeField] WeaponHandler[] _allWeapons = null;
    [SerializeField] GameObject _cratePrefab = null;
    GameController _gameCon;

    //state
    Dictionary<SystemType, SystemHandler> _systems = new Dictionary<SystemType, SystemHandler>();
    Dictionary<WeaponType, WeaponHandler> _weapons = new Dictionary<WeaponType, WeaponHandler>();

    private void Awake()
    {
        _gameCon = FindObjectOfType<GameController>();  
        foreach (var system in _allSystems)
        {
            _systems.Add(system.SystemType, system);
        }
        foreach (var weapon in _allWeapons)
        {
            _weapons.Add(weapon.WeaponType, weapon);
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

    public GameObject GetWeapon(WeaponType weaptype)
    {
        if (_weapons.ContainsKey(weaptype))
        {
            return _weapons[weaptype].gameObject;
        }
        else
        {
            Debug.Log($"No weapon on file for type {weaptype}");
            return null;
        }
    }

    public void SpawnUniqueRandomSystemCrate(List<SystemHandler> systemsOnBoard)
    {
        List<SystemHandler> possibleSystems = new List<SystemHandler>();
        foreach (var system in _allSystems)
        {
            if (!systemsOnBoard.Contains(system))
            {
                possibleSystems.Add(system);
            }
        }
        int rand = UnityEngine.Random.Range(0, possibleSystems.Count);

        GameObject go = Instantiate(_cratePrefab);
        go.GetComponent<SystemCrateHandler>().SystemOrWeaponChunk = possibleSystems[rand].gameObject;
        go.GetComponent<SystemCrateHandler>().Initialize();

        Vector3 offset = (UnityEngine.Random.insideUnitCircle * 2.0f);
        go.transform.position = _gameCon.GetPlayerGO().transform.position + offset;
    }

}
