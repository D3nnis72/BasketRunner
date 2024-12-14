using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float maxTimeScale = 2.0f;
    [SerializeField] private float baseTimeScale = 1.0f;

    private int currentScore = 0;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        ResetTimeScale();
    }

    public void UpdateScore(int score)
    {
        currentScore = score;
        UpdateTimeScale();
    }

    private void UpdateTimeScale()
    {
        // Use an AnimationCurve to control difficulty scaling
        float newTimeScale = speedCurve.Evaluate(currentScore);

        Time.timeScale = Mathf.Clamp(newTimeScale, baseTimeScale, maxTimeScale);
        Debug.Log("UpdateTimeScale | baseTimeScale | " + baseTimeScale);
        Debug.Log("UpdateTimeScale | speedCurve.Evaluate(currentScore) | " + speedCurve.Evaluate(currentScore));
        Debug.Log("UpdateTimeScale | newTimeScale | " + newTimeScale);
        Debug.Log("UpdateTimeScale | timeScale | " + Time.timeScale);
    }

    public void ResetTimeScale()
    {
        Time.timeScale = baseTimeScale;
    }

    private void OnDestroy()
    {
        ResetTimeScale(); // Reset time scale when the TimeManager is destroyed
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple TimeManager instances found. Destroying duplicate!");
            Destroy(gameObject);
        }
    }
}
