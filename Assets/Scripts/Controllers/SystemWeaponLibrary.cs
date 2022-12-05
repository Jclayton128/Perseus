using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SystemWeaponLibrary : MonoBehaviour
{
    public enum SystemType { None, AfterburnerEngine, BlinkEngine, IonEngine, AntennaeCP, StealthPodCP, CompositeCP,
        PDTurretTail, DeionizerTail, ProjectorTail, DroneBayWings, VampireWings, WreckroWings,
        ReactiveHull, HeavyArmorHull, NanoscaleHull, StaticCoreLINT, ShieldCoreLINT, MagnetonCoreLINT,
        BatteriesRINT, RamscoopRINT, RemoteProcessorRINT}

    public enum WeaponType
    {
        PBlaster0, PArcherTurret1, PMissile2, PShotgun3, PRockets4, PScrapedo5, 
        PHarpoon6, PDarkblade7, PFlakTurret8, PMarkerTurret9, PRevolver10, PBeamTurret11, 
        PTorpedo12, PJavelin13,PArcShield14, PWarpGate15,
        Player16, Player17, Player18, Player19,
        TrundlerBlaster20, Enemy21, Enemy22, Enemy23, Enemy24, Enemy25, Enemy26, Enemy27,
        Enemy28, Enemy29, Enemy30, Enemy31, Enemy32, Enemy33, Enemy34,
        Enemy35, Enemy36, Enemy37, Enemy38, Enemy39, None
    }

    public enum SystemLocation {Engine, Cockpit, Tail, Wings, Hull, LeftInt, RightInt}

    [SerializeField] SystemHandler[] _allSystems = null;
    Dictionary<SystemLocation, List<SystemHandler>> _allSystemsByLocation = new Dictionary<SystemLocation, List<SystemHandler>>();

    [SerializeField] WeaponHandler[] _allWeapons = null;
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

    public SystemHandler[] GetAllSystemHandlers_Debug()
    {
        return _allSystems;
    }

    public WeaponHandler[] GetAllWeaponHandlers_Debug()
    {
        return _allWeapons;
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

    public SystemHandler GetSystemHandler(SystemType systemType)
    {
        return _systems[systemType];
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

    public Sprite GetIcon(WeaponType weaponType)
    {
        return _weapons[weaponType].GetIcon();
    }

    public string GetName(WeaponType weaponType)
    {
        return _weapons[weaponType].GetName();
    }

    public Sprite GetIcon(SystemType systemType)
    {
        return _systems[systemType].GetIcon();
    }

    public string GetName(SystemType systemType)
    {
        return _systems[systemType].GetName();
    }

    public WeaponType GetRandomUninstalledSecondaryWeaponType(List<WeaponType> installedWeaponTypes)
    {
        List<WeaponType> uninstalledWeaponTypes = new List<WeaponType>();
        //Debug.Log($"weapon types installed when asked: {installedWeaponTypes.Count}");
        foreach (var wh in _allWeapons)
        {
            if (!installedWeaponTypes.Contains(wh.WeaponType) && wh.IsSecondary)
            {
                uninstalledWeaponTypes.Add(wh.WeaponType);
            }
        }

        int rand = UnityEngine.Random.Range(0, uninstalledWeaponTypes.Count);
        return uninstalledWeaponTypes[rand];
    }

    public SystemType GetRandomUninstalledSystemType(List<SystemType> installedSystemTypes)
    {
        List<SystemType> uninstalledSystemTypes = new List<SystemType>();

        foreach (var sh in _allSystems)
        {
            if (!installedSystemTypes.Contains(sh.SystemType))
            {
                uninstalledSystemTypes.Add(sh.SystemType);
            }
        }

        int rand = UnityEngine.Random.Range(0, uninstalledSystemTypes.Count);
        return uninstalledSystemTypes[rand];
    }

}
