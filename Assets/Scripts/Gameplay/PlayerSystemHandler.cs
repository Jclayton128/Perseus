using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    SystemWeaponLibrary _syslib;
    InputController _inputCon;
    [SerializeField] SystemWeaponLibrary.SystemType[] _startingSystems = null;
    [SerializeField] SystemWeaponLibrary.WeaponType[] _startingWeapons = null;
    ActorMovement _playerHandler;
    EnergyHandler _energyHandler;
    UI_Controller _UICon;

    //These are used to check for overlap between two weapons or two systems.
    Dictionary<SystemWeaponLibrary.WeaponType, GameObject> _weaponsOnBoard =
        new Dictionary<SystemWeaponLibrary.WeaponType, GameObject>();
    Dictionary<SystemWeaponLibrary.SystemLocation, GameObject> _systemsOnBoardByLocation = new Dictionary<SystemWeaponLibrary.SystemLocation, GameObject>();

    //state
   public  int _activeWeaponIndex;
    public WeaponHandler ActiveWeapon { get; protected set; }
    int _maxSystems;
    int _maxWeapons;

    //These lists are to help with scrolling and shooting multiple primary systems at once
    List<SystemHandler> _systemsOnBoard = new List<SystemHandler>();
    [SerializeField] List<WeaponHandler> _primaryWeaponsOnBoard = new List<WeaponHandler>();
    [SerializeField] List<WeaponHandler> _secondaryWeaponsOnBoard = new List<WeaponHandler>();
    private void Awake()
    {
        _syslib = FindObjectOfType<SystemWeaponLibrary>();
        _UICon = FindObjectOfType<UI_Controller>();
        _inputCon = _UICon.GetComponent<InputController>();
        _inputCon.OnScroll += ScrollThroughActiveWeapons;
        _inputCon.OnMouseDown += ActivateWeapons;
        _inputCon.OnMouseUp += DeactivateWeapons;
        _maxSystems = _UICon.GetMaxSystems();
        _maxWeapons = _UICon.GetMaxWeapons();

        _playerHandler = GetComponent<ActorMovement>();
        _energyHandler = GetComponent<EnergyHandler>();

    }

    private void Start()
    {
        LoadStartingSystems();
        LoadStartingWeapons();
    }


    private void LoadStartingSystems()
    {
        foreach (var system in _startingSystems)
        {
            GainSystem(_syslib.GetSystem(system));
        }

    }

    private void LoadStartingWeapons()
    {
        foreach (var weapon in _startingWeapons)
        {
            GainWeapon(_syslib.GetWeapon(weapon));
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        SystemCrateHandler sch;
        if (collision.gameObject.TryGetComponent<SystemCrateHandler>(out sch))
        {

            if (sch.SystemInCrate != SystemWeaponLibrary.SystemType.None)
            {
                if (_systemsOnBoardByLocation.Count >= _maxSystems)
                {
                    Debug.Log("unable to hold any more systems");
                    return;
                }
                GainSystem(sch.SystemInCrate);
            }

            if (sch.WeaponInCrate != SystemWeaponLibrary.WeaponType.None)
            {
                if (_weaponsOnBoard.Count >= _maxWeapons)
                {
                    Debug.Log("unable to hold any more weapons");
                    return;
                }
                GainWeapon(sch.WeaponInCrate);
            }

            Destroy(collision.gameObject);
        }

    }

    private void GainWeapon(SystemWeaponLibrary.WeaponType weaponType)
    {
        GainWeapon(_syslib.GetWeapon(weaponType));
    }

    private void GainWeapon(GameObject newWeapon)
    {
        if (newWeapon == null) return;
        GameObject go = Instantiate<GameObject>(newWeapon, this.transform);
        WeaponHandler wh = go.GetComponent<WeaponHandler>();
        WeaponIconDriver wid = _UICon.IntegrateNewWeapon(wh);
        wh.Initialize(_energyHandler, true, wid);
        _weaponsOnBoard.Add(wh.WeaponType, go);


        if (wh.IsSecondary)
        {
            _secondaryWeaponsOnBoard.Add(wh);
            if (!ActiveWeapon)
            {
                ActiveWeapon = wh;
                _activeWeaponIndex = _secondaryWeaponsOnBoard.IndexOf(wh);
                _UICon.HighlightNewSecondaryWeapon(_activeWeaponIndex);
            }
        }
        else
        {
            _primaryWeaponsOnBoard.Add(wh);
        }

    }

    private void GainSystem(SystemWeaponLibrary.SystemType systemType)
    {
        GainSystem(_syslib.GetSystem(systemType));
    }

    private void GainSystem(GameObject newSystem)
    {
        if (newSystem == null) return;
        GameObject go = Instantiate<GameObject>(newSystem, this.transform);
        SystemHandler sh = newSystem.GetComponent<SystemHandler>();
        if (_systemsOnBoardByLocation.ContainsKey(sh.SystemLocation))
        {
            Debug.Log($"Error - ship already contains a system in {sh.SystemLocation}");
            return;
        }
        _systemsOnBoardByLocation.Add(sh.SystemLocation, go);
        SystemIconDriver sid = _UICon.IntegrateNewSystem(sh);
        sh.IntegrateSystem(sid);
        _systemsOnBoard.Add(sh);
        

    }

    public List<SystemHandler> GetSystemsOnBoard()
    {
        return _systemsOnBoard;
    }

    private void ScrollThroughActiveWeapons(int direction)
    {
        if (_secondaryWeaponsOnBoard.Count == 0) return;
        _activeWeaponIndex += direction;
        _activeWeaponIndex = Mathf.Clamp(_activeWeaponIndex, 0, _secondaryWeaponsOnBoard.Count-1);
        ActiveWeapon = _secondaryWeaponsOnBoard[_activeWeaponIndex];
        _UICon.HighlightNewSecondaryWeapon(_secondaryWeaponsOnBoard.IndexOf(ActiveWeapon));
    }

    private void ActivateWeapons(int priOrSec)
    {
        if (priOrSec == 0)  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap?.Activate();
            }
        }
        if (priOrSec == 1) // Active Secondary Weapon
        {
            ActiveWeapon?.Activate();
        }
    }

    private void DeactivateWeapons(int priOrSec)
    {
        if (priOrSec == 0)  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap?.Deactivate();
            }
        }
        if (priOrSec == 1) // Active Secondary Weapon
        {
            ActiveWeapon?.Deactivate();
        }
    }

    #region Debug tools
    public bool Debug_TryGainWeapon(int indexInLibrary)
    {
        if (_weaponsOnBoard.Count >= _UICon.GetMaxWeapons())
        {
            Debug.Log("Can't gain anymore weapons due to UI limits");
            return false;
        }

        GainWeapon(_syslib.GetWeapon(indexInLibrary));
        return true;
    }

    public bool Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation location, int index)
    {
        if (_systemsOnBoardByLocation.Count >= _UICon.GetMaxSystems())
        {
            Debug.Log("Can't gain anymore systems due to UI limits");
            return false;
        }
        //Destroy any other system already at this Location
        if (_systemsOnBoardByLocation.ContainsKey(location))
        {
            RemoveSystem(location, index);
        }

        //GameObject go = Instantiate<GameObject>(_syslib.GetSystem(location, index), transform);
        //SystemHandler sh = go.GetComponent<SystemHandler>();
        //_systemsOnBoardByLocation.Add(sh.SystemLocation, go);

        GainSystem(_syslib.GetSystem(location, index));
        return true;
    }

    public void RemoveWeapon(SystemWeaponLibrary.WeaponType weaponType)
    {
        WeaponHandler removedWeapon = _weaponsOnBoard[weaponType]?.GetComponent<WeaponHandler>();

        if (removedWeapon == null) return;
        if (removedWeapon.IsSecondary)
        {
            _secondaryWeaponsOnBoard.Remove(removedWeapon);

            //UI: clear all secondary weapon icons
            _UICon.ClearAllSecondaryWeaponSlots();

            //foreach secondary weapon, reintegrate
            foreach (var secondaryWeapon in _secondaryWeaponsOnBoard)
            {
                WeaponIconDriver newWID = _UICon.IntegrateNewWeapon(secondaryWeapon);
                secondaryWeapon.UpdateWeaponIconDriver(newWID);
            }

            //reselect an active weapon

            if (weaponType == ActiveWeapon.WeaponType)
            {
                //Decrement the active weapon index
                _activeWeaponIndex--;
                _activeWeaponIndex = Mathf.Clamp(_activeWeaponIndex, 0, _secondaryWeaponsOnBoard.Count - 1);

                //Update the active weapon, and the highlighted icon
                ActiveWeapon = _secondaryWeaponsOnBoard[_activeWeaponIndex];
                _UICon.HighlightNewSecondaryWeapon(_secondaryWeaponsOnBoard.IndexOf(ActiveWeapon));
            }
        }
        else
        {
            _primaryWeaponsOnBoard.Remove(removedWeapon);
            _UICon.ClearPrimaryWeaponSlot();
        }

        Destroy(_weaponsOnBoard[weaponType]);
        _weaponsOnBoard.Remove(weaponType);

    }

    public void RemoveSystem(SystemWeaponLibrary.SystemLocation location, int index)
    {

        SystemHandler systemToRemove = _syslib.GetSystem(location, index)?.GetComponent<SystemHandler>();

        if (systemToRemove == null) return; 

        _systemsOnBoard.Remove(systemToRemove);
        if (_systemsOnBoardByLocation.ContainsKey(location))
        {
            Destroy(_systemsOnBoardByLocation[location]);
            _systemsOnBoardByLocation.Remove(location);
        }

        _UICon.ClearAllSystemSlots();
        foreach (var system in _systemsOnBoard)
        {
            SystemIconDriver sid = _UICon.IntegrateNewSystem(system);
            system.IntegrateSystem(sid);
        }

    }

    #endregion
}
