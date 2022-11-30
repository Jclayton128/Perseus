using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandaloneTurretBrain : MonoBehaviour
{
    // Look for a target
    // Steer turret weapon to face target
    // Activate weapon if within range

    EnergyHandler _energyHandler;
    WeaponHandler _weaponHandler;
    TurretSteerer _turretSteerer;

    //state
    float _lookAngle;
    float _timeForNewRandomLookAngle = 5;

    private void Awake()
    {
        _energyHandler = GetComponent<EnergyHandler>();
        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _weaponHandler.Initialize(_energyHandler, false, null);
        _turretSteerer = _weaponHandler.GetComponentInChildren<TurretSteerer>();
    }

    private void Update()
    {
        if (Time.time >= _timeForNewRandomLookAngle)
        {
            _lookAngle = UnityEngine.Random.Range(-179f, 179f);
            _turretSteerer.SetLookAngle(_lookAngle);
            _timeForNewRandomLookAngle = Time.time + 10f;
            _weaponHandler.Activate();
        }   
    }

}
