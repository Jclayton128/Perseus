using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ProximityFuze : MonoBehaviour
{
    [SerializeField] bool _targetsPlayer = false;
    [SerializeField] bool _targetsEnemy = false;
    [SerializeField] bool _targetsNeutral = false;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        int tgt = collision.gameObject.layer;

        if (_targetsPlayer && tgt == LayerLibrary.PlayerLayer)
        {
            TriggerProximityFuze(); 
            return;
        }
        if (_targetsEnemy && tgt == LayerLibrary.EnemyLayer)
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
        SendMessageUpwards("DetonateViaProximityFuze", SendMessageOptions.RequireReceiver);
    }


}
