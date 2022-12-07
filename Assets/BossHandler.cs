using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    LevelController _levelController;
    WeaponHandler[] _weaponHandlers;
    RunController _runController;

    private void Awake()
    {
        _levelController = FindObjectOfType<LevelController>();
        _runController = _levelController.GetComponent<RunController>();
        HealthHandler hh = GetComponent<HealthHandler>();
        hh.Dying += HandleDying;
        ScaleBossWeapons();
    }

    private void ScaleBossWeapons()
    {
        _weaponHandlers = GetComponentsInChildren<WeaponHandler>();
        foreach (var wh in _weaponHandlers)
        {
            // subtract one because Run Controller increments before this boss scales.
            for (int i = 0; i < _runController.CurrentBossCount - 1; i++)
            {
                Debug.Log("Upgrading the boss' weapons");
                wh.ImplementWeaponUpgrade_Public();
            }
        }
    }

    private void HandleDying()
    {
        _levelController.UnlockAllWormholes();
        
    }
}
