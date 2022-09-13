using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugController : MonoBehaviour
{
    ActorMovement _playerPH;
    GameController _gameController;
    SystemWeaponLibrary _systemsLibrary;
    PlayerSystemHandler _playerSystemsHandler;
    PlayerStateHandler _playerStateHandler;
    LevelController _levelController;

    [SerializeField] GameObject[] _subpanels = null;

    [SerializeField] Toggle[] _systemToggles_Engine = null;
    [SerializeField] Toggle[] _systemToggles_Cockpit = null;
    [SerializeField] Toggle[] _systemToggles_Tail = null;
    [SerializeField] Toggle[] _systemToggles_Wings = null;
    [SerializeField] Toggle[] _systemToggles_Hull = null;
    [SerializeField] Toggle[] _systemToggles_LeftInt = null;
    [SerializeField] Toggle[] _systemToggles_RightInt = null;

    [SerializeField] TextMeshProUGUI[] _spawnCrateButtonLabels = null;
    [SerializeField] TextMeshProUGUI[] _spawnWeaponCrateButtonLabels = null;

    [SerializeField] Toggle[] _weaponToggles = null;

    //Dictionary<int, bool> _systemToggleStatus = new Dictionary<int, bool>();
    //Dictionary<int, bool> _weaponToggleStatus = new Dictionary<int, bool>();

    //State
    int _currentDebugPanel = -1;


    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _gameController.OnPlayerSpawned += ReactToPlayerSpawning;
        _levelController = GetComponent<LevelController>();
        _systemsLibrary = FindObjectOfType<SystemWeaponLibrary>();
        SetupToggleLabels();
        SetupSpawnCrateLabels();
    }


    private void ReactToPlayerSpawning(GameObject player)
    {
        _playerSystemsHandler = player.GetComponent<PlayerSystemHandler>();
        _playerStateHandler = _playerSystemsHandler.GetComponent<PlayerStateHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _currentDebugPanel++;
            if (_currentDebugPanel >= _subpanels.Length)
            {
                _currentDebugPanel = -1;
            }

            foreach (var panel in _subpanels)
            {
                panel.SetActive(false);
            }

            if (_currentDebugPanel >= 0)
            {
                _subpanels[_currentDebugPanel].SetActive(true);
            }
        }
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

    private void SetupSpawnCrateLabels()
    {
        SystemHandler[] allSystems = _systemsLibrary.GetAllSystemHandlers_Debug();
        for (int i = 0; i < _spawnCrateButtonLabels.Length; i++)
        {
            if (i >= allSystems.Length) break;
            _spawnCrateButtonLabels[i].text = allSystems[i].GetName();
        }

        WeaponHandler[] allWeapons = _systemsLibrary.GetAllWeaponHandlers_Debug();
        for (int j = 0; j < _spawnWeaponCrateButtonLabels.Length; j++)
        {
            if (j >= allWeapons.Length) break;
            _spawnWeaponCrateButtonLabels[j].text = allWeapons[j].GetName();
        }
    }

    #region Ship Changes
    public void HandleMoreThrust()
    {
        if (_playerPH == null) 
            _playerPH = _gameController.Player.GetComponent<ActorMovement>();

        _playerPH.ModifyThrust(20f);

    }
    public void HandleLessThrust()
    {
        if (_playerPH == null)
            _playerPH = _gameController.Player.GetComponent<ActorMovement>();

        _playerPH.ModifyThrust(-20f);

    }
    //public void HandleMoreMass()
    //{
    //    if (_playerPH == null)
    //        _playerPH = _gameController.Player.GetComponent<ActorMovement>();

    //    _playerPH.ModifyMass(2f);

    //}
    //public void HandleLessMass()
    //{
    //    if (_playerPH == null)
    //        _playerPH = _gameController.Player.GetComponent<ActorMovement>();

    //    _playerPH.ModifyMass(-2f);
    //}
    public void HandleMoreTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.Player.GetComponent<ActorMovement>();

        _playerPH.ModifyTurnRate(20f);
    }
    public void HandleLessTurnRate()
    {
        if (_playerPH == null)
            _playerPH = _gameController.Player.GetComponent<ActorMovement>();

        _playerPH.ModifyTurnRate(20f);
    }

    public void HandleGainScrap()
    {
        _playerStateHandler.GainScrap(10);
    }

#endregion

    public void HandleSpawnRandomSystem()
    {
        Debug.LogError("doesn't do anything");
    }

    public void SpawnSystemByIndex(int index)
    {
        SystemHandler[] allsystems = _systemsLibrary.GetAllSystemHandlers_Debug();
        SystemHandler sh = allsystems[index];
        _levelController.SpawnSpecificCrateNearPlayer(sh.SystemType);
    }

    public void SpawnWeaponByIndex(int index)
    {
        WeaponHandler[] allweapons = _systemsLibrary.GetAllWeaponHandlers_Debug();
        WeaponHandler weapon = allweapons[index];
        _levelController.SpawnSpecificCrateNearPlayer(weapon.WeaponType);
    }
    
    #region System Toggles
    //public void HandleSystemToggle_Engine(int index)
    //{
    //    if (_systemToggles_Engine[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Engine, index))
    //        {
    //            _systemToggles_Engine[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Engine, index);
    //    }
    //}
    //public void HandleSystemToggle_Cockpit(int index)
    //{
    //    if (_systemToggles_Cockpit[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Cockpit, index))
    //        {
    //            _systemToggles_Cockpit[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Cockpit, index);
    //    }
    //}
    //public void HandleSystemToggle_Tail(int index)
    //{
    //    if (_systemToggles_Tail[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Tail, index))
    //        {
    //            _systemToggles_Tail[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Tail, index);
    //    }
    //}
    //public void HandleSystemToggle_Wings(int index)
    //{
    //    if (_systemToggles_Wings[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Wings, index))
    //        {
    //            _systemToggles_Wings[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Wings, index);
    //    }
    //}
    //public void HandleSystemToggle_Hull(int index)
    //{
    //    if (_systemToggles_Hull[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.Hull, index))
    //        {
    //            _systemToggles_Hull[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.Hull, index);
    //    }
    //}
    //public void HandleSystemToggle_LeftInt(int index)
    //{
    //    if (_systemToggles_LeftInt[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.LeftInt, index))
    //        {
    //            _systemToggles_LeftInt[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.LeftInt, index);
    //    }
    //}
    //public void HandleSystemToggle_RightInt(int index)
    //{
    //    if (_systemToggles_RightInt[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainSystem(SystemWeaponLibrary.SystemLocation.RightInt, index))
    //        {
    //            _systemToggles_RightInt[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveSystem(SystemWeaponLibrary.SystemLocation.RightInt, index);
    //    }
    //}

    #endregion

    #region Weapon Toggles
    //public void HandleWeaponToggle(int index)
    //{ 
    //    if (_weaponToggles[index].isOn)
    //    {
    //        if (!_playerSystemsHandler.Debug_TryGainWeapon(index))
    //        {
    //            _weaponToggles[index].isOn = false;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        _playerSystemsHandler.RemoveWeapon(_systemsLibrary.GetWeapon(index).
    //            GetComponent<WeaponHandler>().WeaponType);
    //    }
    //}

    #endregion

    #region Level Tools

    public void SpawnEnemy_Debug(int enemyIndex)
    {
        _levelController.SpawnEnemiesInNewSector_Debug();
    }

    #endregion


}
