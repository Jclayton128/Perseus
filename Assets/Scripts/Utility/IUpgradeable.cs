using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable
{
    public void Upgrade();

    public bool CheckIfHasRemainingUpgrades();

    public int GetUpgradeCost();

    public (Sprite, string, string, string, int) GetUpgradeDetails();
}
