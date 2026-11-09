
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

    [Header("Knockback")]
    [SerializeField] private KnockbackReceiver knockbackReceiver;

    private CharacterController controller;
    private Vector3 velocity;
    private float horizontalSpeed;

    private Vector3 previousPlayerPosition;
    private float playerHorizontalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsRemaining = maxJumps;

        if (knockbackReceiver == null)
        {
            knockbackReceiver = GetComponent<KnockbackReceiver>();
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
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        // TRACK PLAYER MOVEMENT
        playerHorizontalVelocity =
            (player.position.x - previousPlayerPosition.x)
            / Mathf.Max(Time.deltaTime, 0.001f);

        previousPlayerPosition = player.position;

        float predictedX = player.position.x +
            playerHorizontalVelocity * predictionTime;

        float distance =
            Mathf.Abs(player.position.x - transform.position.x);

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

        // SMART HORIZONTAL CHASING
        float direction = 0f;

        if (!isStunned &&
            distance <= chaseRange &&
            distance > stopDistance)
        {
            direction = Mathf.Sign(
                predictedX - transform.position.x
            );
        }

        float targetSpeed = direction * speed;

        if (isStunned)
        {
            // Immediately stop chasing during knockback.
            horizontalSpeed = 0f;
        }
        else
        {
            horizontalSpeed = Mathf.MoveTowards(
                horizontalSpeed,
                targetSpeed,
                acceleration * Time.deltaTime
            );
        }

        // FIRST JUMP
        if (!isStunned &&
            controller.isGrounded &&
            distance <= chaseRange &&
            player.position.y >
            transform.position.y + jumpTriggerHeight &&
            jumpsRemaining > 0)
        {
            Jump();
        }

        // SECOND JUMP WHEN FALLING
        else if (!isStunned &&
                 !controller.isGrounded &&
                 velocity.y < 0f &&
                 distance <= chaseRange &&
                 player.position.y >
                 transform.position.y + jumpTriggerHeight &&
                 jumpsRemaining > 0)
        {
            Jump();
        }

        // GRAVITY
        velocity.y += gravity * Time.deltaTime;

        Vector3 movement = new Vector3(
            horizontalSpeed,
            velocity.y,
            0f
        );

        // Apply knockback movement.
        if (knockbackReceiver != null)
        {
            Vector3 knockbackMovement =
                knockbackReceiver.GetKnockbackMovement(
                    Time.deltaTime
                );

            movement += knockbackMovement / Time.deltaTime;
        }

        controller.Move(movement * Time.deltaTime);
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(
            jumpHeight * -2f * gravity
        );

        jumpsRemaining--;
    }
}
