using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mindset : MonoBehaviour
{
    public abstract void InitializeMindset(MindsetHandler mindsetHandlerRef,
        LevelController levelConRef);

    public abstract void EnterMindset();

    public abstract void ExitMindset();

    public abstract void UpdateMindset();
}
