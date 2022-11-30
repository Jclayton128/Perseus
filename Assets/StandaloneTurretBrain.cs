using System;
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
    HealthHandler _hostHealthHandler;

    //settings
    float _timeBetweenTargetScans = 0.5f;
    float _targetScanRange = 12f;
    [SerializeField] bool _targetsPlayers = false;
    [SerializeField] bool _targetsEnemies = false;

    //state
    Vector2 dir;
    float _lookAngle;
    float _timeForNextScan = 0;
    int _layerMask;
    Transform _targetTransform;

    private void Awake()
    {
        _energyHandler = GetComponent<EnergyHandler>();
        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _weaponHandler.Initialize(_energyHandler, false, null);
        _turretSteerer = _weaponHandler.GetComponentInChildren<TurretSteerer>();
        _hostHealthHandler = GetComponentInParent<HealthHandler>();
        _hostHealthHandler.Dying += HandleHostDeath;

        if (_targetsPlayers && !_targetsEnemies) _layerMask = LayerLibrary.PlayerLayerMask;
        if (!_targetsPlayers && _targetsEnemies) _layerMask = LayerLibrary.EnemyLayerMask;
        if (_targetsPlayers && _targetsEnemies) _layerMask = LayerLibrary.PlayerEnemyLayerMask;

    }

    private void HandleHostDeath()
    {
        Destroy(this.gameObject);
        _hostHealthHandler.Dying -= HandleHostDeath;
    }

    private void Update()
    {
        if (_targetTransform && (_targetTransform.position - transform.position).magnitude > _targetScanRange)
        {
            _targetTransform = null;
        }

        if (!_targetTransform && Time.time >= _timeForNextScan)
        {
            UpdateScan();
            _timeForNextScan = Time.time + _timeBetweenTargetScans;
        }

        if (_targetTransform)
        {
            UpdateFacing();
        }

    }

    private void UpdateFacing()
    {
        dir = _targetTransform.position - transform.position;
        _lookAngle = Vector3.SignedAngle(Vector3.up, dir, Vector3.forward);
        _turretSteerer.SetLookAngle(_lookAngle);
        _weaponHandler.Activate();
    }

    private void UpdateScan()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, _targetScanRange, _layerMask);
        if (coll) _targetTransform = coll.transform;

    }
}
