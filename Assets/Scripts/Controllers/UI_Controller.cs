using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System;

public class UI_Controller : MonoBehaviour
{
    #region Scene References
    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] SystemIconDriver[] _systemIcons = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] WeaponIconDriver _primaryWeaponIcon = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] WeaponIconDriver[] _secondaryWeaponIcons = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] Sprite _primaryFireHintSprite = null;

    [FoldoutGroup("Systems & Weapons")]
    [SerializeField] Sprite _secondaryFireHintSprite = null;

    [SerializeField] RadarScreen _radarScreen = null;

    [FoldoutGroup("Scrap & Level")]
    [SerializeField] Image _scrapBarFill = null;

    [FoldoutGroup("Scrap & Level")]
    [SerializeField] TextMeshProUGUI _scrapAmountTMP = null;

    [FoldoutGroup("Scrap & Level")]
    [SerializeField] TextMeshProUGUI _levelTMP = null;
    #endregion

    //settings
    [FoldoutGroup("Scrap & Level")]
    [SerializeField] float _minScrapFactor = 0.13f;

    [FoldoutGroup("Scrap & Level")]
    [SerializeField] float _maxScrapFactor = 0.87f;

    //state
    Image _currentActiveSecondary;

    #region Initialization
    private void Awake()
    {
        InitializeSystemWeaponIcons();
        InitializeScrapLevelPanel();
    }

    private void InitializeScrapLevelPanel()
    {
        // This initialization is now handled by the Player Scrap Handler

        //ModifyScrapAmount(0, 0);
        //ModifyLevel(0);
    }

    private void InitializeSystemWeaponIcons()
    {
        foreach (var sid in _systemIcons)
        {
            sid.Initialize();
        }

        _primaryWeaponIcon.Initialize();

        foreach (var wid in _secondaryWeaponIcons)
        {
            wid.Initialize(_primaryFireHintSprite, _secondaryFireHintSprite);
        }
    }

    #endregion

    #region Scrap and Level

    public void ModifyScrapAmount(float scrapFillFactor, int totalScrapAmount)
    {
        if (scrapFillFactor < 0 || scrapFillFactor > 1f)
        {
            Debug.LogError("Invalid Scrap fill factor");
        }

        _scrapAmountTMP.text = totalScrapAmount.ToString();
        _scrapBarFill.fillAmount = Mathf.Lerp(_minScrapFactor, _maxScrapFactor, scrapFillFactor);
    }

    public void ModifyLevel(int newLevel)
    {
        _levelTMP.text = newLevel.ToString();
    }

    #endregion

    #region System Icons
    public SystemIconDriver IntegrateNewSystem(SystemHandler sh)
    {
        //if (index < 0 || index >= _systemIcons.Length)
        //{
        //    Debug.Log("invalid system integration index");
        //    return;
        //}
        bool foundOpenUISlot = false;
        SystemIconDriver sid = null;
        for (int i = 0; i < _systemIcons.Length; i++)
        {
            if (_systemIcons[i].IsOccupied) continue;
            else
            {
                _systemIcons[i].DisplayNewSystem(sh);
                sid = _systemIcons[i];
                foundOpenUISlot = true;
                break;
            }
        }
        if (foundOpenUISlot == false)
        {
            Debug.Log("did not find an open System slot on UI");
        }
        return sid;
    }

    public void ClearAllSystemSlots()
    {
        foreach (var slot in _systemIcons)
        {
            slot.ClearUIIcon();
        }
    }

    public void ClearSystemSlot(SystemWeaponLibrary.SystemType systemToRemove)
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

    #endregion

    #region Weapon Icons

    public void HighlightNewSecondaryWeapon(int index)
    {
        foreach (var sid in _secondaryWeaponIcons)
        {
            sid.DehighlightAsActive();
        }
        _secondaryWeaponIcons[index].HighlightAsActive();
    }
    public void IntegrateNewWeapon(WeaponHandler wh)
    {
        if (wh == null)
        {
            Debug.LogError("WeaponHandler passed is null!");
            return;
        }
        bool foundOpenWeaponSlot = false;
        if (wh.IsSecondary)
        {
            for (int i = 0; i < _secondaryWeaponIcons.Length; i++)
            {
                if (_secondaryWeaponIcons[i].IsOccupied) continue;
                else
                {
                    _secondaryWeaponIcons[i].DisplayNewWeapon(wh);
                    
                    foundOpenWeaponSlot = true;
                    break;
                }
            }
            if (foundOpenWeaponSlot == false)
            {
                Debug.Log("did not find an open Secondary Weapon slot on UI");
            }
        }
        else
        {
            if (_primaryWeaponIcon.IsOccupied)
            {
                Debug.Log("Primary weapon slot was occupied, but is now overwritten");
            }
            _primaryWeaponIcon.DisplayNewWeapon(wh);
            _primaryWeaponIcon.HighlightAsActive();
        }
        

    }

    public void ClearPrimaryWeaponSlot()
    {
        _primaryWeaponIcon.ClearUIIcon();
        _primaryWeaponIcon.DehighlightAsActive();
    }

    public void ClearAllSecondaryWeaponSlots()
    {
        foreach (var icon in _secondaryWeaponIcons)
        {
            icon.ClearUIIcon();
        }
    }

    #endregion

    #region Public Gets
    public int GetMaxSystems()
    {
        return _systemIcons.Length;
    }

    public int GetMaxWeapons()
    {
        return _secondaryWeaponIcons.Length;
    }

    public RadarScreen GetRadarScreen()
    {
        return _radarScreen;
    }

    #endregion
}
