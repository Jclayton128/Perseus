using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class SystemHandler : MonoBehaviour, IUpgradeable
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
    [SerializeField] protected int _maxUpgradeLevel = 1;
    protected bool _isInstalled  = false;
    
    [Tooltip("If true, this system can never be scrapped from player build")]
    [SerializeField] protected bool _isPermanent = true;

    [ShowInInspector] public int CurrentUpgradeLevel { get; protected set; } = 1;


    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }

    public virtual void IntegrateSystem(SystemIconDriver connectedSID)
    {
        _connectedID = connectedSID;
        //Do all the changes to ship here?
        _isInstalled = true;
    }

    public virtual void DeintegrateSystem()
    {
        _connectedID.ClearUIIcon();
        //Undo all the changes to ship here?
        _isInstalled = false;
    }

    public bool CheckIfHasRemainingUpgrades()
    {
        if (_maxUpgradeLevel <= 0)
        {
            Debug.Log("Invalide upgrade level");
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

    public void Upgrade()
    {
        if (CurrentUpgradeLevel >= _maxUpgradeLevel)
        {
            Debug.Log("Unable to upgrade past max level.");
            return;
        }

        CurrentUpgradeLevel++;
        _connectedID.ModifySystemLevel(CurrentUpgradeLevel);
        ImplementSystemUpgrade();
    }

    public abstract void ImplementSystemUpgrade();

    public abstract object GetUIStatus();

    public (Sprite, string, string, string, int) GetUpgradeDetails()
    {
        (Sprite, string, string, string, int) details;
        details.Item1 = _icon;
        details.Item2 = _name;
        details.Item3 = _description;
        details.Item4 = _upgradeDescription;

        if (CurrentUpgradeLevel >= _maxUpgradeLevel)
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

    public bool CheckIfInstalled()
    {
        return _isInstalled;
    }

    public int GetScrapRefundAmount()
    {
        return Mathf.RoundToInt(CurrentUpgradeLevel / 2f);
    }

    public bool CheckIfScrappable()
    {
        if (_isInstalled && !_isPermanent)
        {
            return true;
        }
        else return false;
    }
}
