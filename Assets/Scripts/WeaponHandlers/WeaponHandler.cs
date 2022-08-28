using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class WeaponHandler : MonoBehaviour
{
    protected PoolController _poolCon;
    protected InputController _inputCon;
    protected AudioController _audioCon;
    protected EnergyHandler _hostEnergyHandler;
    protected Rigidbody2D _rb;

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

    [FoldoutGroup("Audio")]
    [SerializeField] protected AudioClip[] _activationSounds = null;

    [FoldoutGroup("Audio")] [Tooltip("This option clips may be called internally by other WHs")]
    [SerializeField] protected AudioClip[] _firingSounds = null;

    [FoldoutGroup("Audio")]
    [SerializeField] protected AudioClip[] _deactivationSounds = null;


    protected Transform _muzzle;
    public bool IsSecondary = false;
    protected bool _isPlayer;
    protected WeaponIconDriver _connectedWID;
    [SerializeField] protected int _maxUpgradeLevel = 1;
    [ShowInInspector] public int CurrentUpgradeLevel { get; protected set; } = 1;


    public void Initialize(EnergyHandler hostEnergyHandler, bool isPlayer,
        WeaponIconDriver wid)
    {
        _inputCon = FindObjectOfType<InputController>();
        _audioCon = _inputCon.GetComponent<AudioController>();
        _rb = GetComponentInParent<Rigidbody2D>();
        _poolCon = _inputCon.GetComponent<PoolController>();
        _muzzle = GetComponentInChildren<MuzzleTag>().transform;
        _hostEnergyHandler = hostEnergyHandler;
        _isPlayer = isPlayer;
        _connectedWID = wid;
        InitializeWeaponSpecifics();
    }

    #region Universal Weapon Methods
    public abstract object GetUIStatus();

    protected abstract void InitializeWeaponSpecifics();

    public abstract void Activate();

    public abstract void Deactivate();

    protected abstract void ImplementWeaponUpgrade();

    #endregion


    public void UpdateWeaponIconDriver(WeaponIconDriver newWID)
    {
        _connectedWID = newWID;
    }

    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }


    public bool CheckIfUpgradeable()
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

    public virtual void Upgrade()
    {
        if (CurrentUpgradeLevel >= _maxUpgradeLevel)
        {
            Debug.Log("Unable to upgrade past max level.");
            return;
        }
        CurrentUpgradeLevel++;
        ImplementWeaponUpgrade();
    }

    #region Sound Helpers
    protected AudioClip GetRandomActivationClip()
    {
        if (_activationSounds.Length == 0) return null;
        int rand = UnityEngine.Random.Range(0,_activationSounds.Length);
        return _activationSounds[rand];
    }

    protected AudioClip GetRandomFireClip()
    {
        if (_firingSounds.Length == 0) return null;
        int rand = UnityEngine.Random.Range(0, _firingSounds.Length);
        return _firingSounds[rand];
    }

    protected AudioClip GetRandomDeactivationClip()
    {
        if (_deactivationSounds.Length == 0) return null;
        int rand = UnityEngine.Random.Range(0, _deactivationSounds.Length);
        return _deactivationSounds[rand];
    }

    #endregion

}
