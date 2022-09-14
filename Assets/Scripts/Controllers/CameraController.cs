using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera _cvc;

    private void Awake()
    {
        _cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void FocusCameraOnTarget(Transform target)
    {
        _cvc.Follow = target;
    }

    public void ModifyCameraFOV(float FOVtoAdd)
    {
        _cvc.m_Lens.FieldOfView += FOVtoAdd;
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
