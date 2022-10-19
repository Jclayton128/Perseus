using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEngineSH : SystemHandler
{
    LevelController _levelController;
    HealthHandler _healthHandler;

    //settings
    [SerializeField] GameObject _blinkInParticleFX = null;
    [SerializeField] GameObject _blinkOutParticleFX = null;
    [SerializeField] float _rechargeRate = 0.1f; // max charge is 1, so .1 rate = 10 seconds
    [SerializeField] float _minBlinkRange = 2f;
    [SerializeField] float _maxBlinkRange = 5f;

    [Header("Upgrade Settings")]
    [SerializeField] float _rechargeRateAddition_Upgrade = 0.1f;

    //state
    [SerializeField] float _currentCharge = 1;
    Color _currentColor = Color.white;


    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _healthHandler = GetComponentInParent<HealthHandler>();
        _healthHandler.ReceivingHullDamage += ExecuteDamageReflex;
        _levelController = FindObjectOfType<LevelController>();
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
    }

    public override object GetUIStatus()
    {
        return _currentCharge;
    }

    protected override void ImplementSystemUpgrade()
    {
        _rechargeRate += _rechargeRateAddition_Upgrade;
    }
    protected override void ImplementSystemDowngrade()
    {
        _rechargeRate -= _rechargeRateAddition_Upgrade;
    }

    private void ExecuteDamageReflex(DamagePack dp)
    {
        if (_currentCharge < .99f) return;

        Debug.Log("blinking away");
        //TODO cool blink audio sound
        _healthHandler.ActivateDamageInvulnerability();
        Instantiate(_blinkOutParticleFX, transform.position, Quaternion.identity);
        transform.parent.position = CUR.FindRandomPositionWithinRangeBandAndWithinArena(transform.position,
            _minBlinkRange, _maxBlinkRange, Vector3.zero, _levelController.ArenaRadius);
        Instantiate(_blinkInParticleFX, transform.position, Quaternion.identity);

        dp.NullifyDamage();

        _currentCharge = 0;
        _connectedID?.UpdateUI(_currentCharge, Color.red);
    }

    private void Update()
    {
        _currentCharge += Time.deltaTime * _rechargeRate;
        _currentCharge = Mathf.Clamp(_currentCharge, 0, 1.0f);
        _currentColor = Color.Lerp(Color.red, Color.green, _currentCharge);
        _connectedID?.UpdateUI(_currentCharge, _currentColor);
    }
}
