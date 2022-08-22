using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    CameraController _camCon;

    //state
    GameObject _player;

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
}
