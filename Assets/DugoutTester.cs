using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DugoutTester : MonoBehaviour
{
    UI_Controller _uic;

    //state
    [SerializeField][Range(-180, 180)] float _wormholeAngle_0 = 0;
    [SerializeField][Range(0, 1)] float _wormholeDistFactor_0 = 0;
    [SerializeField][Range(-180, 180)] float _wormholeAngle_1 = 0;
    [SerializeField][Range(0, 1)] float _wormholeDistFactor_1 = 0;
    [SerializeField][Range(-180, 180)] float _wormholeAngle_2 = 0;
    [SerializeField][Range(0, 1)] float _wormholeDistFactor_2 = 0;

    private void Awake()
    {
        _uic = FindObjectOfType<UI_Controller>();

    }


    [ContextMenu("PushState")]
    private void PushCurrentState()
    {
        //_uic.UpdateDugoutState(0, _wormholeAngle_0, _wormholeDistFactor_0);
        //_uic.UpdateDugoutState(1, _wormholeAngle_1, _wormholeDistFactor_1);
        //_uic.UpdateDugoutState(2, _wormholeAngle_2, _wormholeDistFactor_2);
    }
}
