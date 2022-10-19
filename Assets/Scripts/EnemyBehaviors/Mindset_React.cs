using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mindset_React : Mindset, IRespondsToThreatVectors
{
    public enum ReactOptions {BackToExplore, FleeAway, ChallengeInto,
        InvertTargetPoint, MoveToDistantPoint }

    MindsetHandler _mindsetHandler;
    LevelController _levelController;

    //settings
    [Tooltip("How long does the ship stay in the React mindset. Acquiring the player" +
        "should short-circuit back into Fight mindset")]
    [SerializeField] float _reactTimeframe = 5f;

    [SerializeField] float _reactDistance = 5f;

    //state
    bool _hasReacted = false;
    float _timeToExitMindset;
    [SerializeField] ReactOptions _reactOption = ReactOptions.BackToExplore;

    Vector2 _threatVector = Vector2.zero;
    public Vector2 ThreatVector => _threatVector;

    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
        HealthHandler hh = GetComponent<HealthHandler>();
        hh.ReceivingThreatVector += SetThreatVector;
    }

    public override void EnterMindset()
    {
        _timeToExitMindset = Time.time + _reactTimeframe;
        _hasReacted = false;
    }

    public override void ExitMindset()
    {

    }

    public override void UpdateMindset()
    {
        if (Time.time >= _timeToExitMindset)
        {
            _mindsetHandler.MoveToNewMindset(_mindsetHandler.ExploreMindset);
        }

        if (_hasReacted) return;

        Vector2 newTargetPosition = Vector2.zero;
        switch (_reactOption)
        {
            case ReactOptions.BackToExplore:
                _mindsetHandler.MoveToNewMindset(_mindsetHandler.ExploreMindset);
                break;

            case ReactOptions.FleeAway:
                newTargetPosition = (Vector2)transform.position +
                    (_threatVector * _reactDistance);
                break;

            case ReactOptions.ChallengeInto:
                newTargetPosition = (Vector2)transform.position -
                    (_threatVector * _reactDistance);
                break;

            case ReactOptions.InvertTargetPoint:
                Vector2 dir = (_mindsetHandler.TargetPosition - (Vector2)transform.position);
                newTargetPosition = (Vector2)transform.position - dir;
                break;

            case ReactOptions.MoveToDistantPoint:
                newTargetPosition = CUR.FindRandomPositionWithinRangeBandAndWithinArena(
                    transform.position, _reactDistance, 3 * _reactDistance,
                    Vector2.zero, _levelController.ArenaRadius);
                break;
        }
        _mindsetHandler.SetTargetPosition(newTargetPosition, 0, false);
        _hasReacted = true;
    }

    public void SetThreatVector(Vector2 threatVector)
    {
        _threatVector = threatVector;
    }
}
