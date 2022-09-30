using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class GameController : MonoBehaviour
{
    CameraController _camCon;
    UI_Controller _uiController;
    PlayerShipLibrary _playerShipLibrary;
    RunController _runStatsController;

    public Action<GameObject> OnPlayerSpawned;

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
        _runStatsController = GetComponent<RunController>();
        _playerShipLibrary = FindObjectOfType<PlayerShipLibrary>();
    }

    void Start()
    {
        _uiController.InstantDeployMetaMenu();
        //PauseGame(1.2f);
    }

    public void SetupNewGame()
    {

        //Spawn Player
        //Retract Meta Menu
        //Create First Level
        GameObject playerPrefab = _playerShipLibrary.GetSelectedPlayerShipPrefab();

        if (playerPrefab == null)
        {
            _uiController.FlashShipSelectionDescription();
            return;
        }

        _runStatsController.ResetRunStats();

        _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        OnPlayerSpawned?.Invoke(_player);

        _uiController.RetractMetaMenu();
        UnpauseGame();
        
        //TODO snap the camera to something interesting?
        _camCon.FocusCameraOnTarget(_player.transform);
    }

    public void EndGameOnPlayerChoice()
    {
        Destroy(_player);
        _player = null;
        _camCon.FocusCameraOnTarget(null);
        _uiController.SetIntroText(_runStatsController.GetGameoverText(false));
        _uiController.DeployMetaMenu();
        _uiController.ResetAllShipRelatedUI();
        PauseGame(0.7f);
    }

    public void EndGameOnPlayerDeath()
    {
        _player = null;
        _camCon.FocusCameraOnTarget(null);
        _uiController.SetIntroText(_runStatsController.GetGameoverText(true));
        _uiController.DeployMetaMenu();
        _uiController.ResetAllShipRelatedUI();
        PauseGame(0.7f);
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
