using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWormholeHandler : MonoBehaviour
{
    LevelController _levelController;

    private void Awake()
    {
        _levelController = FindObjectOfType<LevelController>();
        HealthHandler hh = GetComponent<HealthHandler>();
        hh.Dying += HandleDying;
    }

    private void HandleDying()
    {
        _levelController.UnlockAllWormholes();
    }
}
