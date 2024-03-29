using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcShieldHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem _arcShieldParticle_Local = null;
    [SerializeField] ParticleSystem _arcShieldParticle_World = null;
    Collider2D _arcShieldCollider;

    protected DamagePack _damagePack;

    //state
    protected bool _isOn = false;


    private void Awake()
    {
        _arcShieldCollider = GetComponentInChildren<Collider2D>();
    }

    public void SetDamagePack(DamagePack newDamagePack)
    {
        _damagePack = newDamagePack;
    }

    public void ToggleStatus(bool isOn)
    {
        _isOn = isOn;
        if (!_arcShieldCollider) _arcShieldCollider = GetComponentInChildren<Collider2D>();
        _arcShieldCollider.enabled = _isOn;
    }

    private void Update()
    {
        if (_isOn)
        {
            _arcShieldParticle_Local.Emit(2);
            _arcShieldParticle_World.Emit(1);
        }

    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isOn) return;
        //Debug.Log("shield coll...");
        HealthHandler hh;
        if (collision.gameObject.TryGetComponent<HealthHandler>(out hh))
        {
            //Debug.Log("shield bash!");
            Vector2 dir = collision.transform.position - transform.position;
            //Debug.Log($"Normal: {_damagePack.NormalDamage}. Ion: {_damagePack.IonDamage}." +
            //    $"Knock: {_damagePack.KnockbackAmount}");
            hh.ReceiveNonProjectileDamage(_damagePack, transform.position, dir);
        }
    }



}
