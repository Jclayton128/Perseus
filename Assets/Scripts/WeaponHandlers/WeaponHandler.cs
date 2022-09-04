using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class WeaponHandler : MonoBehaviour, IUpgradeable
{
    protected PoolController _poolCon;
    protected InputController _inputCon;
    protected AudioSource _hostAudioSource;
    protected AudioController _playerAudioSource;
    protected EnergyHandler _hostEnergyHandler;
    protected Rigidbody2D _rb;

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), PreviewField(50, ObjectFieldAlignment.Left)]
    [SerializeField] protected Sprite _icon = null;

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), HideLabel]
    [SerializeField] protected string _name = "default name";

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"),Multiline(3), HideLabel]
    [SerializeField] protected string _description = "default description";

    //[FoldoutGroup("Brochure")]
    [FoldoutGroup("Brochure"), Multiline(3), HideLabel]
    [SerializeField] protected string _upgradeDescription = "upgrade description";

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

    PlayerStateHandler _playerStateHandler;
    protected Transform _muzzle;
    public bool IsSecondary = false;

    [Tooltip("If true, this weapon can never be scrapped.")]
    [SerializeField] protected bool _isPermanent = false;

    protected bool _isPlayer;
    protected WeaponIconDriver _connectedWID;
    [SerializeField] protected int _maxUpgradeLevel;
    [ShowInInspector] public int CurrentUpgradeLevel { get; protected set; } = 1;
    protected bool _isInstalled = false;




    public void Initialize(EnergyHandler hostEnergyHandler, bool isPlayer,
        WeaponIconDriver wid)
    {
        _inputCon = FindObjectOfType<InputController>();
        _rb = GetComponentInParent<Rigidbody2D>();
        _poolCon = _inputCon.GetComponent<PoolController>();
        _muzzle = GetComponentInChildren<MuzzleTag>().transform;
        _hostEnergyHandler = hostEnergyHandler;
        _isPlayer = isPlayer;

        _isInstalled = true;

        if (_isPlayer)
        {

            _playerAudioSource = _inputCon.GetComponent<AudioController>();
        }
        else
        {
            _hostAudioSource = GetComponentInParent<AudioSource>();
        }

        _connectedWID = wid;
        InitializeWeaponSpecifics();
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
            //Debug.Log($"{WeaponType} can't be upgraded. At level {CurrentUpgradeLevel} of {_maxUpgradeLevel}");
            details.Item5 = -1;
        }       

        return details;
    }

    #region Universal Weapon Methods

    /// <summary>
    /// Returning a string sets up the Icon Driver to show a string. Float: Charge Bar. Int: 
    /// Series of pips for discrete counts of things.
    /// </summary>
    /// <returns></returns>
    public abstract object GetUIStatus();

    protected abstract void InitializeWeaponSpecifics();

    public void Activate()
    {
        if (!GameController.IsPaused) ActivateInternal();
    }

    public void Deactivate()
    {
        if (GameController.IsPaused) DeactivateInternal(true);
        else DeactivateInternal(false);
    }
    protected abstract void ActivateInternal();

    protected abstract void DeactivateInternal(bool wasPausedDuringDeactivationAttempt);

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

    public int GetUpgradeCost()
    {
        return CurrentUpgradeLevel;
    }

    public bool CheckIfHasRemainingUpgrades()
    {
        if (_maxUpgradeLevel <= 0)
        {
            Debug.Log("Invalid upgrade level");
            return false;
        }
        if (CurrentUpgradeLevel >= _maxUpgradeLevel)
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
        _connectedWID.ModifySystemLevel(CurrentUpgradeLevel);
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
    public bool CheckIfInstalled()
    {
        return _isInstalled;
    }



    #endregion

}
