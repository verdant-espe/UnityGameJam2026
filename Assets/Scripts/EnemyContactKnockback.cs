
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyContactKnockback : MonoBehaviour
{
    [Header("Player Knockback")]
    [SerializeField] private float verticalStrength = 14f;
    [SerializeField] private float horizontalStrength = 3f;
    [SerializeField] private float stunDuration = 0.15f;

    [Header("Hit Cooldown")]
    [SerializeField] private float hitCooldown = 0.75f;

    private float nextHitTime;

    private void OnControllerColliderHit(
        ControllerColliderHit hit)
    {
        if (Time.time < nextHitTime)
        {
            return;
        }

        PlayerMovement player =
            hit.collider.GetComponentInParent<PlayerMovement>();

        if (player == null)
        {
            return;
        }

        // Determine which side of the enemy the rabbit is on.
        float horizontalDirection = Mathf.Sign(
            player.transform.position.x - transform.position.x
        );

        player.ApplyHitKnockback(
            horizontalDirection,
            horizontalStrength,
            verticalStrength,
            stunDuration
        );

        nextHitTime = Time.time + hitCooldown;
    }
}
