using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Menus")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        bool fullscreen = PlayerPrefs.GetInt(
            "Fullscreen", Screen.fullScreen ? 1 : 0
        ) == 1;

        AudioListener.volume = savedVolume;
        volumeSlider.SetValueWithoutNotify(savedVolume);
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        Screen.fullScreen = fullscreen;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
