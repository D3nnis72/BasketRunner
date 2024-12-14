using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyObject _enemyPrefab;
    [SerializeField] private Transform _swordTransform;
    [SerializeField] private float _spawnInterval = 2f;

    [Header("Enemy Types")]
    [SerializeField] private EnemyType[] _enemyTypes;

    [Header("Player Settings")]
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _followOffset = new Vector3(0, 10, 5);

    [Header("Lane Settings")]
    [SerializeField] private LaneManager _laneManager;

    private Coroutine _spawnCoroutine;
    private ObjectPool<EnemyObject> _enemyPool;

    private const int InitialPoolSize = 10;
    private const float InitialDelay = 1f;


    private void Start()
    {
        _enemyPool = new ObjectPool<EnemyObject>(_enemyPrefab, InitialPoolSize, transform);
    }

    public void StartSpawn()
    {
        ResetSpawner();
        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void ResetSpawner()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void Update()
    {
        if (_player != null)
        {
            FollowPlayer();
        }
        else
        {
            Debug.LogError("Player is not assigned to EnemyManager!");
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(InitialDelay);

        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameActive)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetSpawnPosition();
        EnemyObject enemyObject = _enemyPool.GetFromPool();

        enemyObject.transform.position = spawnPosition;

        EnemyType randomEnemyType = GetRandomEnemyType();
        enemyObject.InitializeObject(randomEnemyType, _player);

        enemyObject.GetComponent<SliceManager>().sword = _swordTransform;

        enemyObject.OnDestroyed += HandleEnemyDestroyed;
    }

    private Vector3 GetSpawnPosition()
    {
        if (_laneManager == null)
        {
            Debug.LogError("LaneManager is not assigned in EnemyManager.");
            return transform.position;
        }

        float laneX = _laneManager.GetRandomLanePosition();
        return new Vector3(laneX, transform.position.y, transform.position.z);
    }

    private EnemyType GetRandomEnemyType()
    {
        int randomIndex = Random.Range(0, _enemyTypes.Length);
        return _enemyTypes[randomIndex];
    }

    private void FollowPlayer()
    {
        transform.position = new Vector3(transform.position.x, _player.position.y + _followOffset.y, _player.position.z + _followOffset.z);
    }

    private void HandleEnemyDestroyed(EnemyObject enemyObject)
    {
        enemyObject.OnDestroyed -= HandleEnemyDestroyed;
        _enemyPool.ReturnToPool(enemyObject);
    }
}
