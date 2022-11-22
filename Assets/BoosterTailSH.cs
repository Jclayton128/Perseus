using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterTailSH : SystemHandler
{
    ActorMovement _actorMovement;
    HealthHandler _healthHandler;

    //settings
    [SerializeField] float _maxDamageThreshold = 10f;
    [SerializeField] float _boostDrainRate = 1f;
    [SerializeField] float _maxBoostAmount = 0.5f;
    Color _maxBoostColor = Color.green;
    Color _minBoostColor = Color.yellow;
    [SerializeField] float _maxBoostAmountAddition_Upgrade = 0.1f;
    [SerializeField] float _boostDrainRateMultiplier_Upgrade = 0.8f;

    //state
    Color _boostColor = Color.yellow;
    float _currentBoostRaw;
    float _currentBoostFactor = 0;
    


    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _actorMovement = GetComponentInParent<ActorMovement>();
        _healthHandler = GetComponentInParent<HealthHandler>();
        _healthHandler.ReceivedHullDamage += HandleDamageReceived;
        _healthHandler.ReceivedShieldDamage += HandleDamageReceived;
    }

    public override object GetUIStatus()
    {
        return _currentBoostFactor;
    }

    protected override void ImplementSystemDowngrade()
    {
        //Self-contained system
    }

    protected override void ImplementSystemUpgrade()
    {
        _boostDrainRate *= _boostDrainRateMultiplier_Upgrade;
        _maxBoostAmount += _maxBoostAmountAddition_Upgrade;
    }

    private void Update()
    {
        if (_currentBoostFactor > 0)
        {
            _currentBoostRaw -= _boostDrainRate * Time.deltaTime;
            _currentBoostFactor = _currentBoostRaw / _maxDamageThreshold;
            _actorMovement.BoostMultiplier = 1 + Mathf.Lerp(0, _maxBoostAmount, _currentBoostFactor);
            UpdateUI();
        }
    }

    private void HandleDamageReceived(float incomingDamage)
    {
        _currentBoostRaw += incomingDamage;
        _currentBoostRaw = Mathf.Clamp(_currentBoostRaw, 0, _maxDamageThreshold);
        _currentBoostFactor = _currentBoostRaw / _maxDamageThreshold;
        //UpdateUI();
    }

    private void UpdateUI()
    {
        _boostColor = Color.Lerp(_minBoostColor, _maxBoostColor, _currentBoostFactor);
        _connectedID.UpdateUI(_currentBoostFactor, _boostColor);
    }
}
