
using UnityEngine;

public class GenieFloatChase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Floating Movement")]
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float hoverHeight = 1f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Attack Recovery")]
    [SerializeField] private float hitRecoveryDuration = 2f;

    private float hitRecoveryTimer;
    private float lockedZ;

    private void OnEnable()
    {
        lockedZ = transform.position.z;
    }

    public void BeginHitRecovery()
    {
        hitRecoveryTimer = hitRecoveryDuration;
    }

    private void Update()
    {
        if (player == null)
            return;

        // Pause briefly after a successful attack.
        if (hitRecoveryTimer > 0f)
        {
            hitRecoveryTimer -= Time.deltaTime;
            return;
        }

        // Follow the rabbit in both horizontal
        // and vertical directions.
        Vector3 target = new Vector3(
            player.position.x,
            player.position.y + hoverHeight,
            lockedZ
        );

        // Stop when close enough to the rabbit.
        if (Vector3.Distance(transform.position, target)
            <= stopDistance)
            return;

        // Float toward the target without gravity
        // or platform navigation.
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            floatSpeed * Time.deltaTime
        );
    }
}
