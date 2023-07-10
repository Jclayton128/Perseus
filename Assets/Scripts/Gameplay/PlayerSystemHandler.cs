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
    HealthHandler _healthHandler;
    MountHandler _mountHandler;
    UI_Controller _UICon;
    Scanner _crateScanner;

    public Action<SystemWeaponLibrary.SystemType> InstalledSystem;
    public Action<SystemWeaponLibrary.WeaponType> InstalledWeapon;
    public Action<SystemWeaponLibrary.SystemType, int> UpgradedSystem;
    public Action<SystemWeaponLibrary.WeaponType, int> UpgradedWeapon;


    //These are used to check for overlap between two weapons or two systems.
    Dictionary<SystemWeaponLibrary.WeaponType, GameObject> _weaponsOnBoard =
        new Dictionary<SystemWeaponLibrary.WeaponType, GameObject>();
    Dictionary<SystemWeaponLibrary.SystemLocation, GameObject> _systemsOnBoardByLocation = new Dictionary<SystemWeaponLibrary.SystemLocation, GameObject>();

    //state
    int _activeWeaponIndex;
    public WeaponHandler ActiveSecondaryWeapon { get; protected set; }
    int _maxSystems;
    int _maxWeapons;


    //These lists are to help with scrolling and shooting multiple primary systems at once
    List<SystemHandler> _systemsOnBoard = new List<SystemHandler>();
    public List<SystemWeaponLibrary.SystemType> SystemTypesOnBoard { get; private set; }
        = new List<SystemWeaponLibrary.SystemType>();
    
    List<WeaponHandler> _primaryWeaponsOnBoard = new List<WeaponHandler>();
    List<WeaponHandler> _secondaryWeaponsOnBoard = new List<WeaponHandler>();
    public List<SystemWeaponLibrary.WeaponType> WeaponTypesOnBoard
        = new List<SystemWeaponLibrary.WeaponType>();
    private void Start()
    {
        _crateScanner = GetComponent<Scanner>();
        _syslib = FindObjectOfType<SystemWeaponLibrary>();
        _UICon = FindObjectOfType<UI_Controller>();
        _inputCon = _UICon.GetComponent<InputController>();
        if (_inputCon)
        {
            _inputCon.ScrollWheelChanged += ScrollThroughActiveWeapons;
            _inputCon.LeftMouseChanged += ActivateOrDeactivatePrimaryWeapon;
            _inputCon.RightMouseChanged += ActivateOrDeactivateSelectedSecondaryWeapon;
        }

        _maxSystems = _UICon.GetMaxSystems();
        _maxWeapons = _UICon.GetMaxWeapons();

        _playerHandler = GetComponent<ActorMovement>();
        _energyHandler = GetComponent<EnergyHandler>();
        _healthHandler = GetComponent<HealthHandler>();
        _mountHandler = GetComponent<MountHandler>();


        LoadStartingSystems();
        LoadStartingWeapons();
    }


    private void LoadStartingSystems()
    {
        _systemsOnBoard.Clear();
        foreach (var system in _startingSystems)
        {
            GainSystem(system, true);
        }

    }

    private void LoadStartingWeapons()
    {
        _weaponsOnBoard.Clear();
        _secondaryWeaponsOnBoard.Clear();
        _primaryWeaponsOnBoard.Clear();
        _activeWeaponIndex = -1;
        ActiveSecondaryWeapon = null;
        foreach (var weapon in _startingWeapons)
        {
            GainWeapon(weapon, true);
        }
    }

    #region System/Weapon Count Checks

    public (bool, string) CheckIfCanGainSecondaryWeapon(WeaponHandler wh)
    {
        (bool, string) outcome;
        if (_secondaryWeaponsOnBoard.Count >= _maxWeapons)
        {
            outcome.Item1 = false;
            outcome.Item2 = "No Available Weapon Slots";
            return outcome;
        }

        if (_secondaryWeaponsOnBoard.Contains(wh))
        {
            outcome.Item1 = false;
            outcome.Item2 = "Identical Weapon Already Installed";
            return outcome;
        }

        outcome.Item1 = true;
        outcome.Item2 = "no error";
        return outcome;
    }

    public (bool,string) CheckIfCanGainSystem(SystemHandler sh)
    {
        (bool, string) outcome;

        if (_systemsOnBoardByLocation.Count >= _maxSystems)
        {
            //Debug.LogError("unable to hold any more systems");
            outcome.Item1 = false;
            outcome.Item2 = "No Available System Slots";
            return outcome;
        }

        if (_systemsOnBoardByLocation.ContainsKey(sh.SystemLocation))
        {
            //Debug.LogError($"Ship already contains a system in {sh.SystemLocation}");
            outcome.Item1 = false;
            outcome.Item2 = "Similar System Already Installed";
            return outcome;
        }

        outcome.Item1 = true;
        outcome.Item2 = "no error";
        return outcome;        
    }

    #endregion

    #region System Gain/Loss
    

    public void GainWeapon(SystemWeaponLibrary.WeaponType weaponType, bool isStartingWeapon)
    {
        GainWeapon(_syslib.GetWeapon(weaponType));
        if (!isStartingWeapon) InstalledWeapon?.Invoke(weaponType);
    }

    private void GainWeapon(GameObject newWeapon)
    {
        if (newWeapon == null) return;

        var mountIndex = _mountHandler.RequisitionWeaponMountIndex();
        Transform mountLocation = _mountHandler.GetWeaponMountTransform(mountIndex);

        GameObject go = Instantiate<GameObject>(newWeapon, mountLocation);
        WeaponHandler wh = go.GetComponent<WeaponHandler>();
        WeaponIconDriver wid = _UICon.IntegrateNewWeapon(wh);
        wh.Initialize(_energyHandler, true, wid);
        wh.WeaponMountIndex = mountIndex;
        _weaponsOnBoard.Add(wh.WeaponType, go);
        WeaponTypesOnBoard.Add(wh.WeaponType);


        if (wh.IsSecondary)
        {
            _secondaryWeaponsOnBoard.Add(wh);
            if (!ActiveSecondaryWeapon)
            {
                ActiveSecondaryWeapon = wh;
                _activeWeaponIndex = _secondaryWeaponsOnBoard.IndexOf(wh);
                _UICon.HighlightNewSecondaryWeapon(_activeWeaponIndex);
            }
        }
        else
        {
            _primaryWeaponsOnBoard.Add(wh);
        }
        _crateScanner.DestroyScannedCrateAfterInstall();
    }

    public void GainSystem(SystemWeaponLibrary.SystemType systemType, bool isStartingSystem)
    {
        GainSystem(_syslib.GetSystem(systemType));
        if (!isStartingSystem) InstalledSystem?.Invoke(systemType);
    }

    private void GainSystem(GameObject newSystem)
    {
        if (newSystem == null) return;
        GameObject go = Instantiate<GameObject>(newSystem, this.transform);
        SystemHandler sh = go.GetComponent<SystemHandler>();

        var smd = _mountHandler.GetSystemMountDescription(sh.SystemLocation);
        go.transform.parent = smd.transform;
        go.transform.localPosition = Vector3.zero;
        if (smd.ShouldBeReflected)
        {
            var mirrorGO = Instantiate<GameObject>(newSystem, go.transform);
            SystemHandler shkill = mirrorGO.GetComponent<SystemHandler>();
            Destroy(shkill);
            mirrorGO.transform.localPosition = smd.MirroredXPosition;
            mirrorGO.transform.localScale = new Vector3(-1, 1, 1);
 
        }

        _systemsOnBoardByLocation.Add(sh.SystemLocation, go);
        SystemIconDriver sid = _UICon.IntegrateNewSystem(sh);
        sh.IntegrateSystem(sid);
        _systemsOnBoard.Add(sh);
        SystemTypesOnBoard.Add(sh.SystemType);
        _crateScanner.DestroyScannedCrateAfterInstall();

    }

    public void RemoveWeapon(SystemWeaponLibrary.WeaponType weaponType)
    {
        WeaponHandler removedWeapon = _weaponsOnBoard[weaponType]?.GetComponent<WeaponHandler>();
        _mountHandler.ReturnWeaponMount(removedWeapon.WeaponMountIndex);

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

            if (weaponType == ActiveSecondaryWeapon.WeaponType)
            {
                if (_secondaryWeaponsOnBoard.Count > 0)
                {
                    //Decrement the active weapon index
                    _activeWeaponIndex--;
                    _activeWeaponIndex = Mathf.Clamp(_activeWeaponIndex, 0, _secondaryWeaponsOnBoard.Count - 1);

                    //Update the active weapon, and the highlighted icon

                    ActiveSecondaryWeapon = _secondaryWeaponsOnBoard[_activeWeaponIndex];
                    _UICon.HighlightNewSecondaryWeapon(_secondaryWeaponsOnBoard.IndexOf(ActiveSecondaryWeapon));
                }
                else ActiveSecondaryWeapon = null;

                
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

    public void RemoveSystem(SystemWeaponLibrary.SystemType removedSystemType)
    {
        SystemHandler systemHandlerToRemove = null;
        foreach (var sh in _systemsOnBoard)
        {
            if (sh.SystemType == removedSystemType)
            {
                systemHandlerToRemove = sh;
                break;
            }
        }

        _systemsOnBoard.Remove(systemHandlerToRemove);
        if (_systemsOnBoardByLocation.ContainsKey(systemHandlerToRemove.SystemLocation))
        {
            Destroy(_systemsOnBoardByLocation[systemHandlerToRemove.SystemLocation]);
            _systemsOnBoardByLocation.Remove(systemHandlerToRemove.SystemLocation);
        }

        _UICon.ClearAllSystemSlots();
        foreach (var system in _systemsOnBoard)
        {
            SystemIconDriver sid = _UICon.IntegrateNewSystem(system);
            system.IntegrateSystem(sid);
        }
    }

    #endregion


    #region Input Responses

    private void ScrollThroughActiveWeapons(int direction)
    {
        if (_secondaryWeaponsOnBoard.Count == 0) return;
        _activeWeaponIndex += direction;
        _activeWeaponIndex = Mathf.Clamp(_activeWeaponIndex, 0, _secondaryWeaponsOnBoard.Count-1);
        ActiveSecondaryWeapon = _secondaryWeaponsOnBoard[_activeWeaponIndex];
        _UICon.HighlightNewSecondaryWeapon(_secondaryWeaponsOnBoard.IndexOf(ActiveSecondaryWeapon));
    }

    private void ActivateOrDeactivatePrimaryWeapon(bool wasDepressed)
    {
        if (wasDepressed)
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap?.Activate();
            }
        }
        else  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap?.Deactivate();
            }
        }
    }

    private void ActivateOrDeactivateSelectedSecondaryWeapon(bool wasDepressed)
    {
        if (wasDepressed)
        {
            //Debug.Log($"activating {ActiveSecondaryWeapon}");
            ActiveSecondaryWeapon?.Activate();
        }
        else // Active Secondary Weapon
        {
            ActiveSecondaryWeapon?.Deactivate();
        }
    }

    #endregion

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

    

    #endregion


    private void OnDestroy()
    {
        _inputCon.ScrollWheelChanged -= ScrollThroughActiveWeapons;
        _inputCon.LeftMouseChanged -= ActivateOrDeactivatePrimaryWeapon;
        _inputCon.RightMouseChanged -= ActivateOrDeactivateSelectedSecondaryWeapon;
    }
}
