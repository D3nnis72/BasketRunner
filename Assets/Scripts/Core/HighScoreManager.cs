using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    private void Start()
    {
        LoadHighScore();
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            SaveHighScore();
        }
    }

    public void SetNewScore(int newScore)
    {
        if (newScore > HighScore)
        {
            HighScore = newScore;
            SaveHighScore();
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
        Debug.Log($"High score saved: {HighScore}");
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void ResetHighScore()
    {
        HighScore = 0;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
    }
}
