
using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDecay = 12f;
    [SerializeField] private float stunDuration = 0.25f;

    private Vector3 knockbackVelocity;
    private float stunTimer;

    // True while the character is recovering from a hit.
    public bool IsStunned
    {
        get { return stunTimer > 0f; }
    }

    // Called by projectiles or other attacks.
    public void ApplyKnockback(Vector3 direction, float strength)
    {
        knockbackVelocity = direction.normalized * strength;

        // Restart stun whenever another hit lands.
        stunTimer = stunDuration;
    }

    // Called by the movement script every frame.
    public Vector3 GetKnockbackMovement(float deltaTime)
    {
        Vector3 movement = knockbackVelocity * deltaTime;

        // Gradually reduce knockback velocity.
        knockbackVelocity = Vector3.MoveTowards(
            knockbackVelocity,
            Vector3.zero,
            knockbackDecay * deltaTime
        );

        // Count down stun.
        if (stunTimer > 0f)
        {
            stunTimer -= deltaTime;
        }

        return movement;
    }
}
