
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Header("Settings")]
    [SerializeField] private float movementThreshold = 0.1f;

    private string currentAnimation;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (controller == null)
        {
            controller = GetComponentInParent<CharacterController>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void PlayJumpSquat()
    {
        PlayAnimation("Rabbit_JumpSquat");
    }

    public void PlayAirborne()
    {
        PlayAnimation("Rabbit_Airborne");
    }

    public void UpdateMovementAnimation(
        bool isJumpSquatting,
        float horizontalInput
    )

    {
        // Face the direction of horizontal movement.
        UpdateSpriteDirection(horizontalInput);

        if (isJumpSquatting)
        {
            return;
        }

        if (!controller.isGrounded)
        {
            PlayAirborne();
            return;
        }

        if (Mathf.Abs(horizontalInput) > movementThreshold)
        {
            PlayAnimation("Rabbit_Run");
        }
        else
        {
            PlayAnimation("Rabbit_Idle");
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null)
        {
            return;
        }

        if (currentAnimation == animationName)
        {
            return;
        }

        animator.Play(animationName);
        currentAnimation = animationName;
    }

    private void UpdateSpriteDirection(float horizontalInput)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (horizontalInput < -movementThreshold)
        {
            // Face left.
            spriteRenderer.flipX = true;
        }
        else if (horizontalInput > movementThreshold)
        {
            // Face right.
            spriteRenderer.flipX = false;
        }
    }
}
