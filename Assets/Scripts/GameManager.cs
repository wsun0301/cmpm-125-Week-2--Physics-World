using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score;
    public int targetScore = 30;
    public CoinSpawner coinSpawner;
    public TMP_Text scoreText;
    public GameObject winPanel;
    public string mainMenuSceneName = "MainMenu";
    [SerializeField, Range(0, 2)] private int levelIndex;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private Rigidbody player;
    [SerializeField, Min(1)] private int startingHealth = 3;

    private int health;
    private float nextDamageTime;
    private float elapsed;
    private TMP_Text timerText;
    private Material enemyMaterial;
    private Material wallMaterial;
    public bool HasWon => score >= targetScore;
    public bool GameEnded { get; private set; }
    public Rigidbody Player => player;
    public ArenaLayout Arena { get; private set; }

    void Awake()
    {
        levelIndex = Mathf.Clamp(levelIndex, 0, LevelProgress.Count - 1);
        targetScore = LevelProgress.Targets[levelIndex];
        Material baseMaterial = player.GetComponent<Renderer>().sharedMaterial;
        wallMaterial = new Material(baseMaterial) { color = new Color(0.2f, 0.35f, 0.5f) };
        enemyMaterial = new Material(baseMaterial) { color = new Color(1f, 0.12f, 0.16f) };
        Arena = new GameObject("Arena Layout").AddComponent<ArenaLayout>();
        Arena.Build(levelIndex, wallMaterial);
        coinSpawner.spawnInterval = 1.5f;
        Physics.SyncTransforms();
    }

    void Start()
    {
        Time.timeScale = 1f;
        health = startingHealth;
        winPanel.SetActive(false);
        scoreText.raycastTarget = false;
        timerText = Instantiate(scoreText, scoreText.transform.parent);
        timerText.name = "Level Timer";
        timerText.rectTransform.anchorMin = new Vector2(1, 1);
        timerText.rectTransform.anchorMax = new Vector2(1, 1);
        timerText.rectTransform.anchoredPosition = new Vector2(-290, -74);
        timerText.rectTransform.sizeDelta = new Vector2(560, 108);
        timerText.alignment = TextAlignmentOptions.Right;
        timerText.fontSize = 30;
        resultTitle.rectTransform.sizeDelta = new Vector2(850, 190);
        resultTitle.rectTransform.anchoredPosition = new Vector2(0, 200);
        resultTitle.fontSize = 38;
        LevelSelectMenu.AddButton(winPanel.transform, "LEVEL SELECT", new Vector2(0, -370), OpenLevelSelect);
        ArcadeMenuStyle.Overlay(winPanel, ArcadeMenuStyle.Gold, new Vector2(780, 850));
        ArcadeMenuStyle.Heading(resultTitle, ArcadeMenuStyle.Gold);
        UpdateHUD();
        StartCoroutine(SpawnChasers());
    }

    void Update()
    {
        if (GameEnded) return;
        elapsed += Time.deltaTime;
        timerText.text = "LEVEL " + (levelIndex + 1) + "  /  " + LevelProgress.Names[levelIndex] +
            "\nTIME " + LevelProgress.TimeLabel(elapsed);
    }

    IEnumerator SpawnChasers()
    {
        yield return new WaitForSeconds(4f);
        Vector3[] spawnPoints = {
            new Vector3(-12, 0.65f, -12), new Vector3(12, 0.65f, 12),
            new Vector3(-12, 0.65f, 12), new Vector3(12, 0.65f, -12),
            new Vector3(0, 0.65f, -7), new Vector3(0, 0.65f, 7)
        };
        for (int i = 0; i < LevelProgress.Enemies[levelIndex]; i++)
        {
            if (GameEnded) yield break;
            Vector3 point = spawnPoints[i];
            if (Vector3.Distance(point, player.position) < 5f) point = -point;
            point.y = 0.65f;
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            enemy.name = "Chaser " + (i + 1);
            enemy.transform.position = point;
            enemy.transform.localScale = Vector3.one * 1.3f;
            enemy.GetComponent<Collider>().isTrigger = true;
            Rigidbody body = enemy.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            enemy.GetComponent<Renderer>().sharedMaterial = enemyMaterial;
            enemy.AddComponent<EnemyChase>().Initialize(this, body, player, 3.5f + levelIndex * 0.6f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AddScore(int amount)
    {
        if (GameEnded || HasWon) return;
        score += amount;
        UpdateHUD();
        if (HasWon) EndGame(true);
    }

    public void TakeDamage(Vector3 direction)
    {
        if (GameEnded || Time.timeScale == 0f || Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + 1.25f;
        health--;
        UpdateHUD();
        if (health == 0) { EndGame(false); return; }
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
        if (won)
        {
            LevelProgress.SaveWin(levelIndex, elapsed);
            resultTitle.text = "LEVEL " + (levelIndex + 1) + " COMPLETE\n" +
                LevelProgress.Stars(levelIndex, elapsed) + " / 3 STARS  |  " + LevelProgress.TimeLabel(elapsed) +
                "\nBEST " + LevelProgress.TimeLabel(LevelProgress.BestTime(levelIndex));
        }
        else resultTitle.text = "YOU LOSE\nTRY AGAIN";
        resultTitle.color = won ? ArcadeMenuStyle.Gold : ArcadeMenuStyle.Pink;
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        if (wallMaterial != null) Destroy(wallMaterial);
        if (enemyMaterial != null) Destroy(enemyMaterial);
    }
}