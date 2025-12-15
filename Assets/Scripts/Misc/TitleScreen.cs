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
        if (!PlayerPrefs.HasKey(PrefManager.Difficulty)) PrefManager.SetDifficulty(1f);
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
        difficultySlider.value = PrefManager.GetDifficulty();
        UpdateDifficultyText(PrefManager.GetDifficulty());

        void UpdateDifficultyText(float value)
        {
            difficultyLabel.text = AutoTranslate.Difficulty($"{value*100:F1}");
            PrefManager.SetDifficulty(value);
        }

        if (!PlayerPrefs.HasKey(PrefManager.StartWave)) PrefManager.SetStartWave(1);
        waveSlider.onValueChanged.AddListener(UpdateWaveText);
        waveSlider.value = PrefManager.GetStartWave();
        UpdateWaveText(PrefManager.GetStartWave());

        void UpdateWaveText(float value)
        {
            waveLabel.text = AutoTranslate.Start_on_Wave(((int)(value)).ToString());
            PrefManager.SetStartWave((int)value);
        }

        if (!PlayerPrefs.HasKey(PrefManager.Juggle)) PrefManager.SetJuggle(0);
        juggleSlider.onValueChanged.AddListener(ChangeJuggleSlider);
        juggleSlider.value = PrefManager.GetJuggle();
        ChangeJuggleSlider(PrefManager.GetJuggle());

        void ChangeJuggleSlider(float value) { PrefManager.SetJuggle((int)value); }

        if (!PlayerPrefs.HasKey(PrefManager.Infinity)) PrefManager.SetInfinity(0);
        infiniteSlider.onValueChanged.AddListener(ChangeInfiniteSlider);
        infiniteSlider.value = PrefManager.GetInfinity();
        ChangeInfiniteSlider(PrefManager.GetInfinity());

        void ChangeInfiniteSlider(float value) {PrefManager.SetInfinity((int)value);}

        if (!PlayerPrefs.HasKey(PrefManager.CurrentLevel)) PrefManager.SetCurrentLevel(0);
        levelDropdown.onValueChanged.AddListener(ChangeLevelDropdown);
        Level[] listOfLevels = Translator.inst.AllLevels();
        for (int i = 0; i < listOfLevels.Length; i++)
        {
            Level nextLevel = listOfLevels[i];
            levelDropdown.AddOptions(new List<string>() { AutoTranslate.DoEnum(nextLevel.levelName) });
            if (i == PrefManager.GetCurrentLevel())
            {
                levelDropdown.value = i;
                ChangeLevelDropdown(i);
            }
        }

        deleteScoreButton.onClick.AddListener(ClearScores);
        void ClearScores()
        {
            foreach (Level level in listOfLevels)
                PrefManager.SetScore(level.levelName.ToString(), 0);
            ChangeLevelDropdown(PrefManager.GetCurrentLevel());
        }

        void ChangeLevelDropdown(int n)
        {
            PrefManager.SetCurrentLevel(n);
            Level newLevel = Translator.inst.AllLevels()[n];

            if (newLevel.endless || newLevel.listOfWaves.Count == 1)
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

            if (PrefManager.GetScore(newLevel.levelName.ToString()) > 0)
                bestRun.text = AutoTranslate.Best_Score($"{PrefManager.GetScore(newLevel.levelName.ToString())}");
            else
                bestRun.text = AutoTranslate.DoEnum(ToTranslate.No_Score);
        }
    }
}
