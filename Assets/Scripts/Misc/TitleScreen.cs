using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Slider difficultySlider;
    [SerializeField] TMP_Text sliderLabel;
    [SerializeField] TMP_Text bestRun;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Difficulty"))
            PlayerPrefs.SetFloat("Difficulty", 1f);

        difficultySlider.onValueChanged.AddListener(UpdateText);
        difficultySlider.value = PlayerPrefs.GetFloat("Difficulty");
        UpdateText(PlayerPrefs.GetFloat("Difficulty"));

        if (PlayerPrefs.HasKey("Best Difficulty"))
        {
            bestRun.text = $"Best Run: {PlayerPrefs.GetFloat("Best Difficulty") * 100:F1}% Difficulty " +
                $"| missed {PlayerPrefs.GetInt("Best Bullet")} bullets, took {PlayerPrefs.GetInt("Best Damage")} damage";
        }
        else
        {
            bestRun.text = "";
        }
    }

    void UpdateText(float value)
    {
        sliderLabel.text = $"{value*100:F1}%";
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("Difficulty", difficultySlider.value);
    }
}
