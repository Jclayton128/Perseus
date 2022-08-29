using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class SystemHandler : MonoBehaviour
{
    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), PreviewField(50, ObjectFieldAlignment.Left)]
    [SerializeField] protected Sprite _icon = null;


    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), Multiline(3), HideLabel]
    [SerializeField] protected string _description = "default description";

    //state
    public SystemWeaponLibrary.SystemType SystemType;
    public SystemWeaponLibrary.SystemLocation SystemLocation;
    
    protected SystemIconDriver _connectedID;
    [SerializeField] protected int _maxUpgradeLevel = 1;

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
    }

    public virtual void DeintegrateSystem()
    {
        _connectedID.ClearUIIcon();
        //Undo all the changes to ship here?
    }

    public bool CheckIfUpgradeable()
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

    public virtual void Upgrade()
    {
        if (CurrentUpgradeLevel >= _maxUpgradeLevel)
        {
            Debug.Log("Unable to upgrade past max level.");
            return;
        }

        CurrentUpgradeLevel++;
    }

    public abstract object GetUIStatus();


}
