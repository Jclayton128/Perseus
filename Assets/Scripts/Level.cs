using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Level")]
public class Level : ScriptableObject
{
    public string LevelName = "default level name";

    public Sprite Icon = null;

    public LevelController.AsteroidAmounts AsteroidAmount =
        LevelController.AsteroidAmounts.None;

    public LevelController.NebulaAmounts NebulaAmount =
        LevelController.NebulaAmounts.None;

    [SerializeField] private List<ShipInfoHolder.ShipType> _possibleEnemies;
    public List<ShipInfoHolder.ShipType> PossibleEnemies => _possibleEnemies;

}
