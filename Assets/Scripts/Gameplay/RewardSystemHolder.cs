using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystemHolder : MonoBehaviour
{
    LevelController _levelController;
    public SystemWeaponLibrary.SystemType HeldSystem = SystemWeaponLibrary.SystemType.None;
    public SystemWeaponLibrary.WeaponType HeldWeapon = SystemWeaponLibrary.WeaponType.None;

    private void Awake()
    {
        GetComponent<HealthHandler>().Dying += DropReward;
    }

    public void Initialize(LevelController levelControllRef)
    {
        _levelController = levelControllRef;
    }

    private void DropReward()
    {
        if (HeldSystem != SystemWeaponLibrary.SystemType.None)
        {
            _levelController.SpawnCrateAtLocation(transform.position, HeldSystem);
        }
        else if (HeldWeapon != SystemWeaponLibrary.WeaponType.None)
        {
            _levelController.SpawnCrateAtLocation(transform.position, HeldWeapon);
        }
        else
        {
            Debug.LogError("No held system or weapon!");
        }
       
    

    }
}
