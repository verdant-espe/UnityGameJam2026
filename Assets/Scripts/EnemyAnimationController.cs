
using UnityEngine;

[RequireComponent(typeof(EnemyChase))]
public class EnemyAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyChase enemyChase;

    [Header("Animation State Names")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string runState = "Run";
    [SerializeField] private string jumpState = "Jump";
    [SerializeField] private string lungeState = "Lunge";

    [Header("Animation Settings")]
    [SerializeField] private float movementThreshold = 0.1f;

    private string currentState;

    void Awake()
    {
        if (enemyChase == null)
        {
            enemyChase = GetComponent<EnemyChase>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (animator == null || enemyChase == null)
        {
            return;
        }

        // LUNGE STARTUP AND ACTIVE LUNGE
        if (enemyChase.IsLunging)
        {
            PlayAnimation(lungeState);
        }

        // JUMP STARTUP AND AIRBORNE MOVEMENT
        else if (enemyChase.IsJumpStarting ||
                 !enemyChase.IsGrounded)
        {
            PlayAnimation(jumpState);
        }

        // RUNNING
        else if (Mathf.Abs(enemyChase.HorizontalSpeed)
                 > movementThreshold)
        {
            PlayAnimation(runState);
        }

        // IDLE
        else
        {
            PlayAnimation(idleState);
        }
    }

    void PlayAnimation(string stateName)
    {
        if (currentState == stateName)
        {
            return;
        }

        currentState = stateName;

        animator.Play(stateName);
    }
}
