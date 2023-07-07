using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryPodLauncherWH : WeaponHandler
{
    LevelController _levelCon; 

    //settings
    [SerializeField] string[] _modeNames = null;
    [SerializeField] SentryPodBrain _sentryPodPrefab = null;

    //state
    [SerializeField] int _currentMode = 0;
    [SerializeField] int _maxMode = 1;
    SentryPodBrain _sentryPod;

    public override object GetUIStatus()
    {
        return _modeNames[_currentMode];
    }

    protected override void ActivateInternal()
    {
        _currentMode++;
        if (_currentMode >= _maxMode)
        {
            _currentMode = 0;
        }
        _connectedWID?.UpdateUI(_modeNames[_currentMode]);
        _sentryPod?.SetSentryMode(_currentMode);
        //TODO audio play some zip zoop UI noises to indicate aurally which mode currently in.
    }

    protected override void DeactivateInternal(bool wasPausedDuringDeactivationAttempt)
    {

    }

    protected override void ImplementWeaponUpgrade()
    {
        _maxMode++;
    }

    protected override void InitializeWeaponSpecifics()
    {
        _levelCon = FindObjectOfType<LevelController>();
        _levelCon.WarpingOutFromOldLevel += DestroySentryPod;
        _levelCon.WarpedIntoNewLevel += DeploySentryPod;
        DeploySentryPod(null); 
    }

    private void DeploySentryPod(Level throwaway)
    {
        Vector2 randPos = CUR.FindRandomPositionWithinRangeBandAndWithinArena(
            transform.position, 1f, 3f, Vector3.zero, _levelCon.ArenaRadius);

        if (_sentryPod)
        {
            _sentryPod.gameObject.SetActive(true);
            _sentryPod.transform.position = randPos;
        }
        else
        {
            _sentryPod = Instantiate(_sentryPodPrefab, randPos, Quaternion.identity).
                GetComponent<SentryPodBrain>();
            _sentryPod.Initialize(transform);
        }
        _sentryPod.SetSentryMode(0);
    }

    private void DestroySentryPod()
    {
        if (_sentryPod) _sentryPod.gameObject.SetActive(false);
    }
}
