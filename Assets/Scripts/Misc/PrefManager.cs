using UnityEngine;

public static class PrefManager
{
    //playerprefs
    public const string Difficulty = nameof(Difficulty);
    public static float GetDifficulty() => PlayerPrefs.GetFloat(Difficulty);
    public static void SetDifficulty(float value) => PlayerPrefs.SetFloat(Difficulty, value);

    public static int GetScore(string levelName) => PlayerPrefs.GetInt($"{levelName} - Best Score");
    public static void SetScore(string levelName, int value) => PlayerPrefs.SetInt($"{levelName} - Best Score", value);

    public const string Infinity = nameof(Infinity);
    public static int GetInfinity() => PlayerPrefs.GetInt(Infinity);
    public static void SetInfinity(int value) => PlayerPrefs.SetInt(Infinity, value);
    
    public const string Juggle = nameof(Juggle);
    public static int GetJuggle() => PlayerPrefs.GetInt(Juggle);
    public static void SetJuggle(int value) => PlayerPrefs.SetInt(Juggle, value);

    public static int CheatChallengeScore()
    {
        int effect = 0;
        if (GetJuggle() == 1)
            effect += 20;
        if (GetInfinity() == 1)
            effect -= 20;
        return effect;
    }

    public const string StartWave = nameof(StartWave);
    public static int GetStartWave() => PlayerPrefs.GetInt(StartWave);
    public static void SetStartWave(int value) => PlayerPrefs.SetInt(StartWave, value);

    public const string CurrentLevel = nameof(CurrentLevel);
    public static int GetCurrentLevel() => PlayerPrefs.GetInt(CurrentLevel);
    public static void SetCurrentLevel(int value) => PlayerPrefs.SetInt(CurrentLevel, value);

}

