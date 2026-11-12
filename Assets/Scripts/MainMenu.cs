
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject creditsPanel;
    public GameObject pauseButton;
    public GameObject restartPanel;

    void Start()
    {
        Time.timeScale = 0f;

        pauseButton.SetActive(false);
        restartPanel.SetActive(false);
    }

    public void PlayGame()
    {
        mainMenuCanvas.SetActive(false);
        pauseButton.SetActive(true);

        Time.timeScale = 1f;
    }

 
    public void QuitGame()
    {
        Time.timeScale = 0f;

        restartPanel.SetActive(false);
        creditsPanel.SetActive(false);
        pauseButton.SetActive(false);

        mainMenuCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    public void ShowRestartPanel()
    {
        restartPanel.SetActive(true);
        pauseButton.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
