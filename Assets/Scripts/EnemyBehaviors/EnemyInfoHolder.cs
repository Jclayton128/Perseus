using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyInfoHolder : MonoBehaviour
{
    public enum EnemyType { Unassigned0, Trundler1, Warper2, Hammer3, Maker4, Mite5,
        Stalker6, Fencer7, Rocker8, Scrapper9}


    //state

    [Tooltip("Threat Score is used by the Enemy Factory to determine how many enemies to create upon jumping to a new sector.")]
    [Range(0, 10)]
    [ShowInInspector] readonly public int ThreatScore = 1;

    [ShowInInspector] readonly public EnemyType EType = EnemyType.Unassigned0;
    [ShowInInspector] readonly public bool LivesAmongAsteroidsOnly = false;
    [ShowInInspector] readonly public bool LivesInNebulaOnly = false;

}
