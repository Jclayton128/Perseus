using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{
    UI_Controller _uiController;
    GameController _gameController;
    InputController _inputController;

    HealthHandler _healthHandler;
    EnergyHandler _energyHandler;

    //Settings
    int _scrapsPerLevelMod = 10;
    float _timeBetweenUpgradeMenuToggles = 0.9f; // Should be the same as the time to deploy the menu
    
    [SerializeField] float _shieldGainOnLevelUp = 1f;
    [SerializeField] float _hullGainOnLevelUp = 1f;
    [SerializeField] float _energyGainOnLevelUp = 2f;

    //State
    int _scrapCollected = 0;
    int _currentUpgradePoints = 0;
    int _scrapNeededForNextUpgradeLevel;
    float _scrapFactor = 0;
    float _timeForNextPossibleUpgradeMenuToggle = -1;
    bool _isUpgradeMenuDeployed = false;


    private void Awake()
    {
        if (GetComponentInParent<ActorMovement>().IsPlayer)
        {
            _uiController = FindObjectOfType<UI_Controller>();
            _uiController.ModifyUpgradePointsAvailable(_currentUpgradePoints);
            _uiController.ModifyScrapAmount(0, 0);
            _uiController.ShowHideTAB(false);
        }

        _inputController = _uiController.GetComponent<InputController>();
        _inputController.OnUpgradeMenuToggled += ToggleUpgradeMenu;

        _gameController = _uiController.GetComponent<GameController>();
        _scrapNeededForNextUpgradeLevel = _scrapsPerLevelMod;

        _energyHandler = GetComponent<EnergyHandler>();
        _healthHandler = GetComponent<HealthHandler>();
    }

 

    private void ToggleUpgradeMenu()
    {
        if (Time.unscaledTime > _timeForNextPossibleUpgradeMenuToggle)
        {
            _isUpgradeMenuDeployed = !_isUpgradeMenuDeployed;
            _timeForNextPossibleUpgradeMenuToggle = Time.unscaledTime + _timeBetweenUpgradeMenuToggles;

            if (_isUpgradeMenuDeployed)
            {
                _uiController.DeployUpgradeMenu();
                _gameController.PauseGame(0.7f);
            }
            else
            {
                _uiController.RetractUpgradeMenu();
                _gameController.UnpauseGame();
            }


        }
    }

    public void GainScrap()
    {
        _scrapCollected++;
        //TODO play collect scrap sound

        if (_scrapCollected >= _scrapNeededForNextUpgradeLevel)
        {
            _scrapCollected = 0;
            _currentUpgradePoints++;
            _uiController.ShowHideTAB(true);
            _uiController.ModifyUpgradePointsAvailable(_currentUpgradePoints);
            _scrapNeededForNextUpgradeLevel += _scrapsPerLevelMod;
        }

        _scrapFactor = (float)_scrapCollected / (float)_scrapNeededForNextUpgradeLevel;
        _uiController.ModifyScrapAmount(_scrapFactor, _scrapCollected);
    }

    public void GainScrap(int amountToGain)
    {
        _scrapCollected += amountToGain;
        if (_scrapCollected >= _scrapNeededForNextUpgradeLevel)
        {
            GainUpgradePointViaScrapPickup();
        }

        _scrapFactor = (float)_scrapCollected / (float)_scrapNeededForNextUpgradeLevel;
        _uiController.ModifyScrapAmount(_scrapFactor, _scrapCollected);
    }

    private void GainUpgradePointViaScrapPickup()
    {
        int overage = _scrapCollected - _scrapNeededForNextUpgradeLevel;
        _currentUpgradePoints++;
        _scrapCollected = overage;

        _uiController.ShowHideTAB(true);
        _uiController.ModifyUpgradePointsAvailable(_currentUpgradePoints);
        _scrapNeededForNextUpgradeLevel += _scrapsPerLevelMod;

        ImplementLevelUpBenefits();
    }

    private void ImplementLevelUpBenefits()
    {
        _healthHandler.AdjustShieldMaximum(_shieldGainOnLevelUp);
        _healthHandler.AdjustHullMaximumAndCurrent(_hullGainOnLevelUp);
        _energyHandler.ModifyEnergyRegenRate(_energyGainOnLevelUp);
    }

    public bool CheckUpgradePoints(int cost)
    {
        if (cost > _currentUpgradePoints) return false;
        else return true;
    }

    public void SpendUpgradePoints(int cost)
    {
        _currentUpgradePoints -= cost;
        _uiController.ModifyUpgradePointsAvailable(_currentUpgradePoints);

        bool pointsLeft = (_currentUpgradePoints > 0) ? true : false;
        _uiController.ShowHideTAB(pointsLeft);
    }

    public void GainUpgradePoints(int gain)
    {
        SpendUpgradePoints(-gain);
    }
}
