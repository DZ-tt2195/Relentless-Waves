using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Translator : MonoBehaviour
{
    public static Translator inst;

    TextAsset[] languageFiles;
    Dictionary<string, Dictionary<string, string>> keyTranslate = new();

    Level[] listOfLevels;
    BaseEnemy[] enemiesToSpawn;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);

            //Debug.Log(string.Format($"hi {0}", "lol"));
            //Debug.Log(string.Format($"hi {{0}}", "lol"));

            listOfLevels = Resources.LoadAll<Level>("Levels");
            enemiesToSpawn = Resources.LoadAll<BaseEnemy>("Enemies");

            languageFiles = Resources.LoadAll<TextAsset>("Languages");
            foreach (TextAsset language in languageFiles)
            {
                (bool success, string converted) = ConvertTxtName(language);
                if (success)
                {
                    Dictionary<string, string> newDictionary = new();
                    keyTranslate.Add(converted, newDictionary);
                    string[] lines = language.text.Split('\n');

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('=');
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        newDictionary[key] = value;
                    }
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public string GetText(string key, params object[] args)
    {
        try
        {
            return string.Format(keyTranslate[PlayerPrefs.GetString("Language")][key], args);
        }
        catch
        {
            return string.Format(keyTranslate["English"][key], args);
        }
    }

    public BaseEnemy RandomEnemy()
    {
        return enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)];
    }

    public Level CurrentLevel()
    {
        return listOfLevels[PlayerPrefs.GetInt("Current Level")];
    }

    public (bool, string) ConvertTxtName(TextAsset asset)
    {
        string pattern = @"^\d+\.\s*(.+)$";
        Match match = Regex.Match(asset.name, pattern);
        if (match.Success)
            return (true, match.Groups[1].Value);
        else
            return (false, "");
    }
}
