using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    SystemsLibrary _syslib;
    InputController _inputCon;
    [SerializeField] SystemsLibrary.SystemType[] _startingSystems = null;
    [SerializeField] SystemsLibrary.WeaponType[] _startingWeapons = null;
    PlayerHandler _playerHandler;
    UI_Controller _UICon;

    //positive int is for weapons, negative for systems
    Dictionary<int, GameObject> _gadgetsOnBoard_Debug = new Dictionary<int, GameObject>();

    //state
   public  int _activeWeaponIndex;
    public WeaponHandler ActiveWeapon { get; protected set; }
    int _maxSystems;
    int _maxWeapons;
    List<SystemHandler> _systemsOnBoard = new List<SystemHandler>();
    public List<WeaponHandler> _primaryWeaponsOnBoard = new List<WeaponHandler>();
    List<WeaponHandler> _allWeaponsOnBoard = new List<WeaponHandler>();
    private void Awake()
    {
        _syslib = FindObjectOfType<SystemsLibrary>();
        _UICon = FindObjectOfType<UI_Controller>();
        _inputCon = _UICon.GetComponent<InputController>();
        _inputCon.OnScroll += ScrollThroughActiveWeapons;
        _inputCon.OnMouseDown += ActivateWeapons;
        _inputCon.OnMouseUp += DeactivateWeapons;

        _maxSystems = _UICon.GetMaxSystems();
        _maxWeapons = _UICon.GetMaxWeapons();
        _playerHandler = GetComponent<PlayerHandler>();

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
            if (_systemsOnBoard.Count >= _maxSystems)
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
        WeaponHandler wh = newWeapon.GetComponent<WeaponHandler>();
        wh.Initialize();
        _allWeaponsOnBoard.Add(wh);
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
        _UICon.IntegrateNewWeapon(_allWeaponsOnBoard.Count - 1, wh.GetIcon(), 2);
        go.transform.localPosition = wh.LocalPosition;
    }
    private void GainSystem(GameObject newSystem)
    {
        GameObject go = Instantiate<GameObject>(newSystem, this.transform);
        SystemHandler sh = newSystem.GetComponent<SystemHandler>();
        sh.IntegrateSystem(_playerHandler);
        _systemsOnBoard.Add(sh);
        _UICon.IntegrateNewSystem(_systemsOnBoard.Count - 1, sh.GetIcon(), 1);
        go.transform.localPosition = sh.LocalPosition;        
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
        _UICon.HighlightNewSecondary(_allWeaponsOnBoard.IndexOf(ActiveWeapon));
    }

    private void ActivateWeapons(int priOrSec)
    {
        if (priOrSec == 0)  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap.BaseSystem.Activate();
            }
        }
        if (priOrSec == 1) // Active Secondary Weapon
        {
            ActiveWeapon.BaseSystem.Activate();
        }
    }

    private void DeactivateWeapons(int priOrSec)
    {
        if (priOrSec == 0)  // Primary Weapons
        {
            foreach (var priweap in _primaryWeaponsOnBoard)
            {
                priweap.BaseSystem.Deactivate();
            }
        }
        if (priOrSec == 1) // Active Secondary Weapon
        {
            ActiveWeapon.BaseSystem.Deactivate();
        }
    }

    #region Debug tools
    public void Debug_GainWeapon(int index)
    {
        GameObject go = Instantiate<GameObject>(_syslib.GetWeapon(index), transform);
        WeaponHandler wh = go.GetComponent<WeaponHandler>();
        go.transform.localPosition = wh.LocalPosition;
        _gadgetsOnBoard_Debug.Add(index, go);
    }

    public void Debug_GainSystem(int index)
    {
        GameObject go = Instantiate<GameObject>(_syslib.GetSystem(index), transform);
        SystemHandler sh = go.GetComponent<SystemHandler>();
        go.transform.localPosition = sh.LocalPosition;
        _gadgetsOnBoard_Debug.Add(-1 * index, go);
    }

    public void Debug_RemoveWeapon(int index)
    {
        Destroy(_gadgetsOnBoard_Debug[index]);
        _gadgetsOnBoard_Debug.Remove(index);
    }

    public void Debug_RemoveSystem(int index)
    {
        Destroy(_gadgetsOnBoard_Debug[-1 * index]);
        _gadgetsOnBoard_Debug.Remove(-1 * index);
    }

    #endregion
}
