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

    public Action<GameObject> OnPlayerSpawned;

    //state
    GameObject _player;

    public GameObject Player
    {
        get => _player;
    }

    Tween _pauseTween;

    public static bool IsPaused { get; private set; } = false;

    [SerializeField] bool _ispausedtattle;

    private void Awake()
    {
        _camCon = GetComponent<CameraController>();
        _uiController = GetComponent<UI_Controller>();
        _playerShipLibrary = FindObjectOfType<PlayerShipLibrary>();
    }

    void Start()
    {
        _uiController.InstantDeployMetaMenu();
        PauseGame(1.2f);
    }

    private void Update()
    {
        _ispausedtattle = IsPaused;
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
        _uiController.DeployMetaMenu();
        PauseGame(1.2f);
    }

    public void EndGameOnPlayerDeath()
    {
        _player = null;
        _camCon.FocusCameraOnTarget(null);
        _uiController.DeployMetaMenu();
        PauseGame(1.2f);
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
