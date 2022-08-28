using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdjustableImageBar : MonoBehaviour
{
    [SerializeField] Image _foregroundImageBar = null;
    [SerializeField] Image _backgroundImageBar = null;
    Tween _backgroundTween;
    Tween _memoryTween;
    
    //settings
    [SerializeField] float _minFactor = 0f;
    [SerializeField] float _maxFactor = 1f;
    [SerializeField] float _backgroundFadeoutTime;

    [Tooltip("This is how long the background fill amount should remain static despite" +
        "receiving multiple foreground fill updates")]
    [SerializeField] float _memoryTimeframe = 2f;

    //state
    Color _backgroundStartingColor;
    Color _foregroundStartingColor;
    float _memoryEndtime = 0;

    private void Start()
    {
        _foregroundStartingColor = _foregroundImageBar.color;
        _backgroundStartingColor = _backgroundImageBar.color;
        _backgroundImageBar.fillAmount = 1;
        SetFactor(1);
    }

    public void SetFactor(float newRawForegroundFactor)
    {
        float newCorrectedForegroundFactor = Mathf.Lerp(_minFactor, _maxFactor, newRawForegroundFactor);

        if (newCorrectedForegroundFactor > 1f || newCorrectedForegroundFactor < 0)
        {
            return;
        }

        if (!_foregroundImageBar)
        {
            Debug.LogError("Must have a foreground Image Bar!");
            return;
        }

        // Factor getting lower
        if (newCorrectedForegroundFactor < _foregroundImageBar.fillAmount)
        {
            //if outside of memory timeframe, update background fill amount
            if (Time.time >= _memoryEndtime)  
            {
                _backgroundImageBar.fillAmount = _foregroundImageBar.fillAmount;
            }
            _memoryEndtime = Time.time + _memoryTimeframe;

            _foregroundImageBar.fillAmount = newCorrectedForegroundFactor;
            float newAmount = _foregroundImageBar.fillAmount;

            if (_backgroundImageBar)
            {
                _backgroundTween.Kill();
                float oldAmount = _backgroundImageBar.fillAmount;
                _backgroundImageBar.color = _backgroundStartingColor;

                _backgroundTween = _backgroundImageBar.DOColor(Color.clear,
                    _backgroundFadeoutTime);
            }

        } 

        else // Factor getting higher
        {
            _foregroundImageBar.fillAmount = Mathf.Lerp(_minFactor, _maxFactor, newRawForegroundFactor);
            //_backgroundImageBar.fillAmount = _foregroundImageBar.fillAmount;
            //_backgroundTween.Kill();
            //_backgroundImageBar.color = _backgroundStartingColor;
        }
        
    }



}
