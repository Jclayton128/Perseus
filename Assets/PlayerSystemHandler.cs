using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    SystemsLibrary _syslib;
    [SerializeField] SystemsLibrary.SystemType[] _startingSystems = null;
    [SerializeField] SystemsLibrary.WeaponType[] _startingWeapons = null;
    PlayerHandler _playerHandler;
    UI_Controller _UICon;

    //state
   public  int _activeWeaponIndex;
    public WeaponHandler ActiveWeapon { get; protected set; }
    int _maxSystems;
    List<SystemHandler> _systemsOnBoard = new List<SystemHandler>();
    List<WeaponHandler> _weaponsOnBoard = new List<WeaponHandler>();
    private void Awake()
    {
        _syslib = FindObjectOfType<SystemsLibrary>();
        _UICon = FindObjectOfType<UI_Controller>();
        _maxSystems = _UICon.GetMaxSystems();
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
            GainSystem(sch.SystemChunk);
            Destroy(collision.gameObject);
        }

    }

    private void GainWeapon(GameObject newWeapon)
    {
        GameObject go = Instantiate<GameObject>(newWeapon, this.transform);
        WeaponHandler wh = newWeapon.GetComponent<WeaponHandler>();
        wh.IntegrateSystem();
        _weaponsOnBoard.Add(wh);
        if (wh.IsSecondary)
        {
            if (!ActiveWeapon)
            {
                ActiveWeapon = wh;
                _activeWeaponIndex = _weaponsOnBoard.IndexOf(wh);
                _UICon.HighlightNewSecondary(_activeWeaponIndex);
            }
        }
        _UICon.IntegrateNewWeapon(_weaponsOnBoard.Count - 1, wh.GetIcon(), 2);
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

    public void ToggleActiveWeaponUp()
    {
        _activeWeaponIndex++;
        _activeWeaponIndex = 
            Mathf.Clamp(_activeWeaponIndex ,1, _weaponsOnBoard.Count - 1);
        ActiveWeapon = _weaponsOnBoard[_activeWeaponIndex];
        _UICon.HighlightNewSecondary(_weaponsOnBoard.IndexOf(ActiveWeapon));
    }

    public void ToggleActiveWeaponDown()
    {
        _activeWeaponIndex--;
        _activeWeaponIndex =
            Mathf.Clamp(_activeWeaponIndex, 1, _weaponsOnBoard.Count - 1);
        ActiveWeapon = _weaponsOnBoard[_activeWeaponIndex];
        _UICon.HighlightNewSecondary(_weaponsOnBoard.IndexOf(ActiveWeapon));
    }

}
