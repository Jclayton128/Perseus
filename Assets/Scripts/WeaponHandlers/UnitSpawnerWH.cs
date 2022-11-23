using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnerWH : WeaponHandler, IMothership
{
    LevelController _levelController;
    HealthHandler _healthHandler;

    //settings
    [SerializeField] ShipInfoHolder.ShipType _spawnSType = ShipInfoHolder.ShipType.Unassigned0;
    [SerializeField] int _maxSpawnCount = 8;
    [SerializeField] int _spawnCountIncrease_Upgrade = 1;
    [SerializeField] float _activationCostMultiplier_Upgrade = .9f;

    //state
    protected int _currentSpawnCount = 0;
    Vector2Int _spawnCount = new Vector2Int(0, 0);
    [SerializeField] List<IMinionShip> _minions = new List<IMinionShip> ();

    public override object GetUIStatus()
    {
        _spawnCount.x = _currentSpawnCount;
        _spawnCount.y = _maxSpawnCount;
        return _spawnCount;
    }

    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost) && _minions.Count < _maxSpawnCount)
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            Fire();
        }
    }

    private void Fire()
    {
        IMinionShip newMinion =
            _levelController.SpawnSingleShipAtPoint(_spawnSType, _muzzle.position).GetComponent<IMinionShip>();
        
        newMinion.InitializeWithAssignedMothership(this, transform);        
        _minions.Add(newMinion);
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        //does nothing
    }

    protected override void ImplementWeaponUpgrade()
    {
        _activationCost *= _activationCostMultiplier_Upgrade;
        _maxSpawnCount += _spawnCountIncrease_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        _levelController = FindObjectOfType<LevelController>();
        _healthHandler = GetComponentInParent<HealthHandler>();
        _healthHandler.Dying += KillAllMinionsUponMothershipDeath;
    }

    /// <summary>
    /// Should be called by a minion on its mothership anytime the minion detects a threat.
    /// </summary>
    public void AlertAllMinionsToTargetTransform(Vector3 targetPosition, Vector3 targetVelocity)
    {
        foreach (var minion in _minions)
        {
            minion.AssignTarget(targetPosition, targetVelocity);
        }
    }

    public void RemoveDeadMinion(IMinionShip deadShip)
    {
        _minions.Remove(deadShip);
    }

    private void KillAllMinionsUponMothershipDeath()
    {
        for (int i = _minions.Count-1; i >= 0; i--)
        {
            _minions[i].KillMinionUponMothershipDeath();
        }
    }
}
