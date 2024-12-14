using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGameActive { get; private set; } = false;

    [SerializeField] private HighScoreManager _highScoreManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyManager _enemyController;

    private int score = 0;

    private void Awake()
    {
        InitializeSingleton();
        UIManager.Instance.ShowStartScreen();
    }

    public void StartGame()
    {
        ResetGame();
        ClearEnemyObjects();
        UIManager.Instance.ShowGamePanel();
        _enemyController.StartSpawn();
    }

    public void GameOver()
    {
        if (!IsGameActive) return;

        IsGameActive = false;
        _playerController.GameOver();
        _highScoreManager.SetNewScore(score);
        UIManager.Instance.DisplayFinalScore(score, _highScoreManager.HighScore);
        UIManager.Instance.ShowGameOverPanel();
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScore();
    }

    private void UpdateScore()
    {
        UIManager.Instance.UpdateScoreText(score);
        TimeManager.Instance.UpdateScore(score);
    }

    private void InitializeSingleton()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void ResetGame()
    {
        score = 0;
        IsGameActive = true;
        UpdateScore();
        _playerController.StartGame();
    }

    private void ClearEnemyObjects()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(obj);
        }
    }
}
