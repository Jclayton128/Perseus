using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScannable
{
    public string ScanName();

    public Sprite ScanIcon();

    public SystemWeaponLibrary.SystemType ScanSystemType();

    public SystemWeaponLibrary.WeaponType ScanWeaponType();

    public void DestroyScannable();


}
