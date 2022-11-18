using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScrapController : MonoBehaviour
{
    [SerializeField] GameObject _scrapPrefab = null;

    [PreviewField(50)]
    [SerializeField] Sprite[] _scrapSprites = null;

    //settings
    float _scrapAverageLifetime = 30f;
    float _scrapLifetimeDeviation = 5f;
    float _scrapAngularVelocityMax = 20f; // degrees per second
    float _scrapExitSpeedAverage = 1f;
    float _scrapExitSpeedDeviation = 0.5f;

    //state
    
    List<ScrapHandler> _activeScraps = new List<ScrapHandler>();
    [SerializeField] Queue<ScrapHandler> _pooledScraps = new Queue<ScrapHandler>();
 
    internal void SpawnScraps(int scrapToMake, Vector2 impactPosition, Vector2 impactHeading)
    {
        if (scrapToMake == 0) { return; }

        for (int i = 0; i < scrapToMake; i++)
        {
            ScrapHandler newScrap;
            if (_pooledScraps.Count == 0)
            {
                newScrap = Instantiate(_scrapPrefab, impactPosition, Quaternion.identity).GetComponent<ScrapHandler>();
                newScrap.Initialize(this);
            }
            else
            {
                newScrap = _pooledScraps.Dequeue();
                newScrap.gameObject.SetActive(true);
                newScrap.transform.position = impactPosition;
            }

            float lifetime = GetRandomScrapLifetime();
            Sprite sprite = GetRandomScrapSprite();
            float spin = GetRandomAngularVelocity();
            Vector2 driftVector = GetRandomDriftVector(impactHeading);

            newScrap.Setup(lifetime, sprite, spin, driftVector);
            _activeScraps.Add(newScrap);
        }       

    }
    public void ReturnUnusedScrap(ScrapHandler scrapToPool)
    {
        scrapToPool.gameObject.SetActive(false);
        _pooledScraps.Enqueue(scrapToPool);

    }

    public void ClearScraps()
    {
        for (int i = _activeScraps.Count -1; i >= 0; i--)
        {
            _activeScraps[i].gameObject.SetActive(false);
            _pooledScraps.Enqueue(_activeScraps[i]);
        }
        _activeScraps.Clear();
    }

    [ContextMenu("Create 5 Random Scrap")]
    private void CreateSingleScrap_Debug()
    {
        SpawnScraps(5, Vector2.zero, new Vector2(0,1));
    }

    #region Helpers
    private float GetRandomScrapLifetime()
    {
        return (_scrapAverageLifetime + UnityEngine.Random.Range(-1 * _scrapLifetimeDeviation, _scrapLifetimeDeviation));
    }

    private Sprite GetRandomScrapSprite()
    {
        int rand = UnityEngine.Random.Range(0, _scrapSprites.Length);
        return _scrapSprites[rand];
    }

    private float GetRandomAngularVelocity()
    {
        return UnityEngine.Random.Range(-1 * _scrapAngularVelocityMax, _scrapAngularVelocityMax);
    }

    private Vector2 GetRandomDriftVector(Vector2 impactHeading)
    {
        float speed = _scrapExitSpeedAverage + UnityEngine.Random.Range(-1 * _scrapExitSpeedDeviation, _scrapExitSpeedDeviation);
        Vector2 dir = ((2 * impactHeading) + UnityEngine.Random.insideUnitCircle).normalized;
        return dir * speed;
    }

    #endregion
}
