using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShipInfoHolder : MonoBehaviour, IScannable
{
    public enum ShipType { Unassigned0, Dummy1, Warper2, Hammer3, Maker4, Mite5,
        Stalker6, Fencer7, Rocker8, Scrapper9, Trundler10 }


    //state

    [Tooltip("Threat Score is used by the Enemy Factory to determine how many enemies to create upon jumping to a new sector.")]
    [Range(0, 10)]
    [ShowInInspector] private int _threatScore = 1;
    public int ThreatScore => _threatScore;

    [ShowInInspector] public ShipType EType = ShipType.Unassigned0;
    [ShowInInspector] public bool LivesAmongAsteroidsOnly = false;
    [ShowInInspector] public bool LivesInNebulaOnly = false;
    [ShowInInspector] public string ScannerName = "default name";


    public string GetScanName()
    {
        return ScannerName;
    }

    public Sprite GetScanIcon()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public Transform GetScanTransform()
    {
        return transform;
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
        
    }
}
