using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistrationHandler : MonoBehaviour
{
    HealthHandler hh;
    LevelController _levelController;

    //state
    public bool ShouldChangeThreatCount = true;

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
        //Initialize may not have been run for a minion ship, hence the ?. operator
        _levelController?.DeregisterDeadEnemy(this);
    }
}
