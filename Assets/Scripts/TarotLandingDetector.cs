
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TarotLandingDetector : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Only react when landing on top of something.
        if (hit.normal.y < 0.5f)
            return;

        // Check whether this is a tarot platform.
        TarotPlatform tarotPlatform =
            hit.collider.GetComponentInParent<TarotPlatform>();

        if (tarotPlatform != null)
        {
            tarotPlatform.ActivateTarot();
        }
    }
}
