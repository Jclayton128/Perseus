using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour
{
    //settings
    [SerializeField] int _startingThreatBudget = 2;

    //state

    int _currentSectorCount = 0;
    public int CurrentSectorCount => _currentSectorCount;
    int _currentThreatBudget = 0;
    public int CurrentThreatBudget => _currentThreatBudget;

    public void ResetRunStats()
    {
        _currentSectorCount = 0;
        _currentThreatBudget = 0;

    }

    public void IncrementSectorCount()
    {
        _currentSectorCount++;

        //TODO do a more clever threat budget increase per level?
        _currentThreatBudget++;
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
        Debug.Log("Budget increased. Current budget is now: " + _currentThreatBudget);
    }
}
