
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChase : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Chasing")]
    public float speed = 4f;
    public float acceleration = 10f;
    public float chaseRange = 25f;
    public float stopDistance = 1.5f;
    public float predictionTime = 0.3f;

    [Header("Jumping")]
    public float jumpHeight = 3f;
    public float gravity = -20f;
    public float jumpTriggerHeight = 1f;

    [Header("Double Jump")]
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Ground Lunge")]
    public float lungeRange = 5f;
    public float lungeSpeed = 12f;
    public float lungeDuration = 0.35f;
    public float lungeCooldown = 2f;
    public float lungeVerticalTolerance = 1.5f;

    [Header("Animation Startup")]
    [Tooltip("Startup frames at the specified animation frame rate.")]
    public int lungeStartupFrames = 9;

    [Tooltip("Frame rate used to calculate startup timing.")]
    public float animationFrameRate = 60f;

    [Header("Sprite Flipping")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool facesRightByDefault = true;

    [Header("Knockback")]
    [SerializeField] private KnockbackReceiver knockbackReceiver;

    private CharacterController controller;
    private Vector3 velocity;
    private float horizontalSpeed;

    private Vector3 previousPlayerPosition;
    private float playerHorizontalVelocity;

    private bool isLunging;
    private bool isLungeStarting;
    private bool isJumpStarting;

    private float lungeTimer;
    private float cooldownTimer;
    private float startupTimer;
    private float lungeDirection;

    private float jumpStartupDuration;
    private float lungeStartupDuration;

    public bool IsLunging
    {
        get { return isLunging || isLungeStarting; }
    }

    public bool IsJumpStarting
    {
        get { return isJumpStarting; }
    }

    public bool IsGrounded
    {
        get { return controller != null && controller.isGrounded; }
    }

    public float HorizontalSpeed
    {
        get { return horizontalSpeed; }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsRemaining = maxJumps;

        if (knockbackReceiver == null)
        {
            knockbackReceiver = GetComponent<KnockbackReceiver>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (player == null)
        {
            GameObject foundPlayer =
                GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }

        if (player != null)
        {
            previousPlayerPosition = player.position;
        }

        float safeFrameRate = Mathf.Max(animationFrameRate, 1f);

        lungeStartupDuration =
            lungeStartupFrames / safeFrameRate;

        jumpStartupDuration =
            lungeStartupDuration * 0.5f;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        // TRACK PLAYER MOVEMENT
        playerHorizontalVelocity =
            (player.position.x - previousPlayerPosition.x)
            / Mathf.Max(deltaTime, 0.001f);

        previousPlayerPosition = player.position;

        float predictedX = player.position.x +
            playerHorizontalVelocity * predictionTime;

        float distance =
            Mathf.Abs(player.position.x - transform.position.x);

        float verticalDistance =
            Mathf.Abs(player.position.y - transform.position.y);

        // RESET JUMPS WHEN GROUNDED
        if (controller.isGrounded)
        {
            velocity.y = -2f;
            jumpsRemaining = maxJumps;
        }

        // CHECK KNOCKBACK STUN
        bool isStunned = false;

        if (knockbackReceiver != null)
        {
            isStunned = knockbackReceiver.IsStunned;
        }

        // COOLDOWN
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;
        }

        // INTERRUPT ATTACK STARTUP OR LUNGE
        if (isStunned)
        {
            isLungeStarting = false;
            isJumpStarting = false;
            isLunging = false;
            horizontalSpeed = 0f;
        }

        // PROCESS STARTUP TIMERS
        if (!isStunned)
        {
            if (isLungeStarting)
            {
                startupTimer -= deltaTime;

                if (startupTimer <= 0f)
                {
                    isLungeStarting = false;
                    isLunging = true;
                    lungeTimer = lungeDuration;
                }
            }
            else if (isJumpStarting)
            {
                startupTimer -= deltaTime;

                if (startupTimer <= 0f)
                {
                    isJumpStarting = false;
                    Jump();
                }
            }
        }

        bool isLocked =
            isLungeStarting ||
            isJumpStarting ||
            isLunging;

        // BEGIN GROUND LUNGE
        if (!isStunned &&
            !isLocked &&
            controller.isGrounded &&
            cooldownTimer <= 0f &&
            distance <= lungeRange &&
            distance > stopDistance &&
            distance <= chaseRange &&
            verticalDistance <= lungeVerticalTolerance)
        {
            isLungeStarting = true;
            startupTimer = lungeStartupDuration;
            cooldownTimer = lungeCooldown;

            lungeDirection = Mathf.Sign(
                predictedX - transform.position.x
            );

            if (lungeDirection == 0f)
            {
                lungeDirection = 1f;
            }

            horizontalSpeed = 0f;
        }

        // BEGIN JUMP STARTUP
        else if (!isStunned &&
                 !isLocked &&
                 distance <= chaseRange &&
                 player.position.y >
                 transform.position.y + jumpTriggerHeight &&
                 jumpsRemaining > 0 &&
                 (controller.isGrounded || velocity.y < 0f))
        {
            isJumpStarting = true;
            startupTimer = jumpStartupDuration;
            horizontalSpeed = 0f;
        }

        // MOVEMENT
        if (isStunned ||
            isLungeStarting ||
            isJumpStarting)
        {
            horizontalSpeed = 0f;
        }
        else if (isLunging)
        {
            horizontalSpeed =
                lungeDirection * lungeSpeed;

            lungeTimer -= deltaTime;

            if (lungeTimer <= 0f)
            {
                isLunging = false;
                horizontalSpeed = 0f;
            }
        }
        else
        {
            float direction = 0f;

            if (distance <= chaseRange &&
                distance > stopDistance)
            {
                direction = Mathf.Sign(
                    predictedX - transform.position.x
                );
            }

            float targetSpeed = direction * speed;

            horizontalSpeed = Mathf.MoveTowards(
                horizontalSpeed,
                targetSpeed,
                acceleration * deltaTime
            );
        }

        // SPRITE FLIPPING
        float facingDirection = horizontalSpeed;

        if (isLungeStarting || isLunging)
        {
            facingDirection = lungeDirection;
        }
        else if (isJumpStarting)
        {
            facingDirection =
                predictedX - transform.position.x;
        }

        if (spriteRenderer != null &&
            Mathf.Abs(facingDirection) > 0.01f)
        {
            bool facingLeft = facingDirection < 0f;

            if (facesRightByDefault)
            {
                spriteRenderer.flipX = facingLeft;
            }
            else
            {
                spriteRenderer.flipX = !facingLeft;
            }
        }

        // GRAVITY
        velocity.y += gravity * deltaTime;

        Vector3 movement = new Vector3(
            horizontalSpeed,
            velocity.y,
            0f
        );

        // KNOCKBACK
        if (knockbackReceiver != null)
        {
            Vector3 knockbackMovement =
                knockbackReceiver.GetKnockbackMovement(
                    deltaTime
                );

            movement += knockbackMovement / deltaTime;
        }

        // ONE MOVE PER FRAME
        controller.Move(movement * deltaTime);
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

        jumpsRemaining--;
    }
}
