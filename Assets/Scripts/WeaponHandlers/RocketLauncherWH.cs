using UnityEngine;

public class RocketLauncherWH : WeaponHandler, IMissileLauncher
{
    //settings
    [Header("Base Rocket Parameters")]
    [SerializeField] float _missileTurnRate = 40f;
    [SerializeField] float _snakeAmount = 30f;
    [SerializeField] int _rocketCount = 2;
    [SerializeField] float _maxMissRange = 2f;
    [SerializeField] float _degreeSpread = 30f;

    [Header("Upgrade Parameters")]
    [SerializeField] int _rocketCount_Upgrade = 2;
    [SerializeField] float _rocketSpeed_Upgrade = 20f;

    int _legalTarget_layerMask;

    #region Weapon Handler
    public override object GetUIStatus()
    {
        return null;
    }

    public override Vector3 GetInitialProjectileVelocity(Transform projectileTransform)
    {
        return (projectileTransform.transform.up * _projectileSpeed);
    }

    protected override void ActivateInternal()
    {
        if (_hostEnergyHandler.CheckEnergy(_activationCost))
        {
            _hostEnergyHandler.SpendEnergy(_activationCost);
            Fire();
        }
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        //none
    }

    private void Fire()
    {
        float spreadSubdivided = _degreeSpread / _rocketCount;

        _projectileLifetime = (_inputCon.MousePos - transform.position).magnitude / _projectileSpeed;

        for (int i = 0; i < _rocketCount; i++)
        {
            Quaternion sector = Quaternion.Euler(0, 0,
                (i * spreadSubdivided) - (_degreeSpread / 2f) + transform.eulerAngles.z);
            Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);

            pb.transform.rotation = sector;

            pb.SetupInstance(this);
            
        }

        if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());
    }

    protected override void ImplementWeaponUpgrade()
    {
        _rocketCount += _rocketCount_Upgrade;
        _projectileSpeed += _rocketSpeed_Upgrade;
    }

    protected override void InitializeWeaponSpecifics()
    {
        // Rockets don't scan, and therefore the mask is irrelevant.
        _legalTarget_layerMask = 1;
    }


    #endregion

    #region Interface

    public Transform GetTargetTransform()
    {
        return null;
    }

    public int GetLegalTargetsLayerMask()
    {
        return _legalTarget_layerMask;
    }

    public float GetMissileScanRadius()
    {
        return 0;
    }

    public float GetSnakeAmount()
    {
        return _snakeAmount;
    }

    public float GetSpeedSpec()
    {
        return _projectileSpeed;
    }

    public Vector3 GetTargetPosition()
    {
        Vector3 pos = CUR.FindRandomPointWithinDistance(_inputCon.MousePos,_maxMissRange);
        return pos;
    }

    public float GetTurnSpec()
    {
        return _missileTurnRate;
    }

    

    #endregion

}
