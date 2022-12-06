using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualCloakingHandler : MonoBehaviour
{
    SpriteRenderer _sr;
    DetectionHandler _detectionHandler;

    //settings
    [SerializeField] float _cloakDecloakThreshold = 7f;
    [SerializeField] float _cloakTime = 1f;
    float _distanceToPlayer = 100f;
    bool _isSupposedToBeCloaked = false;
    Tween _cloakColorTween;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _detectionHandler = GetComponentInChildren<DetectionHandler>();
        _detectionHandler.PlayerDistanceUpdated += HandlePlayerDistanceUpdate;
        _detectionHandler.PlayerPosVelLost += HandlePlayerTransformLost;
    }

    private void HandlePlayerDistanceUpdate(float dist)
    {
        if (dist > _cloakDecloakThreshold)
        {
            Cloak();
            _isSupposedToBeCloaked = true;
        }
        else
        {

            Decloak();
            _isSupposedToBeCloaked = false;
        }
    }

    private void HandlePlayerTransformLost(Vector3 trash, Vector3 trash2)
    {
        Cloak();
    }

    private void Cloak()
    {
        if (_isSupposedToBeCloaked) return;

        _cloakColorTween.Kill();
        _cloakColorTween = _sr.DOColor(Color.black, _cloakTime); //_sr.DOFade(0, _cloakTime);
    }

    private void Decloak()
    {
        if (!_isSupposedToBeCloaked) return;

        _cloakColorTween.Kill();
        _cloakColorTween = _sr.DOColor(Color.white, _cloakTime);//_sr.DOFade(1, _cloakTime);
    }




}
