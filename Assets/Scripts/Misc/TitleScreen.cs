using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Slider difficultySlider;
    [SerializeField] TMP_Text sliderLabel;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Difficulty"))
            PlayerPrefs.SetFloat("Difficulty", 1f);

        difficultySlider.onValueChanged.AddListener(UpdateText);
        difficultySlider.value = PlayerPrefs.GetFloat("Difficulty");
        UpdateText(PlayerPrefs.GetFloat("Difficulty"));
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
