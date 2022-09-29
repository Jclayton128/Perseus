using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemHandler : MonoBehaviour
{
    public enum ParticleType { Shield, Hull, Blast};
    ParticleController _particleController;

    public ParticleType ThisParticleType = ParticleType.Shield;

    internal void Initalize(ParticleController particleController)
    {
        _particleController = particleController;
    }

    void OnParticleSystemStopped()
    {
        _particleController.ReturnParticle(gameObject.GetComponent<ParticleSystem>());
    }

}
