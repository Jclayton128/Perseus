using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SystemCrateHandler : MonoBehaviour, IScannable
{
    [SerializeField] SpriteRenderer _iconSprite = null;
    string _crateName;
    [ShowInInspector] public SystemWeaponLibrary.WeaponType WeaponInCrate { get; private set; } =
        SystemWeaponLibrary.WeaponType.None;

    [ShowInInspector] public SystemWeaponLibrary.SystemType SystemInCrate { get; private set; } =
    SystemWeaponLibrary.SystemType.None;

    public void Initialize(Sprite icon, SystemWeaponLibrary.WeaponType weaponInCrate,
        SystemWeaponLibrary.SystemType systemInCrate, string crateName)
    {
        _iconSprite.sprite = icon;
        WeaponInCrate = weaponInCrate;
        SystemInCrate = systemInCrate;
        _crateName = crateName;
    }

    public string GetScanName()
    {
        return _crateName;
    }

    public Sprite GetScanIcon()
    {
        return _iconSprite.sprite;
    }

    public Transform GetScanTransform()
    {
        return transform;
    }

    public SystemWeaponLibrary.SystemType ScanSystemType()
    {
        return SystemInCrate;
    }

    public SystemWeaponLibrary.WeaponType ScanWeaponType()
    {
        return WeaponInCrate;
    }

    public void DestroyScannable()
    {
        Destroy(gameObject);
    }



    //public (Sprite, string) GetCrateDetails()
    //{
    //    (Sprite, string) details;

    //    details.Item1 = _iconSprite.sprite;
    //    details.Item2 = _crateName;

    //    return details;
    //}


}
