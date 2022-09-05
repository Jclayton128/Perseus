using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SystemIconDriver : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _levelTMP = null;
    [SerializeField] protected Image _systemIcon;
    protected UI_Controller _uiController;

    public SystemWeaponLibrary.SystemType System { get; private set; }
    public bool IsOccupied = false;// { get; protected set; } = false;

    [SerializeField] SystemHandler _heldSystem;
    [SerializeField] protected TextMeshProUGUI _parameterTMP = null;
    [SerializeField] protected Image _parameterImageBar = null;

    public virtual void Initialize()
    {
        _systemIcon.sprite = null;
        _systemIcon.color = Color.clear;
        _levelTMP.text = "";
        IsOccupied = false;
        _uiController = FindObjectOfType<UI_Controller>();
    }

    public void ModifySystemLevel(int newLevel)
    {
        _levelTMP.text = newLevel.ToString();
    }

    protected void SetupUIType(object uiType)
    {
        if (uiType is null)
        {
            _parameterImageBar.transform.parent.gameObject.SetActive(false);
            _parameterImageBar.color = Color.white;
            _parameterTMP.gameObject.SetActive(false);
            _parameterTMP.text = "";
            return;
        }

        if (uiType is string)
        {
            _parameterImageBar.transform.parent.gameObject.SetActive(false);
            _parameterTMP.gameObject.SetActive(true);
            _parameterTMP.text = (string)uiType;
            return;
        }

        if (uiType is float)
        {
            _parameterImageBar.transform.parent.gameObject.SetActive(true);
            _parameterImageBar.fillAmount = (float)uiType;

            _parameterTMP.gameObject.SetActive(false);

            return;
        }

        if (uiType is Vector2Int)
        {
            //TODO implement a discrete set of pips to show charges remaining of something
            //This would be used for the Mega Revolver weapon (ie, 3 shots total, 1 left).
        }

    }

    public void DisplayNewSystem(SystemHandler sh)
    {
        _heldSystem = sh;
        System = sh.SystemType;
        _systemIcon.sprite = sh.GetIcon();
        if (sh.GetIcon() != null)
        {
            _systemIcon.color = Color.white;
        }
        _levelTMP.text = sh.CurrentUpgradeLevel.ToString();
        IsOccupied = true;

        SetupUIType(sh.GetUIStatus());
    }

    public virtual void ClearUIIcon()
    {
        System = SystemWeaponLibrary.SystemType.None;
        _systemIcon.sprite = null;
        _systemIcon.color = Color.clear;
        _levelTMP.text = " ";
        IsOccupied = false;

        SetupUIType(null);
    }

    public void UpdateUI(string newString)
    {
        _parameterTMP.text = newString;
    }

    public void UpdateUI(float factor)
    {
        _parameterImageBar.fillAmount = factor;
    }

    public void UpdateUI(float factor, Color color)
    {
        _parameterImageBar.fillAmount = factor;
        _parameterImageBar.color = color;
    }


    public virtual void PushHeldSystemWeaponAsSelection()
    {
        if (_heldSystem)
        {
            _uiController.UpdateSelection(_heldSystem);
        }
        else
        {
            _uiController.ClearSelection();
        }

    }

}
