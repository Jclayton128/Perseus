using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class EnemyLibrary : MonoBehaviour
{
    //settings
    [SerializeField] List<EnemyInfoHolder> _enemyPrefabs = new List<EnemyInfoHolder>();

    //state
    Dictionary<EnemyInfoHolder.EnemyType, GameObject> _enemies =
        new Dictionary<EnemyInfoHolder.EnemyType, GameObject>();
    List<EnemyInfoHolder.EnemyType> _loadedEnemies = new List<EnemyInfoHolder.EnemyType>();

    private void Awake()
    {
        CreateEnemyDictionary();
    }

    private void CreateEnemyDictionary()
    {
        foreach (var enemy in _enemyPrefabs)
        {
            if (_enemies.ContainsKey(enemy.EType))
            {
                Debug.LogError($"Already loaded a {enemy.EType}");
                continue;
            }
            else
            {
                _enemies.Add(enemy.EType, enemy.gameObject);
                _loadedEnemies.Add(enemy.EType);
            }
        }
        Debug.Log($"Created an menu with {_enemies.Count} enemies");
    }

    public List<GameObject> CreateRandomMenuFromBudget(
        int totalBudget, bool isAsteroidSector, bool isNebulaSector)
    {
        List<GameObject> menu = new List<GameObject>();
        // Given a budget, create a random list of enemies for a particular level.
        // No constraints on which enemies are allowed for this (ie, no nebula-only restrictions)

        if (_enemies.Count > 0)
        {
            //menu.Add(_enemies[EnemyInfoHolder.EnemyType.Dummy1].gameObject);
        }
        else
        {
            Debug.LogError("No enemies on the menu to choose from!");
        }

        return menu;
    }

    public GameObject GetEnemyOfType(EnemyInfoHolder.EnemyType EnemyType)
    {
        return _enemies[EnemyType];
    }

    public EnemyInfoHolder.EnemyType[] GetAllLoadedEnemyTypes()
    {
        return _loadedEnemies.ToArray();
    }
}
