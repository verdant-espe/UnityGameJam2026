
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;

    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    // DOUBLE JUMP
    public int maxJumps = 2;
    private int jumpsRemaining;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        // LEFT AND RIGHT ONLY
        float x = Input.GetAxisRaw("Horizontal");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : speed;

        Vector3 move = new Vector3(x, 0f, 0f);

        controller.Move(move * currentSpeed * Time.deltaTime);

        // GRAVITY AND DOUBLE JUMP
        if (controller.isGrounded)
        {
            velocity.y = -2f;
            jumpsRemaining = maxJumps;
        }

        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpsRemaining--;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
