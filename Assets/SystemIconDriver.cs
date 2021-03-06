using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemIconDriver : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _levelTMP = null;
    [SerializeField] protected Image _systemIcon;
    public Library.SystemType System { get; private set; }
    public bool IsOccupied { get; private set; } = false;

    public virtual void Initialize()
    {
        _systemIcon.sprite = null;
        _systemIcon.color = Color.clear;
        _levelTMP.text = "";
        IsOccupied = false;
    }

    public void ModifyDisplayedSystem(Sprite sprite, int level, Library.SystemType system)
    {
        System = system;
        _systemIcon.sprite = sprite;
        _systemIcon.color = Color.white;
        _levelTMP.text = level.ToString();
        IsOccupied = true;
    }

    public void ClearUIIcon()
    {
        System = Library.SystemType.None;
        _systemIcon.sprite = null;
        _systemIcon.color = Color.clear;
        _levelTMP.text = " ";
        IsOccupied = false;

    }

}
