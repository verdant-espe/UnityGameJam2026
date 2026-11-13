
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

    [Header("Air Control")]
    [Range(0f, 1f)]
    public float airControl = 1f;


    [Header("Double Jump")]
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField, Range(0.1f, 0.95f)]
    private float groundCheckRadiusMultiplier = 0.8f;

    [Header("Animation")]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private float jumpSquatDuration = 0.25f;

    [Header("Hit Knockback")]
    [SerializeField] private float knockbackDecay = 12f;

    [Header("2D Movement Plane")]
    [SerializeField] private bool lockZAxis = true;

    private float lockedZPosition;
    private CharacterController controller;
    private Vector3 velocity;

    private bool isJumpSquatting;
    private float jumpSquatTimer;

    private float horizontalKnockbackVelocity;
    private float airHorizontalVelocity;
    private float hitStunTimer;

    // Prevent jump counts resetting during takeoff.
    private bool groundResetLocked;
    private bool isGrounded;

    public float mouseSensitivity { get; internal set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsRemaining = maxJumps;
        lockedZPosition = transform.position.z;

        if (animationController == null)
        {
            animationController =
                GetComponentInChildren<PlayerAnimationController>();
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        // Check grounding before moving this frame.
        bool grounded = controller.isGrounded;

        // Only allow ground detection while not rising.
        // A launched jump locks resets until descent.
        if (groundResetLocked && velocity.y <= 0f)
        {
            groundResetLocked = false;
        }

        isGrounded = CheckGround();

        if (isGrounded)
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

        // Hit stun.
        if (hitStunTimer > 0f)
        {
            hitStunTimer -= deltaTime;
        }

        bool isHitStunned = hitStunTimer > 0f;

        // Jump input.
        if (!isHitStunned &&
            !isJumpSquatting &&
            Input.GetButtonDown("Jump") &&
            jumpsRemaining > 0)
        {
            if (isGrounded && jumpSquatDuration > 0f)
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
            jumpSquatTimer -= deltaTime;

            if (jumpSquatTimer <= 0f)
            {
                isJumpSquatting = false;
                PerformJump();
            }
        }

        // Apply gravity.
        velocity.y += gravity * Time.deltaTime;


        // Normal horizontal movement.
        float targetMovement = x * currentSpeed;
        float horizontalMovement;

        if (isGrounded || airControl >= 1f)
        {
            // Normal movement responds immediately.
            horizontalMovement = targetMovement;
            airHorizontalVelocity = horizontalMovement;
        }
        else
        {
            // Reduced air control makes direction changes slower.
            airHorizontalVelocity = Mathf.MoveTowards(
                airHorizontalVelocity,
                targetMovement,
                40f * airControl * deltaTime
            );

            horizontalMovement = airHorizontalVelocity;
        }


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
            knockbackDecay * deltaTime
        );

        // Combine movement and gravity.
        Vector3 movement = new Vector3(
            horizontalMovement,
            velocity.y,
            0f
        );

        if (lockZAxis)
        {
            movement.z = 0f;
        }

        controller.Move(movement * deltaTime);

        // Restore XY movement plane.
        if (lockZAxis)
        {
            Vector3 correctedPosition = transform.position;
            correctedPosition.z = lockedZPosition;
            transform.position = correctedPosition;
        }

        if (animationController != null)
        {
            animationController.UpdateMovementAnimation(
                isJumpSquatting,
                x
            );
        }
    }

    private bool CheckGround()
    {
        // Do not accept ground while rising or during
        // the takeoff lockout.
        if (groundResetLocked || velocity.y > 0f)
        {
            return false;
        }

        // Find the bottom of the CharacterController.
        Vector3 center = transform.TransformPoint(
            controller.center
        );

        float radius = controller.radius *
            groundCheckRadiusMultiplier;

        float feetY = center.y -
            controller.height * 0.5f;

        // Start the sphere just above the capsule bottom.
        float startOffset = radius + 0.05f;

        Vector3 origin = new Vector3(
            center.x,
            feetY + startOffset,
            center.z
        );

        float castDistance =
            groundCheckDistance + 0.05f;

        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out RaycastHit hit,
            castDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    // Called when hit by an enemy.
    public void ApplyHitKnockback(
        float horizontalDirection,
        float horizontalStrength,
        float verticalStrength,
        float stunDuration)
    {

        // Play a cloth impact when the rabbit is hit.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClothImpact();
        }
        isJumpSquatting = false;
        jumpSquatTimer = 0f;

        velocity.y = verticalStrength;

        horizontalKnockbackVelocity =
            horizontalDirection * horizontalStrength;

        hitStunTimer = stunDuration;

        // Prevent knockback from resetting jumps in midair.
        if (verticalStrength > 0f)
        {
            groundResetLocked = true;
        }

        if (animationController != null)
        {
            animationController.PlayAirborne();
        }
    }


    private void PerformJump()
    {
        // Play the appropriate sound before consuming a jump.
        if (SoundManager.Instance != null)
        {
            if (jumpsRemaining == maxJumps)
            {
                // First jump.
                SoundManager.Instance.PlayRabbitJump();
            }
            else
            {
                // Double jump.
                SoundManager.Instance.PlayRabbitDoubleJump();
            }
        }

        // Calculate jumping velocity.
        velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

        jumpsRemaining--;

        // Immediately prevent a premature ground reset.
        groundResetLocked = true;
        isGrounded = false;

        if (animationController != null)
        {
            animationController.PlayAirborne();
        }
    }

}
