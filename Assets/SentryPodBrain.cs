using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryPodBrain : MonoBehaviour
{
    public enum SentryMode { Stun0, Move1, Swat2, Heal3, Slay4 }
    Rigidbody2D _rb;


    //settings
    [SerializeField] float _moveForce_Fire = 1f;
    [SerializeField] float _moveForce_Move = 3f;
    [SerializeField] float _turnRate = 40f;
    [SerializeField] float _closeEnough = 2f;
    [SerializeField] float _scanRange = 10f;
    [SerializeField] float _timeBetweenScans = 0.5f;
    [SerializeField] WeaponHandler _stunWH = null;
    [SerializeField] WeaponHandler _swatWH = null;
    [SerializeField] WeaponHandler _slayWH = null;


    //state
    [SerializeField] SentryMode _currentMode = SentryMode.Stun0;
    [SerializeField] float _moveFactor = 0;
    [SerializeField] Transform _currentTarget;
    Transform _player;
    float _lookAngle;
    Vector3 _lookDir = Vector3.positiveInfinity;
    Vector3 _moveDir;
    float _timeForNextScan;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Transform transform)
    {
        _player = transform;
        EnergyHandler eh = GetComponent<EnergyHandler>();
        _stunWH.Initialize(eh, false, null);
        _swatWH.Initialize(eh, false, null);
        _slayWH.Initialize(eh, false, null);
    }

    public void SetSentryMode(int mode)
    {
        _currentMode = (SentryMode)mode;
        _currentTarget = null;
    }


    private void Update()
    {
        UpdateTargeting();
        UpdateShooting();
        UpdateMoving();
        UpdateFacing();
    }


    private void UpdateTargeting()
    {
        if (_currentTarget)
        {
            _lookDir = _currentTarget.position - transform.position;
            if (_lookDir.magnitude <= _scanRange) return;
        }

        if (_currentMode == SentryMode.Stun0 ||
            _currentMode == SentryMode.Swat2 ||
            _currentMode == SentryMode.Slay4)
        {
            _currentTarget = CUR.FindNearestGameObjectOnLayer(_player, LayerLibrary.EnemyLayerMask, _scanRange)?.transform;
        
        }
        else if (_currentMode == SentryMode.Heal3 ||
                _currentMode == SentryMode.Move1)
        {
            _currentTarget = _player;
        }
        else
        {
            _lookDir = _moveDir;
        }
    }

    private void UpdateShooting()
    {
        if (_currentTarget && _currentMode == SentryMode.Stun0)
        {
            if (_lookDir.magnitude < _stunWH.GetMaxWeaponRange() * 0.5f)
            {
                _stunWH.Activate();
                return;
            }
        }

        if (_currentTarget && _currentMode == SentryMode.Swat2)
        {
            if (_lookDir.magnitude < _swatWH.GetMaxWeaponRange() * 0.8f)
            {
                _swatWH.Activate();
                return;
            }
        }

        if (_currentTarget && _currentMode == SentryMode.Slay4)
        {
            if (_lookDir.magnitude < _slayWH.GetMaxWeaponRange() * 0.8f)
            {
                _slayWH.Activate();
                return;
            }
        }

        _slayWH.Deactivate();
        _stunWH.Deactivate();
        _swatWH.Deactivate();
    }

    private void UpdateMoving()
    {
        _moveDir = _player.position - transform.position;
        if (_moveDir.magnitude <= _closeEnough) return;


        _moveFactor = Mathf.InverseLerp(_closeEnough, _closeEnough * 2f, _moveDir.magnitude);
        if (_currentMode == SentryMode.Move1)
        {
            _rb.velocity = (_moveDir.normalized * _moveForce_Move * _moveFactor);
        }
        else
        {
            _rb.velocity = (_moveDir.normalized * _moveForce_Fire * _moveFactor);
        }

    }

    private void UpdateFacing()
    {
        if (!_currentTarget) return;
        _lookAngle = Vector3.SignedAngle(Vector3.up, _lookDir, Vector3.forward);

        Quaternion angleToPoint = Quaternion.Euler(0, 0, _lookAngle);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, angleToPoint,
            _turnRate * Time.deltaTime);
    }
}
