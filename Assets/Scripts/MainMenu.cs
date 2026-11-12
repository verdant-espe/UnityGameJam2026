
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject creditsPanel;
    public GameObject pauseButton;

    void Start()
    {
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
    }

    public void PlayGame()
    {
        mainMenuCanvas.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed!");
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void OpenCredits()
    {
        mainMenuCanvas.SetActive(false);
        creditsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
