using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInstallable
{
    public void Upgrade();

    public void Scrap();

    public bool CheckIfHasRemainingUpgrades();

    public (bool,string) CheckIfInstallable();

    public bool CheckIfInstalled();

    public bool CheckIfScrappable();


    public int GetUpgradeCost();

    public int GetScrapRefundAmount();

    public (Sprite, string, string, string, int) GetUpgradeDetails();

    public SystemWeaponLibrary.WeaponType GetWeaponType();

    public SystemWeaponLibrary.SystemType GetSystemType();

}
