
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
        // Make sure gameplay isn't left paused.
        Time.timeScale = 1f;
    }

    public void PlayGame()
    {
        // Resume normal game time.
        Time.timeScale = 1f;

        // Load the gameplay scene.
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Resume normal time before loading.
        Time.timeScale = 1f;

        // Return to the title screen.
        SceneManager.LoadScene("TitleScene");
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
        // Resume normal time.
        Time.timeScale = 1f;

        // Start a new game.
        SceneManager.LoadScene("GameScene");
    }


    public void ExitApplication()
    {
        Debug.Log("Exiting application...");

        // Close the game in a standalone build.
        Application.Quit();

#if UNITY_EDITOR
        // Stop Play Mode when testing in the Unity Editor.
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}
