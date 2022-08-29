using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLibrary : MonoBehaviour
{
    [SerializeField] List<Level> _possibleLevels = new List<Level> ();

    [SerializeField] GameObject[] _asteroidPrefabs = null;
    [SerializeField] GameObject[] _nebulaPrefabs = null;
    [SerializeField] GameObject[] _wormholePrefabs = null;


    public Level GetRandomLevel()
    {
        if (_possibleLevels.Count == 0)
        {
            Debug.LogError("No levels to choose from!");
            return null;
        }
        return _possibleLevels[Random.Range (0, _possibleLevels.Count)];
    }

    public GameObject GetRandomAsteroid()
    {
        return _asteroidPrefabs[Random.Range(0, _asteroidPrefabs.Length)];
    }

    public GameObject GetRandomNebula()
    {
        return _nebulaPrefabs[Random.Range(0, _nebulaPrefabs.Length)];
    }

    public GameObject GetRandomWormhole()
    {
        return _wormholePrefabs[Random.Range(0, _wormholePrefabs.Length)];
    }

}
