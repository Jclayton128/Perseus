using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WormholeHandler : MonoBehaviour
{
    [SerializeField] float _pullStrength = 2.0f;

    public Action<WormholeHandler> OnPlayerEnterWormhole; //Level Controller should hook into these
    public Action<WormholeHandler> OnPlayerExitWormhole;    

    //state
    Rigidbody2D _playerRB;
    Vector2 _inwardDir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            OnPlayerEnterWormhole?.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _playerRB = null;

            OnPlayerExitWormhole?.Invoke(this);
        }
    }

    private void Update()
    {
        if (_playerRB)
        {
            _inwardDir = (transform.position - _playerRB.transform.position);
            _playerRB.AddForce(_inwardDir.normalized * _pullStrength);
        }
    }




}
