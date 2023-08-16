using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour
{
    //settings
    [SerializeField] int _startingThreatBudget = 2;
    int _levelsBetweenBosses = 7;
    [SerializeField] int _budgetIncreasePerLevel = 3;

    //state
    int _currentSectorCount = 0;
    public int CurrentSectorCount => _currentSectorCount;
    int _currentThreatBudget = 0;
    public int CurrentThreatBudget => _currentThreatBudget;

    [SerializeField] int _currentBossCount = 0;
    public int CurrentBossCount => _currentBossCount;

    public void ResetRunStats()
    {
        _currentSectorCount = 0;
        _currentThreatBudget = _startingThreatBudget;
        _currentBossCount = 0;

    }

    public void IncrementSectorCount()
    {
        _currentSectorCount++;

        //TODO do a more clever threat budget increase per level?
        //_currentThreatBudget++;
        ModifyRunBudget(_budgetIncreasePerLevel);

        if (CheckIfIsBossLevel()) _currentBossCount++;
    }

    public string GetGameoverText(bool wasGameoverForced)
    {
        if (wasGameoverForced)
        {
            if (_currentSectorCount <= 2)
            {
                return "The nebula claimed another unwitting victim. Another try?";
            }
            if (_currentSectorCount <= 5)
            {
                return "The nebula was always hungry. Another try?";
            }
            else
            {
                return "The next run will be better. Another try?";
            }
        }
        else
        {
            return "The nebula's sights could shake even the hardiest pilots. Maybe you'll recover.";
        }

    }

    public void ModifyRunBudget(int amountToIncrease)
    {
        _currentThreatBudget += amountToIncrease;
    }

    public bool CheckIfIsBossLevel()
    {
        if (_currentSectorCount % _levelsBetweenBosses == 0)
        {
            return true;
        }
        else return false;
    }

    public bool CheckIfPreBossLevel()
    {
        //Debug.Log($"csc % lbb {_currentSectorCount} % {_levelsBetweenBosses}. preboss if {_levelsBetweenBosses - 1}");
        if (_currentSectorCount % _levelsBetweenBosses == _levelsBetweenBosses - 1)
        {
            return true;
        }
        else return false;
    }
}
