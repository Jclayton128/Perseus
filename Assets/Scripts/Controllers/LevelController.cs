using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //Refs
    GameController _gameController;
    RunController _runController;
    SystemWeaponLibrary _systemWeaponLibrary;
    EnemyLibrary _enemyLibrary;
    LevelLibrary _levelLibrary;
    PlayerSystemHandler _playerSystemHandler;
    CircleEdgeCollider2D _arenaEdgeCollider;

    public Action WarpingOutFromOldLevel;
    public Action WarpingIntoNewLevel;
    public Action<Level> WarpedIntoNewLevel;
    /// <summary>
    ///  int 1: current sector count. int 2: number of enemies spawned on level.
    /// </summary>
    public Action<int, int> SpawnedLevelEnemies;
    public Action<int> EnemyLevelCountChanged;

    public enum AsteroidAmounts { None, Sparse, Medium, Heavy };
    public enum NebulaAmounts { None, Sparse, Medium, Heavy };

    //settings 
    float _minSeparationBetweenWormholes = 20f;
    float _wormholeSelectionTime = 5f;
    float _enemySpawnRadius_min = .1f;
    float _enemySpawnRadius_max = .9f;
    [SerializeField] GameObject _cratePrefab = null;
    [SerializeField] GameObject _wormholePrefab = null;
    [SerializeField] SpriteRenderer _filterSR;
    [SerializeField] Level _tutorialLevel = null;

    //state
    public Level _currentLevel;
    [SerializeField] Color _filterColor = Color.white;
    Tween _filterTween;

    List<EnemyRegistrationHandler> _enemiesOnLevel = new List<EnemyRegistrationHandler>();
    List<GameObject> _asteroidsOnLevel = new List<GameObject>();
    List<GameObject> _nebulaOnLevel = new List<GameObject>();
    List<WormholeHandler> _wormholesOnLevel = new List<WormholeHandler>();
    GameObject _crateOnLevel;

    WormholeHandler _selectedWormhole = null;
    float _timeToSelectWormhole = 0;

    public float ArenaRadius => _arenaEdgeCollider.Radius;


    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _runController = GetComponent<RunController>();
        _gameController.PlayerSpawned += ReactToPlayerSpawned;
        _enemyLibrary = FindObjectOfType<EnemyLibrary>();
        _levelLibrary = FindObjectOfType<LevelLibrary>();
        _systemWeaponLibrary = _levelLibrary.GetComponent<SystemWeaponLibrary>();
        _arenaEdgeCollider = FindObjectOfType<CircleEdgeCollider2D>();

    }


    private void ReactToPlayerSpawned(GameObject player)
    {
        _playerSystemHandler = player.GetComponent<PlayerSystemHandler>();
    }

    #region Entry Points to Level sequences

    public void StartGameWithTutorial()
    {
        ClearLevel();
        _currentLevel = _tutorialLevel;
        BuildTutorialLevel();
    }

    public void StartGameWithRegular()
    {
        LoadNewRandomLevel();
    }

    #endregion


    #region Flow
    private void LoadNewRandomLevel()
    {
        ClearLevel();
        WarpIntoNewLevel();
        BuildNewRegularLevel();
    }


    private void BuildTutorialLevel()
    {
        Vector2[] tutorialWormholePosition = new Vector2[1] {new Vector2(0,30f)};
        SpawnWormholes(1, tutorialWormholePosition);
        SpawnTutorialEnemy();
    }
    private void WarpIntoNewLevel()
    {
        _runController.IncrementSectorCount();

        _gameController.Player.transform.position = Vector3.zero;

        _filterTween.Kill();
        _filterTween = DOTween.To(() => _filterSR.color, x => _filterSR.color = x, Color.clear, 0.5f);

        _currentLevel = _levelLibrary.GetRandomLevel();
        Debug.Log($"Entering new level: {_currentLevel.name} ");
        WarpedIntoNewLevel?.Invoke(_currentLevel);
        //Player should listen in to this^ to recharge energy, shields, and systems, and reduce profile

        //TODO ripping audio sound for warp in;
    }
    private void BuildNewRegularLevel()
    {
        SpawnWormholes(3);
        SpawnEnemiesInNewSector();         // Populate Enemies according threat budget, add to list
        AssignRewardSystemToRandomEnemy();
        if (_currentLevel.AsteroidAmount > 0)
        {
            // Populate Asteroids and add to list
        }
        if (_currentLevel.NebulaAmount > 0)
        {

            // Populate Nebulae and add to list
        }
    }

   
    private void ReactToPlayerEnteringWormhole(WormholeHandler wh)
    {
        _selectedWormhole = wh;
        _timeToSelectWormhole = 0;
        //Debug.Log("Player in wormhole");
    }

    private void Update()
    {
        if (_selectedWormhole)
        {
            _timeToSelectWormhole += Time.deltaTime;
            float factor = _timeToSelectWormhole / _wormholeSelectionTime;
            _filterColor.a = factor;
            _filterSR.color = _filterColor;
            if (_timeToSelectWormhole > _wormholeSelectionTime)
            {
                LoadNewRandomLevel();
            }
        }
        
    }

    private void ReactToPlayerExitingWormhome(WormholeHandler wh)
    {
        _selectedWormhole = null;

        _filterTween.Kill();
        _filterTween = DOTween.To(() => _filterSR.color, x => _filterSR.color = x, Color.clear, 0.5f);

        //_timeToSelectWormhole = Mathf.Infinity;
        //Debug.Log("Player exits wormhole");
    }
    #endregion


    #region Registers
    private void RegisterEnemy(EnemyRegistrationHandler enemy)

    {
        if (_enemiesOnLevel.Contains(enemy))
        {
            Debug.Log($"already registered this enemy: {enemy}");
        }
        else
        {
            enemy.Initialize(this);
            _enemiesOnLevel.Add(enemy);
        }
    }

    private void RegisterAsteroid(GameObject asteroid)
    {
        if (_asteroidsOnLevel.Contains(asteroid))
        {
            Debug.Log($"already registered this asteroid: {asteroid}");
        }
        else
        {
            _asteroidsOnLevel.Add(asteroid);
        }
    }

    private void RegisterNebule(GameObject nebula)
    {
        if (_nebulaOnLevel.Contains(nebula))
        {
            Debug.Log($"already registered this nebula: {nebula}");
        }
        else
        {
            _nebulaOnLevel.Add(nebula);
        }
    }

    #endregion

    #region Spawning New Level Items

    public void SpawnTutorialEnemy()
    {
        Vector2 tutorialEnemyPosition = new Vector2(10f, 10f);
        Quaternion rot = Quaternion.identity;
        GameObject newEnemy = Instantiate(_enemyLibrary.GetEnemyGameObjectOfType(ShipInfoHolder.ShipType.Dummy1),
            tutorialEnemyPosition, rot);
        RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>());

    }

    public void SpawnEnemiesInNewSector()
    {
        int budget = _runController.CurrentThreatBudget;
        List<GameObject> enemiesToMake =
            _enemyLibrary.CreateMenuFromBudgetAndLevel(budget, _currentLevel);

        foreach (var enemy in enemiesToMake)
        {
            var pos = CUR.FindRandomPointWithinDistance(Vector2.zero,
                _enemySpawnRadius_max * ArenaRadius, _enemySpawnRadius_min * ArenaRadius);
            Quaternion rot = Quaternion.identity;
            GameObject newEnemy = Instantiate(enemy, pos, rot);
            RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>());
        }

        SpawnedLevelEnemies?.Invoke(_runController.CurrentSectorCount, _enemiesOnLevel.Count);
        EnemyLevelCountChanged?.Invoke(_enemiesOnLevel.Count);
    }
    private void AssignRewardSystemToRandomEnemy()
    {
        if (_enemiesOnLevel.Count == 0) return;

        int rand = UnityEngine.Random.Range(0, _enemiesOnLevel.Count);
        GameObject chosenEnemy = _enemiesOnLevel[rand].gameObject;

        RewardSystemHolder rsh = chosenEnemy.AddComponent<RewardSystemHolder>();
        rsh.Initialize(this);
        int sysOrWeap = UnityEngine.Random.Range(0, 2);
        if (sysOrWeap == 0)
        {
            SystemWeaponLibrary.SystemType systype =
                _systemWeaponLibrary.GetRandomUninstalledSystemType(
                _playerSystemHandler.SystemTypesOnBoard);
            rsh.HeldSystem = systype;
        }
        else
        {
            SystemWeaponLibrary.WeaponType weaptype =
                _systemWeaponLibrary.GetRandomUninstalledSecondaryWeaponType(
                _playerSystemHandler.WeaponTypesOnBoard);
            rsh.HeldWeapon = weaptype;
        }

    }
    public GameObject SpawnSingleLevelEnemy_Debug(ShipInfoHolder.ShipType enemyType)
    {
        GameObject enemy = _enemyLibrary.GetEnemyGameObjectOfType(enemyType);
        Vector2 pos = CUR.FindRandomPointWithinDistance(Vector2.zero, ArenaRadius);
        Quaternion rot = Quaternion.identity;
        GameObject newEnemy = Instantiate(enemy, pos, rot);
        RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>());
        return newEnemy;
    }

    public GameObject SpawnSingleShipAtPoint(ShipInfoHolder.ShipType shipType, Vector3 spawnPoint)
    {
        GameObject enemy = _enemyLibrary.GetEnemyGameObjectOfType(shipType);
        float randRot = UnityEngine.Random.Range(-179, 179);
        Quaternion rot = Quaternion.Euler(0, 0, randRot);
        GameObject newEnemy = Instantiate(enemy, spawnPoint, rot);
        RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>());
        return newEnemy;
    }

    public void SpawnWormholes(int count, Vector2[] positions)
    {
        for (int i = 0; i < count; i++)
        {           
            WormholeHandler wh = Instantiate(_wormholePrefab, 
                positions[i], Quaternion.identity).GetComponent<WormholeHandler>();
            _wormholesOnLevel.Add(wh);
            wh.Initialize(_levelLibrary.GetRandomLevel());
            wh.OnPlayerEnterWormhole += ReactToPlayerEnteringWormhole;
            wh.OnPlayerExitWormhole += ReactToPlayerExitingWormhome;
        }
    }

    public void SpawnWormholes(int count)
    {
        List<Vector3> existingPoints = new List<Vector3>();
        existingPoints.Add(Vector3.zero);

        for (int i = 0; i < count; i++)
        {
            
            Vector3 pos = CUR.GetRandomPosWithinArenaAwayFromOtherPoints(Vector3.zero, ArenaRadius,
                existingPoints, _minSeparationBetweenWormholes);
            WormholeHandler wh = Instantiate(_wormholePrefab, pos, Quaternion.identity).GetComponent<WormholeHandler>();
            _wormholesOnLevel.Add(wh);
            existingPoints.Add(wh.transform.position);
            wh.Initialize(_levelLibrary.GetRandomLevel());
            wh.OnPlayerEnterWormhole += ReactToPlayerEnteringWormhole;
            wh.OnPlayerExitWormhole += ReactToPlayerExitingWormhome;
        }
        
    }

    #endregion

    #region Clearing Level

    private void ClearLevel()
    {
        WarpingOutFromOldLevel?.Invoke();
        ClearAllEnemiesFromLevel();
        ClearAllWormholesFromLevel();
        
    }


    [ContextMenu("Clear All Enemies")]
    private void ClearAllEnemiesFromLevel()
    {
        for (int i = _enemiesOnLevel.Count - 1; i >= 0; i--)
        {
            Destroy(_enemiesOnLevel[i]);
            _enemiesOnLevel.Remove(_enemiesOnLevel[i]);
        }
    }

    public void ClearAllWormholesFromLevel()
    {
        for (int i = 0; i < _wormholesOnLevel.Count; i++)
        {
            _wormholesOnLevel[i].OnPlayerEnterWormhole -= ReactToPlayerEnteringWormhole;
            _wormholesOnLevel[i].OnPlayerExitWormhole -= ReactToPlayerExitingWormhome;
            Destroy(_wormholesOnLevel[i].gameObject);
        }
        _wormholesOnLevel.Clear();
    }

    #endregion

    #region Crate Spawning

    public void SpawnCrateAtLocation(Vector2 location, SystemWeaponLibrary.SystemType systemType)
    {
        Destroy(_crateOnLevel);

        GameObject go = Instantiate(_cratePrefab);
        Sprite icon = _systemWeaponLibrary.GetIcon(systemType);
        string crateName = _systemWeaponLibrary.GetName(systemType);
        go.GetComponent<SystemCrateHandler>().Initialize(icon,
            SystemWeaponLibrary.WeaponType.None, systemType, crateName);

        go.transform.position = location;

        _crateOnLevel = go;
    }

    public void SpawnCrateAtLocation(Vector2 location, SystemWeaponLibrary.WeaponType weaponType)
    {
        Destroy(_crateOnLevel);

        GameObject go = Instantiate(_cratePrefab);
        Sprite icon = _systemWeaponLibrary.GetIcon(weaponType);
        string crateName = _systemWeaponLibrary.GetName(weaponType);
        go.GetComponent<SystemCrateHandler>().Initialize(icon,
            weaponType, SystemWeaponLibrary.SystemType.None, crateName);

        go.transform.position = location;

        _crateOnLevel = go;
    }




    #endregion


    #region Enemy Public Manager Methods

    public void DeregisterDeadEnemy(EnemyRegistrationHandler deadEnemy)
    {
        _enemiesOnLevel.Remove(deadEnemy);
        EnemyLevelCountChanged?.Invoke(_enemiesOnLevel.Count);
    }

    #endregion

    #region Helpers

    #endregion

}
