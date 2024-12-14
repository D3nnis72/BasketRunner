using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowStartScreen()
    {
        startScreenPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowGamePanel()
    {
        gamePanel.SetActive(true);
        startScreenPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void DisplayFinalScore(int finalScore, int highScore)
    {
        finalScoreText.text = $"Final Score: {finalScore}";
        highScoreText.text = $"High Score: {highScore}";
    }
}
