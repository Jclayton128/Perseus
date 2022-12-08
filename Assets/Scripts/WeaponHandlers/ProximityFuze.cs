using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ProximityFuze : MonoBehaviour
{
    [SerializeField] bool _targetsPlayer = false;
    [SerializeField] bool _targetsEnemy = false;
    [SerializeField] bool _targetsNeutral = false;

    private void Start()
    {
        if (GetComponentInParent<IProximityFuzed>() == null)
        {
            Debug.Log($"This ({transform.parent.name}) doesn't implement IProximityFuzed but has a proximity fuze");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int tgt = collision.gameObject.layer;

        if (_targetsPlayer && tgt == LayerLibrary.PlayerLayer)
        {
            TriggerProximityFuze(); 
            return;
        }
        if (_targetsEnemy &&
            (tgt == LayerLibrary.EnemyLayer || tgt == LayerLibrary.SpecialEnemyLayer))
        {
            TriggerProximityFuze();
            return;
        }
        if (_targetsNeutral && tgt == LayerLibrary.NeutralLayer)
        {
            TriggerProximityFuze();
            return;
        }
    }

    private void TriggerProximityFuze()
    {
        SendMessageUpwards("DetonateViaProximityFuze", SendMessageOptions.DontRequireReceiver);
    }

}
