using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;

public class Translator : MonoBehaviour
{

#region Setup

    public static Translator inst;
    Dictionary<string, Dictionary<string, string>> keyTranslate = new();

    Level[] listOfLevels;
    BaseEnemy[] enemiesToSpawn;

    string sheetURL = "19CiC2QT3GX_mW_-fsajqhnjdXPyRAB7rgwfp4-efBxQ";
    string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    private void Awake()
    {
        //Debug.Log(string.Format($"hi {0}", "lol"));
        //Debug.Log(string.Format($"hi {{0}}", "lol"));

        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);

            listOfLevels = Resources.LoadAll<Level>("Levels");
            enemiesToSpawn = Resources.LoadAll<BaseEnemy>("Enemies");

            TxtLanguages();
            StartCoroutine(DownloadLanguages());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void TxtLanguages()
    {
        TextAsset[] languageFiles = Resources.LoadAll<TextAsset>("Txt Languages");
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
                    newDictionary[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        (bool, string) ConvertTxtName(TextAsset asset)
        {
            string pattern = @"^\d+\.\s*(.+)$";
            Match match = Regex.Match(asset.name, pattern);
            if (match.Success)
                return (true, match.Groups[1].Value);
            else
                return (false, "");
        }
    }

    #endregion

#region Downloading

    IEnumerator Download(string range)
    {
        if (Application.isEditor)
        {
            string url = $"{baseUrl}{sheetURL}/values/{range}?key={apiKey}";
            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Download failed: {www.error}");
            }
            else
            {
                string filePath = $"Assets/Resources/{range}.txt";
                File.WriteAllText($"{filePath}", www.downloadHandler.text);

                string[] allLines = File.ReadAllLines($"{filePath}");
                List<string> modifiedLines = allLines.ToList();
                modifiedLines.RemoveRange(1, 3);
                File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
                Debug.Log($"downloaded {range}");
            }
        }
    }

    IEnumerator DownloadLanguages()
    {
        yield return Download("Csv Languages");
        GetLanguages(ReadFile("Csv Languages"));

        string[][] ReadFile(string range)
        {
            TextAsset data = Resources.Load($"{range}") as TextAsset;

            string editData = data.text;
            editData = editData.Replace("],", "").Replace("{", "").Replace("}", "");

            string[] numLines = editData.Split("[");
            string[][] list = new string[numLines.Length][];

            for (int i = 0; i < numLines.Length; i++)
            {
                list[i] = numLines[i].Split("\",");
            }
            return list;
        }
    }

    void GetLanguages(string[][] data)
    {
        for (int i = 1; i < data[1].Length; i++)
        {
            data[1][i] = data[1][i].Replace("\"", "").Trim();
            Dictionary<string, string> newDictionary = new();
            keyTranslate.Add(data[1][i], newDictionary);
        }

        List<string> listOfKeys = new();
        for (int i = 2; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                data[i][j] = data[i][j].Replace("\"", "").Replace("\\", "").Replace("]", "").Trim();
                if (j > 0)
                {
                    string language = data[1][j];
                    string key = data[i][0];
                    keyTranslate[language][key] = data[i][j];
                }
                else
                {
                    listOfKeys.Add(data[i][j]);
                }
            }
        }
        CreateBaseTxtFile(listOfKeys);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateBaseTxtFile(List<string> listOfKeys)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "BaseTxtFile.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string input in listOfKeys)
                writer.WriteLine($"{input}=");
        }
    }

    #endregion

#region Helpers

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
        return enemiesToSpawn[UnityEngine.Random.Range(0, enemiesToSpawn.Length)];
    }

    public Level CurrentLevel()
    {
        return listOfLevels[PlayerPrefs.GetInt("Current Level")];
    }

    public Dictionary<string, Dictionary<string, string>> GetTranslations()
    {
        return keyTranslate;
    }

    #endregion

}
