using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemIconDriver : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _levelTMP = null;
    [SerializeField] protected Image _systemIcon;

    public virtual void Initialize()
    {
        _systemIcon.sprite = null;
        _levelTMP.text = "";
    }

    public void ModifyDisplayedSystem(Sprite sprite, int level)
    {
        _systemIcon.sprite = sprite;
        _levelTMP.text = level.ToString();
    }

}
