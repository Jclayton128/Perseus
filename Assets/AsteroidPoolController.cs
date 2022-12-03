using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidPoolController : MonoBehaviour
{
    LevelController _levelController;
    [SerializeField] AsteroidHandler _asteroidPrefab = null;


    //state
    Queue<AsteroidHandler> _pooledAsteroids = new Queue<AsteroidHandler>();
    List<AsteroidHandler> _activeAsteroids = new List<AsteroidHandler>();

    private void Awake()
    {
        _levelController = GetComponent<LevelController>();
    }

    internal void SpawnInitialAsteroids(Level currentLevel)
    {
        Vector3 pos = Vector3.zero;
        switch (currentLevel.AsteroidAmount)
        {
            case LevelController.AsteroidAmounts.None:
                break;

            case LevelController.AsteroidAmounts.Sparse:

                for (int i = 0; i < 8; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Medium, pos);
                }
                for (int i = 0; i < 8; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Small, pos);
                }
                break;

            case LevelController.AsteroidAmounts.Medium:
                for (int i = 0; i < 8; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Big, pos);
                }
                for (int i = 0; i < 12; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Medium, pos);
                }
                for (int i = 0; i < 8; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Small, pos);
                }
                break;

            case LevelController.AsteroidAmounts.Heavy:
                for (int i = 0; i < 14; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Huge, pos);
                }
                for (int i = 0; i < 14; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Big, pos);
                }
                for (int i = 0; i < 14; i++)
                {
                    pos = _levelController.ArenaRadius * UnityEngine.Random.insideUnitCircle;
                    SpawnSingleAsteroid(AsteroidHandler.Size.Medium, pos);
                }
                break;
        }
    }

    public void ClearAsteroids()
    {
        for (int i = _activeAsteroids.Count - 1; i >0; i--)
        {
            ReturnAsteroidToPool(_activeAsteroids[i]);
        }
    }

    private AsteroidHandler GetAsteroidHandler()
    {
        AsteroidHandler newAsteroid;

        if (_pooledAsteroids.Count == 0)
        {
            newAsteroid = Instantiate(_asteroidPrefab).GetComponent<AsteroidHandler>();
            newAsteroid.Initialize(this);
        }
        else
        {
            newAsteroid = _pooledAsteroids.Dequeue();
            newAsteroid.gameObject.SetActive(true);
        }
        _activeAsteroids.Add(newAsteroid);
        return newAsteroid;
    }

    public void ReturnAsteroidToPool(AsteroidHandler asteroidHandler)
    {
        asteroidHandler.gameObject.SetActive(false);
        _pooledAsteroids.Enqueue(asteroidHandler);
        _activeAsteroids.Remove(asteroidHandler);
    }

    public void SpawnSingleAsteroid(AsteroidHandler.Size size, Vector2 pos)
    {
        //if (size == AsteroidHandler.Size.Small) return;
        AsteroidHandler ah = GetAsteroidHandler();
        ah.transform.position = pos;
        ah.SetupInstance(size);
    }
}
