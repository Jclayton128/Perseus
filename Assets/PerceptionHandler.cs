using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionHandler : MonoBehaviour
{
    MindsetHandler _mindsetHandler;

    //state
    [SerializeField] Rigidbody2D _playerRB;
    
    private void Awake()
    {
        _mindsetHandler = GetComponentInParent<MindsetHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            Debug.Log("Found player!");
            _playerRB = collision.GetComponentInParent<Rigidbody2D>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            Debug.Log("Lost player!");
            _playerRB = null;
        }
    }

    private void Update()
    {
        if (_playerRB)
        {
            _mindsetHandler.SetPlayerPositionOnPlayerSighting(_playerRB.position,
                 _playerRB.velocity);
        }

    }
}
