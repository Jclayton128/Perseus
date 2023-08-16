using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Mindset_Fight : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;
    [SerializeField] WeaponHandler[] _weaponHandlers = null;

    public enum FightMovements { Contact, StandoffDumb, StandoffLead, NoSpecialFightMovement, FlyToOffset,
    FlyToRandomRadiusOffset}

    [ShowIf("FightMovement", FightMovements.FlyToOffset)]
    [Tooltip("The local offset (from the player) this enemy should fly towards")]
    [SerializeField] Vector2 _offsetToFlyTo = Vector2.zero;

    [ShowIf("FightMovement", FightMovements.FlyToRandomRadiusOffset)]
    [Tooltip("The radius (from the player) this enemy should fly select as a random offset point")]
    [SerializeField] float _radiusToFlyTo = 1f;

    public enum FightWeaponUse { FireWhenAccurate, FireContinuously,
        FireContinuouslyIfSeePlayer, NeverFire}

    [SerializeField] public FightMovements FightMovement = FightMovements.Contact;
    [SerializeField] FightWeaponUse _fightWeaponUse = FightWeaponUse.FireWhenAccurate;

    [Tooltip("What percentage of max weapon range does this strategy reference")]
    [SerializeField] float _decisionRangeFactor = 1f;

    [SerializeField] float _minBoresightErrorToFire = 5f;

    //state
    [SerializeField] float _decisionRange;
    bool _wasJustActivatingWeapon = false;


    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;

        EnergyHandler eh = GetComponent<EnergyHandler>();
        //_weaponHandlers = GetComponentsInChildren<WeaponHandler>();
        foreach (var wh in _weaponHandlers)
        {
            wh.Initialize(eh, false, null);
        }

        if (_weaponHandlers[0]) _decisionRange = _weaponHandlers[0].GetMaxWeaponRange() * _decisionRangeFactor;
        else _decisionRange = 1;

        if (FightMovement == FightMovements.FlyToRandomRadiusOffset)
        {
            _offsetToFlyTo = UnityEngine.Random.insideUnitCircle.normalized * _radiusToFlyTo;
        }
    }

    public override void EnterMindset()
    {

    }

    public override void ExitMindset()
    {
        foreach (var wh in _weaponHandlers)
        {
            wh.Deactivate();
        }
    }
    public void Update()
    {
        if (_fightWeaponUse == FightWeaponUse.FireContinuously)
        {
            foreach (var wh in _weaponHandlers)
            {
                wh.Activate();
            }
        }
        if (_fightWeaponUse == FightWeaponUse.FireContinuouslyIfSeePlayer)
        {
            if (_mindsetHandler.TargetAge < 0.1f)
            {
                foreach (var wh in _weaponHandlers)
                {
                    wh.Activate();

                }
                _wasJustActivatingWeapon = true;
            }
            else if (_wasJustActivatingWeapon)
            {
                foreach (var wh in _weaponHandlers)
                {
                    wh.Deactivate();
                }
                _wasJustActivatingWeapon = false;
            }
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
            foreach (var wh in _weaponHandlers)
            {
                wh.Activate();
            }
        }
        else
        {
            foreach (var wh in _weaponHandlers)
            {
                wh.Deactivate();
            }
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

            case FightMovements.FlyToOffset:
            case FightMovements.FlyToRandomRadiusOffset:
                if (_mindsetHandler.PlayerTransform != null)
                {
                    newTargetPos = _mindsetHandler.PlayerTransform.TransformPoint(_offsetToFlyTo);
                    _mindsetHandler.SetTargetPosition(newTargetPos, 0, false);
                }               
                break;
        }
    }
}
