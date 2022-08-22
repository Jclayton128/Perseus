using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyLibrary _enemyLibrary;
    SectorController _sectorController;

    //state
    List<GameObject> _activeEnemies = new List<GameObject>();

    void Awake()
    {
        _enemyLibrary = FindObjectOfType<EnemyLibrary>();
        _sectorController = GetComponent<SectorController>();
    }

    [ContextMenu("SpawnRandomEnemies")]
    public void SpawnEnemiesInNewSector()
    {
        int budget = _sectorController.CurrentThreatBudget;
        bool isNebula = ConvertAmountIntoBool(_sectorController.CurrentNebulaAmount);
        bool isAsteroidField = ConvertAmountIntoBool(_sectorController.CurrentAsteroidAmount);
        List<GameObject> enemiesToMake = _enemyLibrary.CreateRandomMenuFromBudget(
            budget, isNebula, isAsteroidField);

        foreach (var enemy in enemiesToMake)
        {
            Vector2 pos = Vector2.one;
            Quaternion rot = Quaternion.identity;
            GameObject newEnemy = Instantiate(enemy, pos, rot);
            _activeEnemies.Add(newEnemy);
        }
    }

    public void ClearAllEnemiesFromSector()
    {
        for (int i = _activeEnemies.Count -1; i >= 0; i--)
        {
            Destroy(_activeEnemies[i]);
            _activeEnemies.Remove(_activeEnemies[i]);
        }
    }

    #region Helpers

    private bool ConvertAmountIntoBool(SectorController.AsteroidAmounts amount)
    {
        if (amount == SectorController.AsteroidAmounts.None)
        {
            return false;
        }
        else return true;
    }

    private bool ConvertAmountIntoBool(SectorController.NebulaAmounts amount)
    {
        if (amount == SectorController.NebulaAmounts.None)
        {
            return false;
        }
        else return true;
    }

    #endregion
}
