using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyComponentsAnimator : MonoBehaviour
{
    WeaponHandler _weaponHandler;

    //settings
    [Tooltip("These are the components of the ship that can extend or retract.")]
    [SerializeField] GameObject[] _deployableGameObjects = null;
    [Tooltip("These are positions each component should get to once extended/deployed")]
    [SerializeField] Vector2[] _deployedPositions;
    [SerializeField] float _extendRetractTime = 1.0f;


    //state
    float _deployedDist = 0;
    bool _areComponentsDeployed = false;
    Tween[] _extensionTweens;
    Vector2[] _retractedPositions;



    private void Awake()
    {
        _weaponHandler = GetComponentInChildren<WeaponHandler>();
        _weaponHandler.gameObject.SetActive(_areComponentsDeployed);
        _retractedPositions = new Vector2[_deployableGameObjects.Length];

        for (int i = 0; i < _deployableGameObjects.Length; i++)
        {
            _retractedPositions[i] = _deployableGameObjects[i].transform.localPosition;
        }

        _extensionTweens = new Tween[_deployableGameObjects.Length];
    }

    [ContextMenu("extend")]
    public void Extend()
    {
        for (int i =0; i < _deployableGameObjects.Length; i++)
        {
            _extensionTweens[i].Kill();
            _extensionTweens[i] = _deployableGameObjects[i].transform.DOLocalMove(_deployedPositions[i], _extendRetractTime);
        }
    }

    [ContextMenu("retract")]
    public void Retract()
    {
        for (int i = 0; i < _deployableGameObjects.Length; i++)
        {
            _extensionTweens[i].Kill();
            _extensionTweens[i] = _deployableGameObjects[i].transform.DOLocalMove(_retractedPositions[i], _extendRetractTime);
        }
    }

    private void Update()
    {
        _deployedDist =
            ((Vector2)_deployableGameObjects[0].transform.position - _retractedPositions[0]).magnitude;
        if (_areComponentsDeployed && _deployedDist <= Mathf.Epsilon)
        {
            _areComponentsDeployed = false;
        }

        if (!_areComponentsDeployed && _deployedDist > Mathf.Epsilon)
        {
            _areComponentsDeployed = true;
        }
    }
}
