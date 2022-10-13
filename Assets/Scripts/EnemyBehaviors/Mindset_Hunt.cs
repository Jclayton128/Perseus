using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mindset_Hunt : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;

    //settings
    [Tooltip("How long the enemy will continue to hunt before going back into explore")]
    [SerializeField] float _timeframeForHunting = 5f;

    public float TimeframeForHunting => _timeframeForHunting;

    //state

    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
    }
    
    public override void EnterMindset()
    {
        
    }
    public override void ExitMindset()
    {
        
    }
    public override void UpdateMindset()
    {
        
    }
}
