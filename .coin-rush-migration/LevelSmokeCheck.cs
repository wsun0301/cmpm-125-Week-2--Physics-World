using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class LevelSmokeCheck
{
    static int level;
    static int phase;
    static double until;
    static readonly Dictionary<string, float> savedFloats = new Dictionary<string, float>();
    static readonly Dictionary<string, int> savedInts = new Dictionary<string, int>();
    static readonly HashSet<string> absent = new HashSet<string>();
    static readonly List<string> results = new List<string>();
    static LevelSmokeCheck() { EditorApplication.update += Tick; }

    [MenuItem("Tools/Check Coin Rush Levels")]
    static void Run()
    {
        if (!EditorApplication.isPlaying) throw new Exception("Enter Play mode first.");
        for (int i = 0; i < 3; i++)
        {
            string key = "CoinRush.Level" + i;
            foreach (string suffix in new[] { ".Best", ".Last", ".Run" + (LevelProgress.Wins(i) + 1) })
            {
                string full = key + suffix;
                if (PlayerPrefs.HasKey(full)) savedFloats[full] = PlayerPrefs.GetFloat(full);
                else absent.Add(full);
            }
            if (PlayerPrefs.HasKey(key + ".Wins")) savedInts[key + ".Wins"] = LevelProgress.Wins(i);
            else absent.Add(key + ".Wins");
        }
        level = 0;
        phase = 1;
        Load();
    }

    static void Load()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(LevelProgress.Scenes[level]);
        until = EditorApplication.timeSinceStartup + 7.2;
    }

    static void Check(bool value, string message) { if (!value) throw new Exception(message); }

    static void Tick()
    {
        if (phase == 0 || EditorApplication.timeSinceStartup < until) return;
        try
        {
            Check(EditorApplication.isPlaying, "Play mode ended during checks.");
            GameManager manager = UnityEngine.Object.FindAnyObjectByType<GameManager>();
            Check(manager != null, "Missing manager");
            if (phase == 1)
            {
                Check(manager.targetScore == LevelProgress.Targets[level], "Wrong score target");
                Check(UnityEngine.Object.FindObjectsByType<EnemyChase>(FindObjectsSortMode.None).Length == LevelProgress.Enemies[level], "Wrong enemy count");
                Check(manager.Arena.transform.childCount == new[] { 2, 4, 8 }[level], "Wrong wall count");
                foreach (Coin coin in UnityEngine.Object.FindObjectsByType<Coin>(FindObjectsSortMode.None))
                    Check(manager.Arena.IsClear(coin.transform.position, 0.6f), "Coin inside wall");
                float threshold = LevelProgress.ThreeStarTimes[level];
                Check(LevelProgress.Stars(level, threshold) == 3, "Three-star boundary");
                Check(LevelProgress.Stars(level, threshold + 0.1f) == 2, "Two-star boundary");
                Check(LevelProgress.Stars(level, LevelProgress.TwoStarTimes[level] + 0.1f) == 1, "One-star boundary");
                int wins = LevelProgress.Wins(level);
                manager.AddScore(manager.targetScore);
                Check(manager.GameEnded && manager.HasWon && manager.winPanel.activeSelf, "Win screen failed");
                Check(LevelProgress.Wins(level) == wins + 1 && LevelProgress.LastTime(level) > 0, "Win was not saved");
                Check(LevelProgress.BestTime(level) <= LevelProgress.LastTime(level), "Best time incorrect");
                manager.RestartGame();
                phase = 2;
                until = EditorApplication.timeSinceStartup + 0.8;
            }
            else
            {
                Check(!manager.GameEnded && manager.score == 0 && Time.timeScale == 1, "Restart failed");
                results.Add("PASS level " + (level + 1) + ": layout, enemies, coins, star boundaries, win persistence, restart");
                level++;
                if (level < 3) { phase = 1; Load(); }
                else Finish(null);
            }
        }
        catch (Exception error) { Finish(error.ToString()); }
    }

    static void Finish(string error)
    {
        phase = 0;
        foreach (var pair in savedFloats) PlayerPrefs.SetFloat(pair.Key, pair.Value);
        foreach (var pair in savedInts) PlayerPrefs.SetInt(pair.Key, pair.Value);
        foreach (string key in absent) PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        if (error != null) results.Add("FAIL " + error);
        File.WriteAllLines("E:/CoinRush/.coin-rush-migration/level-checks.txt", results);
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
        Debug.Log(string.Join("\n", results));
    }
}
