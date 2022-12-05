using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoreSH : SystemHandler
{
    HealthHandler _healthHandler;
    Rigidbody2D _rb;

    //settings
    [SerializeField] Color _lowRegenColor = Color.yellow;
    [SerializeField] Color _highRegenColor = Color.blue;
    [SerializeField] float _maxRegenValue = 3.0f;
    [Tooltip("Above this speed, zero shield bonus regen is given")]
    [SerializeField] float _speedThreshold = 5.0f;
    [SerializeField] float _maxRegenMultiplier_Upgrade = 1.2f;
    [SerializeField] float _speedThresholdMultiplier_Upgrade = 1.2f;
    float _UIUpdateRate = 0.7f;

    //state
    [SerializeField] float _factor;
    Color _regenColor = Color.white;
    float _systemShieldRegenRate = 0;
    float _timeToUpdateUI = 0;
    float _stockShieldRegen = 0;

    public override object GetUIStatus()
    {
        return null;
        //return _regenBonusAmount.ToString("F1");
    }

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _healthHandler = GetComponentInParent<HealthHandler>();
        _stockShieldRegen = _healthHandler.GetShieldHealRate();
        _healthHandler.SetShieldRegenRate(_systemShieldRegenRate);
        _rb = GetComponentInParent<Rigidbody2D>();        
    }

    private void Update()
    {
        UpdateShieldRegen();

        if (Time.time >= _timeToUpdateUI)
        {
            UpdateUI();
            _timeToUpdateUI = Time.time + _UIUpdateRate;
        }

    }

    private void UpdateShieldRegen()
    {
        _factor = 1 - Mathf.Clamp01(_rb.velocity.magnitude / _speedThreshold);
        _systemShieldRegenRate = Mathf.Lerp(0, _maxRegenValue, _factor) ;
        _healthHandler.SetShieldRegenRate(_systemShieldRegenRate);
        //_healthHandler.HealCurrentShieldPoints(_regenBonusAmount * Time.deltaTime);
    }

    private void UpdateUI()
    {
        //_regenColor = Color.Lerp(_lowRegenColor, _highRegenColor, _factor);
        //_connectedID.UpdateUI(_regenBonusAmount.ToString("F1"), _regenColor);
    }

    protected override void ImplementSystemDowngrade()
    {

    }

    protected override void ImplementSystemUpgrade()
    {
        _maxRegenValue *= _maxRegenMultiplier_Upgrade;
        _speedThreshold *= _speedThresholdMultiplier_Upgrade;
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _healthHandler.SetShieldRegenRate(_stockShieldRegen);
    }
}
