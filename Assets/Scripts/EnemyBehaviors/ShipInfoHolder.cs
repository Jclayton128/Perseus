using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShipInfoHolder : MonoBehaviour, IScannable
{
    public enum ShipType { Unassigned0, Dummy1, Warper2, Hammer3, Maker4, Mite5,
        Stalker6, Fencer7, Rocker8, Scrapper9, Trundler10, Stinger11, Shielder12, Grinder13,
    Lamper14, Normal15, Normal16, Normal17, Normal18, Normal19, MedusaBoss20, Hephaestus21, Boss22,
    Boss23, Boss24, Boss25, Boss26, Boss27, Boss28, Boss29}


    //state

    [Tooltip("Threat Score is used by the Enemy Factory to determine how many enemies to create upon jumping to a new sector.")]
    [Range(0, 10)]
    [SerializeField] private int _threatScore;
    public int ThreatScore => _threatScore;

    public bool IsInstallable { get; } = false;

    [ShowInInspector] public ShipType EType = ShipType.Unassigned0;
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
