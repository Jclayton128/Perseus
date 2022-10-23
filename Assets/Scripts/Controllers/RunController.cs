using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour
{
    //settings
    int _startingThreatBudget = 2;

    //state

    int _currentLevelNumber = 0;
    int _currentThreatBudget = 0;

    public void ResetRunStats()
    {
        _currentLevelNumber = 0;
        _currentThreatBudget = 0;

    }

    public void IncrementSectorCount()
    {
        _currentLevelNumber++;

        //TODO do a more clever threat budget increase per level?
        _currentThreatBudget++;
    }

    public int GetThreatBudget()
    {
        return _currentThreatBudget;
    }

    public string GetGameoverText(bool wasGameoverForced)
    {
        if (wasGameoverForced)
        {
            if (_currentLevelNumber <= 2)
            {
                return "The nebula claimed another unwitting victim. Another try?";
            }
            if (_currentLevelNumber <= 5)
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
}
