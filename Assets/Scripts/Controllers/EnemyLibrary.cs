using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class EnemyLibrary : MonoBehaviour
{
    //settings
    [SerializeField] List<GameObject> _enemyPrefabs = new List<GameObject>();

    //state
    Dictionary<EnemyInfoHolder.EnemyType, GameObject> _enemies =
        new Dictionary<EnemyInfoHolder.EnemyType, GameObject>();

    private void Awake()
    {
        CreateEnemyDictionary();
    }

    private void CreateEnemyDictionary()
    {
        foreach (GameObject enemy in _enemyPrefabs)
        {
            _enemies.Add(enemy.GetComponent<EnemyInfoHolder>().EType, enemy);
        }
        //Debug.Log($"Created an menu with {_enemies.Count} enemies");
    }

    public List<GameObject> CreateRandomMenuFromBudget(
        int totalBudget, bool isAsteroidSector, bool isNebulaSector)
    {
        List<GameObject> menu = new List<GameObject>();
        // Given a budget, create a random list of enemies for a particular level.
        // No constraints on which enemies are allowed for this (ie, no nebula-only restrictions)

        if (_enemies.Count > 0)
        {
            menu.Add(_enemies[0].gameObject);
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
}
