using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Slider difficultySlider;
    [SerializeField] TMP_Text sliderLabel;
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
            sliderLabel.text = $"{value * 100:F1}%";
            PlayerPrefs.SetFloat("Difficulty", difficultySlider.value);
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
            string levelName = Translator.inst.AllLevels()[n].name;

            if (PlayerPrefs.HasKey($"{levelName} - Best Score"))
                bestRun.text = $"{Translator.inst.GetText("Best Score")}: {PlayerPrefs.GetInt($"{levelName} - Best Score")}";
            else
                bestRun.text = Translator.inst.GetText("No Score");
        }
    }
}
