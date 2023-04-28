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
    [SerializeField] float _energyBoostRate = 0.5f;
    [SerializeField] Sprite _dugoutIconForSolarDish = null;

    [Header("Upgrade Parameters")]
    [SerializeField] int _dishCountAddition_Upgrade = 1;

    //state
    int _deployCount = 1;
    Vector3[] _dir;
    float[] _angle;
    SolarDishHandler[] _deployedSolarDishes;
    float[] _dist;
    ParticleSystem.ShapeModule _shape;
    Image[] _currentDugoutImages;

    

    public override void IntegrateSystem(SystemIconDriver connectedSID)
    {
        base.IntegrateSystem(connectedSID);
        _levelCon = FindObjectOfType<LevelController>();
        _uiController = _levelCon.GetComponent<UI_Controller>();
        _levelCon.WarpingOutFromOldLevel += DestroySolarDishes;
        _levelCon.WarpedIntoNewLevel += DeploySolarDishes;
        _energyHandler = GetComponentInParent<EnergyHandler>();
        //DeploySolarDishes(null);
        _shape = _energyBeamParticle.shape;
        _energyBeamParticle.Stop();

        _deployedSolarDishes = new SolarDishHandler[0];
        _currentDugoutImages = new Image[0];
    }


    public override void DeintegrateSystem()
    {
        base.DeintegrateSystem();
        DestroySolarDishes();
    }


    public override object GetUIStatus()
    {
        return new Vector2Int(_deployCount, 5);
    }

    private void Update()
    {
        //if (_currentDugoutImages && _deployedSolarDishes.)
        //{
        //    _currentDugoutImages = null;
        //}

        //if (!_deployedSolarDishes.Length) return;

        if (_deployedSolarDishes.Length == 0) return;
        bool isBeaming = false;
        for (int i = 0; i < _deployedSolarDishes.Length; i++)
        {
            if (isBeaming) break;
            _dir[i] = (_deployedSolarDishes[i].transform.position - transform.position);
            _dist[i] = _dir[i].magnitude;
            if (_dist[i] <= _energyBoostRange)
            {
                _energyHandler.SpendEnergy(-1 * _energyBoostRate * Time.deltaTime);
                _energyBeamParticle.Play();
                isBeaming = true;
                _energyBeamParticle.transform.up = (Vector2)_dir[i];
                _energyBeamParticle.transform.position = transform.position + (_dir[i] / 2f);
                _shape.radius = _dir[i].magnitude / 2f;
            }
        }
        if (!isBeaming) _energyBeamParticle.Stop();
        UpdateDugoutIcons();
    }

    private void UpdateDugoutIcons()
    {
        for (int i = 0; i < _deployedSolarDishes.Length; i++)
        {
            _angle[i] = Vector2.SignedAngle(Vector2.up, _dir[i]);
            float distFactor = Mathf.Lerp(0.33f, 1f, 1 - (_dist[i] / _levelCon.ArenaRadius));
            _uiController.UpdateDugoutCustom(_currentDugoutImages[i], _angle[i], distFactor);
        }
    }

    protected override void ImplementSystemUpgrade()
    {
        _deployCount += _dishCountAddition_Upgrade;
        _connectedID?.UpdateUI(new Vector2Int(_deployCount, 5));
    }
    protected override void ImplementSystemDowngrade()
    {

    }


    private void DestroySolarDishes()
    {
        if (_deployedSolarDishes.Length == 0) return;
        for (int i = _deployedSolarDishes.Length-1; i >0 ; i--)
        {
            Destroy(_deployedSolarDishes[i].gameObject);
            _currentDugoutImages = null;
        }
        _uiController.ClearAllCustomDugoutIcons();
    }

    private void DeploySolarDishes(Level obj)
    {
        _deployedSolarDishes = new SolarDishHandler[_deployCount];
        _currentDugoutImages = new Image[_deployCount];
        _dir = new Vector3[_deployCount];
        _dist = new float[_deployCount];
        _angle = new float[_deployCount];

        List<Vector3> existingPoints = new List<Vector3>();
        for (int i = 0; i < _deployCount; i++)
        {
            Vector2 pos = CUR.GetRandomPosWithinArenaAwayFromOtherPoints(
                Vector3.zero, _levelCon.ArenaRadius,
                existingPoints, 10f);
            _deployedSolarDishes[i] = Instantiate(_solarDishPrefab, pos, transform.rotation);
            _currentDugoutImages[i] = _uiController.CreateCustomDugoutIcon(1f, _dugoutIconForSolarDish);
            existingPoints.Add(pos);
        }
        
    }
}
