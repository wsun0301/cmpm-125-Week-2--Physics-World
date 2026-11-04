using UnityEngine;

public static class LevelProgress
{
    public const int Count = 3;
    public static readonly string[] Scenes = { "GameScene", "Level2", "Level3" };
    public static readonly string[] Names = { "Training Grounds", "Crossroads", "The Gauntlet" };
    public static readonly int[] Targets = { 30, 60, 100 };
    public static readonly int[] Enemies = { 2, 4, 6 };
    public static readonly float[] ThreeStarTimes = { 45f, 80f, 120f };
    public static readonly float[] TwoStarTimes = { 75f, 125f, 180f };

    public static int Stars(int level, float seconds)
    {
        if (seconds <= ThreeStarTimes[level]) return 3;
        if (seconds <= TwoStarTimes[level]) return 2;
        return 1;
    }

    public static float BestTime(int level) => PlayerPrefs.GetFloat("CoinRush.Level" + level + ".Best", -1f);
    public static float LastTime(int level) => PlayerPrefs.GetFloat("CoinRush.Level" + level + ".Last", -1f);
    public static int Wins(int level) => PlayerPrefs.GetInt("CoinRush.Level" + level + ".Wins", 0);

    public static void SaveWin(int level, float seconds)
    {
        string key = "CoinRush.Level" + level;
        int wins = Wins(level) + 1;
        // Keep every successful run, as well as the fastest and most recent time.
        PlayerPrefs.SetFloat(key + ".Run" + wins, seconds);
        PlayerPrefs.SetInt(key + ".Wins", wins);
        PlayerPrefs.SetFloat(key + ".Last", seconds);
        if (BestTime(level) < 0f || seconds < BestTime(level))
            PlayerPrefs.SetFloat(key + ".Best", seconds);
        PlayerPrefs.Save();
    }

    public static string TimeLabel(float seconds) => seconds.ToString("0.0") + "s";
}
