using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Slider difficultySlider;
    [SerializeField] TMP_Text difficultyLabel;
    [SerializeField] Slider waveSlider;
    [SerializeField] TMP_Text waveLabel;
    [SerializeField] Slider juggleSlider;

    [SerializeField] TMP_Text bestRun;
    [SerializeField] TMP_Dropdown languageDropdown;
    [SerializeField] TMP_Dropdown levelDropdown;

    void Start()
    {
        if (!PlayerPrefs.HasKey("Difficulty")) PlayerPrefs.SetFloat("Difficulty", 1f);
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
        difficultySlider.value = PlayerPrefs.GetFloat("Difficulty");
        UpdateDifficultyText(PlayerPrefs.GetFloat("Difficulty"));

        void UpdateDifficultyText(float value)
        {
            difficultyLabel.text = $"{Translator.inst.GetText("Difficulty")}: {value * 100:F1}%";
            PlayerPrefs.SetFloat("Difficulty", value);
        }

        if (!PlayerPrefs.HasKey("Starting Wave")) PlayerPrefs.SetInt("Starting Wave", 1);
        waveSlider.onValueChanged.AddListener(UpdateWaveText);
        waveSlider.value = PlayerPrefs.GetInt("Starting Wave");
        UpdateWaveText(PlayerPrefs.GetInt("Starting Wave"));

        void UpdateWaveText(float value)
        {
            waveLabel.text = $"{Translator.inst.GetText("Start on Wave")} {(int)value}";
            PlayerPrefs.SetInt("Starting Wave", (int)value);
            Debug.Log(PlayerPrefs.GetInt("Starting Wave"));
        }

        if (!PlayerPrefs.HasKey("Juggle")) PlayerPrefs.SetInt("Juggle", 0);
        juggleSlider.onValueChanged.AddListener(ChangeJuggleSlider);
        juggleSlider.value = PlayerPrefs.GetInt("Juggle");
        ChangeJuggleSlider(PlayerPrefs.GetInt("Juggle"));

        void ChangeJuggleSlider(float value)
        {
            PlayerPrefs.SetInt("Juggle", (int)(value));
        }

        if (!PlayerPrefs.HasKey("Language")) PlayerPrefs.SetString("Language", "English");
        languageDropdown.onValueChanged.AddListener(ChangeLanguageDropdown);
        List<string> languages = Translator.inst.GetTranslations().Keys.ToList();
        for (int i = 0; i < languages.Count; i++)
        {
            string nextLanguage = languages[i];
            languageDropdown.AddOptions(new List<string>() { nextLanguage });
            if (nextLanguage.Equals(PlayerPrefs.GetString("Language")))
            {
                languageDropdown.value = i;
                ChangeLanguageDropdown(i);
            }
        }
        languageDropdown.gameObject.SetActive(languageDropdown.options.Count >= 2);

        void ChangeLanguageDropdown(int n)
        {
            string selectedText = languageDropdown.options[languageDropdown.value].text;
            if (!PlayerPrefs.GetString("Language").Equals(selectedText))
            {
                PlayerPrefs.SetString("Language", selectedText);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (!PlayerPrefs.HasKey("Current Level")) PlayerPrefs.SetInt("Current Level", 0);
        levelDropdown.onValueChanged.AddListener(ChangeLevelDropdown);
        Level[] listOfLevels = Translator.inst.AllLevels();
        for (int i = 0; i < listOfLevels.Length; i++)
        {
            Level nextLevel = listOfLevels[i];
            levelDropdown.AddOptions(new List<string>() { Translator.inst.GetText(nextLevel.name) });
            if (i == PlayerPrefs.GetInt("Current Level"))
            {
                levelDropdown.value = i;
                ChangeLevelDropdown(i);
            }
        }

        void ChangeLevelDropdown(int n)
        {
            PlayerPrefs.SetInt("Current Level", n);
            waveSlider.maxValue = Translator.inst.AllLevels()[n].listOfWaves.Count;
            UpdateWaveText(1);
            string levelName = Translator.inst.AllLevels()[n].name;

            if (PlayerPrefs.HasKey($"{levelName} - Best Score"))
                bestRun.text = $"{Translator.inst.GetText("Best Score")}: {PlayerPrefs.GetInt($"{levelName} - Best Score")}";
            else
                bestRun.text = Translator.inst.GetText("No Score");
        }
    }
}
