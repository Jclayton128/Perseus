using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ProximityFuze : MonoBehaviour
{
    [SerializeField] bool _targetsPlayer = false;
    [SerializeField] bool _targetsEnemy = false;
    [SerializeField] bool _targetsNeutral = false;

    int _playerLayer = 7;
    int _enemyLayer = 9;
    int _neutralLayer = 11;
    int _layerMask;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        int tgt = collision.gameObject.layer;

        if (_targetsPlayer && tgt == _playerLayer)
        {
            TriggerProximityFuze(); 
            return;
        }
        if (_targetsEnemy && tgt == _enemyLayer)
        {
            TriggerProximityFuze();
            return;
        }
        if (_targetsNeutral && tgt == _neutralLayer)
        {
            TriggerProximityFuze();
            return;
        }
    }

    private void TriggerProximityFuze()
    {
        Debug.Log("detonate via proximity fuze");
        SendMessageUpwards("DetonateViaProximityFuze", SendMessageOptions.RequireReceiver);
    }


}
