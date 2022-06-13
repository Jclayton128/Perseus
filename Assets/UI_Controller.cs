using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] SystemIconDriver[] _systemIcons = null;
    [SerializeField] Sprite _primaryFireHintSprite = null;
    [SerializeField] Sprite _secondaryFireHintSprite = null;

    //state
    Image _currentActiveSecondary;


    private void Awake()
    {
        foreach (var sid in _systemIcons)
        {
            sid.Initialize(_primaryFireHintSprite, _secondaryFireHintSprite);
        }
        _systemIcons[0].HighlightAsActivePrimary(); // Pri weapon must always be first system;
    }

    public void HighlightNewSecondary(int index)
    {
        foreach (var sid in _systemIcons)
        {
            sid.DehighlightAsActiveSecondaryIfNotPrimary();
        }
        _systemIcons[index].HighlightAsActiveSecondaryIfNotPrimary();
    }

    public void IntegrateNewSystem(int index, Sprite sprite, int level)
    {
        if (index < 0 || index >= _systemIcons.Length)
        {
            Debug.Log("invalid system integration index");
            return;
        }

        _systemIcons[index].ModifyDisplayedSystem(sprite, level);
        
    }

    public int GetMaxSystems()
    {
        return _systemIcons.Length;
    }
}
