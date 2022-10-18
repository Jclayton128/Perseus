using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnergyHandler : MonoBehaviour
{
    ActorMovement _movement;
    UI_Controller _uicontroller;
    HealthHandler _health;

    [SerializeField] float _maxEnergyPoints = 30f;
    
    [HideIf("_usesBurstRecharge")]
    [SerializeField] float _energyGainRate = 1f;

    [Tooltip("Burst Recharge keeps energy at zero, and then instantly returns to full once " +
        "sufficient time has elapsed.")]
    [SerializeField] bool _usesBurstRecharge = false;

    [ShowIf("_usesBurstRecharge")]
    [SerializeField] float _timeToBurstRecharge = 5f;

    //state
    [SerializeField] float _currentEnergy;
    public float CurrentEnergy => _currentEnergy;

    [ShowIf("_usesBurstRecharge")]
    [SerializeField] float _burstRechargeCountdown = Mathf.Infinity;

    private void Awake()
    {
        _movement = GetComponent<ActorMovement>();
        _health = GetComponent<HealthHandler>();

        if (_movement == null || _health == null)
        {
            Debug.LogError("This component needs a Movement and Health alongside it!");
            return;
        }

        _uicontroller = FindObjectOfType<UI_Controller>();
    }

    private void Start()
    {
        _currentEnergy = _maxEnergyPoints;
        
        if (_movement.IsPlayer)
        {
            _uicontroller.UpdateEnergyBar(CurrentEnergy, _maxEnergyPoints);
            _uicontroller.UpdateEnergyRegenTMP(_energyGainRate.ToString("F1"), Color.white);
        }

        if (_usesBurstRecharge) _burstRechargeCountdown = _timeToBurstRecharge;
    }

    private void Update()
    {
        if (_usesBurstRecharge)
        {
            if (_currentEnergy > 0) return;
            _burstRechargeCountdown -= Time.deltaTime;
            if (_burstRechargeCountdown <= 0)
            {
                _currentEnergy = _maxEnergyPoints;
                _burstRechargeCountdown = _timeToBurstRecharge;
            }
        }
        else
        {
            _currentEnergy += _energyGainRate * (1 - _health.IonFactor) * Time.deltaTime;
            _currentEnergy = Mathf.Clamp(CurrentEnergy, 0, _maxEnergyPoints);
            if (_movement.IsPlayer)
            {
                _uicontroller.UpdateEnergyBar(CurrentEnergy, _maxEnergyPoints);
            }
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

    #region System Modifiers

    public void ModifyEnergyRegenRate(float rateToAdd)
    {
        _energyGainRate += rateToAdd;
    }

    #endregion


}
