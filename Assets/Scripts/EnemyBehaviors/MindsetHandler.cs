using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindsetHandler : MonoBehaviour
{
    ActorMovement _movement;
    EnergyHandler _energyHandler;
    HealthHandler _health;
    WeaponHandler _weaponHandler;
    LevelController _levelController;

    Mindset_Explore _exploreMindset;
    Mindset_Fight _fightMindset;
    Mindset_Hunt _huntMindset;
    Mindset_React _reactMindset;


    //state
    [SerializeField] Mindset _activeMindset;
    [SerializeField] Vector2 _targetPosition = Vector2.zero;
    public Vector2 TargetPosition => _targetPosition;

    bool _isTargetStrict = false;
    public bool IsTargetStrict => _isTargetStrict;


    Vector2 _playerPosition = Vector2.zero;
    public Vector2 PlayerPosition => _playerPosition;


    Vector2 _playerVelocity = Vector2.zero;
    public Vector2 PlayerVelocity => _playerVelocity;



    [SerializeField] float _targetAge = 0; // how many seconds has it been since the player was detected
    public float TargetAge => _targetAge;


    private void Awake()
    {
        _levelController = FindObjectOfType<LevelController>();

       _movement = GetComponent<ActorMovement>();
        _energyHandler = GetComponent<EnergyHandler>();
        _health = GetComponent<HealthHandler>();

        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _weaponHandler?.Initialize(_energyHandler, false, null);

        _exploreMindset = GetComponent<Mindset_Explore>();
        _fightMindset = GetComponent<Mindset_Fight>();
        _huntMindset = GetComponent<Mindset_Hunt>();
        _reactMindset = GetComponent<Mindset_React>();

        _exploreMindset.InitializeMindset(this, _levelController);
        _fightMindset.InitializeMindset(this, _levelController);
        _huntMindset.InitializeMindset(this, _levelController);
        _reactMindset.InitializeMindset(this, _levelController);

        _activeMindset = _exploreMindset;
        _activeMindset.EnterMindset();
    }

    private void Update()
    {
        _targetAge += Time.deltaTime;
        EvaluateMindsetBasedOnTargetAge();
        _activeMindset.UpdateMindset();
    }

    private void EvaluateMindsetBasedOnTargetAge()
    {
        if (_targetAge < 0.1f)
        {
            if (_activeMindset != _fightMindset) MoveToNewMindset(_fightMindset);
        }
        else if (_targetAge < _huntMindset.TimeframeForHunting)
        {
            if (_activeMindset != _huntMindset) MoveToNewMindset(_huntMindset);
        }
        else
        {
            //stick with current
        }

    }

    public void MoveToNewMindset(Mindset newMindset)
    {
        Debug.Log($"Exiting {_activeMindset}. Entering {newMindset}");
        _activeMindset.ExitMindset();
        _activeMindset = newMindset;
        _activeMindset.EnterMindset();
    }

    public void SetTarget(Vector2 targetPosition, bool isStrict)
    {
        _targetPosition =  targetPosition;
        _playerVelocity = Vector2.zero;
        _isTargetStrict = isStrict;
    }

    public void SetPlayerPositionOnPlayerSighting(Vector2 playerPosition, Vector2 playerVelocity)
    {
        _playerPosition = playerPosition;
        _playerVelocity = playerVelocity;
        _targetAge = 0;
    }



}
