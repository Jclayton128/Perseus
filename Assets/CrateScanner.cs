using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScanner : MonoBehaviour
{
    [SerializeField] CircleCollider2D _crateDetectorCollider = null;
    UI_Controller _uiController;
    SystemWeaponLibrary _systemWeaponLibrary;
    GameObject _scannedCrate;

    //state
    int crateLayerMask = 1 << 19;

    private void Awake()
    {
        _uiController = FindObjectOfType<UI_Controller>();
        _systemWeaponLibrary = FindObjectOfType<SystemWeaponLibrary>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            SystemCrateHandler sch = collision.GetComponent<SystemCrateHandler>();
            (Sprite, string) details = sch.GetCrateDetails();
            _uiController.UpdateCrateScan(details.Item1, details.Item2);

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
            _scannedCrate = collision.gameObject;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _uiController.ClearCrateScan();
            _scannedCrate = null;
        }
        
    }

    public void DestroyScannedCrateAfterInstall()
    {
        if (_scannedCrate)
        {
            Destroy(_scannedCrate);
        }
    }
}
