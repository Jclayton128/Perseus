using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapCollector : MonoBehaviour
{
    [SerializeField] CircleCollider2D _hullCollider = null;
    CircleCollider2D _scrapCollider;

    PlayerStateHandler _playerStateHandler;



    private void Awake()
    {
        _playerStateHandler = GetComponentInParent<PlayerStateHandler>();
    }

    private void Start()
    {
        _scrapCollider = GetComponent<CircleCollider2D>();
        _scrapCollider.radius = _hullCollider.radius;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScrapHandler sh;

        if (collision.TryGetComponent<ScrapHandler>(out sh))
        {
            sh.CollectScrap();
            _playerStateHandler.GainScrap();
        }
    }
}
