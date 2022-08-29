using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyHandler : MonoBehaviour
{
    ActorMovement _movement;
    UI_Controller _uicontroller;
    AdjustableImageBar _energyImageBar;
    HealthHandler _health;

    [SerializeField] float _maxEnergyPoints = 30f;
    [SerializeField] float _energyGainRate = 1f;

    public float CurrentEnergy { get; protected set; }

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
        _energyImageBar = _uicontroller.GetEnergyBar();
    }

    private void Start()
    {
        CurrentEnergy = _maxEnergyPoints;
        
        if (_movement.IsPlayer)
        {
            _energyImageBar.SetFactor(CurrentEnergy / _maxEnergyPoints);   
        }
    }

    private void Update()
    {
        CurrentEnergy += _energyGainRate * (1 - _health.IonFactor) * Time.deltaTime;
        CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, _maxEnergyPoints);

        if (_movement.IsPlayer)
        {
            _energyImageBar.SetFactor(CurrentEnergy / _maxEnergyPoints);
        }
    }

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
        CurrentEnergy -= energySpent;
    }


}