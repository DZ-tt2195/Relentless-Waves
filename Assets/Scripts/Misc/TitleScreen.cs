using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Toggle hardModeToggle;

    void Awake()
    {
        hardModeToggle.isOn = PlayerPrefs.GetInt("Hard Mode") != 0;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("Hard Mode", hardModeToggle.isOn ? 1 : 0);
    }
}
