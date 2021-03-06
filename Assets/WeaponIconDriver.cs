using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponIconDriver : SystemIconDriver
{
    [SerializeField] Image _hintIcon;
    public Library.WeaponType WeaponType { get; private set; }
    Sprite _primarySprite;
    Sprite _secondarySprite;

    public void Initialize(Sprite primary, Sprite secondary)
    {
        base.Initialize();
        _primarySprite = primary;
        _secondarySprite = secondary;
        DehighlightAsActiveSecondaryIfNotPrimary();
        DehighlightAsActivePrimary();

    }

    public void ModifyDisplayedSystem(Sprite sprite, int level, Library.WeaponType weaponType)
    {
        _systemIcon.sprite = sprite;
        _systemIcon.color = Color.white;
        _levelTMP.text = level.ToString();
        WeaponType = weaponType;
    }

    public void HighlightAsActivePrimary()
    {
        _hintIcon.enabled = true;
        _hintIcon.sprite = _primarySprite;
    }

    public void DehighlightAsActivePrimary()
    {
        _hintIcon.enabled = false;
        _hintIcon.sprite = null;
    }
    public void HighlightAsActiveSecondaryIfNotPrimary()
    {
        if (_hintIcon.sprite != _primarySprite)
        {
            _hintIcon.enabled = true;
            _hintIcon.sprite = _secondarySprite;
        }

    }

    public void DehighlightAsActiveSecondaryIfNotPrimary()
    {
        if (_hintIcon.sprite != _primarySprite)
        {
            _hintIcon.enabled = false;
            _hintIcon.sprite = null;
        }
    }
}
