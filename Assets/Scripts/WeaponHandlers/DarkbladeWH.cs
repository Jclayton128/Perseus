using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkbladeWH : WeaponHandler
{
    [SerializeField] ParticleSystem _beamFX = null;

    //settings
    [SerializeField] float _beamLength = 3f;
    [SerializeField] float _beamLengthIncrease_Upgrade = 0.5f;
    [SerializeField] float _beamDamageIncrease_Upgrade = 5f;
    [SerializeField] float _beamWidth = 0.4f;

    //state
    Vector3 _dir;
    float _effectiveRange;
    int _layerMask = 0;


    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost))
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            FireBeam();

            if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomActivationClip());
            else _hostAudioSource.PlayOneShot(GetRandomActivationClip());
        }
    }

    private void FireBeam()
    {
        _dir = _muzzle.transform.up * _beamLength;
        RaycastHit2D rh2d = Physics2D.CircleCast(
            _muzzle.position,
            _beamWidth,
            _dir,
            _beamLength,
            _layerMask) ;
        if (rh2d.collider != null)
        {            
            _effectiveRange = rh2d.distance;

            HealthHandler targetHealthHandler;
            if (rh2d.collider.TryGetComponent<HealthHandler>(out targetHealthHandler))
            {
                float diff = Mathf.Abs(Quaternion.Angle(_muzzle.rotation, rh2d.transform.rotation));
                float damageAngleCoefficient = 1.5f - (diff / 180);

                //Debug.Log($"damage coefficient: {damageAngleCoefficient}");
                DamagePack damagePack = new DamagePack(_normalDamage * damageAngleCoefficient,
                _shieldBonusDamage, _ionDamage, _knockBackAmount,_scrapBonus);

                targetHealthHandler?.ReceiveNonProjectileDamage(damagePack, rh2d.point, _dir);
            }
        }
        else
        {
            //Debug.DrawLine(_turretMuzzle.position, _turretMuzzle.position + dir, Color.blue, 0.1f);
            _effectiveRange = _beamLength;
        }


        Vector3 pos = _muzzle.position + (_muzzle.transform.up * _effectiveRange / 2f);
        Quaternion rot = _muzzle.rotation;
        ParticleSystem ps = Instantiate(_beamFX, pos, rot).GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.radius = _effectiveRange / 2f;

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        //nothing. Fires on MB down.
    }

    public override object GetUIStatus()
    {
        return null;
    }

    protected override void ImplementWeaponUpgrade()
    {
        _normalDamage += _beamDamageIncrease_Upgrade;
        _beamLength += _beamLengthIncrease_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        //none needed
        if (_isPlayer) _layerMask = LayerLibrary.EnemyNeutralLayerMask;
        else _layerMask = LayerLibrary.PlayerLayerMask;
    }
}
