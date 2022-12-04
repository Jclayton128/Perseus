using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectionHandler : MonoBehaviour
{
    /// <summary>
    /// 1st payload is player location, 2nd payload is player velocity
    /// </summary>
    public Action<Vector3, Vector3> PlayerTransformUpdated;
    public Action<float> PlayerDistanceUpdated;
    public Action<Vector3, Vector3> PlayerTransformLost;

    MindsetHandler _mindsetHandler;
    //IPlayerSeeking _playerSeeker;
    CircleCollider2D _circleCollider;

    //state
    Rigidbody2D _playerRB;
    float _distToPlayer = Mathf.Infinity;
    
    private void Awake()
    {
        //_playerSeeker = GetComponentInParent<IPlayerSeeking>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            _playerRB = collision.GetComponentInParent<Rigidbody2D>();
            PlayerTransformUpdated?.Invoke(_playerRB.position, _playerRB.velocity);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            PlayerTransformLost?.Invoke(_playerRB.position, _playerRB.velocity);
            _playerRB = null;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            _distToPlayer = (_playerRB.position - (Vector2)transform.position).magnitude;
            PlayerDistanceUpdated?.Invoke(_distToPlayer);
            PlayerTransformUpdated?.Invoke(_playerRB.position, _playerRB.velocity);
        }
    }

    //private void Update()
    //{
    //    if (_playerRB != null)
    //    {
    //        _playerSeeker.ReportPlayer(_playerRB.position,_playerRB.velocity);

    //    }

    //}

    public void ModifyDetectorRange(float newDetectorRange)
    {
        if (newDetectorRange <= 0)
        {
            _circleCollider.enabled = false;
        }
        else
        {
            _circleCollider.enabled = true;
            _circleCollider.radius = newDetectorRange;
        }

    }
}
