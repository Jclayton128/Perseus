using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemIconDriver : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _levelTMP = null;
    [SerializeField] protected Image _systemIcon;
    public SystemsLibrary.SystemType System { get; private set; }
    public bool IsOccupied = false;

    public virtual void Initialize()
    {
        _systemIcon.sprite = null;
        _levelTMP.text = "";
    }

    public void ModifyDisplayedSystem(Sprite sprite, int level, SystemsLibrary.SystemType system)
    {
        System = system;
        _systemIcon.sprite = sprite;
        _systemIcon.color = Color.white;
        _levelTMP.text = level.ToString();
        IsOccupied = true;
    }

    public void ClearUIIcon()
    {
        System = SystemsLibrary.SystemType.None;
        _systemIcon.sprite = null;
        _systemIcon.color = Color.clear;
        _levelTMP.text = " ";
        IsOccupied = false;

    }

}
