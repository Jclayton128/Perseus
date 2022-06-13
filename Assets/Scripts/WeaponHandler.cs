using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] Sprite _icon = null;
    public SystemsLibrary.WeaponType WeaponType;
    public Vector2 LocalPosition;

    public bool IsSecondary = false;

    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }

    internal void IntegrateSystem()
    {
        
    }
}
