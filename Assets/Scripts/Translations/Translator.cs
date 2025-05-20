using System.Collections.Generic;
using UnityEngine;

public class Translator : MonoBehaviour
{
    public static Translator inst;
    List<Dictionary<string, string>> keyTranslate = new();
    TextAsset[] languageFiles;

    Level[] listOfLevels;
    BaseEnemy[] enemiesToSpawn;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);

            listOfLevels = Resources.LoadAll<Level>("Levels");
            enemiesToSpawn = Resources.LoadAll<BaseEnemy>("Enemies");

            languageFiles = Resources.LoadAll<TextAsset>("Languages");
            foreach (TextAsset language in languageFiles)
            {
                Dictionary<string, string> newDictionary = new();
                keyTranslate.Add(newDictionary);
                string[] lines = language.text.Split('\n');

                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    newDictionary[key] = value;
                }
            }
            if (!PlayerPrefs.HasKey("Language"))
                PlayerPrefs.SetInt("Language", 0);
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
            return string.Format(keyTranslate[PlayerPrefs.GetInt("Language")][key], args);
        }
        catch
        {
            return string.Format(keyTranslate[0][key], args);
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
}
