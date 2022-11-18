using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DugoutHandler : MonoBehaviour
{
    LevelController _levelCon;
    UI_Controller _uiCon;
    Radar _radar;

    //settings
    [Tooltip("Wormholes and crates entering in range get a dugout icon for the rest of the sector")]
    [SerializeField] float _knowledgeRange = 10f;


    //state
    [SerializeField] List<(float, float)> _wormholeState = new List<(float, float)>();
    [SerializeField] List<int> _wormholeKnowledgeState = new List<int>();
    (float, float) _crateState = (0, 0);
    int _crateKnowledgeState = 0;

    private void Awake()
    {
        _wormholeState.Add((0, 0));
        _wormholeKnowledgeState.Add(0);
        _wormholeState.Add((0, 0));
        _wormholeKnowledgeState.Add(0);
        _wormholeState.Add((0, 0));
        _wormholeKnowledgeState.Add(0);

        _radar = GetComponent<Radar>();
        //_radar.RadarScanned += UpdateKnowledgeOnRadarScan;
        _levelCon = FindObjectOfType<LevelController>();
        _uiCon = _levelCon.GetComponent<UI_Controller>();
        _levelCon.WarpedIntoNewLevel += ResetLevelKnowledge;
    }

    private void Update()
    {
        UpdateKnowledgeOnRadarScan();
    }

    private void ResetLevelKnowledge(Level ignored)
    {
        for (int i = 0; i < _wormholeState.Count; i++)
        {
            _wormholeState[i] = (0, 0);
            _wormholeKnowledgeState[i] = 0;
        }
        _crateState = (0, 0);
        _crateKnowledgeState = 0;
        _uiCon.UpdateDugoutState(_wormholeState, _crateState);

    }

    private void UpdateKnowledgeOnRadarScan()
    {
        
        for (int i = 0; i < _wormholeState.Count; i++)
        {
            Vector2 dir = _levelCon.WormholeLocations[i] - (Vector2)transform.position;

            if (dir.magnitude <= _knowledgeRange)
            {
                _wormholeKnowledgeState[i] = 1;
            }

            if (_wormholeKnowledgeState[i] == 1)
            {
                float angle = Vector2.SignedAngle(Vector2.up, dir);
                float distFactor =  (_knowledgeRange - dir.magnitude) / _knowledgeRange * _wormholeKnowledgeState[i];
                distFactor = Mathf.Lerp(0.33f, 1f, distFactor);

                _wormholeState[i] = (angle, distFactor);
            }
            else
            {
                _wormholeState[i] = (0, 0);
            }
        }

        if (_levelCon.CrateOnLevel != null)
        {
            Vector2 cDir = _levelCon.CrateOnLevel.transform.position - transform.position;

            if (cDir.magnitude <= _knowledgeRange)
            {
                _crateKnowledgeState = 1;
            }
            if (_crateKnowledgeState == 1)
            {
                float angle = Vector2.SignedAngle(Vector2.up, cDir);
                float distFactor = (_knowledgeRange - cDir.magnitude) / _knowledgeRange * _crateKnowledgeState;
                distFactor = Mathf.Lerp(0.33f, 1f, distFactor);

                _crateState = (angle, distFactor);
            }
            else
            {
                _crateState = (0, 0);
            }
        }
        else
        {
            _crateState = (0, 0);
        }


        _uiCon.UpdateDugoutState(_wormholeState, _crateState);
    }
}
