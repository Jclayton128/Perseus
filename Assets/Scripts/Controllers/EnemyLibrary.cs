using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class EnemyLibrary : MonoBehaviour
{
    //settings
    [SerializeField] List<ShipInfoHolder> _enemyPrefabs = new List<ShipInfoHolder>();

    //state
    Dictionary<ShipInfoHolder.ShipType, GameObject> _enemyGameObjects =
        new Dictionary<ShipInfoHolder.ShipType, GameObject>();

    Dictionary<ShipInfoHolder.ShipType, int> _enemyThreatScores =
        new Dictionary<ShipInfoHolder.ShipType, int>();

    List<ShipInfoHolder.ShipType> _loadedEnemies = new List<ShipInfoHolder.ShipType>();

    private void Awake()
    {
        CreateEnemyDictionary();
    }

    private void CreateEnemyDictionary()
    {
        foreach (var enemy in _enemyPrefabs)
        {
            if (_enemyGameObjects.ContainsKey(enemy.EType))
            {
                Debug.LogError($"Already loaded a {enemy.EType}");
                continue;
            }
            else
            {
                _enemyGameObjects.Add(enemy.EType, enemy.gameObject);
                _enemyThreatScores.Add(enemy.EType, enemy.ThreatScore);
                _loadedEnemies.Add(enemy.EType);
            }
        }
        Debug.Log($"Created an menu with {_enemyGameObjects.Count} enemies");
    }

    public List<GameObject> CreateMenuFromBossLevel(Level bossLevel)
    {
        List<GameObject> menu = new List<GameObject>();

        if (bossLevel.PossibleEnemies.Count == 0)
        {
            Debug.LogError("No boss enemies to choose from on this level!");
            return null;
        }

        foreach (var en in bossLevel.PossibleEnemies)
        {
            menu.Add(_enemyGameObjects[en]);
        }

        return menu;
    }

    public List<GameObject> CreateMenuFromBudgetAndLevel(int totalBudget, Level level)
    {
        int remainingBudget = totalBudget;
        Debug.Log("starting budget = " + remainingBudget);
        List<GameObject> menu = new List<GameObject>();

        //Prepare the submenu based on max budget and enemies allowed on the level
        List<ShipInfoHolder.ShipType> startingEnemyList = level.PossibleEnemies;
        List<ShipInfoHolder.ShipType> allowedEnemyList = TrimEnemiesByBudget(
            startingEnemyList, remainingBudget);
                

        if (allowedEnemyList.Count == 0)
        {
            Debug.LogError("No in-budget enemies to choose from!");
            return null;
        }
        else
        {
            Debug.Log($"allowed enemy menu has {allowedEnemyList.Count} things");
        }

        //Populate the actual enemy list
        for (int i = 0; i < 100; i++) // max enemy count of 100;
        { 
            int rand = UnityEngine.Random.Range(0, allowedEnemyList.Count);
            ShipInfoHolder.ShipType etype = allowedEnemyList[rand];
            menu.Add(_enemyGameObjects[etype]);
            remainingBudget -= _enemyThreatScores[etype];
            //Debug.Log($"added a {etype} to the menu");
            //Debug.Log("starting budget = " + remainingBudget);

            //Review the allowed menu list and remove anything that exceeds budget
            allowedEnemyList = TrimEnemiesByBudget(allowedEnemyList, remainingBudget);

            if (remainingBudget <= 0 || allowedEnemyList.Count == 0) break;
        }

        return menu;
    }

    private List<ShipInfoHolder.ShipType> TrimEnemiesByBudget(
        List<ShipInfoHolder.ShipType> startingList, int remainingBudget)
    {
        List<ShipInfoHolder.ShipType> enemiesUnderBudget =
            new List<ShipInfoHolder.ShipType>(startingList);

        foreach (var enemy in startingList)
        {
            int threat = _enemyThreatScores[enemy];

            if (threat <= remainingBudget)
            {
                //this enemy is under budget and may stay on the menu
            }
            else
            {
                //Debug.Log($"{enemy} is too threatening for the current budget.");
                enemiesUnderBudget.Remove(enemy);
            }
        }

        //foreach (var enemy in enemiesUnderBudget)
        //{
        //    Debug.Log($"menu includes an {enemy}");
        //}
        return enemiesUnderBudget;

    }

    public GameObject GetEnemyGameObjectOfType(ShipInfoHolder.ShipType EnemyType)
    {
        return _enemyGameObjects[EnemyType];
    }

    public ShipInfoHolder.ShipType[] GetAllLoadedEnemyTypes_Debug()
    {
        return _loadedEnemies.ToArray();
    }


}
