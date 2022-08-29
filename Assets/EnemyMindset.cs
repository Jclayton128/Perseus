using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMindset : MonoBehaviour
{
    ActorMovement _movement;
    EnergyHandler _energyHandler;
    HealthHandler _health;
    WeaponHandler _weaponHandler;

    private void Awake()
    {
       _movement = GetComponent<ActorMovement>();
        _energyHandler = GetComponent<EnergyHandler>();
        _health = GetComponent<HealthHandler>();

        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _weaponHandler.Initialize(_energyHandler, false, null);
        _weaponHandler.Activate();
    }

    private void Update()
    {

    }


}
