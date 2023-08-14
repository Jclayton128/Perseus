using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSpriteHandler : MonoBehaviour
{
    [SerializeField] float _timeToGo = 5f;
    [SerializeField] float _yTravel = 7.5f;

    float _timeLeft;
    float _factor;
    Vector3 _startScale;
    float _yStep;
    float _startingEmitRate;

    [SerializeField] ParticleSystem _ps = null;
    ParticleSystem.EmissionModule _psem;
    ParticleSystem.MinMaxCurve _mmc;

    private void Start()
    {
        _timeLeft = _timeToGo;
        _factor = 1;
        _startScale = transform.localScale;
        _yStep = _yTravel / _timeToGo;
        _psem = _ps.emission;

    }

    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        _factor = _timeLeft/ _timeToGo;
        transform.localScale = _startScale * _factor;
        transform.position += Vector3.up * _yStep * Time.deltaTime;
        if (_timeLeft <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
