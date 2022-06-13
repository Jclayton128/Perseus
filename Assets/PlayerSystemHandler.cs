using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    SystemsLibrary _syslib;
    [SerializeField] SystemsLibrary.SystemType[] _startingSystems = null;
    PlayerMovementHandler _movementHandler;

    private void Awake()
    {
        _syslib = FindObjectOfType<SystemsLibrary>();
        _movementHandler = GetComponent<PlayerMovementHandler>();
        LoadStartingSystems();
    }

    private void LoadStartingSystems()
    {
        foreach (var system in _startingSystems)
        {
            GainSystem(_syslib.GetSystem(system));
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        SystemCrateHandler sch;
        if (collision.gameObject.TryGetComponent<SystemCrateHandler>(out sch))
        {
            GainSystem(sch.SystemChunk);
            Destroy(collision.gameObject);
        }

    }

    private void GainSystem(GameObject newSystem)
    {
        GameObject go = Instantiate<GameObject>(newSystem, this.transform);
        SystemHandler sh = newSystem.GetComponent<SystemHandler>();
        sh.IntegrateSystem(_movementHandler);
        go.transform.localPosition = sh.LocalPosition;
    }
}
