using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderMindsetAnnex : MonoBehaviour
{
    Mindset_Explore _mse;

    //settings
    [SerializeField] PhaseShieldHandler _shieldPrefab = null;
    float _timeBetweenScans = 0.5f;
    [SerializeField] float _targetDetectorRange = 20f;
    [SerializeField] float _shieldingRange = 7f;
    [SerializeField] ParticleSystem _shieldBeam = null;

    //state
    float _timeForNextTargetScan;
    public ShipInfoHolder _targetShip;
    PhaseShieldHandler _currentShield;
    ParticleSystem.ShapeModule _shape;
    Vector3 _dir;

    private void Awake()
    {
        _mse = GetComponent<Mindset_Explore>();
        _shape = _shieldBeam.shape;
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -.1f);
        _mse.ExploreBehavior = Mindset_Explore.ExploreOptions.HoldPosition;
        _timeForNextTargetScan = 0;
        _shieldBeam.Stop();
    }

    private void Update()
    {
        if (_targetShip)
        {
            if ((transform.position - _targetShip.transform.position).magnitude <= _shieldingRange)
            {
                _currentShield.gameObject.SetActive(true);
                _shieldBeam.Play();                
                _dir = (_targetShip.transform.position - transform.position);
                _shieldBeam.transform.up = (Vector2)_dir;
                _shieldBeam.transform.position = transform.position + (_dir/2f);
                _shape.radius = _dir.magnitude/2f;
            }
            else
            {
                _shieldBeam.Stop();
                _currentShield.gameObject.SetActive(false);
            }
            //check if within protection range.
        }
        else
        {
            _shieldBeam.Stop();
            UpdateTargetShipScan();
            if (_currentShield) _currentShield.gameObject.SetActive(false);
        }

    }

    private void UpdateTargetShipScan()
    {
        if (Time.time >= _timeForNextTargetScan)
        {
            ScanForTargetShip();
            _timeForNextTargetScan = Time.time + _timeBetweenScans;
        }   
    }

    private void ScanForTargetShip()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position,
                    _targetDetectorRange, LayerLibrary.EnemyNeutralLayerMask, 0f, 0.1f);

        if (coll && coll.TryGetComponent<ShipInfoHolder>(out _targetShip))
        {
            if (_targetShip.GetComponentInChildren<PhaseShieldHandler>()) return;
            _mse.ExploreBehavior = Mindset_Explore.ExploreOptions.RandomCloseDependentMove;
            _mse.SetDependentTransform(_targetShip.transform);
            _currentShield = Instantiate(_shieldPrefab, _targetShip.transform.position, Quaternion.identity).GetComponent<PhaseShieldHandler>();
            _currentShield.SetFollowTarget(_targetShip.transform);
            _currentShield.gameObject.SetActive(false);
        }
        else
        {
            _mse.ExploreBehavior = Mindset_Explore.ExploreOptions.HoldPosition;
        }
    }

    private void OnDestroy()
    {
        if (_currentShield) Destroy(_currentShield.gameObject);
    }
}
