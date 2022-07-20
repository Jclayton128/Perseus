using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RadarProfileHandler : MonoBehaviour
{
    float _highLevel = 10f;
    float _lowLevel = 1f;
    float _timeForRise = 10f;

    public float RadarProfile = 1;
    bool _isRising = false;

    private void Start()
    {
        //DOTween.To(() => RadarProfile, x => RadarProfile = x, _highLevel, _timeForRise);
        _isRising = true;
    }

    private void Update()
    {
       //if (RadarProfile >= _highLevel && _isRising)
       // {
       //     DOTween.To(() => RadarProfile, x => RadarProfile = x, _lowLevel, _timeForRise);
       //     _isRising = false;
       //     return;
       // }
       //if (RadarProfile <= _lowLevel && !_isRising)
       // {
       //     DOTween.To(() => RadarProfile, x => RadarProfile = x, _highLevel, _timeForRise);
       //     _isRising = true;
       //     return;
       // }
    }

}
