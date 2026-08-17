using UnityEngine;

public static class PrefManager
{
    public const string Difficulty = nameof(Difficulty);
    public static float GetDifficulty() => PlayerPrefs.GetFloat(Difficulty);
    public static void SetDifficulty(float value) => PlayerPrefs.SetFloat(Difficulty, value);

    public static int GetScore(string levelName) => PlayerPrefs.GetInt($"{levelName} - Best Score");
    public static void SetScore(string levelName, int value) => PlayerPrefs.SetInt($"{levelName} - Best Score", value);

    public const string CurrentLevel = nameof(CurrentLevel);
    public static int GetCurrentLevel() => PlayerPrefs.GetInt(CurrentLevel);
    public static void SetCurrentLevel(int value) => PlayerPrefs.SetInt(CurrentLevel, value);

    public static int GetRule(int number) => PlayerPrefs.HasKey($"Rule {number}") ? PlayerPrefs.GetInt($"Rule {number}") : -1;
    public static void SetRule(int number, int value) => PlayerPrefs.SetInt($"Rule {number}", value);

}