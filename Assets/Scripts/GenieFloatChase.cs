
using UnityEngine;

public class GenieFloatChase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Floating Movement")]
    [SerializeField] private float floatSpeed = 6f;
    [SerializeField] private float hoverHeight = 1f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float chaseRange = 25f;

    [Header("Flying Lunge Attack")]
    [SerializeField] private float lungeRange = 5f;
    [SerializeField] private float lungeSpeed = 12f;
    [SerializeField] private float lungeDuration = 0.35f;
    [SerializeField] private float lungeCooldown = 2f;

    [Header("Attack Startup")]
    [SerializeField] private int lungeStartupFrames = 9;
    [SerializeField] private float animationFrameRate = 60f;

    [Header("Sprite Flipping")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool facesRightByDefault = true;

    [Header("Knockback")]
    [SerializeField] private KnockbackReceiver knockbackReceiver;

    [Header("Attack Recovery")]
    [SerializeField] private float hitRecoveryDuration = 2f;

    private EnemyAudio enemyAudio;

    private float hitRecoveryTimer;
    private float cooldownTimer;
    private float startupTimer;
    private float lungeTimer;
    private float lungeStartupDuration;

    private float lockedZ;
    private Vector3 lungeDirection;

    private bool isLungeStarting;
    private bool isLunging;

    // Keep compatibility with scripts checking attack state.
    public bool IsLunging
    {
        get { return isLunging || isLungeStarting; }
    }

    public bool IsJumpStarting
    {
        get { return false; }
    }

    public bool IsGrounded
    {
        get { return false; }
    }

    public float HorizontalSpeed { get; private set; }

    private void OnEnable()
    {
        lockedZ = transform.position.z;
    }

    private void Start()
    {
        enemyAudio = GetComponent<EnemyAudio>();

        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponentInChildren<SpriteRenderer>();
        }

        if (knockbackReceiver == null)
        {
            knockbackReceiver =
                GetComponent<KnockbackReceiver>();
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

        lungeStartupDuration =
            lungeStartupFrames /
            Mathf.Max(animationFrameRate, 1f);
    }

    public void BeginHitRecovery()
    {
        hitRecoveryTimer = hitRecoveryDuration;

        isLungeStarting = false;
        isLunging = false;
        HorizontalSpeed = 0f;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        // Update cooldown.
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;
        }

        // Face the rabbit even while stationary.
        float facingDirection =
            player.position.x - transform.position.x;

        if (isLungeStarting || isLunging)
        {
            facingDirection = lungeDirection.x;
        }

        UpdateFacing(facingDirection);

        // Pause movement after a successful attack.
        if (hitRecoveryTimer > 0f)
        {
            hitRecoveryTimer -= deltaTime;
            HorizontalSpeed = 0f;
            return;
        }


        // Apply knockback and update the stun timer every frame.
        if (knockbackReceiver != null)
        {
            // Check stun before updating its timer.
            bool isStunned = knockbackReceiver.IsStunned;

            Vector3 knockbackMovement =
                knockbackReceiver.GetKnockbackMovement(deltaTime);

            knockbackMovement.z = 0f;

            if (knockbackMovement.sqrMagnitude > 0f)
            {
                transform.position += knockbackMovement;
                LockZ();
            }

            if (isStunned)
            {
                isLungeStarting = false;
                isLunging = false;
                HorizontalSpeed = 0f;
                return;
            }
        }


        // Process attack startup.
        if (isLungeStarting)
        {
            startupTimer -= deltaTime;

            HorizontalSpeed = 0f;

            if (startupTimer <= 0f)
            {
                isLungeStarting = false;
                isLunging = true;
                lungeTimer = lungeDuration;

                // Play the Genie's attack vocalization.
                if (enemyAudio != null)
                {
                    enemyAudio.PlayLunge();
                }
            }

            return;
        }

        // Perform flying lunge.
        if (isLunging)
        {
            Vector3 movement =
                lungeDirection *
                lungeSpeed *
                deltaTime;

            transform.position += movement;
            LockZ();

            HorizontalSpeed =
                lungeDirection.x * lungeSpeed;

            lungeTimer -= deltaTime;

            if (lungeTimer <= 0f)
            {
                isLunging = false;
                HorizontalSpeed = 0f;
            }

            return;
        }

        // Target position above the rabbit.
        Vector3 target = new Vector3(
            player.position.x,
            player.position.y + hoverHeight,
            lockedZ
        );

        float distance =
            Vector3.Distance(transform.position, target);

        // Begin attack when close enough.
        if (distance <= lungeRange &&
            distance > stopDistance &&
            distance <= chaseRange &&
            cooldownTimer <= 0f)
        {
            isLungeStarting = true;
            startupTimer = lungeStartupDuration;
            cooldownTimer = lungeCooldown;

            // Lock the attack direction at startup.
            lungeDirection =
                (target - transform.position).normalized;

            if (lungeDirection.sqrMagnitude < 0.001f)
            {
                lungeDirection = Vector3.right;
            }

            HorizontalSpeed = 0f;
            return;
        }

        // Stop when close enough.
        if (distance <= stopDistance ||
            distance > chaseRange)
        {
            HorizontalSpeed = 0f;
            return;
        }

        // Fly toward the rabbit.
        Vector3 previousPosition = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            floatSpeed * deltaTime
        );

        LockZ();

        HorizontalSpeed =
            (transform.position.x - previousPosition.x) /
            Mathf.Max(deltaTime, 0.001f);
    }

    private void UpdateFacing(float direction)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (Mathf.Abs(direction) < 0.01f)
        {
            return;
        }

        bool facingLeft = direction < 0f;

        if (facesRightByDefault)
        {
            spriteRenderer.flipX = facingLeft;
        }
        else
        {
            spriteRenderer.flipX = !facingLeft;
        }
    }

    private void LockZ()
    {
        Vector3 position = transform.position;
        position.z = lockedZ;
        transform.position = position;
    }
}
