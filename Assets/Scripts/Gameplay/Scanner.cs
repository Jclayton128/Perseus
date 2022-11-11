using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Scanner : MonoBehaviour
{
    UI_Controller _uiController;
    GameController _gameController;
    InputController _inputController;
    SystemWeaponLibrary _systemWeaponLibrary;
    [SerializeField] GameObject _scanReticlePrefab = null;
    [ShowInInspector] List<IScannable> _scannablesInRange = new List<IScannable>();

    //settings
    int _scannableLayer = 6;

    //state
    int _indexOfCurrentScan = -1;
    GameObject _scanReticle;

    private void Awake()
    {
        _uiController = FindObjectOfType<UI_Controller>();
        _gameController = _uiController.GetComponent<GameController>();
        _inputController = _uiController.GetComponent<InputController>();
        _inputController.ScanDecremented += DecrementCurrentScan;
        _inputController.ScanIncremented += IncrementCurrentScan;
        _systemWeaponLibrary = FindObjectOfType<SystemWeaponLibrary>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _scannableLayer) // Crate
        {
            IScannable scannedThing = collision.gameObject.GetComponentInParent<IScannable>();
            if (scannedThing != null && !_scannablesInRange.Contains(scannedThing))
            {
                _scannablesInRange.Add(scannedThing);
                //TODO add subtle 'gained scan info' audio clip
                if (_indexOfCurrentScan < 0)
                {
                    _indexOfCurrentScan = 0;
                    _scanReticle = Instantiate(_scanReticlePrefab, collision.transform);
                }
                PushScannedObjectToUI();
            }

        }
    }

    private void PushScannedObjectToUI()
    {
        string counterStatus;
        if (_indexOfCurrentScan >= 0)
        {
            string currentName = _scannablesInRange[_indexOfCurrentScan].ScanName();
            Sprite currentIcon = _scannablesInRange[_indexOfCurrentScan].ScanIcon();

            counterStatus = $"{_indexOfCurrentScan + 1} of {_scannablesInRange.Count}";
            _uiController.UpdateScanner(currentIcon, currentName, counterStatus);

            if (_scannablesInRange[_indexOfCurrentScan] is SystemCrateHandler sch)
            {
                if (sch.WeaponInCrate != SystemWeaponLibrary.WeaponType.None)
                {
                    IInstallable crate = _systemWeaponLibrary.GetWeaponHandler(sch.WeaponInCrate);
                    _uiController.UpdateCrateScannerSelectable(crate);
                }

                if (sch.SystemInCrate != SystemWeaponLibrary.SystemType.None)
                {
                    IInstallable crate = _systemWeaponLibrary.GetSystemHandler(sch.SystemInCrate);
                    _uiController.UpdateCrateScannerSelectable(crate);
                }
            }
        }
        else
        {
            counterStatus = "";
            _uiController.UpdateScanner(null, null, counterStatus);
        }


        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _scannableLayer)
        {
            IScannable scannedThing = collision.gameObject.GetComponentInParent<IScannable>();
            if (_scannablesInRange.Contains(scannedThing))
            {
                _scannablesInRange.Remove(scannedThing);
                //TODO add subtle 'lost scan info' audio clip
                if (_scannablesInRange.Count > 0)
                {
                    _indexOfCurrentScan = 0;
                    _scanReticle.transform.parent = _scannablesInRange[_indexOfCurrentScan].GetScanTransform();
                    _scanReticle.transform.localPosition = Vector2.zero;
                    PushScannedObjectToUI();
                }
                else
                {
                    _indexOfCurrentScan = -1;
                    Destroy(_scanReticle);
                    _uiController.ClearCrateScan();
                }  

            }

        }

    }

    public void DestroyScannedCrateAfterInstall()
    {
        if (_indexOfCurrentScan < 0) return;
        IScannable destroyedThing = _scannablesInRange[_indexOfCurrentScan];
        _scannablesInRange.RemoveAt(_indexOfCurrentScan);
        Destroy(_scanReticle);
        destroyedThing.DestroyScannable();
        
        if (_scannablesInRange.Count > 0)
        {
            _indexOfCurrentScan = 0;
            PushScannedObjectToUI();
        }
        else
        {
            _indexOfCurrentScan = -1;
            PushScannedObjectToUI();
        }
        

    }

    private void DecrementCurrentScan()
    {
        if (GameController.IsPaused) return;
        _indexOfCurrentScan--;
        if (_indexOfCurrentScan < 0)
        {
            _indexOfCurrentScan = _scannablesInRange.Count - 1;
        }
        _scanReticle.transform.parent = _scannablesInRange[_indexOfCurrentScan].GetScanTransform();
        _scanReticle.transform.localPosition = Vector2.zero;
        PushScannedObjectToUI();
    }

    private void IncrementCurrentScan()
    {
        if (GameController.IsPaused) return;
        _indexOfCurrentScan++;
        if (_indexOfCurrentScan > _scannablesInRange.Count - 1)
        {
            _indexOfCurrentScan = 0;
        }
        _scanReticle.transform.parent = _scannablesInRange[_indexOfCurrentScan].GetScanTransform();
        _scanReticle.transform.localPosition = Vector2.zero;
        PushScannedObjectToUI();
    }
}
