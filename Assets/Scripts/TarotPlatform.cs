
using UnityEngine;

public class TarotPlatform : MonoBehaviour
{
    private bool hasActivated = false;

    public void ActivateTarot()
    {
        // Prevent the same platform from activating twice.
        if (hasActivated)
            return;

        hasActivated = true;


        TarotRouletteOpener rouletteOpener =
            FindFirstObjectByType<TarotRouletteOpener>();

        if (rouletteOpener != null)
        {
            rouletteOpener.ShowRoulette();
        }
        else
        {
            Debug.LogWarning("Tarot Roulette Opener not found!");
        }

    }
}
