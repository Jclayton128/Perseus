using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    CameraController _camCon;
    UI_Controller _uiController;

    //state
    GameObject _player;

    public GameObject Player
    {
        get => _player;
    }

    Tween _pauseTween;

    public static bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        _camCon = GetComponent<CameraController>();
        _uiController = GetComponent<UI_Controller>();
    }

    public void RegisterPlayer(GameObject player)
    {
        _player = player;
    }


    void Start()
    {
        _uiController.InstantDeployMetaMenu();
        PauseGame(1.2f);
    }

    [ContextMenu("Setup New Game")]
    private void SetupNewGame()
    {
        //Spawn Player
        //Retract Meta Menu
        //Create First Level

        _uiController.RetractMetaMenu();
        UnpauseGame();
        _camCon.FocusCameraOnTarget(_player.transform);
    }

    public void EndGameOnPlayerDeath()
    {
        _player = null;
    }

    #region Time Scale

    public void PauseGame()
    {
        _pauseTween.Kill();
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void PauseGame(float timeframe)
    {
        //_pauseTween.Kill();

        //// Tween a float called myFloat to 52 in 1 second
        //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, timeframe).SetUpdate(true);

        Invoke(nameof(PauseGame), timeframe);
    }

    public void UnpauseGame()
    {
        _pauseTween.Kill();
        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void UnpauseGame(float timeframe)
    {
        //_pauseTween.Kill();

        //// Tween a float called myFloat to 52 in 1 second
        //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, timeframe).SetUpdate(true);

        Invoke(nameof(UnpauseGame), timeframe);
    }

    #endregion
}
