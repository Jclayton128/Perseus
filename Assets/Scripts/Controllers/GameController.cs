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
    LevelController _levelController;
    PlayerShipLibrary _playerShipLibrary;
    RunController _runStatsController;
    TutorialController _tutorialController;
    AudioController _audioController;

    public Action<GameObject> PlayerSpawned;


    //settings
    [SerializeField] float _deathDwellTime = 5f;


    //state
    GameObject _player;

    public GameObject Player => _player;

    Tween _timescaleTween;


    public static bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        _camCon = GetComponent<CameraController>();
        _uiController = GetComponent<UI_Controller>();
        _runStatsController = GetComponent<RunController>();
        _tutorialController = GetComponent<TutorialController>();
        _levelController = GetComponent<LevelController>();
        _audioController = GetComponent<AudioController>();
        _playerShipLibrary = FindObjectOfType<PlayerShipLibrary>();
    }

    void Start()
    {
        _uiController.InstantDeployMetaMenu();
        //PauseGame(1.2f);
    }

    public void SetupNewGame()
    {

        GameObject playerPrefab = _playerShipLibrary.GetSelectedPlayerShipPrefab();

        if (playerPrefab == null)
        {
            _uiController.FlashShipSelectionDescription();
            return;
        }

        _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerSpawned?.Invoke(_player);

        _runStatsController.ResetRunStats();     

        _uiController.RetractMetaMenu();
        UnpauseGame();
        if (_uiController.GetTutorialModeCheckStatus())
        {
            _tutorialController.BeginTutorialSequence();
            _levelController.StartGameWithTutorial();
        }
        else
        {
            _tutorialController.EndTutorial();
            _levelController.StartGameWithRegular();
        }
        //TODO snap the camera to something interesting?
        _camCon.FocusCameraOnTarget(_player.transform);
        _camCon.ResetZoomToStarting();
    }

    public void EndGameOnPlayerChoice()
    {
        _levelController.ClearLevel();
        Destroy(_player);
        _player = null;
        //_camCon.FocusCameraOnTarget(null);
        _uiController.SetIntroText(_runStatsController.GetGameoverText(false));
        _uiController.DeployMetaMenu();
        _uiController.ResetAllShipRelatedUI();
        PauseGame(0.7f);
    }

    public void EndGameOnPlayerDeath()
    {
        BeginPlayerDeathSequence();
    }

    private void BeginPlayerDeathSequence()
    {
        _camCon.FocusCameraOnPlayerDeathZoom(_deathDwellTime);

        _timescaleTween.Kill();
        //Slow timescale to half over 1 real-time second.
        _timescaleTween =
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.5f, 1f).SetUpdate(true);

        _audioController.PlayUIClip(AudioLibrary.ClipID.PlayerDeath);
        Invoke(nameof(FinalizePlayerDeath), _deathDwellTime * 0.5f);
    }


    private void FinalizePlayerDeath()
    {
        Destroy(_player.gameObject);
        _player = null;
        _uiController.SetIntroText(_runStatsController.GetGameoverText(true));
        _uiController.DeployMetaMenu();
        _uiController.ResetAllShipRelatedUI();
        PauseGame(0.7f);
    }

    #region Time Scale

    public void PauseGame()
    {
        _timescaleTween.Kill();
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void PauseGame(float timeframe)
    {
        //_pauseTween.Kill();

        //// Tween a float called myFloat to 52 in 1 second
        //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, timeframe).SetUpdate(true);

        //Invoke(nameof(PauseGame), timeframe);
        PauseGame();
    }

    public void UnpauseGame()
    {
        _timescaleTween.Kill();
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
