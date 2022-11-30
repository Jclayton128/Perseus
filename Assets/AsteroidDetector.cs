using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDetector : MonoBehaviour
{
    Mindset_Explore _mindsetExplore;
    AsteroidHandler _currentAsteroidHome;
    HealthHandler _currentAsteroidHealth;

    //settings
    [SerializeField] GameObject _turretPrefab = null;
    float _timeBetweenAsteroidScans = 1f;
    float _asteroidScanRange = 10f;

    //state
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
        }
        else
        {
            //Look for new asteroid
            UpdateAsteroidScan();
        }
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
            _asteroidScanRange, LayerLibrary.NeutralLayerMask, 0f, 0.1f);

        if (coll && coll.TryGetComponent<AsteroidHandler>(out _currentAsteroidHome))
        {
            Debug.Log("found a home!");
            _currentAsteroidHome.ClaimAsteroid();
            _currentAsteroidHealth = _currentAsteroidHome.GetComponent<HealthHandler>();
            _currentAsteroidHealth.Dying += HandleDyingAsteroid;
            _mindsetExplore.ExploreBehavior = Mindset_Explore.ExploreOptions.RandomCloseDependentMove;
            _mindsetExplore.SetDependentTransform(_currentAsteroidHome.transform);
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
