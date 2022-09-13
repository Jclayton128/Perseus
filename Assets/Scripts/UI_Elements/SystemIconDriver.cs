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
    [SerializeField] protected Image[] _parametersChargesImages = null;

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
            foreach (var im in _parametersChargesImages)
            {
                im.gameObject.SetActive(false);
            }
            return;
        }

        if (uiType is string str)
        {
            _parameterImageBar.transform.parent.gameObject.SetActive(false);
            _parameterTMP.gameObject.SetActive(true);
            _parameterTMP.text = str;
            foreach (var im in _parametersChargesImages)
            {
                im.gameObject.SetActive(false);
            }
            return;
        }

        if (uiType is float flt)
        {
            _parameterImageBar.transform.parent.gameObject.SetActive(true);
            _parameterImageBar.fillAmount = flt;
            _parameterTMP.gameObject.SetActive(false);
            foreach (var im in _parametersChargesImages)
            {
                im.gameObject.SetActive(false);
            }
            return;
        }

        if (uiType is Vector2Int v2i)
        {
            _parameterImageBar.transform.parent.gameObject.SetActive(false);
            _parameterImageBar.color = Color.white;
            _parameterTMP.gameObject.SetActive(false);
            _parameterTMP.text = "";
            if (v2i.y > _parametersChargesImages.Length)
            {
                Debug.LogError("More charges than UI space!");
                return;
            }

            for (int i = 0; i < v2i.y; i++)
            {
                _parametersChargesImages[i].gameObject.SetActive(true);
                _parametersChargesImages[i].color = Color.red;
            }

            for (int j = 0; j < v2i.x; j++)
            {
                _parametersChargesImages[j].color = Color.green;
            }

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

    public void UpdateUI(Vector2Int v2i)
    {
        for (int i = 0; i < v2i.y; i++)
        {
            _parametersChargesImages[i].gameObject.SetActive(true);
            _parametersChargesImages[i].color = Color.red;
        }

        for (int j = 0; j < v2i.x; j++)
        {
            _parametersChargesImages[j].color = Color.green;
        }
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
