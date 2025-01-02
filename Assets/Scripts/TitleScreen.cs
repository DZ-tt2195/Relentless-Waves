using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] TMP_Text debugText;
    /*
    void OnEnable()
    {
        Application.logMessageReceived += DebugMessages;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= DebugMessages;
    }

    void DebugMessages(string logString, string stackTrace, LogType type)
    {
        debugText.text += ($"{logString} | {stackTrace}\n");
    }
    */
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            //Debug.Log($"Scene {i}: {sceneName}");
        }
    }
}
