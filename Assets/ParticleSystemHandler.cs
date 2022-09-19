using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemHandler : MonoBehaviour
{
    ParticleController _particleController;

    internal void Initalize(ParticleController particleController)
    {
        _particleController = particleController;
    }

    void OnParticleSystemStopped()
    {
        _particleController.ReturnParticleSystem(gameObject.GetComponent<ParticleSystem>());
    }

}
