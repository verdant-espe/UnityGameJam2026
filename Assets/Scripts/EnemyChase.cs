
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

    private CharacterController controller;
    private Vector3 velocity;
    private float horizontalSpeed;

    private Vector3 previousPlayerPosition;
    private float playerHorizontalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsRemaining = maxJumps;

        if (player == null)
        {
            GameObject foundPlayer =
                GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (player != null)
            previousPlayerPosition = player.position;
    }

    void Update()
    {
        if (player == null)
            return;

        // TRACK PLAYER MOVEMENT
        playerHorizontalVelocity =
            (player.position.x - previousPlayerPosition.x)
            / Mathf.Max(Time.deltaTime, 0.001f);

        previousPlayerPosition = player.position;

        // PREDICT WHERE PLAYER WILL BE
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

        // SMART HORIZONTAL CHASING
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
            acceleration * Time.deltaTime
        );

        // FIRST JUMP
        if (controller.isGrounded &&
            distance <= chaseRange &&
            player.position.y >
            transform.position.y + jumpTriggerHeight &&
            jumpsRemaining > 0)
        {
            Jump();
        }

        // SECOND JUMP WHEN FALLING
        else if (!controller.isGrounded &&
                 velocity.y < 0 &&
                 distance <= chaseRange &&
                 player.position.y >
                 transform.position.y + jumpTriggerHeight &&
                 jumpsRemaining > 0)
        {
            Jump();
        }

        // GRAVITY
        velocity.y += gravity * Time.deltaTime;

        // MOVE LEFT AND RIGHT ONLY
        Vector3 movement = new Vector3(
            horizontalSpeed,
            velocity.y,
            0f
        );

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
