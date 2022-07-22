using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] SystemIconDriver[] _systemIcons = null;
    [SerializeField] WeaponIconDriver[] _weaponIcons = null;
    [SerializeField] Sprite _primaryFireHintSprite = null;
    [SerializeField] Sprite _secondaryFireHintSprite = null;
    [SerializeField] RadarScreen _radarScreen = null;

    //state
    Image _currentActiveSecondary;


    private void Awake()
    {
        foreach (var sid in _systemIcons)
        {
            sid.Initialize();
        }

        foreach (var wid in _weaponIcons)
        {
            wid.Initialize(_primaryFireHintSprite, _secondaryFireHintSprite);
        }
        _weaponIcons[0].HighlightAsActivePrimary(); // Pri weapon must always be first system;
    }

    public void HighlightNewSecondary(int index)
    {
        foreach (var sid in _weaponIcons)
        {
            sid.DehighlightAsActiveSecondaryIfNotPrimary();
        }
        _weaponIcons[index].HighlightAsActiveSecondaryIfNotPrimary();
    }

    public void DepictAsPrimary(int index)
    {
        _weaponIcons[index].HighlightAsActivePrimary();
    }

    public SystemIconDriver AddNewSystem(Sprite sprite, int level, Library.SystemType system)
    {
        //if (index < 0 || index >= _systemIcons.Length)
        //{
        //    Debug.Log("invalid system integration index");
        //    return;
        //}
        bool foundOpenUISLot = false;
        SystemIconDriver sid = null;
        for (int i = 0; i < _systemIcons.Length; i++)
        {
            if (_systemIcons[i].IsOccupied) continue;
            else
            {
                _systemIcons[i].ModifyDisplayedSystem(sprite, level, system);
                sid = _systemIcons[i];
                foundOpenUISLot = true;
                break;
            }
        }
        if (foundOpenUISLot == false)
        {
            Debug.Log("did not find an open System slot on UI");
        }
        return sid;
    }

    public void ClearSystemSlot(Library.SystemType systemToRemove)
    {
        Debug.Log($"trying to clear {systemToRemove}");
        foreach (var systemIcon in _systemIcons)
        {
            if (systemIcon.System == systemToRemove)
            {
                systemIcon.ClearUIIcon();
                return;
            }
        }
    }

    public void IntegrateNewWeapon(Sprite sprite, int level, Library.WeaponType wType)
    {
        bool foundOpenWeaponSLot = false;
        for (int i = 0; i < _weaponIcons.Length; i++)
        {
            if (_weaponIcons[i].IsOccupied) continue;
            else
            {
                _weaponIcons[i].ModifyDisplayedSystem(sprite, level, wType);
                foundOpenWeaponSLot = true;
                break;
            }
        }
        if (foundOpenWeaponSLot == false)
        {
            Debug.Log("did not find an open Weapon slot on UI");
        }

    }

    public void ClearWeaponSlot(Library.WeaponType weaponToRemove)
    {
        Debug.Log($"Trying to clear {weaponToRemove}");
        foreach (var weaponIcon in _weaponIcons)
        {
            if (weaponIcon.WeaponType == weaponToRemove)
            {
                weaponIcon.ClearUIIcon();
                return;
            }
        }
    }

    public int GetMaxSystems()
    {
        return _systemIcons.Length;
    }

    public int GetMaxWeapons()
    {
        return _weaponIcons.Length;
    }

    public RadarScreen GetRadarScreen()
    {
        return _radarScreen;
    }
}
