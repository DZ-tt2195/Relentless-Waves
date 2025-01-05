using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Toggle hardModeToggle;

    private void OnDisable()
    {
        PlayerPrefs.SetInt("Hard Mode", hardModeToggle.isOn ? 1 : 0);
        Debug.Log($"hard mode set to {PlayerPrefs.GetInt("Hard Mode")}");
    }
}
