
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;

    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Double Jump")]
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Animation")]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private float jumpSquatDuration = 0.25f;

    [Header("Hit Knockback")]
    [SerializeField] private float knockbackDecay = 12f;

    private CharacterController controller;
    private Vector3 velocity;

    private bool isJumpSquatting;
    private float jumpSquatTimer;

    private float horizontalKnockbackVelocity;
    private float hitStunTimer;

    public float mouseSensitivity { get; internal set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsRemaining = maxJumps;

        if (animationController == null)
        {
            animationController =
                GetComponentInChildren<PlayerAnimationController>();
        }
    }

    void Update()
    {
        // Check grounding before moving this frame.
        bool grounded = controller.isGrounded;

        if (grounded)
        {
            if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            // Landing restores both jumps.
            jumpsRemaining = maxJumps;
        }

        // Horizontal movement and sprinting.
        float x = Input.GetAxisRaw("Horizontal");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : speed;

        // Count down hit stun.
        if (hitStunTimer > 0f)
        {
            hitStunTimer -= Time.deltaTime;
        }

        bool isHitStunned = hitStunTimer > 0f;

        // Jump input, including double jump.
        if (!isHitStunned &&
            !isJumpSquatting &&
            Input.GetButtonDown("Jump") &&
            jumpsRemaining > 0)
        {
            if (grounded && jumpSquatDuration > 0f)
            {
                // Play the jump-squat animation first.
                isJumpSquatting = true;
                jumpSquatTimer = jumpSquatDuration;

                if (animationController != null)
                {
                    animationController.PlayJumpSquat();
                }
            }
            else
            {
                // Airborne jumps happen immediately.
                PerformJump();
            }
        }

        // Finish the jump squat before jumping.
        if (isJumpSquatting)
        {
            jumpSquatTimer -= Time.deltaTime;

            if (jumpSquatTimer <= 0f)
            {
                isJumpSquatting = false;
                PerformJump();
            }
        }

        // Apply gravity.
        velocity.y += gravity * Time.deltaTime;

        // Normal horizontal movement.
        float horizontalMovement = x * currentSpeed;

        // Disable player input during hit stun.
        if (isHitStunned)
        {
            horizontalMovement = 0f;
        }

        // Add knockback from enemy attacks.
        horizontalMovement += horizontalKnockbackVelocity;

        // Gradually reduce knockback.
        horizontalKnockbackVelocity = Mathf.MoveTowards(
            horizontalKnockbackVelocity,
            0f,
            knockbackDecay * Time.deltaTime
        );

        // Combine movement and gravity.
        Vector3 movement = new Vector3(
            horizontalMovement,
            velocity.y,
            0f
        );

        // Move only ONCE per frame.
        controller.Move(movement * Time.deltaTime);

        // Update character animations.
        if (animationController != null)
        {
            animationController.UpdateMovementAnimation(
                isJumpSquatting,
                x
            );
        }
    }

    // Called when the player is hit by an enemy.
    public void ApplyHitKnockback(
        float horizontalDirection,
        float horizontalStrength,
        float verticalStrength,
        float stunDuration)
    {
        // Cancel any pending jump squat.
        isJumpSquatting = false;
        jumpSquatTimer = 0f;

        // Apply vertical knockback.
        velocity.y = verticalStrength;

        // Push the player away from the enemy.
        horizontalKnockbackVelocity =
            horizontalDirection * horizontalStrength;

        // Temporarily disable player input.
        hitStunTimer = stunDuration;

        if (animationController != null)
        {
            animationController.PlayAirborne();
        }
    }

    private void PerformJump()
    {
        // Calculate jumping velocity.
        velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

        jumpsRemaining--;

        if (animationController != null)
        {
            animationController.PlayAirborne();
        }
    }
}
