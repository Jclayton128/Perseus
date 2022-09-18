using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTurretWH : WeaponHandler
{
    //settings
    [SerializeField] Transform _turretMuzzle = null;
    [SerializeField] float _maxCharge = 3f;
    [SerializeField] float _maxChargeIncrease_Upgrade = 0.5f;
    [SerializeField] ParticleSystem _beamFX = null;
    [SerializeField] float _chargeRate = 1.0f;
    float _particlePerFrame = 1f;

    //state
    bool _isBeaming = false;

    GameObject _beamFXObject;
    ParticleSystem _ps;
    ParticleSystem.ShapeModule shape;
    ParticleSystem.EmissionModule emission;
    Vector3 endPoint;
    Vector3 dir;
    float angle;
    Quaternion rot;
    [SerializeField] Vector3 midway;
    

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
        if (_hostEnergyHandler.CheckEnergy(_activationCost))
        {
            FireBeam();
            _isBeaming = true;
        }
    }

    private void FireBeam()
    {
        endPoint = _inputCon.MousePos;
        dir = (endPoint - _turretMuzzle.position);
        angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
        rot = Quaternion.Euler(0, 0, angle);
        midway = _turretMuzzle.position + (endPoint - _turretMuzzle.position) / 2f;
        Debug.Log($"muzzle Pos: {_turretMuzzle.position}");

        if (_beamFXObject == null)
        {
            _beamFXObject = Instantiate(_beamFX.gameObject, midway, rot);
            _beamFXObject.transform.parent = this.transform;
            _ps = _beamFXObject.GetComponent<ParticleSystem>();
            shape = _ps.shape;
            emission = _ps.emission;
        }
        else
        {
            _beamFXObject.transform.position = midway;
            _beamFXObject.transform.rotation = rot;
            _beamFXObject.SetActive(true);
        }

        shape.radius = dir.magnitude / 2f;
        int count = Mathf.RoundToInt(dir.magnitude / 2f * _particlePerFrame);
        _ps.Emit(count);
        //ParticleSystem.Burst newBurst = new ParticleSystem.Burst(0.00f, count, 3, 0.1f);
        //emission.SetBurst(0, newBurst);
    }

    private void Update()
    {
        if (_isBeaming)
        {
            _currentCharge -= _chargeRate * Time.deltaTime * 2f;
            UpdateBeam();
            if (_currentCharge < 0)
            {
                _isBeaming = false;
                //_beamFXObject.SetActive(false);
                _currentCharge = 0;
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
        endPoint = _inputCon.MousePos;
        dir = (-endPoint + _turretMuzzle.position);
        angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
        rot = Quaternion.Euler(0, 0,angle);

        midway = _turretMuzzle.position + (endPoint - _turretMuzzle.position) / 2f;

        _beamFXObject.transform.position = midway;
        _beamFXObject.transform.rotation = rot;

        shape.radius = dir.magnitude / 2f;
        int count = Mathf.RoundToInt(dir.magnitude / 2f * _particlePerFrame);
        _ps.Emit(count);
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        //does nothing. weapon only does something on mouse down.
    }

    protected override void ImplementWeaponUpgrade()
    {
        _maxCharge += _maxChargeIncrease_Upgrade;
        UpdateUI();
    }

    protected override void InitializeWeaponSpecifics()
    {
        _currentCharge = _maxCharge;
    }
}
