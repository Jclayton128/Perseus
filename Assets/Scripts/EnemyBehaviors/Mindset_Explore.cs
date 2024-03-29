using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// This is when the enemy has zero confidence of player�s location.
/// Updates the MindsetHandler's target position or target transform
/// Random Close Move: Pick a point within a certain range, and move towards it.Must check that the point is within Arena.
/// Random Far Move: Pick a point outside of a certain range, and move towards it. Must check that it is within Arena.
/// Strict Octagonal Movement: Move like the Fencer around the world.
/// Random Close Dependent Move: Pick a point within a certain range of a target game object. Must check that it is within Arena and continually evaluate whether the destination is still within a certain range of the target game object.
/// </summary>
public class Mindset_Explore : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;

    /// 
    public enum ExploreOptions {RandomCloseMove, RandomFarMove, StrictOctagonalMove,
    RandomCloseDependentMove, HoldPosition}

    //state
    public ExploreOptions ExploreBehavior = ExploreOptions.RandomCloseMove;

    [Tooltip("For Close moves, this is the no-further-than range. For far moves, this is " +
        "the no-closer-than range.")]
    [SerializeField] float _range = 5f;

    [Tooltip("Considered to be at destination when distance to destination is less " +
        "than this.")]
    [SerializeField] float _closeEnough = 0.5f;

    //state
    float _distanceToTargetPosition;
    Transform _dependentTransform;


    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
    }

    public void InitializeMindset(MindsetHandler mindsetHandlerRef,
        LevelController levelConRef, Transform dependentTransform)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
        _dependentTransform = dependentTransform;
    }

    public void SetDependentTransform(Transform dependentTransform)
    {
        if (dependentTransform == null)
        {
            _mindsetHandler.SetTargetPosition(transform.position, 0, false);
        }
        else
        {
            _dependentTransform = dependentTransform;
            _mindsetHandler.SetTargetPosition(_dependentTransform.position, 0, false);
        }

    }

    public override void EnterMindset()
    {
        UpdateTargetPosition();
    }

    public override void ExitMindset()
    {

    }

    public override void UpdateMindset()
    {
        _distanceToTargetPosition = (_mindsetHandler.TargetPosition -
            (Vector2)transform.position).magnitude;

        if (_distanceToTargetPosition < _closeEnough)
        {
            //Debug.Log("generating new point to explore");
            UpdateTargetPosition();
        }

        if (ExploreBehavior == ExploreOptions.RandomCloseDependentMove &&
            _dependentTransform &&
            ((_mindsetHandler.TargetPosition - (Vector2)_dependentTransform.position).magnitude > _range))
        {
            UpdateTargetPosition();
        }

    }

    private void UpdateTargetPosition()
    {
        Vector2 newTargetPosition = Vector2.zero;
        switch (ExploreBehavior)
        {
            case ExploreOptions.RandomCloseMove:
                newTargetPosition = FindRandomCloseMove();
                _mindsetHandler.SetTargetPosition(newTargetPosition,0, false);
                break;

            case ExploreOptions.RandomFarMove:
                newTargetPosition = FindRandomFarMove();
                _mindsetHandler.SetTargetPosition(newTargetPosition, 0, false);
                break;

            case ExploreOptions.StrictOctagonalMove:
                newTargetPosition = FindStrictOctagonalMove();
                _mindsetHandler.SetTargetPosition(newTargetPosition, 0, true);
                break;

            case ExploreOptions.RandomCloseDependentMove:
                if (!_dependentTransform) Debug.Log("Need a dependent transform!");
                else newTargetPosition = FindRandomCloseDependentMove();
                _mindsetHandler.SetTargetPosition(newTargetPosition, 0, false);
                break;

            case ExploreOptions.HoldPosition:
                //newTargetPosition = transform.position;
                //_mindsetHandler.SetTargetPosition(newTargetPosition, 0, true);
                break;

        }
    }

    private Vector2 FindRandomCloseMove()
    {
        Vector2 pos = CUR.FindRandomPositionWithinRangeBandAndWithinArena(
            transform.position, 0, _range,
            Vector2.zero, _levelController.ArenaRadius);
        return pos;
    }

    private Vector2 FindRandomFarMove()
    {
        Vector2 pos = CUR.FindRandomPositionWithinRangeBandAndWithinArena(
            transform.position, _range, _levelController.ArenaRadius,
            Vector2.zero, _levelController.ArenaRadius);
        return pos;
    }

    private Vector2 FindStrictOctagonalMove()
    {
        Vector2 pos = Vector2.zero;
        int leftStraightRight = UnityEngine.Random.Range(0, 3);
        int cardinalDir = GetCardinalDirection();
        if (leftStraightRight == 0)
        {            
            if (cardinalDir == 1)
            {
                pos = transform.position + new Vector3(-_range, _range, 0);
            }
            if (cardinalDir == 2)
            {
                pos = transform.position + new Vector3(_range, _range, 0);
            }
            if (cardinalDir == 3)
            {
                pos = transform.position + new Vector3(_range, -_range, 0);
            }
            if (cardinalDir == 4)
            {
                pos = transform.position + new Vector3(-_range, -_range, 0);
            }
        }
        if (leftStraightRight == 1)
        {
            if (cardinalDir == 1)
            {
                pos = transform.position + new Vector3(0, _range, 0);
            }
            if (cardinalDir == 2)
            {
                pos = transform.position + new Vector3(_range, 0, 0);
            }
            if (cardinalDir == 3)
            {
                pos = transform.position + new Vector3(0, -_range, 0);
            }
            if (cardinalDir == 4)
            {
                pos = transform.position + new Vector3(-_range, 0, 0);
            }
        }
        if (leftStraightRight == 2)
        {
            if (cardinalDir == 1)
            {
                pos = transform.position + new Vector3(_range, _range, 0);
            }
            if (cardinalDir == 2)
            {
                pos = transform.position + new Vector3(_range, -_range, 0);
            }
            if (cardinalDir == 3)
            {
                pos = transform.position + new Vector3(-_range, -_range, 0);
            }
            if (cardinalDir == 4)
            {
                pos = transform.position + new Vector3(-_range, _range, 0);
            }
        }
        pos = CheckPointAgainstArenaBoundaryAndCorrect(pos);
        return pos;
    }
    private Vector2 CheckPointAgainstArenaBoundaryAndCorrect(Vector2 testPoint)
    {
        Vector2 outputPoint;
        if (testPoint.magnitude >= _levelController.ArenaRadius * .95f)
        {
            Debug.Log("correcting a nav point outside of 95% arena radius");
            outputPoint = transform.position + (transform.up * -1 * _range);
        }
        else
        {
            outputPoint = testPoint;
        }
        return outputPoint;
    }

    private int GetCardinalDirection()
    {
        float angleFromNorth = Vector3.SignedAngle(transform.up, Vector3.up, Vector3.forward);
        int cardinalDirection = 0;
        if (Mathf.Abs(angleFromNorth) <= 45f)
        {
            cardinalDirection = 1;
        }
        if (angleFromNorth > 45f && angleFromNorth <= 135f)
        {
            cardinalDirection = 2;
        }
        if (Mathf.Abs(angleFromNorth) > 135f)
        {
            cardinalDirection = 3;
        }
        if (angleFromNorth < -45f && angleFromNorth >= -135f)
        {
            cardinalDirection = 4;
        }

        return cardinalDirection;
    }

    private Vector2 FindRandomCloseDependentMove()
    {
        Vector2 pos = CUR.FindRandomPositionWithinRangeBandAndWithinArena(
            _dependentTransform.position, 0, _range,
            Vector2.zero, _levelController.ArenaRadius);

        pos += _dependentTransform.GetComponent<Rigidbody2D>().velocity/3f;

        return pos;
    }

}
