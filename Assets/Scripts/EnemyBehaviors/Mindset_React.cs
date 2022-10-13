using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mindset_React : Mindset
{
    MindsetHandler _mindsetHandler;
    LevelController _levelController;
    public override void InitializeMindset(MindsetHandler mindsetHandlerRef, LevelController levelConRef)
    {
        _mindsetHandler = mindsetHandlerRef;
        _levelController = levelConRef;
    }

    public override void EnterMindset()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitMindset()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateMindset()
    {
        throw new System.NotImplementedException();
    }
}
