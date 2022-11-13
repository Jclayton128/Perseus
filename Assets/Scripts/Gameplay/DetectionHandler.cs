using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DetectionHandler : MonoBehaviour
{
    public Action<Vector3, Vector3> PlayerTransformLocated;

    MindsetHandler _mindsetHandler;
    IPlayerSeeking _playerSeeker;
    CircleCollider2D _circleCollider;

    //state
    Rigidbody2D _playerRB;
    
    private void Awake()
    {
        _playerSeeker = GetComponentInParent<IPlayerSeeking>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            _playerRB = collision.GetComponentInParent<Rigidbody2D>();
            PlayerTransformLocated?.Invoke(_playerRB.position, _playerRB.velocity);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            PlayerTransformLocated?.Invoke(_playerRB.position, _playerRB.velocity);
            _playerRB = null;

        }
    }

    private void Update()
    {
        if (_playerRB != null)
        {
            _playerSeeker.ReportPlayer(_playerRB.position,_playerRB.velocity);

        }

    }

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
