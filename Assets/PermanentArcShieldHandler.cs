using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentArcShieldHandler : ArcShieldHandler
{

    [SerializeField] ParticleSystem[] _arcShieldParticles_Local = null;
    [SerializeField] ParticleSystem[] _arcShieldParticles_World = null;

    //settings
    [SerializeField] float _normalDamage = 1;
    [SerializeField] float _shieldBonusDamage = 2;
    [SerializeField] float _ionDamage = 0;
    [SerializeField] float _knockbackAmount = 1;



    private void Awake()
    {
        _isOn = true;
        _damagePack = new DamagePack(_normalDamage, _shieldBonusDamage, _ionDamage, _knockbackAmount, 0);
    }

    private void Update()
    {
        if (_isOn)
        {
            foreach (var ps in _arcShieldParticles_Local)
            {
                ps.Emit(2);
            }
            foreach (var pw in _arcShieldParticles_World)
            {
                pw.Emit(1);
            }
        }
    }

}
