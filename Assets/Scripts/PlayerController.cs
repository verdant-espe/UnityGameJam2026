
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
        // Check grounding before moving this frame.
        bool grounded = controller.isGrounded;

        if (grounded)
        {
            if (velocity.y < 0f)
                velocity.y = -2f;

            // Landing restores both jumps.
            jumpsRemaining = maxJumps;
        }

        // Horizontal movement.
        float x = Input.GetAxisRaw("Horizontal");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : speed;

        // First jump or double jump.
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpsRemaining--;
        }

        // Gravity.
        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal and vertical movement.
        Vector3 movement = new Vector3(
            x * currentSpeed,
            velocity.y,
            0f
        );

        // Move only once per frame.
        controller.Move(movement * Time.deltaTime);
    }

}
