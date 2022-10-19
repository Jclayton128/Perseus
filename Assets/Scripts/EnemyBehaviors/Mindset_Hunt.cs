using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mindset_Hunt : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;
    ActorMovement _actorMovement;

    public enum HuntOption {BackToExplore, SniffCurrentPosition, SniffFuturePosition,  }

    //settings
    [Tooltip("How long the enemy will continue to hunt before going back into explore")]
    [SerializeField] float _timeframeForHunting = 5f;
    [SerializeField] HuntOption _huntOption = HuntOption.BackToExplore;

    public float TimeframeForHunting => _timeframeForHunting;

    //state
    bool _isHunting = false;

    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
        _actorMovement = GetComponent<ActorMovement>();
    }
    
    public override void EnterMindset()
    {
        _isHunting = false;
        if (_huntOption == HuntOption.BackToExplore)
        {
            _mindsetHandler.MoveToNewMindset(_mindsetHandler.ExploreMindset);
        }
    }
    public override void ExitMindset()
    {
        
    }
    public override void UpdateMindset()
    {
        if (_mindsetHandler.TargetAge > TimeframeForHunting)
        {
            _mindsetHandler.MoveToNewMindset(_mindsetHandler.ExploreMindset);
        }

        if (_isHunting) return; // a hunt point has already been issued.

        Vector2 newTargetPos = transform.position;
        switch (_huntOption)
        {
            case HuntOption.SniffCurrentPosition:
                newTargetPos = _mindsetHandler.PlayerPosition;
                break;

            case HuntOption.SniffFuturePosition:
                float timeToReachPlayer =
                    (((Vector2)transform.position - _mindsetHandler.PlayerPosition).magnitude /
                    _actorMovement.TopSpeed_Chosen / 2f) * 1.25f; //1.25 for buffer

                Vector2 futureTravel = _mindsetHandler.PlayerVelocity * timeToReachPlayer;
                newTargetPos = _mindsetHandler.PlayerPosition + futureTravel;

                Debug.DrawLine(_mindsetHandler.PlayerPosition,
                    _mindsetHandler.PlayerPosition + futureTravel,
                    Color.blue, 5f);
                break;
        }

        _mindsetHandler.SetTargetPosition(newTargetPos, 0, false);
        _isHunting = true;

    }
}
