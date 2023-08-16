using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveHullSH : SystemHandler
{
    HealthHandler _healthHandler;
    RocketLauncherWH _weaponHandler;
    EnergyHandler _energyHandler;
    Transform _muzzle;

    //settings
    [SerializeField] float _reactionThreshold = 5f;
    [SerializeField] float _reactionDecreaseRate = 1f;
    [SerializeField] Color _reactionEmptyColor = Color.red;
    [SerializeField] Color _reactionFullColor = Color.yellow;
    [SerializeField] float _reactionThresholdSubtract_Upgrade = 0.5f;
    [SerializeField] float _reactionThresholdDecreaseMultiplier_Upgrade = 0.8f;


    //state
    [SerializeField] Vector2 _mostRecentThreatVector;
    float _receivedDamageRaw = 0;
    float _receivedDamageFactor = 0;
    Color _receivedDamageColor = Color.white;
    bool _isPlayer;


    private void Start()
    {
        _isPlayer = GetComponentInParent<ActorMovement>().IsPlayer;
        if (!_isPlayer)
        {
            IntegrateSystem(null);
        }
    }

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        if (connectedSID) base.IntegrateSystem(connectedSID);
        _healthHandler = GetComponentInParent<HealthHandler>();
        _healthHandler.ReceivingShieldDamage += HandleDamageReceived;
        _healthHandler.ReceivingThreatVector += HandleNewThreatVector;
        _energyHandler = GetComponentInParent<EnergyHandler>();
        _weaponHandler = transform.root.GetComponentInChildren<RocketLauncherWH>();
        _muzzle = _weaponHandler.GetComponentInChildren<MuzzleTag>().transform;
        _isPlayer = GetComponentInParent<ActorMovement>().IsPlayer;
        _weaponHandler.Initialize(_energyHandler, _isPlayer, null);
    }




    public override object GetUIStatus()
    {
        return _receivedDamageFactor;
    }    

    protected override void ImplementSystemUpgrade()
    {
        _reactionThreshold -= _reactionThresholdSubtract_Upgrade;
        _reactionDecreaseRate *= _reactionThresholdDecreaseMultiplier_Upgrade;
    }
    protected override void ImplementSystemDowngrade()
    {
        // not necessary since the upgrades are wholly organic to this instance
    }

    private void HandleDamageReceived(DamagePack dp)
    {
        _receivedDamageRaw += dp.NormalDamage;
    }

    private void HandleNewThreatVector(Vector2 newThreatVector)
    {
        _mostRecentThreatVector = newThreatVector;
    }

    private void Update()
    {
        if (_receivedDamageRaw > _reactionThreshold)
        {
            Quaternion rot = Quaternion.LookRotation(_mostRecentThreatVector, Vector3.forward);
            _muzzle.rotation = rot;
            _weaponHandler.Activate();
            //TODO put AUDIO clip here
            _receivedDamageRaw = 0;
        }
        else
        {
            _receivedDamageRaw -= Time.deltaTime * _reactionDecreaseRate;
            _receivedDamageRaw = Mathf.Clamp(_receivedDamageRaw, 0, 999);
        }
        
        if (_isPlayer) UpdateUI();
    }

   
    private void UpdateUI()
    {
        _receivedDamageFactor = Mathf.Clamp01(_receivedDamageRaw / _reactionThreshold);
        _receivedDamageColor = Color.Lerp(_reactionEmptyColor, _reactionFullColor, _receivedDamageFactor);
        _connectedID.UpdateUI(_receivedDamageFactor, _receivedDamageColor);
    }
}
