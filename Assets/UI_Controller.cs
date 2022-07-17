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

    public void AddNewSystem(Sprite sprite, int level, SystemsLibrary.SystemType system)
    {
        //if (index < 0 || index >= _systemIcons.Length)
        //{
        //    Debug.Log("invalid system integration index");
        //    return;
        //}
        bool foundOpenUISLot = false;
        for (int i = 0; i < _systemIcons.Length; i++)
        {
            if (_systemIcons[i].IsOccupied) continue;
            else
            {
                _systemIcons[i].ModifyDisplayedSystem(sprite, level, system);
                foundOpenUISLot = true;
                break;
            }
        }
        if (foundOpenUISLot == false)
        {
            Debug.Log("did not find an open System slot on UI");
        }
    }

    public void ClearSystemSlot(SystemsLibrary.SystemType systemToRemove)
    {
        foreach (var systemIcon in _systemIcons)
        {
            if (systemIcon.System == systemToRemove)
            {
                systemIcon.ClearUIIcon();
                return;
            }
        }
    }

    public void IntegrateNewWeapon(int index, Sprite sprite, int level)
    {
        if (index < 0 || index >= _weaponIcons.Length)
        {
            Debug.Log("invalid weapon integration index");
            return;
        }

        _weaponIcons[index].ModifyDisplayedSystem(sprite, level);
    }

    public int GetMaxSystems()
    {
        return _systemIcons.Length;
    }

    public int GetMaxWeapons()
    {
        return _weaponIcons.Length;
    }
}
