using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    ActorMovement _playerPH;
    GameController _gameController;
    SystemWeaponLibrary _systemsLibrary;
    PlayerSystemHandler _playerSystemsHandler;

    [SerializeField] Toggle[] _systemToggles_Engine = null;
    [SerializeField] Toggle[] _systemToggles_Cockpit = null;
    [SerializeField] Toggle[] _systemToggles_Tail = null;
    [SerializeField] Toggle[] _systemToggles_Wings = null;
    [SerializeField] Toggle[] _systemToggles_Hull = null;
    [SerializeField] Toggle[] _systemToggles_LeftInt = null;
    [SerializeField] Toggle[] _systemToggles_RightInt = null;

    [SerializeField] Toggle[] _weaponToggles = null;

    //Dictionary<int, bool> _systemToggleStatus = new Dictionary<int, bool>();
    //Dictionary<int, bool> _weaponToggleStatus = new Dictionary<int, bool>();    

    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _systemsLibrary = FindObjectOfType<SystemWeaponLibrary>();

    }

    private void Start()
    {
        _playerSystemsHandler = _gameController.GetPlayerGO().GetComponent<PlayerSystemHandler>();
        SetupToggleLabels();
    }

    private void SetupToggleLabels()
    {
        foreach (var toggle in _systemToggles_Engine)
        {
            int index = Array.IndexOf(_systemToggles_Engine, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.Engine, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.Engine, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Cockpit)
        {
            int index = Array.IndexOf(_systemToggles_Cockpit, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.Cockpit, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.Cockpit, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Tail)
        {
            int index = Array.IndexOf(_systemToggles_Tail, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.Tail, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.Tail, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Wings)
        {
            int index = Array.IndexOf(_systemToggles_Wings, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.Wings, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.Wings, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Hull)
        {
            int index = Array.IndexOf(_systemToggles_Hull, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.Hull, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.Hull, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_LeftInt)
        {
            int index = Array.IndexOf(_systemToggles_LeftInt, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.LeftInt, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.LeftInt, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_RightInt)
        {
            int index = Array.IndexOf(_systemToggles_RightInt, toggle);
            if (_systemsLibrary.GetSystem(SystemWeaponLibrary.SystemLocation.RightInt, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemWeaponLibrary.SystemLocation.RightInt, index).ToString();
            toggle.isOn = false;
        }


        foreach (var toggle in _weaponToggles)
        {
            int index = Array.IndexOf(_weaponToggles, toggle);
            if (_systemsLibrary.GetWeapon(index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.GetWeapon(index).ToString();
            toggle.isOn = false;
        }
    }

    public void HandleMoreThrust()
    {
        if (_playerPH == null) 
            _playerPH = _gameController.GetPlayerGO().GetComponent<ActorMovement>();

        _playerPH.ModifyThrust(20f);

    }
    public void HandleLessThrust()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<ActorMovement>();

        _playerPH.ModifyThrust(-20f);

    }
    public void HandleMoreMass()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<ActorMovement>();

        _playerPH.ModifyMass(2f);

    }
    public void HandleLessMass()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<ActorMovement>();

        _playerPH.ModifyMass(-2f);
    }
    public void HandleMoreTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<ActorMovement>();

        _playerPH.ModifyTurnRate(20f);
    }
    public void HandleLessTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.GetPlayerGO().GetComponent<ActorMovement>();

        _playerPH.ModifyTurnRate(20f);
    }

    public void HandleSpawnRandomSystem()
    {
        _systemsLibrary.SpawnUniqueRandomSystemCrate(_playerSystemsHandler.GetSystemsOnBoard());
    }

    public void HandleSystemToggle_Engine(int index)
    {
        if (_systemToggles_Engine[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Engine, index))
            {
                _systemToggles_Engine[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Engine, index);
        }
    }
    public void HandleSystemToggle_Cockpit(int index)
    {
        if (_systemToggles_Cockpit[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Cockpit, index))
            {
                _systemToggles_Cockpit[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Cockpit, index);
        }
    }
    public void HandleSystemToggle_Tail(int index)
    {
        if (_systemToggles_Tail[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Tail, index))
            {
                _systemToggles_Tail[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Tail, index);
        }
    }
    public void HandleSystemToggle_Wings(int index)
    {
        if (_systemToggles_Wings[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Wings, index))
            {
                _systemToggles_Wings[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Wings, index);
        }
    }
    public void HandleSystemToggle_Hull(int index)
    {
        if (_systemToggles_Hull[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Hull, index))
            {
                _systemToggles_Hull[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Hull, index);
        }
    }
    public void HandleSystemToggle_LeftInt(int index)
    {
        if (_systemToggles_LeftInt[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.LeftInt, index))
            {
                _systemToggles_LeftInt[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.LeftInt, index);
        }
    }
    public void HandleSystemToggle_RightInt(int index)
    {
        if (_systemToggles_RightInt[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.RightInt, index))
            {
                _systemToggles_RightInt[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.RightInt, index);
        }
    }


    public void HandleWeaponToggle(int index)
    { 
        if (_weaponToggles[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_TryGainWeapon(index))
            {
                _weaponToggles[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.RemoveWeapon(_systemsLibrary.GetWeapon(index).
                GetComponent<WeaponHandler>().WeaponType);
        }
    }


}