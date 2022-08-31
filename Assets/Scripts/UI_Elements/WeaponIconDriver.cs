using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponIconDriver : SystemIconDriver
{
    [SerializeField] Image _hintIcon;
    WeaponHandler _heldWeapon;

    public SystemWeaponLibrary.WeaponType WeaponType { get; private set; }

    public void Initialize(Sprite primary, Sprite secondary)
    {
        base.Initialize();
        
        DehighlightAsActive();
    }

    public void DisplayNewWeapon(WeaponHandler wh)
    {
        if (!wh)
        {
            Debug.LogError("No WeaponHandler passed!");
            IsOccupied = false;
            return;
        }
        IsOccupied = true;
        _systemIcon.sprite = wh.GetIcon();
        if (_systemIcon.sprite != null)
        {
            _systemIcon.color = Color.white;
        }
        else
        {
            _systemIcon.color = Color.clear;
        }
        _levelTMP.text = wh.CurrentUpgradeLevel.ToString();
        WeaponType = wh.WeaponType;
        _heldWeapon = wh;
        SetupUIType(wh.GetUIStatus());
    }

    public void HighlightAsActive()
    {
        _hintIcon.enabled = true;
    }

    public void DehighlightAsActive()
    {
        _hintIcon.enabled = false;
    }
    public override void PushHeldSystemWeaponAsSelection()
    {
        if (_heldWeapon)
        {
            _uiController.UpdateSelection(_heldWeapon);
        }
        else
        {
            _uiController.ClearSelection();
        }
    }
}
