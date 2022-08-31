using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWH : WeaponHandler
{
    //settings
    [Tooltip("Every shot attempts to spread bullets over this range")]
    [SerializeField] float _degreeSpread = 30f;

    [SerializeField] float _chargeRate = 3.3f; // units per second;
    float _maxCharge = 10f;

    [SerializeField] float _shotLifetime = 0.6f;
    [SerializeField] float _shotSpeed = 10f;

    //state
    Color _chargeColor;
    public float _chargeLevel;
    bool _isFiring;
    float _timeOfNextShot;
    float _timeToToggleModes;

    private void Update()
    {
        _chargeLevel += _chargeRate * Time.deltaTime;
        _chargeLevel = Mathf.Clamp(_chargeLevel, 0, _maxCharge);
        UpdateUI();
    }

    private void UpdateUI()
    {
        _chargeColor = Color.Lerp(Color.red, Color.green, _chargeLevel / _maxCharge);
        _connectedWID?.UpdateUI(_chargeLevel / _maxCharge, _chargeColor);
    }

    protected override void ActivateInternal()
    {
        if (_chargeLevel < 3) return;
        float cost = _chargeLevel * _activationCost;
        if (_hostEnergyHandler.CheckEnergy(cost))
        {
            _hostEnergyHandler.SpendEnergy(cost);
            Fire();
        }
    }

    private void Fire()
    {
        DamagePack dp = new DamagePack(_normalDamage, _shieldBonusDamage, _ionDamage, _knockBackAmount, _scrapBonus);

        int amount = Mathf.RoundToInt(_chargeLevel);

        float spreadSubdivided = _degreeSpread / amount;
        for (int i = 0; i < amount; i++)
        {
            Quaternion sector = Quaternion.Euler(0, 0, 
                (i * spreadSubdivided) - (_degreeSpread / 2f) + transform.eulerAngles.z);
            ProjectileBrain pb = _poolCon.SpawnProjectile(_projectileType, _muzzle);

            float thisLifetime = _shotLifetime * UnityEngine.Random.Range(0.8f, 1.2f);
            pb.SetupBrain(ProjectileBrain.Behaviour.Bolt, ProjectileBrain.Allegiance.Player,
                ProjectileBrain.DeathBehaviour.Fizzle, thisLifetime, -1, dp, Vector3.zero);
            pb.transform.rotation = sector;
            pb.GetComponent<Rigidbody2D>().velocity = pb.transform.up * _shotSpeed;
        }

        if (_isPlayer) _playerAudioSource.PlayGameplayClipForPlayer(GetRandomFireClip());
        else _hostAudioSource.PlayOneShot(GetRandomFireClip());

        _chargeLevel = 0;
        UpdateUI();
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {
        
    }

    public override object GetUIStatus()
    {
        return _chargeLevel;
    }

    protected override void ImplementWeaponUpgrade()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeWeaponSpecifics()
    {
        _chargeLevel = _maxCharge;
        UpdateUI();
    }
}
