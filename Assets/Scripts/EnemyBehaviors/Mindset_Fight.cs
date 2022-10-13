using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mindset_Fight : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;
    WeaponHandler _weaponHandler;

    public enum FightOptions { Contact, CloseRange, StandoffRange}

    [SerializeField] FightOptions _fightOption = FightOptions.Contact;

    [Tooltip("What percentage of max weapon range does this strategy reference")]
    [SerializeField] float _decisionRangeFactor = 1f;

    [SerializeField] float _minBoresightErrorToFire = 5f;
    //state
    float _decisionRange;


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

    }

    public override void UpdateMindset()
    {
        UpdateNavigation();
        UpdateWeaponry();

    }

    private void UpdateWeaponry()
    {
        Vector2 dir = _mindsetHandler.PlayerPosition - (Vector2)transform.position;
        bool isInRange = (dir).magnitude < _decisionRange;

        float angleOffComputedSteering = Vector3.SignedAngle(transform.up, dir, Vector3.forward);

        bool isInAngle = Mathf.Abs(angleOffComputedSteering) < _minBoresightErrorToFire;

        if (isInRange && isInAngle)
        {
            _weaponHandler.Activate();
        }
    }

    private void UpdateNavigation()
    {
        Vector2 newTargetPos = Vector2.zero;
        switch (_fightOption)
        {
            case FightOptions.Contact:
                newTargetPos = _mindsetHandler.PlayerPosition;
                _mindsetHandler.SetTarget(newTargetPos, true);
                break;

            case FightOptions.CloseRange:
                _mindsetHandler.SetTarget(newTargetPos, false);
                break;

            case FightOptions.StandoffRange:
                _mindsetHandler.SetTarget(newTargetPos, false);
                break;
        }
    }
}
