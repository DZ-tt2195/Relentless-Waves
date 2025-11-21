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
    [SerializeField] Slider infiniteSlider;

    [SerializeField] TMP_Text bestRun;
    [SerializeField] TMP_Dropdown levelDropdown;
    [SerializeField] Button deleteScoreButton;

    void Start()
    {
        if (!PlayerPrefs.HasKey("Difficulty")) PlayerPrefs.SetFloat("Difficulty", 1f);
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
        difficultySlider.value = PlayerPrefs.GetFloat("Difficulty");
        UpdateDifficultyText(PlayerPrefs.GetFloat("Difficulty"));

        void UpdateDifficultyText(float value)
        {
            difficultyLabel.text = $"{Translator.inst.Translate("Difficulty")}: {value * 100:F1}%";
            PlayerPrefs.SetFloat("Difficulty", value);
        }

        if (!PlayerPrefs.HasKey("Starting Wave")) PlayerPrefs.SetInt("Starting Wave", 1);
        waveSlider.onValueChanged.AddListener(UpdateWaveText);
        waveSlider.value = PlayerPrefs.GetInt("Starting Wave");
        UpdateWaveText(PlayerPrefs.GetInt("Starting Wave"));

        void UpdateWaveText(float value)
        {
            waveLabel.text = $"{Translator.inst.Translate("Start on Wave")} {(int)value}";
            PlayerPrefs.SetInt("Starting Wave", (int)value);
        }

        if (!PlayerPrefs.HasKey("Juggle")) PlayerPrefs.SetInt("Juggle", 0);
        juggleSlider.onValueChanged.AddListener(ChangeJuggleSlider);
        juggleSlider.value = PlayerPrefs.GetInt("Juggle");
        ChangeJuggleSlider(PlayerPrefs.GetInt("Juggle"));

        void ChangeJuggleSlider(float value)
        {
            PlayerPrefs.SetInt("Juggle", (int)(value));
        }

        if (!PlayerPrefs.HasKey("Infinite")) PlayerPrefs.SetInt("Infinite", 0);
        infiniteSlider.onValueChanged.AddListener(ChangeInfiniteSlider);
        infiniteSlider.value = PlayerPrefs.GetInt("Infinite");
        ChangeInfiniteSlider(PlayerPrefs.GetInt("Infinite"));

        void ChangeInfiniteSlider(float value)
        {
            PlayerPrefs.SetInt("Infinite", (int)(value));
        }

        if (!PlayerPrefs.HasKey("Current Level")) PlayerPrefs.SetInt("Current Level", 0);
        Level[] listOfLevels = Translator.inst.AllLevels();

        deleteScoreButton.onClick.AddListener(ClearScores);
        levelDropdown.onValueChanged.AddListener(ChangeLevelDropdown);
        for (int i = 0; i < listOfLevels.Length; i++)
        {
            Level nextLevel = listOfLevels[i];
            levelDropdown.AddOptions(new List<string>() { Translator.inst.Translate(nextLevel.name) });
            if (i == PlayerPrefs.GetInt("Current Level"))
            {
                levelDropdown.value = i;
                ChangeLevelDropdown(i);
            }
        }

        void ClearScores()
        {
            foreach (Level level in listOfLevels)
                PlayerPrefs.SetInt($"{level.name} - Best Score", 0);
            ChangeLevelDropdown(PlayerPrefs.GetInt("Current Level"));
        }

        void ChangeLevelDropdown(int n)
        {
            PlayerPrefs.SetInt("Current Level", n);
            Level newLevel = Translator.inst.AllLevels()[n];

            if (newLevel.endless)
            {
                waveSlider.value = 1;
                waveSlider.gameObject.SetActive(false);
            }
            else
            {
                waveSlider.maxValue = newLevel.listOfWaves.Count;
                waveSlider.value = 1;
                waveSlider.gameObject.SetActive(true);
            }

            if (PlayerPrefs.GetInt($"{newLevel.levelName} - Best Score") > 0)
                bestRun.text = $"{Translator.inst.Translate("Best Score")}: {PlayerPrefs.GetInt($"{newLevel.levelName} - Best Score")}";
            else
                bestRun.text = Translator.inst.Translate("No Score");
        }
    }
}
