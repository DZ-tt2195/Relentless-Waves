using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using MyBox;

public class TitleScreen : MonoBehaviour
{
    [Foldout("UI", true)]
    [SerializeField] Button extrasButton;
    [SerializeField] Transform allExtras;
    [SerializeField] TMP_Text bestRun;
    [SerializeField] TMP_Dropdown levelDropdown;
    [SerializeField] Button deleteScoreButton;
    [SerializeField] Button soundCreditsButton;
    [SerializeField] GameObject soundCreditsScreen;
    [Foldout("Texts", true)]
    [SerializeField] TMP_Text designer;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text lastUpdate;
    [SerializeField] TMP_Text controls;
    [SerializeField] TMP_Text play;
    [SerializeField] TMP_Text chooseLevel;
    [SerializeField] TMP_Text deleteScores;
    [SerializeField] TMP_Text soundCredits;

    void Start()
    {
        designer.text = AutoTranslate.Designer();
        description.text = AutoTranslate.Description();
        lastUpdate.text = AutoTranslate.Last_Update();
        controls.text = AutoTranslate.Controls();
        play.text = AutoTranslate.Play();
        chooseLevel.text = AutoTranslate.Choose_Level();
        deleteScores.text = AutoTranslate.Delete();
        soundCredits.text = AutoTranslate.Sound_Credits();

        extrasButton.onClick.AddListener(EnableExtras);
        extrasButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Extras();
        void EnableExtras()
        {
            extrasButton.gameObject.SetActive(false);
            allExtras.gameObject.SetActive(true);
        }

        soundCreditsScreen.SetActive(false);
        soundCreditsButton.onClick.AddListener(CreditsToggle);
        void CreditsToggle()
        {
            AudioManager.instance.Menu();
            if (soundCreditsScreen.activeSelf)
                soundCreditsScreen.SetActive(false);
            else
                soundCreditsScreen.SetActive(true);
        }    

        LevelInfo();
    }
    void LevelInfo()
    {
        if (!PlayerPrefs.HasKey(PrefManager.CurrentLevel)) PrefManager.SetCurrentLevel(0);
        List<Level> listOfLevels = GameFiles.inst.AllLevels();

        levelDropdown.onValueChanged.AddListener(ChangeLevelDropdown);
        for (int i = 0; i < listOfLevels.Count; i++)
        {
            Level nextLevel = listOfLevels[i];
            if (nextLevel.levelName == AutoTranslate.Blank() && !Application.isEditor)
                continue;
            
            levelDropdown.AddOptions(new List<string>() { Translator.inst.Translate(nextLevel.levelName) });
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
            AudioManager.instance.Menu();
            PrefManager.SetCurrentLevel(n);
            Level newLevel = listOfLevels[n];

            if (PrefManager.GetScore(newLevel.levelName.ToString()) > 0)
                bestRun.text = AutoTranslate.Best_Score($"{PrefManager.GetScore(newLevel.levelName.ToString())}");
            else
                bestRun.text = AutoTranslate.No_Score();
        }        
    }
}
