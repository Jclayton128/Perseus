using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CrateScanner : MonoBehaviour
{
    [SerializeField] CircleCollider2D _crateDetectorCollider = null;
    UI_Controller _uiController;
    SystemWeaponLibrary _systemWeaponLibrary;
    [ShowInInspector] List<IScannable> _scannablesInRange = new List<IScannable>();

    //settings
    int crateLayerMask = 1 << 19;

    //state
    int _indexOfCurrentScan = -1;

    private void Awake()
    {
        _uiController = FindObjectOfType<UI_Controller>();
        _systemWeaponLibrary = FindObjectOfType<SystemWeaponLibrary>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            IScannable scannedThing = collision.gameObject.GetComponent<IScannable>();
            if (scannedThing != null)
            {
                _scannablesInRange.Add(scannedThing);
            }
            if (_scannablesInRange.Count == 1)
            {
                _indexOfCurrentScan = 0;
            }
            PushScannedObjectToUI();
        }
    }

    private void PushScannedObjectToUI()
    {
        string currentName = _scannablesInRange[_indexOfCurrentScan].ScanName();
        Sprite currentIcon = _scannablesInRange[_indexOfCurrentScan].ScanIcon();
        _uiController.UpdateCrateScan(currentIcon, currentName);

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _uiController.ClearCrateScan();
        }
        
    }

    public void DestroyScannedCrateAfterInstall()
    {
        if (_indexOfCurrentScan < 0) return;
        IScannable destroyedThing = _scannablesInRange[_indexOfCurrentScan];
        _scannablesInRange.RemoveAt(_indexOfCurrentScan);
        destroyedThing.DestroyScannable();

    }
}
