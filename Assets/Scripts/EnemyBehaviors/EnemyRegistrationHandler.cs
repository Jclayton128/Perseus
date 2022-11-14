using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistrationHandler : MonoBehaviour
{
    HealthHandler hh;
    LevelController _levelController;

    private void Awake()
    {
        hh = GetComponent<HealthHandler>();
        hh.Dying += HandleDying;
    }

    public void Initialize(LevelController levelConRef)
    {
        _levelController = levelConRef;
    }

    private void HandleDying()
    {
        _levelController.DeregisterDeadEnemy(this);
    }
}
