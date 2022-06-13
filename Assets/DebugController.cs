using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    PhysicsHandler _playerPH;
    GameController _gameController;

    private void Awake()
    {
        _gameController = GetComponent<GameController>();
    }


    public void HandleMoreThrust()
    {
        if (_playerPH == null) 
            _playerPH = _gameController.GetPlayerGO().GetComponent<PhysicsHandler>();

        _playerPH.ModifyThrust(20f);

    }
    public void HandleLessThrust()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PhysicsHandler>();

        _playerPH.ModifyThrust(-20f);

    }
    public void HandleMoreMass()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PhysicsHandler>();

        _playerPH.ModifyMass(2f);

    }
    public void HandleLessMass()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PhysicsHandler>();

        _playerPH.ModifyMass(-2f);
    }
    public void HandleMoreTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PhysicsHandler>();

        _playerPH.ModifyTurnRate(20f);
    }
    public void HandleLessTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PhysicsHandler>();

        _playerPH.ModifyTurnRate(20f);
    }
}
