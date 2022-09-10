using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    //init
    RadarScreen _rs;
    //Rigidbody2D _rb;
    RadarProfileHandler _radarProfileHandler;
    UI_Controller _uiCon;
    [SerializeField] CircleCollider2D _radarDetector;
    Dictionary<int, float> _sectorIntensities = new Dictionary<int, float>();
    [SerializeField] List<RadarProfileHandler> _radarTargets = new List<RadarProfileHandler>(); 
    

    //param
    [Header("parameter")]
    [SerializeField] float _timeBetweenScans;  //0.3f
    [SerializeField] float _radarAccuracy; //15  //how far off can the direction-of-arrival be, in degrees.
    float _radarRange;
    [SerializeField] float _signalFudge;  //0.05
    [SerializeField] float _maxRandomNoise; //0.1

    //state
    float _timeSinceLastScan = 0;
    //public float SelfProfile { get; private set; } = 0; // Speed divided by stealth factor.


    #region initial setup
    private void Awake()
    {
        _radarRange = _radarDetector.radius;
        //_rb = GetComponent<Rigidbody2D>();
        _radarProfileHandler = GetComponentInChildren<RadarProfileHandler>();
        _uiCon = FindObjectOfType<UI_Controller>();
        _rs = _uiCon.GetRadarScreen();
        PopulateSectorIntensitieswithZero();
    }   

    private void PopulateSectorIntensitieswithZero()
    {
        for (int i = 0; i < 8; i++)
        {
            _sectorIntensities.Add(i, 0);
        }
    }
    #endregion

    void Update()
    {
        _timeSinceLastScan += Time.deltaTime;
        if (_timeSinceLastScan >= _timeBetweenScans)
        {
            Scan();
            _timeSinceLastScan = 0;
        }
    }

    private void Scan()
    {
        ResetSectorIntensityToZero();
        IncreaseIntensityFromTargetsInEachSector();
        InjectRandomNoiseToRandomSector();
        InjectRandomNoiseToRandomSector();
        ClampIntensityLevelFloorToSelfNoiseInEachSector();

        PushSectorIntensityToRadarScreen();
    }

    private void ResetSectorIntensityToZero()
    {
        for (int i = 0; i < 8; i++)
        {
            _sectorIntensities[i] = 0;
        }
    }

    private void IncreaseIntensityFromTargetsInEachSector()
    {
        foreach (var target in _radarTargets)
        {
            int sector = DetermineSector(target);
            float signalIntensity = DetermineSignalIntensity(target);
            _sectorIntensities[sector] = _sectorIntensities[sector] + signalIntensity;
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
        // signedAngFromNorth += GetRandomSignalSpread();

        float approxSector = (signedAngFromNorth / 45);
        int sector = Mathf.RoundToInt(approxSector);

        if (sector >= 8)
        {
            sector = 0;
        }
        if (sector < 0)
        {
            sector = 7;
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
        int randSector = UnityEngine.Random.Range(0, 7);
        float randNoise = UnityEngine.Random.Range(0, _maxRandomNoise);
        _sectorIntensities[randSector] += randNoise;
    }
    private float DetermineSignalIntensity(RadarProfileHandler target)
    {
        if (target == null) { return 0; }
        float dist = (target.transform.position - transform.position).magnitude;
        double dist_normalized = dist / _radarRange;

        ///complicate dbit o comment?
        double intensity = target.CurrentRadarProfile / Math.Pow(dist_normalized,2f) * _signalFudge;
        return (float)intensity;

    }

    private void ClampIntensityLevelFloorToSelfNoiseInEachSector()
    {
        //SelfProfile = _rb.velocity.magnitude / _stealthFactor;
        float selfProfileFactor = _radarProfileHandler.CurrentRadarProfileFactor;
        for (int i = 0; i < 8; i++)
        {
            _sectorIntensities[i] = Mathf.Clamp(_sectorIntensities[i], selfProfileFactor, 1);
        }

    }

    private void PushSectorIntensityToRadarScreen()
    {
        for (int i = 0; i < _sectorIntensities.Count; i++)
        {
            _rs.AssignCurrentIntensityToEachSector(i, _sectorIntensities[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            _radarTargets.Add(collision.GetComponent<RadarProfileHandler>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            RadarProfileHandler rph = collision.GetComponent<RadarProfileHandler>();
            if (_radarTargets.Contains(rph))
            {
                _radarTargets.Remove(rph);
            }
        }
    }
}
