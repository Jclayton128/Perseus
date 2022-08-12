using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyLibrary : MonoBehaviour
{
    [SerializeField] List<EnemyInfoHolder> _enemyMenu = null;

    public List<GameObject> CreateRandomMenuFromBudget(
        int totalBudget, bool isAsteroidSector, bool isNebulaSector)
    {
        List<GameObject> menu = new List<GameObject>();
        // Given a budget, create a random list of enemies for a particular level.
        // No constraints on which enemies are allowed for this (ie, no nebula-only restrictions)

        if (_enemyMenu.Count > 0)
        {
            menu.Add(_enemyMenu[0].gameObject);
        }
        else
        {
            Debug.LogError("No enemies on the menu to choose from!");
        }

        return menu;
    }
}
