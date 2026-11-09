
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;

    public Slider volumeSlider;

    public Toggle fullscreenToggle;
    public TMP_Dropdown graphicsDropdown;


    void Start()
    {
        // Make sure the main menu appears first
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

       

        // Volume
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volumeSlider.value;

        // Sensitivity
  
       

        // Fullscreen
        fullscreenToggle.isOn =
            PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        // Graphics
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new List<string>(QualitySettings.names));

        graphicsDropdown.value = Mathf.Clamp(
            PlayerPrefs.GetInt("Graphics", QualitySettings.GetQualityLevel()),
            0,
            QualitySettings.names.Length - 1
        );

        ApplySettings();

        // Connect sliders and dropdown
        volumeSlider.onValueChanged.AddListener(SetVolume);
      
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        graphicsDropdown.onValueChanged.AddListener(SetGraphics);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

   
    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    public void SetGraphics(int value)
    {
        QualitySettings.SetQualityLevel(value);
        PlayerPrefs.SetInt("Graphics", value);
    }

    void ApplySettings()
    {
        SetVolume(volumeSlider.value);

        SetFullscreen(fullscreenToggle.isOn);
        SetGraphics(graphicsDropdown.value);
    }
}
