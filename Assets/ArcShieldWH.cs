using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcShieldWH : WeaponHandler
{
    InputController _inputController;
    ArcShieldHandler _arcShieldHandler;

    //settings
    [SerializeField] GameObject _arcShieldPrefab = null;
    [SerializeField] float _shieldTime = 5f;
    [SerializeField] float _coolRate = 0.2f;

    [SerializeField] float _normalDamageMultiplier_Upgrade = 1.2f;
    [SerializeField] float _cooldownRateMultiplier_Upgrade = 1.5f;
    [SerializeField] Color _emitColor_full = Color.green;
    [SerializeField] Color _emitColor_empty = Color.yellow;

    //state
    float _timeToDeactivate = Mathf.Infinity;
    DamagePack _damagePack;
    bool _isEmitting = false;
    Vector3 _rotation = Vector3.zero;
    float _emitTime = 0;
    float _factor = 1;
    Color _color = Color.white;


    public override object GetUIStatus()
    {
        return _factor;
    }

    

    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost) && _factor >= 1f)
        {
            _isEmitting = true;
            _arcShieldHandler.ToggleStatus(true);
            _timeToDeactivate = Time.time + _shieldTime;
            _emitTime = 0;
            _factor = 1;
        }
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        // deactivated via time elapsed or energy exhausted
    }

    private void DeactivateForced()
    {
        _isEmitting = false;
        _arcShieldHandler.ToggleStatus(false);
    }

    protected override void ImplementWeaponUpgrade()
    {
        _normalDamage *= _normalDamageMultiplier_Upgrade;
        _coolRate *= _cooldownRateMultiplier_Upgrade;

        _damagePack.NormalDamage = _normalDamage;
        _arcShieldHandler.SetDamagePack(_damagePack);
    }

    protected override void InitializeWeaponSpecifics()
    {
        _inputController = FindObjectOfType<InputController>();

        _damagePack = new DamagePack(_normalDamage, _shieldBonusDamage,
            _ionDamage, _knockBackAmount,  _scrapBonus);

        if (!_arcShieldHandler)
        {
            _arcShieldHandler =
                Instantiate(_arcShieldPrefab, transform.position, Quaternion.identity).
                GetComponent<ArcShieldHandler>();
            _arcShieldHandler.SetDamagePack(_damagePack);
            _arcShieldHandler.ToggleStatus(false);
        }
    }

    private void Update()
    {
        UpdateUI();
        UpdateFacingAndPosition();

        if (_isEmitting)
        {
            _emitTime += Time.deltaTime;
            if (Time.time >= _timeToDeactivate)
            {
                DeactivateForced();
                return;
            }
            if (!_hostEnergyHandler.CheckEnergy(_activationCost * Time.deltaTime))
            {
                DeactivateForced();
                return;
            }

            _hostEnergyHandler.SpendEnergy(_activationCost * Time.deltaTime);
        }
        else
        {
            _emitTime -= Time.deltaTime * _coolRate;
        }

        _factor = 1 - (_emitTime / _shieldTime);
        _factor = Mathf.Clamp01(_factor);
    }

    private void UpdateUI()
    {
        if (_factor < 1f)
        {
            _color = _emitColor_empty;
        }
        else
        {
            _color = _emitColor_full;
        }
        _connectedWID?.UpdateUI(_factor, _color);
    }

    private void UpdateFacingAndPosition()
    {
        if (_isPlayer)
        {
            _arcShieldHandler.transform.position = transform.position;
            _rotation.z = _inputController.LookAngle;
            _arcShieldHandler.transform.rotation = Quaternion.Euler(_rotation);
        }
        else
        {
            _arcShieldHandler.transform.position = transform.position;
            _arcShieldHandler.transform.rotation = transform.rotation;
        }

    }
    private void OnDestroy()
    {
        if (_arcShieldHandler != null) Destroy(_arcShieldHandler.gameObject);
    }




}
