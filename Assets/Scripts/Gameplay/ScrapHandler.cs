using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScrapHandler : MonoBehaviour
{
    ScrapController _scrapController;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rb;

    //settings
    float _fadeoutDuration = 2.0f;

    //state
    [SerializeField] float _lifetimeRemaining;
    Tween _visualTween;


    internal void Initialize(ScrapController scrapController)
    {
        _scrapController = scrapController;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(float lifetime, Sprite sprite, float initialAngularVelocity, Vector2 driftVector)
    {
        _visualTween.Kill();
        _lifetimeRemaining = lifetime;
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = Color.white;
        _rb.angularVelocity = initialAngularVelocity;
        _rb.velocity = driftVector;
    }

    private void Update()
    {
        _lifetimeRemaining -= Time.deltaTime;
        if (_lifetimeRemaining <= 0)
        {
            Fadeaway();
            Invoke(nameof(ReturnToPool), _fadeoutDuration);
        }
    }

    private void Fadeaway()
    {
        _visualTween = DOTweenModuleSprite.DOColor(_spriteRenderer, Color.clear, _fadeoutDuration);
    }

    private void ReturnToPool()
    {
        _scrapController.ReturnUnusedScrap(this);
    }

    public void CollectScrap()
    {
        
        ReturnToPool();
    }
}
