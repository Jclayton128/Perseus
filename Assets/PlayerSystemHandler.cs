using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    Library _syslib;
    InputController _inputCon;
    [SerializeField] Library.SystemType[] _startingSystems = null;
    [SerializeField] Library.WeaponType[] _startingWeapons = null;
    ActorMovement _playerHandler;
    UI_Controller _UICon;

    //These are used to check for overlap between two weapons or two systems.
    Dictionary<Library.WeaponType, GameObject> _weaponsOnBoard =
        new Dictionary<Library.WeaponType, GameObject>();
    Dictionary<Library.SystemLocation, GameObject> _systemsOnBoardByLocation = new Dictionary<Library.SystemLocation, GameObject>();

    //state
   public  int _activeWeaponIndex;
    public WeaponHandler ActiveWeapon { get; protected set; }
    int _maxSystems;
    int _maxWeapons;

    //These lists are to help with scrolling and shooting multiple primary systems at once
    List<SystemHandler> _systemsOnBoard = new List<SystemHandler>();
    [SerializeField] List<WeaponHandler> _primaryWeaponsOnBoard = new List<WeaponHandler>();
    List<WeaponHandler> _allWeaponsOnBoard = new List<WeaponHandler>();
    private void Awake()
    {
        _syslib = FindObjectOfType<Library>();
        _UICon = FindObjectOfType<UI_Controller>();
        _inputCon = _UICon.GetComponent<InputController>();
        _inputCon.OnScroll += ScrollThroughActiveWeapons;
        _inputCon.OnMouseDown += ActivateWeapons;
        _inputCon.OnMouseUp += DeactivateWeapons;
        _maxSystems = _UICon.GetMaxSystems();
        _maxWeapons = _UICon.GetMaxWeapons();
        _playerHandler = GetComponent<ActorMovement>();

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
            if (_systemsOnBoardByLocation.Count >= _maxSystems)
            {
                Debug.Log("unable to hold any more systems");
                return;
            }

            if (sch.GetComponent<SystemHandler>())
            {
                GainSystem(sch.SystemOrWeaponChunk);
            }
            if (sch.GetComponent<WeaponHandler>())
            {
                GainWeapon(sch.SystemOrWeaponChunk);
            }

            Destroy(collision.gameObject);
        }

    }

    private void GainWeapon(GameObject newWeapon)
    {
        GameObject go = Instantiate<GameObject>(newWeapon, this.transform);
        WeaponHandler wh = go.GetComponent<WeaponHandler>();
        wh.Initialize();
        _allWeaponsOnBoard.Add(wh);
        _weaponsOnBoard.Add(wh.WeaponType, go);
        if (wh.IsSecondary)
        {
            if (!ActiveWeapon)
            {
                ActiveWeapon = wh;
                _activeWeaponIndex = _allWeaponsOnBoard.IndexOf(wh);
                _UICon.HighlightNewSecondary(_activeWeaponIndex);
            }
        }
        else
        {
            _primaryWeaponsOnBoard.Add(wh);
            _UICon.DepictAsPrimary(_allWeaponsOnBoard.IndexOf(wh));

        }
        _UICon.IntegrateNewWeapon(wh.GetIcon(), 2, wh.WeaponType);
        Debug.Log($"Gained a {wh.WeaponType} the right way");
    }
    private void GainSystem(GameObject newSystem)
    {
        GameObject go = Instantiate<GameObject>(newSystem, this.transform);
        SystemHandler sh = newSystem.GetComponent<SystemHandler>();
        if (_systemsOnBoardByLocation.ContainsKey(sh.SystemLocation))
        {
            Debug.Log($"Error - ship already contains a system in {sh.SystemLocation}");
            return;
        }
        _systemsOnBoardByLocation.Add(sh.SystemLocation, go);
        sh.IntegrateSystem(_playerHandler);
        _systemsOnBoard.Add(sh);
        
        _UICon.AddNewSystem(sh.GetIcon(), 1, sh.SystemType);      
    }

    public List<SystemHandler> GetSystemsOnBoard()
    {
        return _systemsOnBoard;
    }

    private void ScrollThroughActiveWeapons(int direction)
    {
        _activeWeaponIndex += direction;
        _activeWeaponIndex = Mathf.Clamp(_activeWeaponIndex, 1, _allWeaponsOnBoard.Count - 1);
        ActiveWeapon = _allWeaponsOnBoard[_activeWeaponIndex];
        if (_allWeaponsOnBoard.Count == 0) return;
        _UICon.HighlightNewSecondary(_allWeaponsOnBoard.IndexOf(ActiveWeapon));
    }

    private void ActivateWeapons(int priOrSec)
    {
        if (priOrSec == 0)  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap.Activate();
            }
        }
        if (priOrSec == 1) // Active Secondary Weapon
        {
            ActiveWeapon.Activate();
        }
    }

    private void DeactivateWeapons(int priOrSec)
    {
        if (priOrSec == 0)  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap.Deactivate();
            }
        }
        if (priOrSec == 1) // Active Secondary Weapon
        {
            ActiveWeapon.Deactivate();
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

    public bool Debug_TryGainSystem(Library.SystemLocation location, int index)
    {
        if (_systemsOnBoardByLocation.Count >= _UICon.GetMaxSystems())
        {
            Debug.Log("Can't gain anymore systems due to UI limits");
            return false;
        }
        //Destroy any other system already at this Location
        if (_systemsOnBoardByLocation.ContainsKey(location))
        {
            Debug_RemoveSystem(location, index);
        }

        //GameObject go = Instantiate<GameObject>(_syslib.GetSystem(location, index), transform);
        //SystemHandler sh = go.GetComponent<SystemHandler>();
        //_systemsOnBoardByLocation.Add(sh.SystemLocation, go);

        GainSystem(_syslib.GetSystem(location, index));
        return true;
    }

    public void Debug_RemoveWeapon(Library.WeaponType weaponType)
    {
        _UICon.ClearWeaponSlot(weaponType);
        Destroy(_weaponsOnBoard[weaponType]);
        _weaponsOnBoard.Remove(weaponType);
    }

    public void Debug_RemoveSystem(Library.SystemLocation location, int index)
    {
        _UICon.ClearSystemSlot(_syslib.GetSystem(location, index).GetComponent<SystemHandler>().SystemType);
        if (_systemsOnBoardByLocation.ContainsKey(location))
        {
            Destroy(_systemsOnBoardByLocation[location]);
            _systemsOnBoardByLocation.Remove(location);
        }

    }

    #endregion
}
