﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarScreen : MonoBehaviour
{
    //The radar screen just manages the graphical depiction of what the Radar object has knowledge of in the form of a sector/intensity dictionary.


    //init

    [SerializeField] RadarSector[] radarSectors = null;

    //param

    public float fadePerSecond;
    public float risePerSecond;



    //hood


    // Start is called before the first frame update
    void Start()
    {
        SetFadeTimeInEachSector();
    }

    private void SetFadeTimeInEachSector()
    {
        foreach (RadarSector rs in radarSectors)
        {
            rs.SetRates(risePerSecond, fadePerSecond);
        }
    }
 
    public void AssignCurrentIntensityToEachSector(int i, float thisSectorIntensity)
    {
        radarSectors[i].SetIntensityLevel(thisSectorIntensity);
    }









}
