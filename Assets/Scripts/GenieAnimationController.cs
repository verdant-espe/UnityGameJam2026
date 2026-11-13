
using UnityEngine;

[RequireComponent(typeof(GenieFloatChase))]
public class GenieAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GenieFloatChase genieChase;

    [Header("Animation State Names")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string runState = "Run";
    [SerializeField] private string lungeState = "Lunge";

    [Header("Animation Settings")]
    [SerializeField] private float movementThreshold = 0.1f;

    private string currentState;

    private void Awake()
    {
        if (genieChase == null)
        {
            genieChase = GetComponent<GenieFloatChase>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null || genieChase == null)
        {
            return;
        }

        // Attack animation takes priority.
        if (genieChase.IsLunging)
        {
            PlayAnimation(lungeState);
        }

        // Flying movement uses the Run animation.
        else if (Mathf.Abs(genieChase.HorizontalSpeed)
                 > movementThreshold)
        {
            PlayAnimation(runState);
        }

        // Hovering uses Idle.
        else
        {
            PlayAnimation(idleState);
        }
    }

    private void PlayAnimation(string stateName)
    {
        if (currentState == stateName)
        {
            return;
        }

        currentState = stateName;
        animator.Play(stateName);
    }
}
