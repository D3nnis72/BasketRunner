using System.Collections.Generic;
using UnityEngine;

public class SegmentsManager : MonoBehaviour
{
    [Header("Segment Settings")]
    [SerializeField] private GameObject[] _segmentPrefabs;
    [SerializeField] private int _poolSize = 8;
    [SerializeField] private float _segmentLength = 12f;
    [SerializeField] private float _safeZone = 24f;

    private Queue<GameObject> _segmentPool;
    private Transform _playerTransform;
    private float _lastSpawnZ;
    private float _recycleThresholdZ;

    private void Start()
    {
        InitializePlayerTransform();
        InitializeSegmentPool();
        SpawnInitialSegments();
        SetInitialRecycleThreshold();
    }

    private void Update()
    {
        DrawDebugLines();
        ManageSegments();
    }

    private void InitializePlayerTransform()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene.");
        }
    }

    private void InitializeSegmentPool()
    {
        _segmentPool = new Queue<GameObject>();

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject segment = InstantiateRandomSegment();
            segment.SetActive(false);
            _segmentPool.Enqueue(segment);
        }
    }

    private GameObject InstantiateRandomSegment()
    {
        int randomIndex = Random.Range(0, _segmentPrefabs.Length);
        return Instantiate(_segmentPrefabs[randomIndex]);
    }

    private void SpawnInitialSegments()
    {
        _lastSpawnZ = -_segmentLength;

        for (int i = 0; i < _poolSize; i++)
        {
            SpawnSegment();
        }
    }

    private void SetInitialRecycleThreshold()
    {
        _recycleThresholdZ = _segmentLength;
    }

    private void ManageSegments()
    {
        if (ShouldSpawnNewSegment())
        {
            SpawnSegment();
        }

        if (ShouldRecycleSegment())
        {
            RecycleSegment();
            _recycleThresholdZ += _segmentLength;
        }
    }

    private bool ShouldSpawnNewSegment()
    {
        return _playerTransform.position.z + _safeZone > _lastSpawnZ;
    }

    private bool ShouldRecycleSegment()
    {
        return _playerTransform.position.z > _recycleThresholdZ;
    }

    private void SpawnSegment()
    {
        GameObject segment = _segmentPool.Dequeue();
        segment.SetActive(true);
        segment.transform.position = new Vector3(0, 0, _lastSpawnZ);
        _lastSpawnZ += _segmentLength;
        _segmentPool.Enqueue(segment);
    }

    private void RecycleSegment()
    {
        GameObject oldSegment = _segmentPool.Peek();

        if (oldSegment.transform.position.z + _segmentLength < _playerTransform.position.z)
        {
            oldSegment = _segmentPool.Dequeue();
            oldSegment.SetActive(false);
            _segmentPool.Enqueue(oldSegment);
        }
    }

    private void DrawDebugLines()
    {
        DrawDebugLine(_lastSpawnZ, Color.green);
        DrawDebugLine(_recycleThresholdZ, Color.red);
        DrawDebugLine(_playerTransform.position.z, Color.blue);
    }

    private void DrawDebugLine(float zPosition, Color color)
    {
        Debug.DrawLine(
            new Vector3(-5, 1, zPosition),
            new Vector3(5, 1, zPosition),
            color
        );
    }
}