using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeamTurretWH : WeaponHandler
{
    AudioSource _onboardAudioSource;

    //settings
    [SerializeField] Transform _turretMuzzle = null;
    [SerializeField] float _maxCharge = 3f;
    [SerializeField] float _maxChargeIncrease_Upgrade = 0.5f;
    [SerializeField] float _chargeRateIncrease_Upgrade = 0.5f;
    [SerializeField] ParticleSystem _beamFX = null;
    [SerializeField] float _chargeRate = 1.0f;
    [SerializeField] float _expendRate = 2.0f;
    float _particlePerFrame = 1f;
    [SerializeField] float _maxRange = 10f;
    int _enemyLayerMask = 1 << 9;
    float _minChargeFactorToFire = 0.5f;

    //state
    bool _isBeaming = false;
    GameObject _beamFXObject;
    ParticleSystem _ps;
    ParticleSystem.ShapeModule _shape;
    Vector3 _dir;
    float _effectiveRange;
    Vector3 _midway;
    HealthHandler _targetHealthHandler;

    DamagePack _damagePack;

    float _currentCharge;
    float _chargeFactor = 1;
    Color _chargeColor;

    public override object GetUIStatus()
    {
        return _chargeFactor;
    }

    private void UpdateUI()
    {
        _chargeFactor = (_currentCharge / _maxCharge);
        _chargeColor = Color.Lerp(Color.red, Color.green, _chargeFactor);
        _connectedWID?.UpdateUI(_chargeFactor, _chargeColor);
    }
    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost) && _chargeFactor > _minChargeFactorToFire)
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            FireBeam();
            //_playerAudioSource.PlayGameplayClipForPlayer(GetRandomActivationClip());
            _hostRadarProfileHandler.AddToCurrentRadarProfile(_profileIncreaseOnActivation);
            _isBeaming = true;
        }
    }

    private void FireBeam()
    {
        if (_beamFXObject == null)
        {
            _beamFXObject = Instantiate(_beamFX.gameObject, Vector3.zero, Quaternion.identity);
            _beamFXObject.transform.parent = _turretMuzzle;
            _ps = _beamFXObject.GetComponent<ParticleSystem>();
            _shape = _ps.shape;
        }
        _onboardAudioSource.volume = _chargeFactor;
        _onboardAudioSource.Play();
        UpdateBeam();
    }

    private void Update()
    {
        if (_isBeaming)
        {
            _currentCharge -= _expendRate * Time.deltaTime;
            UpdateBeam();
            _onboardAudioSource.volume = _chargeFactor;
            if (_currentCharge < 0)
            {
                _isBeaming = false;
                _currentCharge = 0;                
                _onboardAudioSource.Stop();
                //_playerAudioSource.PlayGameplayClipForPlayer(GetRandomDeactivationClip());
            }
            UpdateUI();
        }
        else
        {
            _currentCharge += _chargeRate * Time.deltaTime;
            _currentCharge = Mathf.Clamp(_currentCharge, 0, _maxCharge);
            UpdateUI();
        }
    }

    private void UpdateBeam()
    {
        _dir = _turretMuzzle.transform.up * _maxRange;
        

        RaycastHit2D rh2d = Physics2D.Linecast(_turretMuzzle.position, _turretMuzzle.position + _dir, _enemyLayerMask);
        if (rh2d.collider != null)
        {
            //Debug.DrawLine(_turretMuzzle.position, rh2d.point, Color.red, 0.1f);
            _effectiveRange = rh2d.distance;
            
            if (_targetHealthHandler == null || rh2d.collider.gameObject != _targetHealthHandler.gameObject)
            {
                rh2d.collider.TryGetComponent<HealthHandler>(out _targetHealthHandler);
            }
            _damagePack = new DamagePack(_normalDamage * Time.deltaTime,
                _shieldBonusDamage * Time.deltaTime,
                _ionDamage * Time.deltaTime,
                _knockBackAmount * Time.deltaTime,
                _scrapBonus * Time.deltaTime);

            _targetHealthHandler?.ReceiveNonColliderDamage(_damagePack, rh2d.point, _dir);

        }
        else
        {
            //Debug.DrawLine(_turretMuzzle.position, _turretMuzzle.position + dir, Color.blue, 0.1f);
            _effectiveRange = _maxRange;
        }       



        _midway = Vector3.up * _effectiveRange / 2f;

        _beamFXObject.transform.localPosition = _midway;
        _beamFXObject.transform.localEulerAngles = Vector3.zero;

        _shape.radius = _effectiveRange / 2f;
        int count = Mathf.RoundToInt(_effectiveRange / 2f * _particlePerFrame);
        _ps.Emit(count);
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        //does nothing. weapon only does something on mouse down.
    }

    protected override void ImplementWeaponUpgrade()
    {
        _maxCharge += _maxChargeIncrease_Upgrade;
        _chargeRate += _chargeRateIncrease_Upgrade;
        UpdateUI();
    }

    protected override void InitializeWeaponSpecifics()
    {
        _currentCharge = _maxCharge;
        _onboardAudioSource = GetComponent<AudioSource>();
        _onboardAudioSource.clip = GetRandomFireClip();
    }
}
