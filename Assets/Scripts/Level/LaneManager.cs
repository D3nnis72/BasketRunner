using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class LaneManager : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] private int _numberOfLanes = 3;
    [SerializeField] private float _laneDistance = 2f;

    private float[] _lanePositions;

    private void Awake()
    {
        InitializeLanePositions();
    }

    private void InitializeLanePositions()
    {
        _lanePositions = GenerateLanePositions();
    }

    private float[] GenerateLanePositions()
    {
        float[] positions = new float[_numberOfLanes];
        int centerIndex = _numberOfLanes / 2;

        for (int i = 0; i < _numberOfLanes; i++)
        {
            positions[i] = (i - centerIndex) * _laneDistance;
        }

        return positions;
    }

    public float GetLanePosition(int laneIndex)
    {
        if (!IsValidLaneIndex(laneIndex))
        {
            LogInvalidLaneIndex(laneIndex);
            return 0f;
        }

        return _lanePositions[laneIndex];
    }

    public float GetRandomLanePosition()
    {
        int randomLaneIndex = UnityEngine.Random.Range(0, _numberOfLanes);
        return GetLanePosition(randomLaneIndex);
    }

    public float SnapToClosestLane(float xPosition)
    {
        return _lanePositions.OrderBy(lane => Mathf.Abs(lane - xPosition)).FirstOrDefault();
    }

    public bool CanSwitchLaneLeft(int currentLaneIndex)
    {
        return IsValidLaneIndex(currentLaneIndex - 1);
    }

    public bool CanSwitchLaneRight(int currentLaneIndex)
    {
        return IsValidLaneIndex(currentLaneIndex + 1);
    }

    private bool IsValidLaneIndex(int laneIndex)
    {
        return laneIndex >= 0 && laneIndex < _lanePositions.Length;
    }




    private void LogInvalidLaneIndex(int laneIndex)
    {
        Debug.LogError($"Invalid lane index: {laneIndex}. Valid range is 0 to {_lanePositions.Length - 1}.");
    }
}