using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera _cvc;

    //settings
    [SerializeField] float _deathZoomZoominTime = 1.0f;
    [SerializeField] float _deathZoomZoomedInFOV = 80f;
    const float _startingFOV = 100f;

    //state
    Tween _zoomTween;

    private void Awake()
    {
        _cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void FocusCameraOnTarget(Transform target)
    {
        if (target == null)
        {
            _cvc.Follow = this.transform;
        }
        else
        {
            _cvc.Follow = target;
        }

    }

    public void ModifyCameraFOV(float FOVtoAdd)
    {
        _zoomTween.Kill();
        _cvc.m_Lens.FieldOfView += FOVtoAdd;
    }

    public void FocusCameraOnPlayerDeathZoom(float dwellTime)
    {
        //FocusCameraOnTarget(null);
        _zoomTween.Kill();
        _zoomTween =
            DOTween.To(() => _cvc.m_Lens.FieldOfView, x => _cvc.m_Lens.FieldOfView = x,
            _deathZoomZoomedInFOV,
            _deathZoomZoominTime).SetUpdate(true);

    }

    public void ResetZoomToStarting()
    {
        _zoomTween.Kill();
        _cvc.m_Lens.FieldOfView = _startingFOV;
    }

    //private void Start()
    //{        
    //    Vector2 resTarget = new Vector2(960f, 960f);
    //    Vector2 resViewport = new Vector2(Screen.width, Screen.height);
    //    Vector2 resNormalized = resTarget / resViewport; // target res in viewport space
    //    Vector2 size = resNormalized / Mathf.Max(resNormalized.x, resNormalized.y);
    //    Camera.main.rect = new Rect(default, size) { center = new Vector2(0.5f, 0.5f) };
        
    //}

}
