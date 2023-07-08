using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthCockpitSH : SystemHandler
{
    RadarProfileHandler _hostRadarProfileHandler;
    SpriteRenderer[] _spriteRenderers;
    LevelController _levelController;
    ParticleSystem _particleSystem;

    //settings
    [SerializeField] float _cloakingTime_Install = 10f;
    [SerializeField] float _cloakingTime_Upgrade = 5f;

    bool _isCloaked = false;
    float _cloakTimeTotal;
    float _cloakTimeRemaining;
    float _cloakFactor = 0;
    Color _cloakColor = Color.white;

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _hostRadarProfileHandler = transform.root.GetComponentInChildren<RadarProfileHandler>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _levelController = FindObjectOfType<LevelController>();
        _levelController.WarpedIntoNewLevel += OnWarpToNewLevel;
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        
    }

    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        _hostRadarProfileHandler.Decloak();
        _levelController.WarpedIntoNewLevel -= OnWarpToNewLevel;
    }

    public override object GetUIStatus()
    {
        return _cloakFactor;
    }

    protected override void ImplementSystemUpgrade()
    {
        
    }
    protected override void ImplementSystemDowngrade()
    {

    }

    private void Update()
    {
        if (_isCloaked)
        {
            _cloakTimeRemaining -= Time.deltaTime;

            if (_cloakTimeRemaining > 0)
            {
                _cloakFactor = _cloakTimeRemaining / _cloakTimeTotal;
                _cloakColor = Color.Lerp(Color.red, Color.green, _cloakFactor);
                _connectedID.UpdateUI(_cloakFactor, _cloakColor);
            }
            else
            {
                _isCloaked = false;
                _particleSystem.Stop();
                _hostRadarProfileHandler.Decloak();
            }
        }
    }

    private void OnWarpToNewLevel(Level throwawayRef)
    {
        _cloakTimeTotal = _cloakingTime_Install + ((CurrentUpgradeLevel - 1) * _cloakingTime_Upgrade);
        _hostRadarProfileHandler.Cloak();

        _isCloaked = true;
        _cloakTimeRemaining = _cloakTimeTotal;
        _cloakFactor = 1f;
        _cloakColor = Color.Lerp(Color.red, Color.green, _cloakFactor);
        _connectedID.UpdateUI(_cloakFactor, _cloakColor);
        _particleSystem.Play();
    }

    
}
