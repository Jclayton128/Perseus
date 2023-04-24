using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolarDishProjectorSH : SystemHandler
{
    LevelController _levelCon;
    EnergyHandler _energyHandler;
    UI_Controller _uiController;

    //settings
    [SerializeField] ParticleSystem _energyBeamParticle = null;
    [SerializeField] SolarDishHandler _solarDishPrefab = null;
    [SerializeField] float _energyBoostRange = 10f;
    [SerializeField] float _baseEnergyBoostRate = 0.5f;
    [SerializeField] Sprite _dugoutIconForSolarDish = null;

    [Header("Upgrade Parameters")]
    [SerializeField] float _energyBoostRateMultiplier_Upgrade = 1.2f;
    [SerializeField] float _energyBoostRangeMultiplier_Upgrade = 1.2f;

    //state
    Vector3 _dir;
    float _angle;
    SolarDishHandler _currentSolarDish;
    float _dist;
    float _energyBoostRateCurrent;
    ParticleSystem.ShapeModule _shape;
    Image _currentDugoutImage;

    

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _levelCon = FindObjectOfType<LevelController>();
        _uiController = _levelCon.GetComponent<UI_Controller>();
        _levelCon.WarpingOutFromOldLevel += DestroySolarDish;
        _levelCon.WarpedIntoNewLevel += DeploySolarDish;
        _energyHandler = GetComponentInParent<EnergyHandler>();
        DeploySolarDish(null);
        _energyBoostRateCurrent = _baseEnergyBoostRate;
        _shape = _energyBeamParticle.shape;
        _energyBeamParticle.Stop();
    }


    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        DestroySolarDish();
    }


    public override object GetUIStatus()
    {
        return null;
    }

    private void Update()
    {
        if (_currentDugoutImage && !_currentSolarDish)
        {
            _currentDugoutImage = null;
        }

        if (!_currentSolarDish) return;

        _dir = (_currentSolarDish.transform.position - transform.position);
        _dist = _dir.magnitude;
        if (_dist <= _energyBoostRange)
        {
            _energyHandler.SpendEnergy(-1 * _energyBoostRateCurrent * Time.deltaTime);
            _energyBeamParticle.Play();


            _energyBeamParticle.transform.up = (Vector2)_dir;
            _energyBeamParticle.transform.position = transform.position + (_dir / 2f);
            _shape.radius = _dir.magnitude / 2f;
        }
        else
        {
            _energyBeamParticle.Stop();
        }

        if (_currentDugoutImage)
        {
            UpdateDugoutIcon();
        }

    }

    private void UpdateDugoutIcon()
    {
        _angle = Vector2.SignedAngle(Vector2.up, _dir);
        float distFactor = Mathf.Lerp(0.33f, 1f, 1 - (_dist/_levelCon.ArenaRadius));
        _uiController.UpdateDugoutCustom(_currentDugoutImage, _angle, distFactor);
    }

    protected override void ImplementSystemUpgrade()
    {
        _energyBoostRateCurrent *= _energyBoostRateMultiplier_Upgrade;
        _energyBoostRange *= _energyBoostRangeMultiplier_Upgrade;
    }
    protected override void ImplementSystemDowngrade()
    {

    }


    private void DestroySolarDish()
    {
        if (_currentSolarDish)
        {
            Destroy(_currentSolarDish.gameObject);
            _currentDugoutImage = null;
        }
    }

    private void DeploySolarDish(Level obj)
    {
        _currentSolarDish = Instantiate(_solarDishPrefab, transform.position, transform.rotation);
        _currentDugoutImage = _uiController.CreateCustomDugoutIcon(1f, _dugoutIconForSolarDish);
    }
}
