using UnityEngine;

public class RocketLauncherWH : WeaponHandler, IMissileLauncher
{
    //MindsetHandler _mindsetHandler;

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

    int _legalTarget_layerMask = LayerLibrary.PlayerEnemyNeutralLayerMask;

    #region Weapon Handler
    public override object GetUIStatus()
    {
        return null;
    }

    //public override Vector3 GetInitialProjectileVelocity(Transform projectileTransform)
    //{
    //    return projectileTransform.transform.up + (Vector3)_rb.velocity;
    //}

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

        for (int i = 0; i < _rocketCount; i++)
        {
            float rand = Random.Range(0.9f, 1.1f);
            //_projectileLifetime = rand * (_inputCon._mousePos - transform.position).magnitude / _projectileSpeed;

            Quaternion sector = Quaternion.Euler(0, 0,
                (i * spreadSubdivided) - (_degreeSpread / 2f) + _muzzle.eulerAngles.z);
            Projectile pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);

            pb.transform.rotation = sector;

            pb.SetupInstance(this);
            
        }

        if (_isPlayer) _playerAudioSource.PlayClipAtPlayer(GetRandomFireClip());
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
        float rand = Random.Range(0.8f, 1.2f);
        return _snakeAmount * rand;
    }

    public float GetSpeedSpec()
    {
        float rand = Random.Range(0.8f, 1.2f);
        return _projectileSpeed * rand;
    }

    public Vector3 GetTargetPosition()
    {
        Vector3 pos;

        if (_isPlayer)
        {
            pos = CUR.FindRandomPointWithinDistance(_inputCon.LookDirection, _maxMissRange);
        }
        else
        {
            if (!_mindsetHandler) _mindsetHandler = GetComponentInParent<MindsetHandler>();
            return _mindsetHandler.PlayerPosition;
        }
        
        return pos;
    }

    public float GetTurnSpec()
    {
        return _missileTurnRate;
    }

    

    #endregion

}
