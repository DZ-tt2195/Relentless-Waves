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

    void Start()
    {
        if (!PlayerPrefs.HasKey("Difficulty")) PlayerPrefs.SetFloat("Difficulty", 1f);
        difficultySlider.onValueChanged.AddListener(UpdateText);
        difficultySlider.value = PlayerPrefs.GetFloat("Difficulty");
        UpdateText(PlayerPrefs.GetFloat("Difficulty"));

        if (!PlayerPrefs.HasKey("Juggle")) PlayerPrefs.SetInt("Juggle", 0);
        juggleSlider.value = (float)PlayerPrefs.GetInt("Juggle");

        if (!PlayerPrefs.HasKey("Language")) PlayerPrefs.SetString("Language", "English");
        languageDropdown.onValueChanged.AddListener(ChangeDropdown);
        List<string> languages = Translator.inst.GetTranslations().Keys.ToList();
        for (int i = 0; i < languages.Count; i++)
        {
            string nextLanguage = languages[i];
            languageDropdown.AddOptions(new List<string>() { nextLanguage });
            if (nextLanguage.Equals(PlayerPrefs.GetString("Language")))
                languageDropdown.value = i;
        }
        languageDropdown.gameObject.SetActive(languageDropdown.options.Count >= 2);

        if (PlayerPrefs.HasKey("Best Difficulty"))
        {
            bestRun.text = $"{Translator.inst.GetText("Best Run")}:\n" +
                $"{Translator.inst.GetText("Difficulty")}: {PlayerPrefs.GetFloat("Best Difficulty") * 100:F1}%\n" +
                $"{Translator.inst.GetText("Bullets Missed")}: {PlayerPrefs.GetInt("Best Bullet")}\n" +
                $"{Translator.inst.GetText("Health Lost")}: {PlayerPrefs.GetInt("Best Damage")}";
        }
        else
        {
            bestRun.text = Translator.inst.GetText("No Run");
        }
    }

    void UpdateText(float value)
    {
        sliderLabel.text = $"{value*100:F1}%";
    }

    void ChangeDropdown(int n)
    {
        string selectedText = languageDropdown.options[languageDropdown.value].text;
        if (!PlayerPrefs.GetString("Language").Equals(selectedText))
        {
            PlayerPrefs.SetString("Language", selectedText);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("Difficulty", difficultySlider.value);
        PlayerPrefs.SetInt("Juggle", (int)juggleSlider.value);
    }
}
