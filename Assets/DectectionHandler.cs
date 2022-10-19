using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DectectionHandler : MonoBehaviour
{
    MindsetHandler _mindsetHandler;
    CircleCollider2D _circleCollider;

    //state
    Rigidbody2D _playerRB;
    
    private void Awake()
    {
        _mindsetHandler = GetComponentInParent<MindsetHandler>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            _playerRB = collision.GetComponentInParent<Rigidbody2D>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            _playerRB = null;
        }
    }

    private void Update()
    {
        if (_playerRB != null)
        {
            _mindsetHandler.SetPlayerPositionOnPlayerSighting(_playerRB.position,
                 _playerRB.velocity);
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
