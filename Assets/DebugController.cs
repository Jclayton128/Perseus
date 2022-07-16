using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    PlayerHandler _playerPH;
    GameController _gameController;
    SystemsLibrary _systemsLibrary;
    PlayerSystemHandler _playerSystemsHandler;
    [SerializeField] Toggle[] _systemToggles;
    [SerializeField] Toggle[] _weaponToggles;
    Dictionary<int, bool> _systemToggleStatus = new Dictionary<int, bool>();
    Dictionary<int, bool> _weaponToggleStatus = new Dictionary<int, bool>();    

    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _systemsLibrary = FindObjectOfType<SystemsLibrary>();
        _playerSystemsHandler = _gameController.GetPlayerGO().GetComponent<PlayerSystemHandler>();

        SetupToggleLabels();
    }

    private void SetupToggleLabels()
    {
        foreach (var toggle in _systemToggles)
        {
            int index = Array.IndexOf(_systemToggles, toggle);
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.GetSystem(index).ToString();
            toggle.isOn = false;
            _systemToggleStatus[index] = false;
        }

        foreach (var toggle in _weaponToggles)
        {
            int index = Array.IndexOf(_weaponToggles, toggle);
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.GetWeapon(index).ToString();
            toggle.isOn = false;

            _weaponToggleStatus[index] = false;
        }
    }

    public void HandleMoreThrust()
    {
        if (_playerPH == null) 
            _playerPH = _gameController.GetPlayerGO().GetComponent<PlayerHandler>();

        _playerPH.ModifyThrust(20f);

    }
    public void HandleLessThrust()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PlayerHandler>();

        _playerPH.ModifyThrust(-20f);

    }
    public void HandleMoreMass()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PlayerHandler>();

        _playerPH.ModifyMass(2f);

    }
    public void HandleLessMass()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PlayerHandler>();

        _playerPH.ModifyMass(-2f);
    }
    public void HandleMoreTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PlayerHandler>();

        _playerPH.ModifyTurnRate(20f);
    }
    public void HandleLessTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<PlayerHandler>();

        _playerPH.ModifyTurnRate(20f);
    }

    public void HandleSpawnRandomSystem()
    {
        _systemsLibrary.SpawnUniqueRandomSystemCrate(_playerSystemsHandler.GetSystemsOnBoard());
    }

    public void HandleSystemToggle(int index)
    {
        if (_systemToggleStatus[index] == false)
        {
            _systemToggleStatus[index] = true;
            _playerSystemsHandler.Debug_GainSystem(index);
        }
        else
        {
            _systemToggleStatus[index] = false;
            _playerSystemsHandler.Debug_RemoveSystem(index);
        }
    }

    public void HandleWeaponToggle(int index)
    {
        if (_weaponToggleStatus[index] == false)
        {
            _weaponToggleStatus[index] = true;
            _playerSystemsHandler.Debug_GainWeapon(index);
        }
        else
        {
            _weaponToggleStatus[index] = false;
            _playerSystemsHandler.Debug_RemoveWeapon(index);
        }
    }


}
