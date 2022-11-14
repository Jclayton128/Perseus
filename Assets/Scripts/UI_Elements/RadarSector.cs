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
    [SerializeField] Color _radarGreen = new Color(0.2195399f, 0.95f, .2134212f);
    Color _radarYellow = Color.yellow;
    Color _radarRed = Color.red;

    //state
    private float _currentIntensity = 0;

    //hood
    //float _intensityActual;
    //float _intensityTarget;
    //float _fadeRate;
    //float _riseRate;

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
   
    private void IlluminateDotsBasedOnIntensity()
    {
        //float alpha_0 = (_currentIntensity - 0) / .2f;
        //float alpha_1 = (_currentIntensity - .20f) / .2f;
        //float alpha_2 = (_currentIntensity - .4f) / .2f;
        //float alpha_3 = (_currentIntensity - .6f) / .2f;
        //float alpha_4 = (_currentIntensity - .8f) / .2f;
        //dotLevels[0].color = new Color(_radarGreen.r, _radarGreen.g, _radarGreen.b, alpha_0);
        //dotLevels[1].color = new Color(_radarGreen.r, _radarGreen.g, _radarGreen.b, alpha_1);
        //dotLevels[2].color = new Color(_radarGreen.r, _radarGreen.g, _radarGreen.b, alpha_2);
        //dotLevels[3].color = new Color(_radarYellow.r, _radarYellow.g, _radarYellow.b, alpha_3);
        //dotLevels[4].color = new Color(_radarRed.r, _radarRed.g, _radarRed.b, alpha_4);

        float portion = (float)(1f / dotLevels.Length);
        for (int i = 0; i < dotLevels.Length; i++)
        {            
            float alpha = (_currentIntensity - (i * portion)) / portion;
            _radarGreen.a = alpha;
            dotLevels[i].color = _radarGreen;
        }
    
    
    }

    public void SetIntensityLevel(float value)
    {
        _currentIntensity = value;
        IlluminateDotsBasedOnIntensity();
    }
}
