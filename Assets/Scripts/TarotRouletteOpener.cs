
using UnityEngine;

public class TarotRouletteOpener : MonoBehaviour
{
    [SerializeField] private GameObject rouletteCanvas;
    [SerializeField] private PlayerMovement playerMovement;

    public void ShowRoulette()
    {
        if (rouletteCanvas == null)
        {
            Debug.LogWarning("Roulette Canvas is not assigned!");
            return;
        }

        // Prevent the rabbit from jumping or moving
        // while choosing a tarot card.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Pause gameplay while the roulette is open.
        Time.timeScale = 0f;

        rouletteCanvas.SetActive(true);

        Debug.Log("TAROT ROULETTE OPENED!");
    }

    public void CloseRoulette()
    {
        rouletteCanvas.SetActive(false);
        // Resume normal gameplay.
        Time.timeScale = 1f;

        // Give control back to the rabbit.
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Debug.Log("TAROT ROULETTE CLOSED!");
    }
}
