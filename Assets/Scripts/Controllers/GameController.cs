using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    CameraController _camCon;

    //state
    GameObject _player;
    Tween _pauseTween;

    public static bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        _camCon = GetComponent<CameraController>();
    }

    public void RegisterPlayer(GameObject player)
    {
        _player = player;
    }


    void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        _camCon.FocusCameraOnTarget(_player.transform);
    }

    public GameObject GetPlayerGO()
    {
        return _player;
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
