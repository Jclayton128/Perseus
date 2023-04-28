using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class EnergyHandler : MonoBehaviour
{
    public Action<float, float> EnergyPointsChanged;
    public Action<string, Color> EnergyRegenChanged;

    ActorMovement _movement;
    UI_Controller _uicontroller;
    HealthHandler _health;

    [SerializeField] float _maxEnergyPoints = 30f;
    
    [HideIf("_usesBurstRecharge")]
    [SerializeField] float _energyGainRate = 1f;
    public float EnergyGainRate => _energyGainRate;

    [Tooltip("Burst Recharge keeps energy at zero, and then instantly returns to full once " +
        "sufficient time has elapsed.")]
    [SerializeField] bool _usesBurstRecharge = false;

    [ShowIf("_usesBurstRecharge")]
    [SerializeField] float _timeToBurstRecharge = 5f;

    //state
    [SerializeField] float _currentEnergy;
    public float CurrentEnergy => _currentEnergy;
    Color _energyRegenColor;

    [ShowIf("_usesBurstRecharge")]
    [SerializeField] float _burstRechargeCountdown = Mathf.Infinity;

    private void Awake()
    {
        _movement = GetComponent<ActorMovement>();
        _health = GetComponent<HealthHandler>();
        if (_health)
        {
            _health.IonFactorChanged += HandleIonFactorChange;
            _uicontroller = FindObjectOfType<UI_Controller>();
        }

        //if (_movement == null || _health == null)
        //{
        //    Debug.LogError("This component needs a Movement and Health alongside it!");
        //    return;
        //}

    }

    private void Start()
    {
        _currentEnergy = _maxEnergyPoints;        

        EnergyPointsChanged?.Invoke(CurrentEnergy, _maxEnergyPoints);
        EnergyRegenChanged?.Invoke(_energyGainRate.ToString("F1"), Color.white);

        if (_usesBurstRecharge) _burstRechargeCountdown = _timeToBurstRecharge;
    }

    private void Update()
    {
        if (_usesBurstRecharge)
        {
            _burstRechargeCountdown -= Time.deltaTime * (1 - _health.IonFactor);
            if (_burstRechargeCountdown <= 0)
            {
                _currentEnergy = _maxEnergyPoints;
                _burstRechargeCountdown = _timeToBurstRecharge;
            }
        }
        else
        {
            if (_health)
            {
                _currentEnergy += _energyGainRate * (1 - _health.IonFactor) * Time.deltaTime;
            }
            else
            {
                _currentEnergy += _energyGainRate * Time.deltaTime;
            }

            _currentEnergy = Mathf.Clamp(CurrentEnergy, 0, _maxEnergyPoints);
            EnergyPointsChanged?.Invoke(CurrentEnergy, _maxEnergyPoints);
        }        


    }

    /// <summary>
    /// Returns TRUE if there is enough energy in reserve to cover the expense, else FALSE.
    /// </summary>
    /// <param name="prospectiveEnergyExpense"></param>
    /// <returns></returns>
    public bool CheckEnergy(float prospectiveEnergyExpense)
    {
        if (CurrentEnergy < prospectiveEnergyExpense)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SpendEnergy(float energySpent)
    {
        _currentEnergy -= energySpent;
    }

    private void HandleIonFactorChange(float currentIonization, float throwaway)
    {
        _energyRegenColor = Color.Lerp(Color.white, Color.green, currentIonization);
        EnergyRegenChanged?.Invoke((_energyGainRate * (1 - _health.IonFactor)).ToString("F1"),
            _energyRegenColor);
    }

    #region System Modifiers

    public void ModifyEnergyRegenRate(float rateToAdd)
    {
        _energyGainRate += rateToAdd;
        EnergyRegenChanged?.Invoke(_energyGainRate.ToString("F1"), Color.white);
    }

    public void ModifyMaxEnergyLevel(float amountToAdd)
    {
        _maxEnergyPoints += amountToAdd;
        EnergyPointsChanged?.Invoke(CurrentEnergy, _maxEnergyPoints);
    }

    public void SetEnergyRegenRate(float newEnergRegen)
    {
        _energyGainRate = newEnergRegen;
        EnergyRegenChanged?.Invoke(_energyGainRate.ToString("F1"), Color.white);
    }

    #endregion


}
