using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarSector : MonoBehaviour
{
    //init
    [SerializeField] Image[] dotLevels = null;

    //param
    Color _radarGreen = new Color(0.2195399f, 0.95f, .2134212f);
    Color _radarYellow = Color.yellow;
    Color _radarRed = Color.red;

    //hood
    float _intensityActual;
    float _intensityTarget;
    float _fadeRate;
    float _riseRate;

    void Awake()
    {
        SetAllDotsToZero();
    }

    private void SetAllDotsToZero()
    {
        foreach (Image dot in dotLevels)
        {
            dot.color = _radarGreen;
        }
    }

    public void SetRates(float newRise, float newFade)
    {
        _riseRate = newRise;
        _fadeRate = newFade;
    }

    void Update()
    {
        DrainActualIntensityToMatchTargetIntensity();
        IlluminateDotsBasedOnIntensity();
    }

    private void DrainActualIntensityToMatchTargetIntensity()
    {
        if (_intensityActual < _intensityTarget)
        {
            _intensityActual = Mathf.MoveTowards(_intensityActual, _intensityTarget, _riseRate * Time.deltaTime);
        }
        if (_intensityActual > _intensityTarget)
        {
            _intensityActual = Mathf.MoveTowards(_intensityActual, _intensityTarget, _fadeRate * Time.deltaTime);
        }


    }

    private void IlluminateDotsBasedOnIntensity()
    {
        float alpha_0 = (_intensityActual - 0) / .2f;
        float alpha_1 = (_intensityActual - .20f) / .2f;
        float alpha_2 = (_intensityActual - .4f) / .2f;
        float alpha_3 = (_intensityActual - .6f) / .2f;
        float alpha_4 = (_intensityActual - .8f) / .2f;
        dotLevels[0].color = new Color(_radarGreen.r, _radarGreen.g, _radarGreen.b, alpha_0);
        dotLevels[1].color = new Color(_radarGreen.r, _radarGreen.g, _radarGreen.b, alpha_1);
        dotLevels[2].color = new Color(_radarGreen.r, _radarGreen.g, _radarGreen.b, alpha_2);
        dotLevels[3].color = new Color(_radarYellow.r, _radarYellow.g, _radarYellow.b, alpha_3);
        dotLevels[4].color = new Color(_radarRed.r, _radarRed.g, _radarRed.b, alpha_4);
    }

    public void SetIntensityLevel(float value)
    {
        _intensityTarget = value;
    }
}
