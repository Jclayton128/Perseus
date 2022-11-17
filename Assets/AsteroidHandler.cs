using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHandler : MonoBehaviour
{
    public enum Size { Huge, Big, Medium, Small}
    HealthHandler _healthHandler;
    SpriteRenderer _sr;
    CircleCollider2D _collider;
    Rigidbody2D _rb;
    AsteroidPoolController _asteroidPoolController;
    [SerializeField] Sprite[] _sprites_Huge = null;
    [SerializeField] Sprite[] _sprites_Big = null;
    [SerializeField] Sprite[] _sprites_Medium = null;
    [SerializeField] Sprite[] _sprites_Small = null;

    //settings
    [Tooltip("Huge to small")]
    [SerializeField] int[] _contents = new int[4];
    [SerializeField] float[] _radius = new float[4];
    [SerializeField] float[] _hull = new float[4];
    [SerializeField] float _initialDriftMaxSpeed = 3f;
    
    //state
    Size _size;

    public void Initialize(AsteroidPoolController apc)
    {
        _healthHandler = GetComponent<HealthHandler>();
        _healthHandler.Dying += HandleAsteroidDeath;
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _asteroidPoolController = apc;
    }

    public void SetupInstance(Size size)
    {
        int rand = 0;
        _size = size;
        switch (size)
        {
            case Size.Huge:
                 rand = UnityEngine.Random.Range(0, _sprites_Huge.Length);
                _sr.sprite = _sprites_Huge[rand];
                break;

            case Size.Big:
                rand = UnityEngine.Random.Range(0, _sprites_Big.Length);
                _sr.sprite = _sprites_Big[rand];
                break;

            case Size.Medium:
                rand = UnityEngine.Random.Range(0, _sprites_Medium.Length);
                _sr.sprite = _sprites_Medium[rand];
                break;

            case Size.Small:
                 rand = UnityEngine.Random.Range(0, _sprites_Small.Length);
                _sr.sprite = _sprites_Small[rand];
                break;
        }
        _rb.velocity = UnityEngine.Random.insideUnitCircle * _initialDriftMaxSpeed;
        _healthHandler.SetHullMaximumAndCurrent(_hull[(int)size]);
        _collider.radius = _radius[(int)size];
    }

    public void HandleAsteroidDeath()
    {
        Vector3 pos = Vector3.zero;// UnityEngine.Random.insideUnitCircle.normalized;
        for(int i = 0; i <= _contents[(int)_size]; i++)
        {
            _asteroidPoolController.SpawnSingleAsteroid((Size)((int)_size +1),
                transform.position + pos);
        }

        _asteroidPoolController.ReturnAsteroidToPool(this);
    }
    



}
