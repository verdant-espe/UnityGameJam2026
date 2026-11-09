
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
        bool grounded = controller.isGrounded;

        if (grounded)
        {
            if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            jumpsRemaining = maxJumps;
        }

        // Horizontal input.
        float x = Input.GetAxisRaw("Horizontal");

        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }

        // Count down hit stun.
        if (hitStunTimer > 0f)
        {
            hitStunTimer -= Time.deltaTime;
        }

        bool isHitStunned = hitStunTimer > 0f;

        // Jump input is disabled during hit stun.
        if (!isHitStunned &&
            Input.GetButtonDown("Jump") &&
            jumpsRemaining > 0)
        {
            if (grounded && jumpSquatDuration > 0f)
            {
                isJumpSquatting = true;
                jumpSquatTimer = jumpSquatDuration;

                if (animationController != null)
                {
                    animationController.PlayJumpSquat();
                }
            }
            else if (!isJumpSquatting)
            {
                PerformJump();
            }
        }

        // Jump squat countdown.
        if (isJumpSquatting)
        {
            jumpSquatTimer -= Time.deltaTime;

            if (jumpSquatTimer <= 0f)
            {
                isJumpSquatting = false;
                PerformJump();
            }
        }

        // Gravity.
        velocity.y += gravity * Time.deltaTime;

        // Disable horizontal player input during hit stun.
        float horizontalMovement = x * currentSpeed;

        if (isHitStunned)
        {
            horizontalMovement = 0f;
        }

        // Add horizontal knockback.
        horizontalMovement += horizontalKnockbackVelocity;

        // Gradually slow horizontal knockback.
        horizontalKnockbackVelocity = Mathf.MoveTowards(
            horizontalKnockbackVelocity,
            0f,
            knockbackDecay * Time.deltaTime
        );

        // Single CharacterController movement call.
        Vector3 movement = new Vector3(
            horizontalMovement,
            velocity.y,
            0f
        );

        controller.Move(movement * Time.deltaTime);

        // Preserve animation system.
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
        // Cancel pending jump squat.
        isJumpSquatting = false;
        jumpSquatTimer = 0f;

        // Launch upward using existing gravity physics.
        velocity.y = verticalStrength;

        // Push slightly away from the enemy.
        horizontalKnockbackVelocity =
            horizontalDirection * horizontalStrength;

        // Temporarily disable player control.
        hitStunTimer = stunDuration;

        if (animationController != null)
        {
            animationController.PlayAirborne();
        }
    }

    private void PerformJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpsRemaining--;

        if (animationController != null)
        {
            animationController.PlayAirborne();
        }
    }
}
