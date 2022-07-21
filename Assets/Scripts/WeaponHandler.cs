using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponHandler : MonoBehaviour
{
    [SerializeField] protected PoolController _poolCon;
    protected InputController _inputCon;

    [SerializeField] protected Sprite _icon = null;
    public Library.WeaponType WeaponType;
    [SerializeField] protected ProjectileBrain.PType _projectileType;
    [SerializeField] protected float _activationCost = 0;
    [SerializeField] protected float _sustainCostRate = 0;
    [SerializeField] protected Transform _muzzle;

    public bool IsSecondary = false;

    public virtual void Initialize()
    {
        _inputCon = FindObjectOfType<InputController>();
        _poolCon = _inputCon.GetComponent<PoolController>();
        _muzzle = GetComponentInChildren<MuzzleTag>().transform;
    }
    public Sprite GetIcon()
    {
        if (_icon == null)
        {
            _icon = GetComponent<SpriteRenderer>().sprite;
        }
        return _icon;
    }
    public abstract void Activate();
    public abstract void Deactivate();


}
