using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyComponentsAnimator : MonoBehaviour
{
    WeaponHandler[] _weaponHandlers;
    DetectionHandler _detectionHandler;

    //settings
    [Tooltip("These are the components of the ship that can extend or retract.")]
    [SerializeField] GameObject[] _deployableGameObjects = null;
    [Tooltip("These are positions each component should get to once extended/deployed")]
    [SerializeField] Vector2[] _deployedPositions;
    [SerializeField] float _deployRetractTime = 1.0f;
    [SerializeField] float _playerDistanceExtendRetractThreshold = 7f;

    //state
    float _deployedDist = 0;
    bool _areComponentsDeployed = false;
    Tween[] _deployTweens;
    Vector2[] _retractedPositions;
    float _distToPlayer = Mathf.Infinity;



    private void Awake()
    {
        _weaponHandlers = GetComponentsInChildren<WeaponHandler>();
        _retractedPositions = new Vector2[_deployableGameObjects.Length];

        for (int i = 0; i < _deployableGameObjects.Length; i++)
        {
            _retractedPositions[i] = _deployableGameObjects[i].transform.localPosition;
        }

        _deployTweens = new Tween[_deployableGameObjects.Length];
        _detectionHandler = GetComponentInChildren<DetectionHandler>();
        _detectionHandler.PlayerDistanceUpdated += HandlePlayerDistanceUpdated;
        _detectionHandler.PlayerTransformLost += HandlePlayerLost;
    }

    private void Start()
    {
        Retract();
    }


    public void Deploy()
    {
        for (int i =0; i < _deployableGameObjects.Length; i++)
        {
            _deployTweens[i].Kill();
            _deployTweens[i] = _deployableGameObjects[i].transform.DOLocalMove(_deployedPositions[i], _deployRetractTime);
            
        }

        Invoke(nameof(MarkAsDeployComplete), _deployRetractTime);
    }

    private void MarkAsDeployComplete()
    {
        _areComponentsDeployed = true;
        foreach (var wh in _weaponHandlers)
        {
            wh.enabled = (_areComponentsDeployed);
        }
    }


    public void Retract()
    {
        MarkAsRetractComplete();
        for (int i = 0; i < _deployableGameObjects.Length; i++)
        {
            _deployTweens[i].Kill();
            _deployTweens[i] = _deployableGameObjects[i].transform.DOLocalMove(_retractedPositions[i], _deployRetractTime);
        }
        //Invoke(nameof(MarkAsRetractComplete), _deployRetractTime);
    }

    private void MarkAsRetractComplete()
    {
        _areComponentsDeployed = false;
        foreach (var wh in _weaponHandlers)
        {
            wh.enabled = (_areComponentsDeployed);
        }
    }

    //private void Update()
    //{
    //    _deployedDist =
    //        ((Vector2)_deployableGameObjects[0].transform.position - _retractedPositions[0]).magnitude;
    //    if (_areComponentsDeployed && _deployedDist <= Mathf.Epsilon)
    //    {
    //        _areComponentsDeployed = false;
    //        _weaponHandler.gameObject.SetActive(_areComponentsDeployed);
    //    }

    //    if (!_areComponentsDeployed && _deployedDist > Mathf.Epsilon)
    //    {
    //        _areComponentsDeployed = true;
    //        _weaponHandler.gameObject.SetActive(_areComponentsDeployed);
    //    }
    //}

    private void HandlePlayerDistanceUpdated(float dist)
    {
        _distToPlayer = dist;
        if (!_areComponentsDeployed && _distToPlayer <= _playerDistanceExtendRetractThreshold)
        {
            Deploy();
        }
        if (_areComponentsDeployed && _distToPlayer > _playerDistanceExtendRetractThreshold)
        {
            Retract();
        }
    }

    private void HandlePlayerLost(Vector3 throwaway, Vector3 throwaway2)
    {
        Retract();
    }



    [ContextMenu("deploy")]
    private void CheckDeploy()
    {
        Awake();
        for (int i = 0; i < _deployableGameObjects.Length; i++)
        {
           _deployableGameObjects[i].transform.localPosition = _deployedPositions[i];
        }
    }



    [ContextMenu("retract")]
    private void CheckRetract()
    {
        Awake();
        for (int i = 0; i < _deployableGameObjects.Length; i++)
        {
            _deployableGameObjects[i].transform.localPosition = Vector3.zero;
        }
    }
}
