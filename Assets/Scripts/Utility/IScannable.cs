using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScannable
{
    public string GetScanName();

    public Sprite GetScanIcon();

    public SystemWeaponLibrary.SystemType ScanSystemType();

    public SystemWeaponLibrary.WeaponType ScanWeaponType();

    public void DestroyScannable();

    public Transform GetScanTransform();


}
