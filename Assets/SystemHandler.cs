using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemHandler : MonoBehaviour
{
    [SerializeField] Sprite _icon = null;

    //state
    public Library.SystemType SystemType;
    public Library.SystemLocation SystemLocation;
    protected SystemIconDriver _connectedSID;
    [SerializeField] protected int _maxUpgradeLevel = 1;
    protected int _currentUpgradeLevel = 1;

    private void Awake()
    {

    }

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
        _connectedSID = connectedSID;
    }

    public virtual void DeintegrateSystem()
    {
        _connectedSID.ClearUIIcon();
    }

    public bool CheckIfUpgradeable()
    {
        if (_maxUpgradeLevel <= 0)
        {
            Debug.Log("Invalide upgrade level");
            return false;
        }
        if (_currentUpgradeLevel == _maxUpgradeLevel)
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
        if (_currentUpgradeLevel >= _maxUpgradeLevel)
        {
            Debug.Log("Unable to upgrade past max level.");
            return;
        }

        _currentUpgradeLevel++;
    }


}
