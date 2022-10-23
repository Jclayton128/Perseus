using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindsetHandler : MonoBehaviour, IPlayerSeeking
{
    ActorMovement _movement;
    EnergyHandler _energyHandler;
    HealthHandler _health;
    WeaponHandler _weaponHandler;
    LevelController _levelController;


    public Mindset_Explore ExploreMindset { get; private set; }
    public Mindset_Fight FightMindset { get; private set; }
    public Mindset_Hunt HuntMindset { get; private set; }
    public Mindset_React ReactMindset { get; private set; }

    //settings
    [Tooltip("Radius of the player detector collider. If negative, that collider is disabled," +
        "and the player will never be detected.")]
    [SerializeField] float _detectorRange = 10f;

    //state
    [SerializeField] Mindset _activeMindset;
    [SerializeField] Vector2 _targetPosition = Vector2.zero;
    public Vector2 TargetPosition => _targetPosition;

    bool _shouldLeadTargetPos = false;
    public bool ShouldLeadTargetPos => _shouldLeadTargetPos;

    float _standoffRange = 0;
    public float StandoffRange => _standoffRange;


    Vector2 _playerPosition = Vector2.zero;
    public Vector2 PlayerPosition => _playerPosition;


    Vector2 _playerVelocity = Vector2.zero;
    public Vector2 PlayerVelocity => _playerVelocity;


    [SerializeField] float _targetAge = Mathf.Infinity; // how many seconds has it been since the player was detected
    public float TargetAge => _targetAge;




    private void Awake()
    {
        _levelController = FindObjectOfType<LevelController>();

       _movement = GetComponent<ActorMovement>();
        _energyHandler = GetComponent<EnergyHandler>();
        _health = GetComponent<HealthHandler>();
        _health.ReceivingDamagePack += HandleReceivingDamage;

        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _weaponHandler?.Initialize(_energyHandler, false, null);

        SystemHandler[] shs = GetComponentsInChildren<SystemHandler>();
        foreach (SystemHandler sh in shs)
        {
            sh.IntegrateSystem(null);
        }

        ExploreMindset = GetComponent<Mindset_Explore>();
        FightMindset = GetComponent<Mindset_Fight>();
        HuntMindset = GetComponent<Mindset_Hunt>();
        ReactMindset = GetComponent<Mindset_React>();

        ExploreMindset.InitializeMindset(this, _levelController);
        FightMindset.InitializeMindset(this, _levelController);
        HuntMindset.InitializeMindset(this, _levelController);
        ReactMindset.InitializeMindset(this, _levelController);

        _activeMindset = ExploreMindset;
        _activeMindset.EnterMindset();
    }

    private void Start()
    {
        GetComponentInChildren<DetectionHandler>().ModifyDetectorRange(_detectorRange);
    }

    private void Update()
    {
        _targetAge += Time.deltaTime;
        EvaluateMindsetBasedOnTargetAge();
        _activeMindset.UpdateMindset();
    }

    private void EvaluateMindsetBasedOnTargetAge()
    {
        if (_targetAge < 0.1f &&
            FightMindset.FightMovement != Mindset_Fight.FightMovements.NoSpecialFightMovement)
        {
            if (_activeMindset != FightMindset) MoveToNewMindset(FightMindset);
        }

    }

    public void MoveToNewMindset(Mindset newMindset)
    {
        //Debug.Log($"Exiting {_activeMindset}. Entering {newMindset}");
        _activeMindset.ExitMindset();
        _activeMindset = newMindset;
        _activeMindset.EnterMindset();
    }

    public void SetTargetPosition(Vector2 targetPosition, float standoffRange, bool shouldLeadTarget)
    {
        _targetPosition =  targetPosition;
        _shouldLeadTargetPos = shouldLeadTarget;
    }

    public void ReportPlayer(Vector2 playerPosition, Vector2 playerVelocity)
    {
        _playerPosition = playerPosition;
        _playerVelocity = playerVelocity;
        _targetAge = 0;
    }

    private void HandleReceivingDamage(DamagePack dp)
    {
        MoveToNewMindset(ReactMindset);
    }


    [ContextMenu("Set New Detector Range")]
    private void SetDetectorRange()
    {
        GetComponentInChildren<DetectionHandler>().ModifyDetectorRange(_detectorRange);
    }


}
