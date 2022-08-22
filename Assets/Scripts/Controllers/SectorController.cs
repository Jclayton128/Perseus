using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SectorController : MonoBehaviour
{
    /// This keeps track of the current level, the current level budget,
    /// the current level's asteroid level, and the current level's status as a nebula
    /// 

    public enum AsteroidAmounts { None, Sparse, Medium, Heavy};
    public enum NebulaAmounts { None, Sparse, Medium, Heavy};

    [ShowInInspector] public int CurrentLevel { get; private set; } = 0;
    [ShowInInspector] public int CurrentThreatBudget { get; private set; } = 10;
    [ShowInInspector] public AsteroidAmounts CurrentAsteroidAmount { get; private set; } = AsteroidAmounts.None;
    [ShowInInspector] public NebulaAmounts CurrentNebulaAmount { get; private set; } = NebulaAmounts.None;
}
