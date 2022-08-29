using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //Refs
    EnemyLibrary _enemyLibrary;
    LevelLibrary _levelLibrary;
    CircleEdgeCollider2D _arenaEdgeCollider;
    public enum AsteroidAmounts { None, Sparse, Medium, Heavy };
    public enum NebulaAmounts { None, Sparse, Medium, Heavy };

    //settings 
    int _startingThreatBudget = 2;

    //state
    public Level _currentLevel;
    public int CurrentLevelNumber;
    public int CurrentThreatBudget;

    List<GameObject> _enemiesOnLevel = new List<GameObject>();
    List<GameObject> _asteroidsOnLevel = new List<GameObject>();
    List<GameObject> _nebulaOnLevel = new List<GameObject>();
    GameObject _wormholeOnLevel;

    public float ArenaRadius { get; private set; }


    private void Awake()
    {
        _enemyLibrary = FindObjectOfType<EnemyLibrary>();
        _levelLibrary = FindObjectOfType<LevelLibrary>();

        CurrentThreatBudget = _startingThreatBudget;
        CurrentLevelNumber = 1;
        LoadNewRandomLevel();
    }

    private void LoadNewRandomLevel()
    {
        _currentLevel = _levelLibrary.GetRandomLevel();
        _arenaEdgeCollider = FindObjectOfType<CircleEdgeCollider2D>();
        ArenaRadius = _arenaEdgeCollider.Radius;
        
        // Populate Asteroids and add to list
        // Populate Nebulae and add to list
        // Populate Wormhole and add to reference
        // Populate Enemies according threat budget, add to list
    }


    #region Registers
    private void RegisterEnemy(GameObject enemy)

    {
        if (_enemiesOnLevel.Contains(enemy))
        {
            Debug.Log($"already registered this enemy: {enemy}");
        }
        else
        {
            _enemiesOnLevel.Add(enemy);
        }
    }

    private void RegisterAsteroid(GameObject asteroid)
    {
        if (_asteroidsOnLevel.Contains(asteroid))
        {
            Debug.Log($"already registered this asteroid: {asteroid}");
        }
        else
        {
            _asteroidsOnLevel.Add(asteroid);
        }
    }

    private void RegisterNebule(GameObject nebula)
    {
        if (_nebulaOnLevel.Contains(nebula))
        {
            Debug.Log($"already registered this nebula: {nebula}");
        }
        else
        {
            _nebulaOnLevel.Add(nebula);
        }
    }

    #endregion

    #region Spawning Enemies

    [ContextMenu("SpawnRandomEnemies")]
    public void SpawnEnemiesInNewSector_Debug()
    {
        int budget = CurrentThreatBudget;
        bool hasNebula = (_currentLevel.NebulaAmount == NebulaAmounts.None) ? false : true;
        bool hasAsteroids = (_currentLevel.AsteroidAmount == AsteroidAmounts.None) ? false : true;

        List<GameObject> enemiesToMake = _enemyLibrary.CreateRandomMenuFromBudget(
            budget, hasNebula, hasAsteroids);

        foreach (var enemy in enemiesToMake)
        {
            Vector2 pos = CUR.FindRandomPointWithinDistance(Vector2.zero, ArenaRadius);
            Quaternion rot = Quaternion.identity;
            GameObject newEnemy = Instantiate(enemy, pos, rot);
            RegisterEnemy(newEnemy);
        }
    }

    #endregion

    #region Clearing Level

    [ContextMenu("Clear All Enemies")]
    public void ClearAllEnemiesFromLevel()
    {
        for (int i = _enemiesOnLevel.Count - 1; i >= 0; i--)
        {
            Destroy(_enemiesOnLevel[i]);
            _enemiesOnLevel.Remove(_enemiesOnLevel[i]);
        }
    }

    #endregion

    #region Helpers



    #endregion

}
