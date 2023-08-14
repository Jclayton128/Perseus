using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //Refs
    GameController _gameController;
    RunController _runController;
    ScrapController _scrapController;
    AsteroidPoolController _asteroidPoolController;
    SystemWeaponLibrary _systemWeaponLibrary;
    EnemyLibrary _enemyLibrary;
    LevelLibrary _levelLibrary;
    ProjectilePoolController _projectilePoolController;
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
    Level _currentLevel;
    public Level CurrentLevel => _currentLevel;
    [SerializeField] Color _filterColor = Color.white;
    Tween _filterTween;

    List<EnemyRegistrationHandler> _enemiesOnLevel = new List<EnemyRegistrationHandler>();
    List<WormholeHandler> _wormholesOnLevel = new List<WormholeHandler>();
    List<Vector2> _wormholeLocationsOnLevel = new List<Vector2>();
    public List<Vector2> WormholeLocations => _wormholeLocationsOnLevel;
    GameObject _crateOnLevel;
    public GameObject CrateOnLevel => _crateOnLevel;

    WormholeHandler _selectedWormhole = null;
    float _timeToSelectWormhole = 0;

    public float ArenaRadius => _arenaEdgeCollider.Radius;
    GameObject _currentBoss;


    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _runController = GetComponent<RunController>();
        _scrapController = GetComponent<ScrapController>();
        _projectilePoolController = GetComponent<ProjectilePoolController>();
        _asteroidPoolController = GetComponent<AsteroidPoolController>();
        _gameController.PlayerSpawned += ReactToPlayerSpawned;
        _enemyLibrary = FindObjectOfType<EnemyLibrary>();
        _levelLibrary = FindObjectOfType<LevelLibrary>();
        _systemWeaponLibrary = _levelLibrary.GetComponent<SystemWeaponLibrary>();
        _arenaEdgeCollider = FindObjectOfType<CircleEdgeCollider2D>();

    }


    private void ReactToPlayerSpawned(GameObject player)
    {
        _playerSystemHandler = player.GetComponent<PlayerSystemHandler>();
        _currentBoss = null;
    }


    #region Entry Points to Level sequences

    public void StartGameWithTutorial()
    {
        _currentLevel = _tutorialLevel;
        BuildTutorialLevel();
        Debug.Log($"Entering level: {_currentLevel.name} ");
    }

    public void StartGameWithRegular()
    {
        LoadLevelOfCurrentWormhole();
    }

    #endregion


    #region Flow

    private void LoadLevelOfCurrentWormhole()
    {
        _levelLibrary.RemoveBeatenBossLevel(_currentLevel);

        if (_selectedWormhole) _currentLevel = _selectedWormhole.AssociatedLevel;
        else _currentLevel = _levelLibrary.GetRandomLevel();

        ClearLevel();
        WarpIntoNewLevel();
        BuildNewLevel();
    }


    private void BuildTutorialLevel()
    {
        Vector2[] tutorialWormholePosition = new Vector2[1] {new Vector2(0,30f)};
        SpawnWormholeForTutorialLevel(1, tutorialWormholePosition);
        _wormholesOnLevel[0].gameObject.SetActive(false);
        SpawnTutorialEnemy();
    }
    private void WarpIntoNewLevel()
    {
        WarpingIntoNewLevel?.Invoke();
        _runController.IncrementSectorCount();

        _gameController.Player.transform.position =
            CUR.FindRandomPositionWithinRangeBandAndWithinArena(
                Vector2.zero, 20f, 40f, Vector2.zero, ArenaRadius);

        _filterTween.Kill();
        _filterTween = DOTween.To(() =>
            _filterSR.color, x => _filterSR.color = x, _currentLevel.BackgroundColor, 0.5f);

        //_currentLevel = _selectedWormhole.AssociatedLevel;// _levelLibrary.GetRandomLevel();
        Debug.Log($"Entering level: {_currentLevel.name} ");
        WarpedIntoNewLevel?.Invoke(_currentLevel);
        //Player should listen in to this^ to recharge energy, shields, and systems, and reduce profile

        //TODO ripping audio sound for warp in;
        MusicController.Instance.PlayNewMusic(_currentLevel.Music);
    }

    private void BuildNewLevel()
    {
        SpawnWormholesForNormalLevel();

        if (_runController.CheckIfIsBossLevel())
        {
            SpawnBossInNewSector();
            AssignRewardSystemToFirstEnemy();
        }
        else
        {
            SpawnEnemiesInNewSector();
            AssignRewardSystemToRandomEnemy();
        }


        _asteroidPoolController.SpawnInitialAsteroids(_currentLevel);
        //Spawn Nebula too?
    }

   
    private void ReactToPlayerEnteringWormhole(WormholeHandler wh)
    {
        _selectedWormhole = wh;
        _timeToSelectWormhole = 0;
        //Debug.Log("Player in wormhole");
        MusicController.Instance.BeginWarpInMusic();
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
                LoadLevelOfCurrentWormhole();
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
        MusicController.Instance.CeaseWarp();
    }
    #endregion


    #region Registers
    private void RegisterEnemy(EnemyRegistrationHandler enemy, bool shouldUpdateRadarCounter)
    {
        //if (enemy == null) Debug.Log($"Attempt to register an enemy was null");
        if (_enemiesOnLevel.Contains(enemy))
        {
            Debug.Log($"already registered this enemy: {enemy}");
        }
        else
        {
            enemy.Initialize(this);
            //Debug.Log("added " + enemy);
            _enemiesOnLevel.Add(enemy);

            if (shouldUpdateRadarCounter) EnemyLevelCountChanged?.Invoke(_enemiesOnLevel.Count);
        }
    }

    #endregion

    #region Spawning New Level Items

    public void SpawnTutorialEnemy()
    {
        Vector2 tutorialEnemyPosition = new Vector2(15f, 15f);
        Quaternion rot = Quaternion.identity;
        GameObject newEnemy = Instantiate(_enemyLibrary.GetEnemyGameObjectOfType(ShipInfoHolder.ShipType.Dummy1),
            tutorialEnemyPosition, rot);
        RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>(), true);
        SpawnedLevelEnemies?.Invoke(_runController.CurrentSectorCount, _enemiesOnLevel.Count);
        //EnemyLevelCountChanged?.Invoke(_enemiesOnLevel.Count);
    }

    private void SpawnBossInNewSector()
    {
        List<GameObject> enemiesToMake =
            _enemyLibrary.CreateMenuFromBossLevel(_currentLevel);

        _currentBoss = Instantiate(enemiesToMake[0], Vector2.zero, Quaternion.identity);
        RegisterEnemy(_currentBoss.GetComponent<EnemyRegistrationHandler>(), false);

        _currentBoss.GetComponent<HealthHandler>().Dying += HandleDyingBoss;

        enemiesToMake.RemoveAt(0);
        
        foreach (var enemy in enemiesToMake)
        {
            var pos = CUR.FindRandomPointWithinDistance(Vector2.zero,
                _enemySpawnRadius_max * ArenaRadius, _enemySpawnRadius_min * ArenaRadius);
            Quaternion rot = Quaternion.identity;
            GameObject newEnemy = Instantiate(enemy, pos, rot);
            RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>(), false);
        }

        SpawnedLevelEnemies?.Invoke(_runController.CurrentSectorCount, -1);
        EnemyLevelCountChanged?.Invoke(-1);
    }

    private void HandleDyingBoss()
    {
        _filterTween.Kill();
        _filterTween = DOTween.To(() =>
            _filterSR.color, x => _filterSR.color = x, Color.clear, 0.5f);
        _currentBoss.GetComponent<HealthHandler>().Dying -= HandleDyingBoss;
        _currentBoss = null;
    }

    private void SpawnEnemiesInNewSector()
    {
        List<GameObject> enemiesToMake =
            _enemyLibrary.CreateMenuFromBudgetAndLevel(_runController.CurrentThreatBudget, _currentLevel);

        foreach (var enemy in enemiesToMake)
        {
            var pos = CUR.FindRandomPointWithinDistance(Vector2.zero,
                _enemySpawnRadius_max * ArenaRadius, _enemySpawnRadius_min * ArenaRadius);
            Quaternion rot = Quaternion.identity;
            GameObject newEnemy = Instantiate(enemy, pos, rot);
            RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>(), true);
        }

        SpawnedLevelEnemies?.Invoke(_runController.CurrentSectorCount, _enemiesOnLevel.Count);
        //EnemyLevelCountChanged?.Invoke(_enemiesOnLevel.Count);
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

    private void AssignRewardSystemToFirstEnemy()
    {
        if (_enemiesOnLevel.Count == 0) return;

        GameObject chosenEnemy = _enemiesOnLevel[0].gameObject;

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
        RegisterEnemy(newEnemy.GetComponent<EnemyRegistrationHandler>(), true);
        return newEnemy;
    }

    /// <summary>
    /// This spawns a single enemy of a specific ShipType as a minion. Ships
    /// added this way do not get tracked by Level Controller.
    /// </summary>
    /// <param name="shipType"></param>
    /// <param name="spawnPoint"></param>
    /// <returns></returns>
    public GameObject SpawnMinionShipAtPoint(ShipInfoHolder.ShipType shipType, Vector3 spawnPoint)
    {
        GameObject enemy = _enemyLibrary.GetEnemyGameObjectOfType(shipType);
        float randRot = UnityEngine.Random.Range(-179, 179);
        Quaternion rot = Quaternion.Euler(0, 0, randRot);
        GameObject newEnemy = Instantiate(enemy, spawnPoint, rot);      
        return newEnemy;
    }

    public void SpawnWormholeForTutorialLevel(int count, Vector2[] positions)
    {
        for (int i = 0; i < count; i++)
        {           
            WormholeHandler wh = Instantiate(_wormholePrefab, 
                positions[i], Quaternion.identity).GetComponent<WormholeHandler>();
            _wormholesOnLevel.Add(wh);
            _wormholeLocationsOnLevel.Add(wh.transform.position);
            wh.Initialize(_levelLibrary.GetRandomLevel());
            wh.OnPlayerEnterWormhole += ReactToPlayerEnteringWormhole;
            wh.OnPlayerExitWormhole += ReactToPlayerExitingWormhome;
        }

        int breaker = 0;
        while (_wormholeLocationsOnLevel.Count < 3)
        {
            _wormholeLocationsOnLevel.Add(Vector3.one * 999f);
            breaker++;
            if (breaker > 10) break;
        }
    }



    public void SpawnWormholesForNormalLevel()
    {
        bool isBossLevel = _runController.CheckIfIsBossLevel();
        bool isPrebossLevel = _runController.CheckIfPreBossLevel();

        List<Vector3> existingPoints = new List<Vector3>();
        existingPoints.Add(Vector3.zero);

        for (int i = 0; i < 3; i++)
        {
            
            Vector3 pos = CUR.GetRandomPosWithinArenaAwayFromOtherPoints(Vector3.zero, ArenaRadius,
                existingPoints, _minSeparationBetweenWormholes);
            WormholeHandler wh = Instantiate(_wormholePrefab, pos, Quaternion.identity).GetComponent<WormholeHandler>();
            _wormholesOnLevel.Add(wh);
            _wormholeLocationsOnLevel.Add(wh.transform.position);
            existingPoints.Add(wh.transform.position);

            if (!isPrebossLevel) wh.Initialize(_levelLibrary.GetRandomLevel());
            else wh.Initialize(_levelLibrary.GetRandomBossLevel());

            if (isBossLevel) wh.gameObject.SetActive(false);

            wh.OnPlayerEnterWormhole += ReactToPlayerEnteringWormhole;
            wh.OnPlayerExitWormhole += ReactToPlayerExitingWormhome;
        }
        
    }

    #endregion

    #region Clearing Level

    public void ClearLevel()
    {
        WarpingOutFromOldLevel?.Invoke();
        _asteroidPoolController.ClearAsteroids();
        _projectilePoolController.ClearProjectiles();
        _scrapController.ClearScraps();
        ClearAllEnemiesFromLevel();
        ClearAllWormholesFromLevel();
        Destroy(_crateOnLevel);
    }


    [ContextMenu("Clear All Enemies")]
    private void ClearAllEnemiesFromLevel()
    {
        for (int i = _enemiesOnLevel.Count - 1; i >= 0; i--)
        {
            if (_enemiesOnLevel[i] == null) continue;
            //Debug.Log($"Destroying enemy #{i} out of {_enemiesOnLevel.Count}");
            Destroy(_enemiesOnLevel[i].gameObject);
            _enemiesOnLevel.Remove(_enemiesOnLevel[i]);
        }
        _enemiesOnLevel.Clear();
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
        _wormholeLocationsOnLevel.Clear();
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
        if (deadEnemy == null || !_enemiesOnLevel.Contains(deadEnemy)) return;
        _enemiesOnLevel.Remove(deadEnemy);
        EnemyLevelCountChanged?.Invoke(_enemiesOnLevel.Count);

    }

    #endregion

    #region Helpers

    /// <summary>
    /// This is used to making the tutorial enemy vulnerable
    /// </summary>
    public void WeakenTutorialEnemy()
    {
        _enemiesOnLevel[0].GetComponent<HealthHandler>().AdjustShieldMaximum(-90);
        _enemiesOnLevel[0].GetComponent<HealthHandler>().AdjustShieldHealRate(-99);
    }

    public void UnlockTutorialWormhole()
    {
        _wormholesOnLevel[0].gameObject.SetActive(true);
    }

    public void UnlockAllWormholes()
    {
        foreach (var wh in _wormholesOnLevel)
        {
            wh.gameObject.SetActive(true);
        }
    }

    #endregion

}
