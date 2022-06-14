using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretWeaponSystem : BaseSystem
{
    //settings
    [SerializeField] float _turretTurnRate = 50f;


    public override void Activate()
    {
        Debug.Log("pew");
        _poolCon.SpawnProjectile(_weaponType, _muzzle);
    }

    public override void Deactivate()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (!_inputCon) Initialize();
        if (!_isInstalled)
        {
            Debug.Log("not installed");
            return;
        }

        UpdateTurretFacingToMousePos();
    }

    private void UpdateTurretFacingToMousePos()
    {
        Vector3 targetDir = _inputCon.MousePos - transform.position;
        float angleToTargetFromNorth = Vector3.SignedAngle(targetDir, Vector2.up, transform.forward);
        Quaternion angleToPoint = Quaternion.Euler(0, 0, -1 * angleToTargetFromNorth);
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, angleToPoint,
            _turretTurnRate * Time.deltaTime);
    }
}
