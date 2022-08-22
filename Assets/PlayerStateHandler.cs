using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{
    UI_Controller _uiController;

    //Settings
    float _scrapsPerLevelMod = 15f;

    //State
    public int ScrapCollected = 0;
    public int CurrentLevel = 1;
    [SerializeField] int _scrapNeededForNextLevel;
    [SerializeField] float _scrapFactor = 0;


    private void Awake()
    {
        if (GetComponentInParent<ActorMovement>().IsPlayer)
        {
            _uiController = FindObjectOfType<UI_Controller>();
            _uiController.ModifyLevel(CurrentLevel);
            _uiController.ModifyScrapAmount(0, 0);
        }
        _scrapNeededForNextLevel = Mathf.RoundToInt((float)CurrentLevel * _scrapsPerLevelMod);
    }

    public void GainScrap()
    {
        ScrapCollected++;
        //TODO play collect scrap sound
        _scrapFactor = (float)ScrapCollected / (float)_scrapNeededForNextLevel;
        _uiController.ModifyScrapAmount(_scrapFactor, ScrapCollected);
    }
}
