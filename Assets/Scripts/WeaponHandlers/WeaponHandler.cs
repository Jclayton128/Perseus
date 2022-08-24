using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class WeaponHandler : MonoBehaviour
{
    protected PoolController _poolCon;
    protected InputController _inputCon;


    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), PreviewField(50, ObjectFieldAlignment.Left)]
    [SerializeField] protected Sprite _icon = null;


    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"),Multiline(3), HideLabel]
    [SerializeField] protected string _description = "default description";

    public SystemWeaponLibrary.WeaponType WeaponType;
    [SerializeField] protected ProjectileBrain.PType _projectileType;

    [FoldoutGroup("Damage Pack")]
    [SerializeField] protected float _normalDamage = 0;
    [FoldoutGroup("Damage Pack")]
    [SerializeField] protected float _shieldBonusDamage = 0;
    [FoldoutGroup("Damage Pack")]
    [SerializeField] protected float _ionDamage = 0;
    [FoldoutGroup("Damage Pack")]
    [SerializeField] protected float _knockBackAmount = 0;
    [FoldoutGroup("Damage Pack")]
    [SerializeField] protected float _scrapBonus = 0;


    [SerializeField] protected float _activationCost = 0;
    [SerializeField] protected float _sustainCostRate = 0;
    protected Transform _muzzle;

    public bool IsSecondary = false;

    protected WeaponIconDriver _connectedWID;
    [SerializeField] protected int _maxUpgradeLevel = 1;
    [ShowInInspector] public int CurrentUpgradeLevel { get; protected set; } = 1;


    public virtual void Initialize()
    {
        _inputCon = FindObjectOfType<InputController>();
        _poolCon = _inputCon.GetComponent<PoolController>();
        _muzzle = GetComponentInChildren<MuzzleTag>().transform;
    }
    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }
    public abstract void Activate();
    public abstract void Deactivate();

    //Very likely that individual weapons don't need to integrate uniquely
    //Weapons shouldn't be changing anything about the ship itself.
    public void IntegrateSystem(WeaponIconDriver connectedWID)
    {
        _connectedWID = connectedWID;
        BroadcastMessage("Initialize");
    }

    public void DeintegrateSystem()
    {
        _connectedWID.ClearUIIcon();
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

}