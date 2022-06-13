using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystemHandler : MonoBehaviour
{
    SystemsLibrary _syslib;
    [SerializeField] SystemsLibrary.SystemType[] _startingSystems = null;
    PlayerHandler _playerHandler;
    UI_Controller _UICon;

    //state
    int _activeSecondarySystemIndex;
    public SystemHandler ActiveSecondarySystem { get; protected set; }
    int _maxSystems;
    List<SystemHandler> _systemsOnBoard = new List<SystemHandler>();
    List<SystemHandler> _secondarySystems = new List<SystemHandler>();
    private void Awake()
    {
        _syslib = FindObjectOfType<SystemsLibrary>();
        _UICon = FindObjectOfType<UI_Controller>();
        _maxSystems = _UICon.GetMaxSystems();
        _playerHandler = GetComponent<PlayerHandler>();

    }

    private void Start()
    {
        LoadStartingSystems();
    }

    private void LoadStartingSystems()
    {
        foreach (var system in _startingSystems)
        {
            GainSystem(_syslib.GetSystem(system));
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        SystemCrateHandler sch;
        if (collision.gameObject.TryGetComponent<SystemCrateHandler>(out sch))
        {
            if (_systemsOnBoard.Count >= _maxSystems)
            {
                Debug.Log("unable to hold any more systems");
                return;
            }
            GainSystem(sch.SystemChunk);
            Destroy(collision.gameObject);
        }

    }

    private void GainSystem(GameObject newSystem)
    {
        GameObject go = Instantiate<GameObject>(newSystem, this.transform);
        SystemHandler sh = newSystem.GetComponent<SystemHandler>();
        sh.IntegrateSystem(_playerHandler);
        _systemsOnBoard.Add(sh);
        _UICon.IntegrateNewSystem(_systemsOnBoard.Count - 1, sh.GetIcon(), 1);
        go.transform.localPosition = sh.LocalPosition;

        if (sh.IsSecondary)
        {
            _secondarySystems.Add(sh);
            if (!ActiveSecondarySystem)
            {
                ActiveSecondarySystem = sh;
                _activeSecondarySystemIndex = _secondarySystems.IndexOf(sh);
            }
        }
    }

    public List<SystemHandler> GetSystemsOnBoard()
    {
        return _systemsOnBoard;
    }

    public void ToggleActiveSystemUp()
    {
        _activeSecondarySystemIndex++;
        _activeSecondarySystemIndex = 
            Mathf.Clamp(_activeSecondarySystemIndex ,0, _secondarySystems.Count - 1);
        ActiveSecondarySystem = _systemsOnBoard[_activeSecondarySystemIndex];
        _UICon.HighlightNewSecondary(_systemsOnBoard.IndexOf(ActiveSecondarySystem));
    }

    public void ToggleActiveSystemDown()
    {
        _activeSecondarySystemIndex--;
        _activeSecondarySystemIndex =
            Mathf.Clamp(_activeSecondarySystemIndex, 0, _secondarySystems.Count - 1);
        ActiveSecondarySystem = _systemsOnBoard[_activeSecondarySystemIndex];
        _UICon.HighlightNewSecondary(_systemsOnBoard.IndexOf(ActiveSecondarySystem));
    }

}
