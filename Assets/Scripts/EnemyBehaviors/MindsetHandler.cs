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

    Mindset _exploreMindset;
    Mindset _fightMindset;
    Mindset _huntMindset;
    Mindset _reactMindset;


    //state
    [SerializeField] Mindset _activeMindset;
    [SerializeField] Vector2 _targetPosition = Vector2.zero;
    public Vector2 TargetPosition => _targetPosition;
    Vector2 _targetVelocity = Vector2.zero;
    public Vector2 TargetVelocity => _targetVelocity;
    bool _isTargetStrict = false;
    public bool IsTargetStrict => _isTargetStrict;


    float _targetConfidence = 0; // 0-1, how long has it been since the player was detected
    public float TargetConfidence => _targetConfidence;


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
        _activeMindset.UpdateMindset();
    }

    public void MoveToNewMindset(Mindset newMindset)
    {
        Debug.Log($"Exiting {_activeMindset}. Entering {newMindset}");
        _activeMindset.ExitMindset();
        _activeMindset = newMindset;
        _activeMindset.EnterMindset();
    }

    public void SetTarget(Vector2 targetPosition, Vector2 targetVelocity, bool isStrict)
    {
        _targetPosition =  targetPosition;
        _targetVelocity = targetVelocity;
        _isTargetStrict = isStrict;
    }



}
