using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    //init
    RadarScreen _rs;
    Rigidbody2D _rb;
    UI_Controller _uiCon;
    [SerializeField] CircleCollider2D _radarDetector;
    Dictionary<int, float> sectorIntensities = new Dictionary<int, float>();
    [SerializeField] List<RadarProfileHandler> _radarTargets = new List<RadarProfileHandler>(); 

    //param
    [SerializeField] float timeBetweenScans;  //0.3f
    [SerializeField] float radarAccuracy; //15  //how far off can the direction-of-arrival be, in degrees.
    float radarRange;
    [SerializeField] float signalFudge;  //0.05
    [SerializeField] float maxRandomNoise; //0.1

    //state
    float timeSinceLastScan = 0;
    public float SelfProfile = 0;

    [Tooltip("Coefficient between speed and self profile. Higher = Stealthier @ top speed")]
    float _stealthFactor = 20f;

    #region initial setup
    private void Awake()
    {
        radarRange = _radarDetector.radius;
        _rb = GetComponent<Rigidbody2D>();
        _uiCon = FindObjectOfType<UI_Controller>();
        _rs = _uiCon.GetRadarScreen();
        PopulateSectorIntensitieswithZero();
    }   

    private void PopulateSectorIntensitieswithZero()
    {
        for (int i = 0; i < 8; i++)
        {
            sectorIntensities.Add(i, 0);
        }
    }
    #endregion

    void Update()
    {
        timeSinceLastScan += Time.deltaTime;
        if (timeSinceLastScan >= timeBetweenScans)
        {
            Scan();
            timeSinceLastScan = 0;
        }
    }

    private void Scan()
    {
        ResetSectorIntensityToZero();
        IncreaseIntensityFromTargetsInEachSector();
        InjectRandomNoise();
        InjectRandomNoise();
        ClampIntensityLevelFloorToSelfNoiseInEachSector();

        PushSectorIntensityToRadarScreen(); //TODO don't let this get called on AI-controlled tanks.
    }

    private void ResetSectorIntensityToZero()
    {
        for (int i = 0; i < 8; i++)
        {
            sectorIntensities[i] = 0;
        }
    }

    private void IncreaseIntensityFromTargetsInEachSector()
    {
        foreach (var target in _radarTargets)
        {
            int sector = DetermineSector(target);
            float signalIntensity = DetermineSignalIntensity(target);
            sectorIntensities[sector] = sectorIntensities[sector] + signalIntensity;
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
        float randomSpread = UnityEngine.Random.Range(-radarAccuracy, radarAccuracy);
        signedAngFromNorth += randomSpread;
        return signedAngFromNorth;
    }
    private void InjectRandomNoise()
    {
        int randSector = UnityEngine.Random.Range(0, 7);
        float randNoise = UnityEngine.Random.Range(0, maxRandomNoise);
        sectorIntensities[randSector] += randNoise;
    }
    private float DetermineSignalIntensity(RadarProfileHandler target)
    {
        if (target == null) { return 0; }
        float dist = (target.transform.position - transform.position).magnitude;
        double dist_normalized = dist / radarRange;
        double intensity = target.RadarProfile / Math.Pow(dist_normalized,1.5) * signalFudge;
        return (float)intensity;

    }

    private void ClampIntensityLevelFloorToSelfNoiseInEachSector()
    {
        SelfProfile = _rb.velocity.magnitude/_stealthFactor;

        for (int i = 0; i < 8; i++)
        {
            sectorIntensities[i] = Mathf.Clamp(sectorIntensities[i], SelfProfile, 1);
        }

    }

    private void PushSectorIntensityToRadarScreen()
    {
        for (int i = 0; i < sectorIntensities.Count; i++)
        {
            _rs.AssignCurrentIntensityToEachSector(i, sectorIntensities[i]);
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
