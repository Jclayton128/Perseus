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

}
