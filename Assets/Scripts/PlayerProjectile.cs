
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] public float knockbackStrength = 8f;

    private Vector3 direction;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        Vector3 nextPosition =
            rb.position + direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore the player.
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            return;
        }

        // Look for a knockback component on the target.
        KnockbackReceiver receiver =
            other.GetComponentInParent<KnockbackReceiver>();

        if (receiver != null)
        {
            receiver.ApplyKnockback(
                direction,
                knockbackStrength
            );
        }

        Destroy(gameObject);
    }
}
