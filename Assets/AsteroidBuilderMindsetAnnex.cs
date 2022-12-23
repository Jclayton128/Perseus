using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBuilderMindsetAnnex : MonoBehaviour
{
    Mindset_Explore _mindsetExplore;
    AsteroidHandler _currentAsteroidHome;
    HealthHandler _currentAsteroidHealth;

    //settings
    [SerializeField] GameObject _turretPrefab = null;
    float _timeBetweenAsteroidScans = 1f;
    float _asteroidScanRange = 10f;
    float _asteroidConstructionRate = 1f;
    float _asteroidConstructionRange = 1f;
    [SerializeField] float _timeRequiredToConstructAsteroid = 5f;

    //state
    float _timeSpentOnCurrentAsteroid = 0;
    float _timeForNextAsteroidScan;


    private void Awake()
    {
        _mindsetExplore = GetComponent<Mindset_Explore>();
    }

    private void Start()
    {
        _mindsetExplore.ExploreBehavior = Mindset_Explore.ExploreOptions.RandomFarMove;
    }

    private void Update()
    {
        if (_currentAsteroidHome)
        {
            //Work on the turret
            UpdateAsteroidConstruction();

        }
        else
        {
            //Look for new asteroid
            UpdateAsteroidScan();
        }
    }

    private void UpdateAsteroidConstruction()
    {
        if ((_currentAsteroidHome.transform.position - transform.position).magnitude < _asteroidConstructionRange)
        {
            _timeSpentOnCurrentAsteroid += Time.deltaTime * _asteroidConstructionRate;
            if (_timeSpentOnCurrentAsteroid >= _timeRequiredToConstructAsteroid)
            {
                ConstructAsteroid();
                _timeSpentOnCurrentAsteroid = 0;
            }
        }
    }

    private void ConstructAsteroid()
    {
        if (_currentAsteroidHome.HasTurret) _currentAsteroidHome.ActivateExistingTurret();
        else
        {
            GameObject newTurret = Instantiate(_turretPrefab, _currentAsteroidHome.transform);
            _currentAsteroidHome.InstallNewTurret(newTurret.GetComponent<StandaloneTurretBrain>());
        }

        _currentAsteroidHealth.Dying -= HandleDyingAsteroid;
        _currentAsteroidHealth = null;
        _currentAsteroidHome = null;
    }

    private void UpdateAsteroidScan()
    {
        if (Time.time >= _timeForNextAsteroidScan)
        {
            ScanForAsteroid();
            _timeForNextAsteroidScan = Time.time + _timeBetweenAsteroidScans;
        }
    }

    private void ScanForAsteroid()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position,
            _asteroidScanRange, LayerLibrary.EnemyNeutralLayerMask, 0f, 0.1f);

        if (coll && coll.TryGetComponent<AsteroidHandler>(out _currentAsteroidHome))
        {
            _currentAsteroidHome.ClaimAsteroid();
            _currentAsteroidHealth = _currentAsteroidHome.GetComponent<HealthHandler>();
            _currentAsteroidHealth.Dying += HandleDyingAsteroid;
            _mindsetExplore.ExploreBehavior = Mindset_Explore.ExploreOptions.RandomCloseDependentMove;
            _mindsetExplore.SetDependentTransform(_currentAsteroidHome.transform);
        }
        else
        {
            _mindsetExplore.ExploreBehavior = Mindset_Explore.ExploreOptions.RandomFarMove;
        }
    }

    private void HandleDyingAsteroid()
    {
        _mindsetExplore.ExploreBehavior = Mindset_Explore.ExploreOptions.RandomFarMove;
        _mindsetExplore.SetDependentTransform(null);

        _currentAsteroidHealth.Dying -= HandleDyingAsteroid;
        _currentAsteroidHome = null;
        _currentAsteroidHealth = null;
    }

    private void OnDestroy()
    {
        if (_currentAsteroidHome)
        {
            _currentAsteroidHome.DeclaimAsteroid();
            _currentAsteroidHealth.Dying -= HandleDyingAsteroid;
        }
    }
}
