using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //Refs
    GameController _gameController;
    SystemWeaponLibrary _systemWeaponLibrary;
    EnemyLibrary _enemyLibrary;
    LevelLibrary _levelLibrary;
    PlayerSystemHandler _playerSystemHandler;
    CircleEdgeCollider2D _arenaEdgeCollider;

    public enum AsteroidAmounts { None, Sparse, Medium, Heavy };
    public enum NebulaAmounts { None, Sparse, Medium, Heavy };

    //settings 
    int _startingThreatBudget = 2;
    float _minSeparationBetweenWormholes = 20f;
    float _wormholeSelectionTime = 5f;
    [SerializeField] GameObject _cratePrefab = null;
    [SerializeField] GameObject _wormholePrefab = null;

    //state
    public Level _currentLevel;
    public int CurrentLevelNumber;
    public int CurrentThreatBudget;

    List<GameObject> _enemiesOnLevel = new List<GameObject>();
    List<GameObject> _asteroidsOnLevel = new List<GameObject>();
    List<GameObject> _nebulaOnLevel = new List<GameObject>();
    List<WormholeHandler> _wormholesOnLevel = new List<WormholeHandler>();
    GameObject _crateOnLevel;

    WormholeHandler _selectedWormhole;
    float _timeToSelectWormhole = Mathf.Infinity;

    public float ArenaRadius { get; private set; }


    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _gameController.OnPlayerSpawned += ReactToPlayerSpawned;
        _enemyLibrary = FindObjectOfType<EnemyLibrary>();
        _levelLibrary = FindObjectOfType<LevelLibrary>();
        _systemWeaponLibrary = _levelLibrary.GetComponent<SystemWeaponLibrary>();   

        CurrentThreatBudget = _startingThreatBudget;
        CurrentLevelNumber = 1;

        _arenaEdgeCollider = FindObjectOfType<CircleEdgeCollider2D>();
        ArenaRadius = _arenaEdgeCollider.Radius;

    }

    private void Start()
    {

    }
    private void ReactToPlayerSpawned(GameObject player)
    {
        _playerSystemHandler = player.GetComponent<PlayerSystemHandler>();
        LoadNewRandomLevel();
    }

    #region Flow

    private void LoadNewRandomLevel()
    {
        ClearLevel();

        _gameController.Player.transform.position = Vector3.zero;

        _currentLevel = _levelLibrary.GetRandomLevel();
        Debug.Log($"Entering new level: {_currentLevel.name} ");

        SpawnWormholes(3);
        SpawnEnemiesInNewSector_Debug();         // Populate Enemies according threat budget, add to list

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
        _timeToSelectWormhole = Time.time + _wormholeSelectionTime;
        Debug.Log("Player in wormhole");
    }

    private void Update()
    {
        if (_selectedWormhole && Time.time >= _timeToSelectWormhole)
        {
            LoadNewRandomLevel();
        }
    }

    private void ReactToPlayerExitingWormhome(WormholeHandler wh)
    {
        _selectedWormhole = null;
        //_timeToSelectWormhole = Mathf.Infinity;
        Debug.Log("Player exits wormhole");
    }
    #endregion

    #region Registers
    private void RegisterEnemy(GameObject enemy)

    {
        if (_enemiesOnLevel.Contains(enemy))
        {
            Debug.Log($"already registered this enemy: {enemy}");
        }
        else
        {
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

    [ContextMenu("SpawnRandomEnemies")]
    public void SpawnEnemiesInNewSector_Debug()
    {
        int budget = CurrentThreatBudget;
        bool hasNebula = (_currentLevel.NebulaAmount == NebulaAmounts.None) ? false : true;
        bool hasAsteroids = (_currentLevel.AsteroidAmount == AsteroidAmounts.None) ? false : true;

        List<GameObject> enemiesToMake = _enemyLibrary.CreateRandomMenuFromBudget(
            budget, hasNebula, hasAsteroids);

        foreach (var enemy in enemiesToMake)
        {
            Vector2 pos = CUR.FindRandomPointWithinDistance(Vector2.zero, ArenaRadius);
            Quaternion rot = Quaternion.identity;
            GameObject newEnemy = Instantiate(enemy, pos, rot);
            RegisterEnemy(newEnemy);
        }
    }

    public void SpawnSingleLevelEnemy(int index)
    {
        GameObject enemy = _enemyLibrary.GetEnemyOfType((EnemyInfoHolder.EnemyType)index);
        Vector2 pos = CUR.FindRandomPointWithinDistance(Vector2.zero, ArenaRadius);
        Quaternion rot = Quaternion.LookRotation(pos, Vector3.forward);
        GameObject newEnemy = Instantiate(enemy, pos, rot);
        RegisterEnemy(newEnemy);
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

    public void SpawnSpecificSystemCrateNearPlayer(SystemWeaponLibrary.SystemType systemType)
    {
        Destroy(_crateOnLevel);

        GameObject go = Instantiate(_cratePrefab);
        Sprite icon = _systemWeaponLibrary.GetIcon(systemType);
        string crateName = _systemWeaponLibrary.GetName(systemType);
        go.GetComponent<SystemCrateHandler>().Initialize(icon,
            SystemWeaponLibrary.WeaponType.None, systemType, crateName);

        Vector3 offset = (UnityEngine.Random.insideUnitCircle.normalized * 3.0f);
        go.transform.position = _gameController.Player.transform.position + offset;
    }

    [ContextMenu("Spawn Weapon Crate Near Player")]
    public void SpawnRandomWeaponCrateNearPlayer()
    {
     
        List<SystemWeaponLibrary.WeaponType> weaponsAlreadyInstalled =
            _playerSystemHandler.GetSecondaryWeaponTypesOnBoard();

        SystemWeaponLibrary.WeaponType weaponInCrate =
            _systemWeaponLibrary.GetRandomUninstalledSecondaryWeaponType(weaponsAlreadyInstalled);

        GameObject go = Instantiate(_cratePrefab);
        Sprite icon = _systemWeaponLibrary.GetIcon(weaponInCrate);
        string crateName = _systemWeaponLibrary.GetName(weaponInCrate);
        go.GetComponent<SystemCrateHandler>().Initialize(
            icon, weaponInCrate, SystemWeaponLibrary.SystemType.None, crateName);

        Vector3 offset = (UnityEngine.Random.insideUnitCircle.normalized * 3.0f);
        go.transform.position = _gameController.Player.transform.position + offset;
    }

    [ContextMenu("Spawn System Crate Near Player")]
    public void SpawnRandomSystemCrateNearPlayer()
    {
        List<SystemWeaponLibrary.SystemType> systemsAlreadyInstalled =
            _playerSystemHandler.GetSystemTypesOnBoard();

        SystemWeaponLibrary.SystemType systemInCrate =
            _systemWeaponLibrary.GetRandomUninstalledSystemType(systemsAlreadyInstalled);

        GameObject go = Instantiate(_cratePrefab);
        Sprite icon = _systemWeaponLibrary.GetIcon(systemInCrate);
        string crateName = _systemWeaponLibrary.GetName(systemInCrate);
        go.GetComponent<SystemCrateHandler>().Initialize(icon, 
            SystemWeaponLibrary.WeaponType.None, systemInCrate, crateName);

        Vector3 offset = (UnityEngine.Random.insideUnitCircle.normalized * 3.0f);
        go.transform.position = _gameController.Player.transform.position + offset;
    }


    #endregion

    #region Helpers

    #endregion

}
