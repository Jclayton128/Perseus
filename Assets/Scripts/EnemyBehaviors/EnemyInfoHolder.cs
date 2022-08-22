using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyInfoHolder : MonoBehaviour
{
    [Tooltip("Threat Score is used by the Enemy Factory to determine how many enemies to create upon jumping to a new sector.")]
    [Range(0,10)]
    [ShowInInspector] public int ThreatScore = 1;

    [ShowInInspector] public bool LivesAmongAsteroidsOnly = false;
    [ShowInInspector] public bool LivesInNebulaOnly = false;
}
