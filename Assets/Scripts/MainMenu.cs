
using UnityEngine;
public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;

    public void PlayGame()
    {
        mainMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }


    public void QuitGame()
    {
        Debug.Log("Quit button pressed!");
        Application.Quit();
    }

    public GameObject creditsPanel;

    public void OpenCredits()
    {
        mainMenuCanvas.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

}