using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WormholeHandler : MonoBehaviour, IScannable
{
    [SerializeField] float _pullStrength = 2.0f;

    public Action<WormholeHandler> OnPlayerEnterWormhole; //Level Controller should hook into these
    public Action<WormholeHandler> OnPlayerExitWormhole;

    public bool IsInstallable { get; } = false;

    //state
    public Level AssociatedLevel { get; private set; }
    Rigidbody2D _playerRB;
    Vector2 _inwardDir;

    public void Initialize(Level associatedLevel)
    {
        AssociatedLevel = associatedLevel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            OnPlayerEnterWormhole?.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _playerRB = null;

            OnPlayerExitWormhole?.Invoke(this);
        }
    }

    private void Update()
    {
        if (_playerRB)
        {
            _inwardDir = (transform.position - _playerRB.transform.position);
            _playerRB.AddForce(_inwardDir.normalized * _pullStrength);
        }
    }

    public string GetScanName()
    {
        return AssociatedLevel.LevelName;
    }

    public Sprite GetScanIcon()
    {
        return AssociatedLevel.Icon;
    }

    public SystemWeaponLibrary.SystemType ScanSystemType()
    {
        return SystemWeaponLibrary.SystemType.None;
    }

    public SystemWeaponLibrary.WeaponType ScanWeaponType()
    {
        return SystemWeaponLibrary.WeaponType.None;
    }

    public void DestroyScannable()
    {
        Debug.Log("cannot destroy this wormhole scannable");
    }

    public Transform GetScanTransform()
    {
        return transform;
    }
}
