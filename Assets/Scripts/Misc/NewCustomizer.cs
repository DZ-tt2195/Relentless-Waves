using UnityEngine;
using TMPro;
using MyBox;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class NewCustomizer : MonoBehaviour
{
    public static int numTwists = 2;
    [Foldout("UI", true)]
        [SerializeField] Button openCustomizer;
        [SerializeField] TMP_Text customizerText;
        [SerializeField] Transform customizerScreen;
        [SerializeField] Button confirmButton;
        [SerializeField] TMP_Text confirmText;
        [SerializeField] TMP_Text chooseTwists;

    [Foldout("Customize", true)]
        [SerializeField] RulesDisplay displayPrefab;
        [SerializeField] Transform storeTwists;
        List<int> currentTwists = new();

    void Awake()
    {
        chooseTwists.text = AutoTranslate.Choose_Rule(numTwists.ToString());
        confirmText.text = AutoTranslate.Confirm();
        customizerText.text = AutoTranslate.Open_Customizer();
        customizerScreen.gameObject.SetActive(false);
        openCustomizer.onClick.AddListener(OpenCustomizer);
        void OpenCustomizer()
        {
            AudioManager.instance.Menu();
            customizerScreen.gameObject.SetActive(true);
        }

        List<Rule> allRules = GameFiles.inst.AllRules();
        for (int i = 0; i<allRules.Count; i++)
        {
            RulesDisplay nextDisplay = Instantiate(displayPrefab, storeTwists);
            nextDisplay.AssignRule(allRules[i]);
            int number = i;

            nextDisplay.toggle.onValueChanged.AddListener(ShapeToggle);
            if (AlreadySaved(number))
            {
                nextDisplay.toggle.isOn = true;
                currentTwists.Add(number);
            }
            else
            {
                nextDisplay.toggle.isOn = false;
            }

            void ShapeToggle(bool enabled)
            {
                if (enabled)
                {
                    currentTwists.Add(number);
                    if (currentTwists.Count > numTwists)
                        nextDisplay.toggle.isOn = false;
                    else
                        AudioManager.instance.Menu();
                }
                else
                {
                    AudioManager.instance.Menu();
                    currentTwists.Remove(number);
                }
            }
        }        

        confirmButton.onClick.AddListener(Done);
        void Done()
        {
            AudioManager.instance.Menu();
            for (int i = 0; i<numTwists; i++)
            {
                if (i < currentTwists.Count)
                    PrefManager.SetRule(i, currentTwists[i]);
                else
                    PrefManager.SetRule(i, -1);
            }
            PlayerPrefs.Save();
            customizerScreen.gameObject.SetActive(false);
        }
    }
    bool AlreadySaved(int num)
    {
        for (int i = 0; i<numTwists; i++)
        {
            if (PrefManager.GetRule(i) == num)
                return true;
        }
        return false;
    }
}
