using UnityEngine;

public class EnemyAudioEvents : MonoBehaviour
{
    public void Footstep()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayRabbitFootstep();
        }
    }
}