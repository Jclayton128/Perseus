using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Scanner : MonoBehaviour
{
    UI_Controller _uiController;
    InputController _inputController;
    AudioController _audioController;
    SystemWeaponLibrary _systemWeaponLibrary;
    [SerializeField] ScanReticleHandler _scanReticlePrefab = null;

    //settings
    int _scannableLayer = 6;
    [SerializeField] float _scanRange = 10f;
    [SerializeField] float _minTimeBetweenScans = 0.5f;

    //state
    IInstallable _scannedInstallable;
    IScannable _scannedThing;
    IScannable _previousScannedThing;
    ScanReticleHandler _scanReticle;
    float _timeForNextScan = 0;

    private void Awake()
    {
        _uiController = FindObjectOfType<UI_Controller>();
        _inputController = _uiController.GetComponent<InputController>();
        _audioController = _uiController.GetComponent<AudioController>();
        _systemWeaponLibrary = FindObjectOfType<SystemWeaponLibrary>();
        _inputController.LookDirChanged += HandleLookDirectionChanged;
    }

    private void HandleLookDirectionChanged(Vector2 throwawayVec, float throwawayFloat)
    {
        if (Time.time < _timeForNextScan) return;

        _timeForNextScan = Time.time + _minTimeBetweenScans;

        _previousScannedThing = _scannedThing;
        _scannedThing = RaycastInLookDirection();

        if (_scannedThing != _previousScannedThing)
        {
            AttachReticleToScannedThing();
            PushScannedThingToUI();

            _audioController.PlayUIClip(AudioLibrary.ClipID.ScannerPickup);
        }
        else if (_previousScannedThing != null && _scannedThing == null)
        {
            _audioController.PlayUIClip(AudioLibrary.ClipID.ScannerDrop);
        }

    }

    private void AttachReticleToScannedThing()
    {
        if (_scannedThing != null)
        {
            if (!_scanReticle) _scanReticle = Instantiate(_scanReticlePrefab);

            _scanReticle.transform.parent = _scannedThing.GetScanTransform();
            _scanReticle.transform.localPosition = Vector3.zero;
        }
        else
        {
            if (_scanReticle)
            {
                _scanReticle.transform.parent = null;
                _scanReticle.transform.position = new Vector2(999, 999);
            }
        }
    }

    private IScannable RaycastInLookDirection()
    {
        var hit = Physics2D.CircleCast(transform.position, 2f,
            _inputController.LookDirection, _scanRange, 1<< _scannableLayer);
        Debug.DrawLine(transform.position,
            (Vector2)transform.position + (_inputController.LookDirection * _scanRange), Color.yellow);
        
        IScannable hitscan;
        if (hit && hit.transform.TryGetComponent<IScannable>(out hitscan))
        {
            if (hitscan is SystemCrateHandler sch)
            {
                if (sch.WeaponInCrate != SystemWeaponLibrary.WeaponType.None)
                {
                    _scannedInstallable = _systemWeaponLibrary.GetWeaponHandler(sch.WeaponInCrate);
                    _uiController.UpdateCrateScannerSelectable(_scannedInstallable);
                }
                else if (sch.SystemInCrate != SystemWeaponLibrary.SystemType.None)
                {
                    _scannedInstallable = _systemWeaponLibrary.GetSystemHandler(sch.SystemInCrate);
                    _uiController.UpdateCrateScannerSelectable(_scannedInstallable);                    
                }
            }
            return hitscan;
        }
        else
        {
            _scannedInstallable = null;
            return null;
        }

    }

    private void PushScannedThingToUI()
    {
        if (_scannedThing != null)
        {
            _uiController.UpdateScanner(_scannedThing.GetScanIcon(), _scannedThing.GetScanName(), "ray");

            if (_scannedThing.IsInstallable) _scanReticle?.ToggleTabTip(true);
            else _scanReticle?.ToggleTabTip(false);
        }
        else
        {
            _uiController?.ClearCrateScan();
        }
    }

    public void DestroyScannedCrateAfterInstall()
    {
        if (_scanReticle)
        {
            _scanReticle.transform.parent = null;
            _scanReticle.transform.position = new Vector3(999, 999, 0);
        }
        _scannedThing?.DestroyScannable();
        PushScannedThingToUI();
    }

    private void OnDestroy()
    {
        _inputController.LookDirChanged -= HandleLookDirectionChanged;
    }
}
