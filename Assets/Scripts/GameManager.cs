using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int score = 0;
    public int targetScore = 50;

    [Header("References")]
    public CoinSpawner coinSpawner;
    public TMP_Text scoreText;

    [Header("Win Menu")]
    public GameObject winPanel;
    public string mainMenuSceneName = "MainMenu";

    public bool HasWon => score >= targetScore;

    void Start()
    {
        Time.timeScale = 1f;
        winPanel.SetActive(false);
        UpdateHUD();
    }

    public void AddScore(int amount)
    {
        if (HasWon) return;

        score += amount;
        Debug.Log("Score: " + score);

        UpdateHUD();

        if (HasWon)
        {
            WinGame();
        }
    }

    void UpdateHUD()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "SCORE: " + score + " / " + targetScore;
        }
    }

    void WinGame()
    {
        Debug.Log("You Win!");

        if (coinSpawner != null)
        {
            coinSpawner.StopSpawning();
        }

        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
