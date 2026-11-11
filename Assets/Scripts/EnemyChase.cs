
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


    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField, Range(0.1f, 0.95f)]
    private float groundCheckRadiusMultiplier = 0.8f;

    private bool isGrounded;
    private bool groundResetLocked;


    [Header("Ground Lunge")]
    public float lungeRange = 5f;
    public float lungeSpeed = 12f;
    public float lungeDuration = 0.35f;
    public float lungeCooldown = 2f;
    public float lungeVerticalTolerance = 1.5f;

    [Tooltip("Maximum height of the lunge's shallow jump.")]
    public float lungeJumpHeight = 1.25f;

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

    [Header("2D Movement Plane")]
    [SerializeField] private bool lockZAxis = true;

    [Header("Platform Navigation")]
    [SerializeField] private bool usePlatformNavigation = true;
    [SerializeField] private float platformSearchRadius = 15f;
    [SerializeField] private float maxPlatformStepHeight = 12f;


    [Header("Attack Recovery")]
    [SerializeField] private float hitRecoveryDuration = 2f;
    private float hitRecoveryTimer;


    private float lockedZPosition;

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
        get { return isGrounded; }
    }


    public float HorizontalSpeed
    {
        get { return horizontalSpeed; }
    }

    public void BeginHitRecovery()
    {
        hitRecoveryTimer = hitRecoveryDuration;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Remember the enemy's starting Z position.
        lockedZPosition = transform.position.z;

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


        // Release takeoff protection when the enemy begins descending.
        if (groundResetLocked && velocity.y <= 0f)
        {
            groundResetLocked = false;
        }

        isGrounded = CheckGround();


        // TRACK PLAYER MOVEMENT
        playerHorizontalVelocity =
            (player.position.x - previousPlayerPosition.x)
            / Mathf.Max(deltaTime, 0.001f);

        previousPlayerPosition = player.position;

        float predictedX = player.position.x +
            playerHorizontalVelocity * predictionTime;

                    float targetX = predictedX;

        if (usePlatformNavigation &&
            player.position.y >
            transform.position.y + maxPlatformStepHeight)
        {
            targetX = FindNextPlatformX(predictedX);
        }


        float targetDistance =
            Mathf.Abs(targetX - transform.position.x);


        float distance =
            Mathf.Abs(player.position.x - transform.position.x);

        float verticalDistance =
            Mathf.Abs(player.position.y - transform.position.y);

        // RESET JUMPS WHEN GROUNDED
        if (isGrounded)
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


        if (hitRecoveryTimer > 0f)
        {
            hitRecoveryTimer = Mathf.Max(
                0f,
                hitRecoveryTimer - deltaTime
            );

            isStunned = true;
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

                    // Launch upward into a shallow arc.
                    velocity.y = Mathf.Sqrt(
                        lungeJumpHeight * -2f * gravity
                    );
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
            isGrounded &&
            cooldownTimer <= 0f &&
            distance <= lungeRange &&
            distance > stopDistance &&
            distance <= chaseRange &&
            verticalDistance <= lungeVerticalTolerance)
        {
            isLungeStarting = true;
            startupTimer = lungeStartupDuration;
            cooldownTimer = lungeCooldown;

            // Lock direction at the start of the attack.
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
                 (isGrounded || velocity.y < 0f))
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
                targetDistance > stopDistance)
            {
                direction = Mathf.Sign(
                    targetX - transform.position.x
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

        // Prevent knockback from moving the enemy along Z.
        if (lockZAxis)
        {
            movement.z = 0f;
        }

        // Move the CharacterController normally.
        controller.Move(movement * deltaTime);

        // Correct any Z displacement caused by collision sliding.
        if (lockZAxis)
        {
            Vector3 correctedPosition = transform.position;

            correctedPosition.z = lockedZPosition;

            transform.position = correctedPosition;
        }
    }

    private float FindNextPlatformX(float fallbackX)
    {
        if (!usePlatformNavigation || player == null)
            return fallbackX;

        float feetY = controller.bounds.min.y;

        Collider[] nearbyPlatforms = Physics.OverlapSphere(
            transform.position,
            platformSearchRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        float bestScore = float.MaxValue;
        float bestX = fallbackX;

        foreach (Collider platform in nearbyPlatforms)
        {
            if (platform == controller)
                continue;

            float heightAbove =
                platform.bounds.max.y - feetY;

            // Only consider platforms above the spider
            // that are within its allowed climbing range.
            if (heightAbove < 1f ||
                heightAbove > maxPlatformStepHeight)
                continue;

            float platformX = platform.bounds.center.x;

            float horizontalDistance =
                Mathf.Abs(platformX - transform.position.x);

            float playerDistance =
                Mathf.Abs(platformX - player.position.x);

            // Prefer nearby platforms that lead
            // in the rabbit's direction.
            float score = heightAbove +
                horizontalDistance * 0.3f +
                playerDistance * 0.2f;

            if (score < bestScore)
            {
                bestScore = score;
                bestX = platformX;
            }
        }

        return bestX;
    }


    void Jump()
    {
        velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

        jumpsRemaining--;

        groundResetLocked = true;
        isGrounded = false;

    }


    private bool CheckGround()
    {
        if (groundResetLocked || velocity.y > 0f)
        {
            return false;
        }

        Vector3 center = transform.TransformPoint(
            controller.center
        );

        float radius = controller.radius *
            groundCheckRadiusMultiplier;

        float feetY = center.y -
            controller.height * 0.5f;

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

}
