using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSystem : MonoBehaviour
{
    protected PlayerHandler _ph;
    public InputController _inputCon;
    protected PoolController _poolCon;
    protected SystemHandler _systemHandler;

    [SerializeField] protected PoolController.WeaponType _weaponType;
    [SerializeField] protected  float _activationCost = 0;
    [SerializeField] protected float _sustainCostRate = 0;

    protected bool _isInstalled;


    public virtual void Initialize()
    {
        //Debug.Log("initializing");
        _isInstalled = true;
        _inputCon = FindObjectOfType<InputController>();
        _poolCon = _inputCon.GetComponent<PoolController>();

        _systemHandler = GetComponent<SystemHandler>();
        
    }

    public abstract void Activate();
    public abstract void Deactivate();

}
