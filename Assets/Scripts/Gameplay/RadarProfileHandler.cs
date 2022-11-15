using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Collider2D))]
public class RadarProfileHandler : MonoBehaviour
{
    SpriteRenderer[] _spriteRenderers;
    CircleCollider2D _radarProfileCollider;
    
    //settings
    float _highLevel = 30f; //At high level, this is the half-radius of the arena, meaning lots of enemies will see the player
    float _lowLevel = 0.1f;
    float _changeRate = 10f; //units per second.

    //state
    public float CurrentRadarProfile;
    public float CurrentRadarProfileFactor;
    private float _baseRadarProfile;

    [SerializeField] bool _isCloaked = false;
    Color halftone = new Color(1, 1, 1, .5f);

    /// <summary>
    /// If true, this radar profile never changes size. Used for enemies. If false, it can change size,
    /// such as for the player.
    /// </summary>
    [SerializeField] bool _isStaticSized = true;

    private void Awake()
    {
        _radarProfileCollider = GetComponent<CircleCollider2D>();
        _baseRadarProfile = CurrentRadarProfile;

        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();   
    }

    private void Update()
    {
        if (_isCloaked) return;

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
        if (_isCloaked) return;
        CurrentRadarProfile += profileToAdd;
        CurrentRadarProfile = Mathf.Clamp(CurrentRadarProfile, _lowLevel, _highLevel);
        CurrentRadarProfileFactor = CurrentRadarProfile / _highLevel;
    }

    public void Cloak()
    {
        _isCloaked = true;
        _radarProfileCollider.enabled = false;
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.color = halftone;
        }
    }

    public void Decloak()
    {
        _isCloaked = false;
        _radarProfileCollider.enabled = true;
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.color = Color.white;
        }
    }
}
