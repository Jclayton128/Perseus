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
        _systemsLibrary = FindObjectOfType<SystemsLibrary>();

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
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.Engine, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.Engine, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Cockpit)
        {
            int index = Array.IndexOf(_systemToggles_Cockpit, toggle);
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.Cockpit, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.Cockpit, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Tail)
        {
            int index = Array.IndexOf(_systemToggles_Tail, toggle);
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.Tail, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.Tail, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Wings)
        {
            int index = Array.IndexOf(_systemToggles_Wings, toggle);
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.Wings, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.Wings, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_Hull)
        {
            int index = Array.IndexOf(_systemToggles_Hull, toggle);
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.Hull, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.Hull, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_LeftInt)
        {
            int index = Array.IndexOf(_systemToggles_LeftInt, toggle);
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.LeftInt, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.LeftInt, index).ToString();
            toggle.isOn = false;
        }

        foreach (var toggle in _systemToggles_RightInt)
        {
            int index = Array.IndexOf(_systemToggles_RightInt, toggle);
            if (_systemsLibrary.GetSystem(SystemsLibrary.SystemLocation.RightInt, index) == null) continue;
            toggle.GetComponentInChildren<Text>().text = _systemsLibrary.
                GetSystem(SystemsLibrary.SystemLocation.RightInt, index).ToString();
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

    public void HandleSystemToggle_Engine(int index)
    {
        if (_systemToggles_Engine[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.Engine, index))
            {
                _systemToggles_Engine[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.Engine, index);
        }
    }
    public void HandleSystemToggle_Cockpit(int index)
    {
        if (_systemToggles_Cockpit[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.Cockpit, index))
            {
                _systemToggles_Cockpit[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.Cockpit, index);
        }
    }
    public void HandleSystemToggle_Tail(int index)
    {
        if (_systemToggles_Tail[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.Tail, index))
            {
                _systemToggles_Tail[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.Tail, index);
        }
    }
    public void HandleSystemToggle_Wings(int index)
    {
        if (_systemToggles_Wings[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.Wings, index))
            {
                _systemToggles_Wings[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.Wings, index);
        }
    }
    public void HandleSystemToggle_Hull(int index)
    {
        if (_systemToggles_Hull[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.Hull, index))
            {
                _systemToggles_Hull[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.Hull, index);
        }
    }
    public void HandleSystemToggle_LeftInt(int index)
    {
        if (_systemToggles_LeftInt[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.LeftInt, index))
            {
                _systemToggles_LeftInt[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.LeftInt, index);
        }
    }
    public void HandleSystemToggle_RightInt(int index)
    {
        if (_systemToggles_RightInt[index].isOn)
        {
            if (!_playerSystemsHandler.Debug_GainSystem(SystemsLibrary.SystemLocation.RightInt, index))
            {
                _systemToggles_RightInt[index].isOn = false;
                return;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveSystem(SystemsLibrary.SystemLocation.RightInt, index);
        }
    }


    public void HandleWeaponToggle(int index)
    { 
        if (_weaponToggles[index].isOn)
        {
            if (_playerSystemsHandler.Debug_GainWeapon(index))
            {

            }
            else
            {
                _weaponToggles[index].isOn = false;
            }
        }
        else
        {
            _playerSystemsHandler.Debug_RemoveWeapon(index);
        }
    }


}
