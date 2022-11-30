using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mindset_Fight : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;
    WeaponHandler _weaponHandler;

    public enum FightMovements { Contact, StandoffDumb, StandoffLead, NoSpecialFightMovement}
    public enum FightWeaponUse { FireWhenAccurate, FireContinuously,
        FireContinuouslyIfSeePlayer, NeverFire}

    [SerializeField] public FightMovements FightMovement = FightMovements.Contact;
    [SerializeField] FightWeaponUse _fightWeaponUse = FightWeaponUse.FireWhenAccurate;

    [Tooltip("What percentage of max weapon range does this strategy reference")]
    [SerializeField] float _decisionRangeFactor = 1f;

    [SerializeField] float _minBoresightErrorToFire = 5f;

    //state
    [SerializeField] float _decisionRange;


    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        if (_weaponHandler) _decisionRange = _weaponHandler.GetMaxWeaponRange() * _decisionRangeFactor;
        else _decisionRange = 1;
    }

    public override void EnterMindset()
    {

    }

    public override void ExitMindset()
    {
        _weaponHandler.Deactivate();
    }
    public void Update()
    {
        if (_fightWeaponUse == FightWeaponUse.FireContinuously)
        {
            _weaponHandler.Activate();
        }
        if (_fightWeaponUse == FightWeaponUse.FireContinuouslyIfSeePlayer &&
            _mindsetHandler.TargetAge < 0.1f)
        {
            _weaponHandler.Activate();
        }
    }

    public override void UpdateMindset()
    {
        CheckForExitFightMindset();
        UpdateNavigation();
        UpdateWeaponry();

    }

    private void CheckForExitFightMindset()
    {
        if (_mindsetHandler.TargetAge > 0.1f)
        {
            _mindsetHandler.MoveToNewMindset(_mindsetHandler.HuntMindset);
        }
    }

    private void UpdateWeaponry()
    {
        if (_fightWeaponUse == FightWeaponUse.NeverFire) return;

        Vector2 dir = _mindsetHandler.PlayerPosition - (Vector2)transform.position;
        bool isInRange = (dir).magnitude <= _decisionRange;
        float angleOffComputedSteering = Vector3.SignedAngle(transform.up, dir, Vector3.forward);
        bool isInAngle = Mathf.Abs(angleOffComputedSteering) < _minBoresightErrorToFire;

        switch (_fightWeaponUse)
        {
            case FightWeaponUse.FireWhenAccurate:
                FireWhenAccurate(isInRange, isInAngle);
                break;
        }


    }

    private void FireWhenAccurate(bool isInRange, bool isInAngle)
    {
        if (isInRange && isInAngle)
        {
            _weaponHandler.Activate();
        }
        else
        {
            _weaponHandler.Deactivate();
        }
    }

    private void UpdateNavigation()
    {
        Vector2 newTargetPos = Vector2.zero;     
        switch (FightMovement)
        {
            case FightMovements.Contact:
                newTargetPos = _mindsetHandler.PlayerPosition;
                _mindsetHandler.SetTargetPosition(newTargetPos,0, true);
                break;

            case FightMovements.StandoffDumb:
                newTargetPos = _mindsetHandler.PlayerPosition;
                _mindsetHandler.SetTargetPosition(newTargetPos, _decisionRange, false);
                break;

            case FightMovements.StandoffLead:
                newTargetPos = _mindsetHandler.PlayerPosition;
                _mindsetHandler.SetTargetPosition(newTargetPos, _decisionRange, true);
                break;
        }
    }
}
