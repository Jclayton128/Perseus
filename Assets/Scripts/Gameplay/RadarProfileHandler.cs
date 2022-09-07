using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Collider2D))]
public class RadarProfileHandler : MonoBehaviour
{
    CircleCollider2D _radarProfileCollider;
    float _highLevel = 60f; //At high level, this is the radius of the arena, meaning everything sees the player
    float _lowLevel = 0.1f;
    float _changeRate = 10f; //units per second.

    //state
    public float CurrentRadarProfile;
    public float CurrentRadarProfileFactor;
    private float _baseRadarProfile;

    /// <summary>
    /// If true, this radar profile never changes size. Used for enemies. If false, it can change size,
    /// such as for the player.
    /// </summary>
    [SerializeField] bool _isStaticSized = true;

    private void Awake()
    {
        _radarProfileCollider = GetComponent<CircleCollider2D>();
        _baseRadarProfile = CurrentRadarProfile;
    }

    private void Update()
    {
        UpdateBackToBaseRate();
        if (!_isStaticSized)
        {
            _radarProfileCollider.radius = CurrentRadarProfile;
        }
    }

    private void UpdateBackToBaseRate()
    {
        CurrentRadarProfile = Mathf.MoveTowards(CurrentRadarProfile,
            _baseRadarProfile, _changeRate * Time.deltaTime);
        CurrentRadarProfileFactor = CurrentRadarProfile / _highLevel;
    }

    public void SetProfileBaseRate(float amount)
    {
        _baseRadarProfile = amount;
        if (!_isStaticSized)
        {
            _radarProfileCollider.radius = CurrentRadarProfile;
        }
    }

    public void AddToCurrentRadarProfile(float profileToAdd)
    {
        CurrentRadarProfile += profileToAdd;
        CurrentRadarProfile = Mathf.Clamp(CurrentRadarProfile, _lowLevel, _highLevel);
        CurrentRadarProfileFactor = CurrentRadarProfile / _highLevel;
    }
}
