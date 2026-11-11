
using UnityEngine;

public class RabbitAudioEvents : MonoBehaviour
{
    // Called by the rabbit's run animation.
    public void Footstep()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayRabbitFootstep();
        }
    }
}
