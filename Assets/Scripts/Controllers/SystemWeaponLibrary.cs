using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SystemWeaponLibrary : MonoBehaviour
{
    public enum SystemType { None, AfterburnerEngine, BlinkEngine, IonEngine, AntennaeCP, StealthPodCP, CompositeCP,
        PDTurretTail, MineDropperTail, ProjectorTail, DroneBayWings, VampireWings, WreckroWings,
        ThornHull, HeavyArmorHull, PhaseHull, EmergencyCoreLINT, ShieldCoreLINT, EnergyCoreLINT,
        AuxBattRINT, ShieldBattRINT, EnergBattRINT}

    public enum WeaponType
    {
        PBlaster0, PArcherTurret1, PMissile2, PShotgun3, PPopRockets4, PScrapedo5, 
        PHarpoon6, PDarkblade7, PFlakTurret8, PMarkerTurret9, Player10, Player11, 
        Player12, Player13,Player14, Player15,
        Player16, Player17, Player18, Player19,
        TrundlerBlaster20, Enemy21, Enemy22, Enemy23, Enemy24, Enemy25, Enemy26, Enemy27,
        Enemy28, Enemy29, Enemy30, Enemy31, Enemy32, Enemy33, Enemy34,
        Enemy35, Enemy36, Enemy37, Enemy38, Enemy39
    }

    public enum SystemLocation {Engine, Cockpit, Tail, Wings, Hull, LeftInt, RightInt}

    [SerializeField] SystemHandler[] _allSystems = null;
    Dictionary<SystemLocation, List<SystemHandler>> _allSystemsByLocation = new Dictionary<SystemLocation, List<SystemHandler>>();

    [SerializeField] WeaponHandler[] _allWeapons = null;
    [SerializeField] GameObject _cratePrefab = null;
    GameController _gameCon;

    //state
    Dictionary<SystemType, SystemHandler> _systems = new Dictionary<SystemType, SystemHandler>();
    Dictionary<WeaponType, WeaponHandler> _weapons = new Dictionary<WeaponType, WeaponHandler>();

    private void Awake()
    {
        _gameCon = FindObjectOfType<GameController>();

        List<SystemHandler> _engineSystems = new List<SystemHandler>();
        List<SystemHandler> _cockpitSystems = new List<SystemHandler>();
        List<SystemHandler> _tailSystems = new List<SystemHandler>();
        List<SystemHandler> _wingSystems = new List<SystemHandler>();
        List<SystemHandler> _hullSystems = new List<SystemHandler>();
        List<SystemHandler> _leftIntSystems = new List<SystemHandler>();
        List<SystemHandler> _rightIntSystems = new List<SystemHandler>();

        _allSystemsByLocation[SystemLocation.Engine] = _engineSystems;
        _allSystemsByLocation[SystemLocation.Cockpit] = _cockpitSystems;
        _allSystemsByLocation[SystemLocation.Tail] = _tailSystems;
        _allSystemsByLocation[SystemLocation.Wings] = _wingSystems;
        _allSystemsByLocation[SystemLocation.Hull] = _hullSystems;
        _allSystemsByLocation[SystemLocation.LeftInt] = _leftIntSystems;
        _allSystemsByLocation[SystemLocation.RightInt] = _rightIntSystems;

        foreach (var system in _allSystems)
        {
            _systems.Add(system.SystemType, system);
            _allSystemsByLocation[system.SystemLocation].Add(system);

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

    public GameObject GetSystem(SystemLocation location, int index)
    {
        if (index >= _allSystemsByLocation[location].Count) return null;
        return _allSystemsByLocation[location][index].gameObject;
    }

    public GameObject GetWeapon(int indexInLibrary)
    {
        if(indexInLibrary >= _allWeapons.Length) return null;
        return _allWeapons[indexInLibrary].gameObject;
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

    public WeaponHandler GetWeaponHandler(WeaponType weaponType)
    {
        return _weapons[weaponType];
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
        go.transform.position = _gameCon.Player.transform.position + offset;
    }

}
