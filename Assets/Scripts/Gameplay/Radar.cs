using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public Action RadarScanned;

    //init
    RadarScreen _rs;
    //Rigidbody2D _rb;
    RadarProfileHandler _radarProfileHandler;
    UI_Controller _uiCon;
    [SerializeField] CircleCollider2D _radarDetector = null;
    int _sectorCount = 8;
    float[] _actualIntensities;
    float[] _depictedIntensities;
    //Dictionary<int, float> _sectorIntensities = new Dictionary<int, float>();
    [SerializeField] List<RadarProfileHandler> _radarTargets = new List<RadarProfileHandler>(); 
    

    //param
    [Header("parameter")]
    [SerializeField] float _timeBetweenScans;  //0.3f
    [SerializeField] float _radarAccuracy; //15  //how far off can the direction-of-arrival be, in degrees.
    float _radarRange;
    [SerializeField] float _signalFudge;  //0.05
    [SerializeField] float _maxRandomNoise; //0.1
    [SerializeField] float _signalFadeRate = 0.5f; // how much intensity fades each second.
    int _radarProfileLayer = 13;

    //state
    float _timeForNextScan = 0;
    //public float SelfProfile { get; private set; } = 0; // Speed divided by stealth factor.


    #region initial setup
    private void Awake()
    {
        _actualIntensities = new float[_sectorCount];
        _depictedIntensities = new float[_sectorCount];
        _radarRange = _radarDetector.radius;
        //_rb = GetComponent<Rigidbody2D>();
        _radarProfileHandler = GetComponentInChildren<RadarProfileHandler>();
        _uiCon = FindObjectOfType<UI_Controller>();
        PopulateSectorIntensitieswithZero();
    }   

    private void PopulateSectorIntensitieswithZero()
    {
        for (int i = 0; i < _sectorCount; i++)
        {
            _actualIntensities[i] = 0;
            _depictedIntensities[i] = 0;
            //_sectorIntensities.Add(i, 0);
        }
    }
    #endregion

    void Update()
    {
        if (Time.time >= _timeForNextScan)
        {
            //DrainIntensityFromSectors();
            Scan();
            _timeForNextScan = Time.time + _timeBetweenScans;
            RadarScanned?.Invoke();
        }

        MoveCurrentIntensityToActualIntensity();
        PushSectorIntensityToRadarScreen();
    }

    private void DrainIntensityFromSectors()
    {
        for (int i = 0; i < _sectorCount; i++)
        {
            _actualIntensities[i] -= _signalFadeRate*_timeBetweenScans;
            _actualIntensities[i] = Mathf.Clamp(_actualIntensities[i],
                0, 100f);
            //_sectorIntensities.Add(i, 0);
        }
    }

    private void Scan()
    {
        ResetSectorIntensityToZero();
        IncreaseIntensityFromTargetsInEachSector();
        InjectRandomNoiseToRandomSector();
        InjectRandomNoiseToRandomSector();
        ClampIntensityLevelFloorToSelfNoiseInEachSector();

    }

    private void ResetSectorIntensityToZero()
    {
        for (int i = 0; i < _sectorCount; i++)
        {
            _actualIntensities[i] = 0;
        }
    }

    private void IncreaseIntensityFromTargetsInEachSector()
    {
        foreach (var target in _radarTargets)
        {
            int sector = DetermineSector(target);
            float signalIntensity = DetermineSignalIntensity(target);
            _actualIntensities[sector] += signalIntensity;
        }

    }

    private int DetermineSector(RadarProfileHandler target)
    {
        Vector3 dir = target.transform.position - transform.position;
        float signedAngFromNorth = Vector3.SignedAngle(dir, Vector3.up, Vector3.forward) - 22.5f;
        if (signedAngFromNorth < 0)
        {
            signedAngFromNorth += 360;
        }

        signedAngFromNorth = InjectRandomSignalSpread(signedAngFromNorth);

        float approxSector = (signedAngFromNorth / 45);
        int sector = Mathf.RoundToInt(approxSector);

        if (sector >= _sectorCount)
        {
            sector = 0;
        }
        if (sector < 0)
        {
            sector = _sectorCount-1;
        }

        return sector;
    }

    private float InjectRandomSignalSpread(float signedAngFromNorth)
    {
        float randomSpread = UnityEngine.Random.Range(-_radarAccuracy, _radarAccuracy);
        signedAngFromNorth += randomSpread;
        return signedAngFromNorth;
    }
    private void InjectRandomNoiseToRandomSector()
    {
        int randSector = UnityEngine.Random.Range(0, _sectorCount);
        float randNoise = UnityEngine.Random.Range(0, _maxRandomNoise);
        _actualIntensities[randSector] += randNoise;
    }
    private float DetermineSignalIntensity(RadarProfileHandler target)
    {
        if (target == null) { return 0; }
        float dist = (target.transform.position - transform.position).magnitude;
        double distanceFactor = 1 - (dist / _radarRange);

        ///intensity is strength / (dist); inverse linear function.
        //ex: 100 profile at distanceFactor 0 should be 0.0;
        //ex: 100 profile at distanceFactor 0.25 should be .75
        //ex: 100 profile at distanceFactor 0.5 (and higher) should be 1.0;

        //ex: 50 profile at distanceFactor 0.25 should be 0.25;
        //ex: 50 profile at distanceFactor 0.5 should 0.5;
        //ex: 50 profile at distanceFactor 0.75 should be .75

        //ex: 10 profile at distanceFactor 0.5 should be 0.1;

        //so, half distance should be true reflection of actual profile. closer increases signal.
        double intensity = (target.CurrentRadarProfile / 50f) * (distanceFactor);
        return (float)intensity;

    }

    private void ClampIntensityLevelFloorToSelfNoiseInEachSector()
    {
        //SelfProfile = _rb.velocity.magnitude / _stealthFactor;
        float selfProfileFactor = _radarProfileHandler.CurrentRadarProfileFactor;
        for (int i = 0; i < _sectorCount; i++)
        {
            _actualIntensities[i] =
                Mathf.Clamp(_actualIntensities[i], selfProfileFactor, 1);
        }

    }

    private void MoveCurrentIntensityToActualIntensity()
    {
        for (int i = 0; i < _actualIntensities.Length; i++)
        {
            _depictedIntensities[i] = Mathf.MoveTowards(_depictedIntensities[i],
                _actualIntensities[i], _signalFadeRate * Time.deltaTime);
        }
    }

    private void PushSectorIntensityToRadarScreen()
    {
        _uiCon.UpdateRadar(_depictedIntensities);
        //for (int i = 0; i < _sectorCount; i++)
        //{
        //    _rs.AssignCurrentIntensityToEachSector(i, _sectorIntensitiesAsArray[i]);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _radarProfileLayer)
        {
            _radarTargets.Add(collision.GetComponent<RadarProfileHandler>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _radarProfileLayer)
        {
            RadarProfileHandler rph = collision.GetComponent<RadarProfileHandler>();
            if (_radarTargets.Contains(rph))
            {
                _radarTargets.Remove(rph);
            }
        }
    }

    public void ModifyRadarRange(float radiusToAdd)
    {
        _radarDetector.radius += radiusToAdd;
    }

    public void ModifyArrivalError(float errorToAdd)
    {
        _radarAccuracy += errorToAdd;
        _radarAccuracy = Mathf.Clamp(_radarAccuracy, 0, 360f);
    }
}
