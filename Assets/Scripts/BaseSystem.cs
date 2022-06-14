using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : MonoBehaviour
{
    protected PlayerHandler _ph;
    protected InputController _inputCon;
    protected PoolController _poolCon;
    protected SystemHandler _systemHandler;

    [SerializeField] protected PoolController.ProjectileType _weaponType;
    [SerializeField] protected  float _activationCost = 0;
    [SerializeField] protected float _sustainCostRate = 0;
    [SerializeField] protected Transform _muzzle;

    public bool _isInstalled;


    public void Initialize()
    {
        _isInstalled = true;
        _inputCon = FindObjectOfType<InputController>();
        _poolCon = _inputCon.GetComponent<PoolController>();
        _systemHandler = GetComponent<SystemHandler>();
        
    }

    public abstract void Activate();
    public abstract void Deactivate();

}
