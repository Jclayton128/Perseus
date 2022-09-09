using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEngineSH : SystemHandler, IDamageReflexable
{
    LevelController _levelController;

    //settings
    [SerializeField] GameObject _blinkInParticleFX = null;
    [SerializeField] GameObject _blinkOutParticleFX = null;
    [SerializeField] float _rechargeRate = 0.1f; // max charge is 1, so .1 rate = 10 seconds
    [SerializeField] float _minBlinkRange = 2f;
    [SerializeField] float _maxBlinkRange = 5f;

    [Header("Upgrade Settings")]
    [SerializeField] float _rechargeRateAddition_Upgrade = 0.1f;

    //state
    float _currentCharge = 1;
    Color _currentColor = Color.white;


    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
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

    public bool ModifyDamagePack(DamagePack receivedDamagePack)
    {
        if (_currentCharge >= .99f)
        {
            receivedDamagePack.NullifyDamage();
            
            _currentCharge = 0;
            _connectedID.UpdateUI(_currentCharge, Color.red);
            return true;
        }
        else return false;
    }

    public void ExecuteDamageReflex()
    {
        //TODO cool blink audio sound
        Instantiate(_blinkOutParticleFX, transform.position, Quaternion.identity);
        transform.parent.position = CUR.FindRandomBlinkWithinRangeBandAndWithinArena(transform.position,
            _minBlinkRange, _maxBlinkRange, Vector3.zero, _levelController.ArenaRadius);
        Instantiate(_blinkInParticleFX, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        _currentCharge += Time.deltaTime * _rechargeRate;
        _currentCharge = Mathf.Clamp(_currentCharge, 0, 1.0f);
        _currentColor = Color.Lerp(Color.red, Color.green, _currentCharge);
        _connectedID.UpdateUI(_currentCharge, _currentColor);
    }
}
