
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

        Debug.Log(
            "TAROT PLATFORM ACTIVATED! Open Shelby's roulette Canvas!"
        );
    }
}
