using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int targetScore = 50;
    public CoinSpawner coinSpawner;
    public TMP_Text scoreText;
    public GameObject winPanel;
    public string mainMenuSceneName = "MainMenu";

    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private Rigidbody player;
    [SerializeField, Min(1)] private int startingHealth = 3;

    private int health;
    private float nextDamageTime;

    public bool HasWon => score >= targetScore;
    public bool GameEnded { get; private set; }
    public Rigidbody Player => player;

    void Start()
    {
        Time.timeScale = 1f;
        health = startingHealth;
        winPanel.SetActive(false);
        scoreText.raycastTarget = false;
        UpdateHUD();
        StartCoroutine(SpawnChaser());
    }

    IEnumerator SpawnChaser()
    {
        yield return new WaitForSeconds(4f);
        if (GameEnded || player == null) yield break;

        Vector3 position = player.position;
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.name = "Chaser";
        enemy.transform.position = new Vector3(
            position.x >= 0f ? -7f : 7f,
            0.65f,
            position.z >= 0f ? -7f : 7f);
        enemy.transform.localScale = Vector3.one * 1.3f;
        enemy.GetComponent<Collider>().isTrigger = true;

        Rigidbody body = enemy.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        Renderer renderer = enemy.GetComponent<Renderer>();
        renderer.sharedMaterial = player.GetComponent<Renderer>().sharedMaterial;
        renderer.material.color = new Color(1f, 0.12f, 0.16f);
        enemy.AddComponent<EnemyChase>().Initialize(this, body, player);
    }

    public void AddScore(int amount)
    {
        if (GameEnded || HasWon) return;

        score += amount;
        UpdateHUD();

        if (HasWon)
        {
            EndGame(true);
        }
    }

    public void TakeDamage(Vector3 direction)
    {
        if (GameEnded || Time.time < nextDamageTime) return;

        nextDamageTime = Time.time + 1.25f;
        health--;
        UpdateHUD();

        if (health == 0)
        {
            EndGame(false);
            return;
        }

        player.AddForce(direction * 6f + Vector3.up * 3f, ForceMode.VelocityChange);
    }

    void UpdateHUD()
    {
        scoreText.text = $"SCORE: {score} / {targetScore}\nHEALTH: {health} / {startingHealth}";
    }

    void EndGame(bool won)
    {
        GameEnded = true;
        if (coinSpawner != null) coinSpawner.StopSpawning();
        resultTitle.text = won ? "YOU WIN!!!" : "YOU LOSE";
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
