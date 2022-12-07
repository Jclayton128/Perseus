using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSteerer : MonoBehaviour, ITurret
{
    InputController _ic;
    //settings
    [SerializeField] float _turretTurnRate = 50f;

    [Tooltip("This is the maximum amount of degrees injected into a set lookangle")]
    [SerializeField] float _inaccuracyAmount = 0f;

    //state
    float _lookAngle = 0;

  
    private void Update()
    {
        UpdateTurretFacingToLookAngle();
    }

    private void UpdateTurretFacingToLookAngle()
    {
        if (!_ic) _ic = FindObjectOfType<InputController>();    
        //Vector3 targetDir = _inputCon.LookDirection;
        //float angleToTargetFromNorth = Vector3.SignedAngle(targetDir, Vector2.up, transform.forward);
        Quaternion angleToPoint = Quaternion.Euler(0, 0, _lookAngle);
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, angleToPoint,
            _turretTurnRate * Time.deltaTime);
    }

    public void SetLookAngle(Vector2 throwaway, float angle)
    {
        _lookAngle = angle;
    }

    public void SetLookAngle(float angle)
    {
        _lookAngle = angle;
    }
}
