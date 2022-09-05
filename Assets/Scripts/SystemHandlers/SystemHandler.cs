using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class SystemHandler : MonoBehaviour, IInstallable
{
    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), PreviewField(50, ObjectFieldAlignment.Left)]
    [SerializeField] protected Sprite _icon = null;

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), HideLabel]
    [SerializeField] protected string _name = "default name";

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), Multiline(3), HideLabel]
    [SerializeField] protected string _description = "default description";

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), Multiline(3), HideLabel]
    [SerializeField] protected string _upgradeDescription = "upgrade description";

    //state
    public SystemWeaponLibrary.SystemType SystemType;
    public SystemWeaponLibrary.SystemLocation SystemLocation;
    
    protected SystemIconDriver _connectedID;
    [SerializeField] protected int _maxUpgradeLevel = 5;
    public bool IsInstalled { get; private set; } = false;

    [Tooltip("If true, this system can never be scrapped from player build")]
    protected readonly bool _isPermanent = false;

    [ShowInInspector] public int CurrentUpgradeLevel { get; protected set; } = 1;

 
    #region Interface Compliance
    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }

    public string GetName()
    {
        return _name;
    }
    public SystemWeaponLibrary.WeaponType GetWeaponType()
    {
        return SystemWeaponLibrary.WeaponType.None;
    }

    public SystemWeaponLibrary.SystemType GetSystemType()
    {
        return SystemType;
    }

    public bool CheckIfHasRemainingUpgrades()
    {
        if (_maxUpgradeLevel <= 0)
        {
            Debug.Log("Invalid upgrade level");
            return false;
        }
        if (CurrentUpgradeLevel == _maxUpgradeLevel)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// This is used to apply a Level 2 and up bonus to an already-installed system.
    /// </summary>
    public void Upgrade()
    {
        if (CurrentUpgradeLevel >= _maxUpgradeLevel)
        {
            Debug.Log("Unable to upgrade past max level.");
            return;
        }

        CurrentUpgradeLevel++;
        _connectedID.ModifySystemLevel(CurrentUpgradeLevel);
        Debug.Log($"Implementing system-specific upgrades for level {CurrentUpgradeLevel}");
        ImplementSystemUpgrade();
    }

    /// <summary>
    /// This is used to remove all Level 2 and up incremental bonuses provided by a system.
    /// </summary>
    private void Downgrade()
    {
        for (int i = CurrentUpgradeLevel; i > 1; i--)
        {
            Debug.Log($"Implementing system-specific downgrades for level {CurrentUpgradeLevel}");
            ImplementSystemDowngrade();
            CurrentUpgradeLevel--;
        }

    }

    public (Sprite, string, string, string, int) GetUpgradeDetails()
    {
        (Sprite, string, string, string, int) details;
        details.Item1 = _icon;
        details.Item2 = _name;
        details.Item3 = _description;
        details.Item4 = _upgradeDescription;

        if (CurrentUpgradeLevel < _maxUpgradeLevel)
        {
            details.Item5 = CurrentUpgradeLevel;
        }
        else
        {
            details.Item5 = -1;
        }

        return details;
    }

    public int GetUpgradeCost()
    {
        return CurrentUpgradeLevel;
    }

    public bool CheckIfInstallable()
    {
        return !IsInstalled;
    }

    public int GetScrapRefundAmount()
    {
        return Mathf.RoundToInt(CurrentUpgradeLevel / 2f);
    }

    public bool CheckIfScrappable()
    {
        if (IsInstalled && !_isPermanent)
        {
            return true;
        }
        else return false;
    }

    public void Scrap()
    {
        Downgrade();
        DeintegrateSystem();
    }
    #endregion;

    /// <summary>
    /// Contains all the logic for the initial install of a system (ie, going from nothing to level 1)
    /// </summary>
    /// <param name="connectedSID"></param>
    public virtual void IntegrateSystem(SystemIconDriver connectedSID)
    {
        _connectedID = connectedSID;
        //Do all the level 1 changes to ship here
        IsInstalled = true;
    }

    /// <summary>
    /// Contains all the logic for the last de-install of a system (ie, removing all traces after a Scrap System action)
    /// </summary>
    public virtual void DeintegrateSystem()
    {
        _connectedID.ClearUIIcon();
        //Undo all the level 1 upgrades here
        IsInstalled = false;
    }


    protected abstract void ImplementSystemUpgrade();

    protected abstract void ImplementSystemDowngrade();

    /// <summary>
    /// Sets up the associated Icon Driver for a system or weapon. Returning a string sets it up for 
    /// short texts. A Float sets it up for a charge bar. An Int sets up a charge-based UI 
    /// (ie, 3 of 6 remaining). A null sets it up to be blank.
    /// </summary>
    /// <returns></returns>
    public abstract object GetUIStatus();

    
}
