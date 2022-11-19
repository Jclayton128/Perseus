using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName ="TutorialStep")]
public class TutorialStep : ScriptableObject
{
    public enum CompletionCriteria
    {
        None,
        Accelerate,
        Decelerate,
        Turn,
        OnDetectSignal,
        FirePrimary,
        GainScrap,
        GainUpgradePoint,
        FireSecondary,
        ScrollSecondary,
        Timed,
        OnKillEnemy,
        OnScan,
        OnWarpIntoNewLevel,
        OpenUpgradeMenu,
        CrateSeen,
        Ionized
    }

    public enum Location
    {
        TopLeft,
        TopMiddle,
        TopRight,
        BottomLeft,
        BottomMiddle,
        BottomRight,
        Hidden
    }

    public enum Outcome
    {
        Nothing,
        WeakenEnemy,
        UnlockWormhole
    }

    [SerializeField] Location _location = Location.BottomMiddle;
    [SerializeField] CompletionCriteria _completionCriteria = CompletionCriteria.None;
    [SerializeField] Outcome _outcome = Outcome.Nothing;
    [SerializeField] [Multiline(3), HideLabel] string _tutorialText = "Default tutorial step";


    public string GetText()
    {
        return _tutorialText;
    }

    public Location GetLocation()
    {
        return _location;
    }

    public CompletionCriteria GetCompletionCriteria()
    {
        return _completionCriteria;
    }

    public Outcome GetOutcome()
    {
        return _outcome;
    }
}
