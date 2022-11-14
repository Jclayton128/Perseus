using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DugoutHandler : MonoBehaviour
{
    LevelController _levelCon;
    UI_Controller _uiCon;
    Radar _radar;
    (float, float) _wormholeState_Unknown = ((0f, 0f));

    //settings
    [Tooltip("Wormholes and crates entering in range get a dugout icon for the rest of the sector")]
    [SerializeField] float _knowledgeRange = 10f;


    //state
    [SerializeField] List<(float, float)> _wormholeState = new List<(float, float)>();
    [SerializeField] List<int> _wormholeKnowledgeState = new List<int>();

    private void Awake()
    {
        _wormholeState.Add((0, 0));
        _wormholeKnowledgeState.Add(0);
        _wormholeState.Add((0, 0));
        _wormholeKnowledgeState.Add(0);
        _wormholeState.Add((0, 0));
        _wormholeKnowledgeState.Add(0);

        _radar = GetComponent<Radar>();
        _radar.RadarScanned += UpdateKnowledgeOnRadarScan;
        _levelCon = FindObjectOfType<LevelController>();
        _uiCon = _levelCon.GetComponent<UI_Controller>();
        _levelCon.WarpedIntoNewLevel += ResetLevelKnowledge;
    }

    private void ResetLevelKnowledge(Level ignored)
    {
        for (int i = 0; i < _wormholeState.Count; i++)
        {
            _wormholeState[i] = _wormholeState_Unknown;
            _wormholeKnowledgeState[i] = 0;
        }
        _uiCon.UpdateDugoutState(_wormholeState);
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
                float distFactor =  (_levelCon.ArenaRadius - dir.magnitude) / _levelCon.ArenaRadius * _wormholeKnowledgeState[i];
                distFactor = Mathf.Lerp(0.5f, 1f, distFactor);

                _wormholeState[i] = (angle, distFactor);
            }
            else
            {
                _wormholeState[i] = (0, 0);
            }
        }
        _uiCon.UpdateDugoutState(_wormholeState);
    }
}
