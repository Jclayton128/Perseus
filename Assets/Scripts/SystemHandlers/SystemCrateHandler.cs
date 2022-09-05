using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCrateHandler : MonoBehaviour
{
    [SerializeField] SpriteRenderer _iconSprite = null;

    public SystemWeaponLibrary.WeaponType WeaponInCrate { get; private set; } =
        SystemWeaponLibrary.WeaponType.None;

    public SystemWeaponLibrary.SystemType SystemInCrate { get; private set; } =
    SystemWeaponLibrary.SystemType.None;

    public void Initialize(Sprite icon, SystemWeaponLibrary.WeaponType weaponInCrate,
        SystemWeaponLibrary.SystemType systemInCrate)
    {
        _iconSprite.sprite = icon;
        WeaponInCrate = weaponInCrate;
        SystemInCrate = systemInCrate;
    }

}
