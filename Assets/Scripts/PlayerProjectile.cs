
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] public float knockbackStrength = 8f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    private Vector3 direction;
    private Rigidbody rb;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    public void ConfigureCombat(int newDamage, float newKnockback)
    {
        damage = Mathf.Max(1, newDamage);
        knockbackStrength = Mathf.Max(0f, newKnockback);
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
        if (hasHit)
            return;

        // Ignore the player.
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            return;
        }

        hasHit = true;

        // Apply knockback if the target supports it.
        KnockbackReceiver receiver =
            other.GetComponentInParent<KnockbackReceiver>();

        if (receiver != null)
        {
            receiver.ApplyKnockback(
                direction,
                knockbackStrength
            );
        }

        // Damage any enemy that has EnemyHealth.
        EnemyHealth enemyHealth =
            other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);

            // Play a cloth impact when an enemy is shot.
            if (SoundManager.Instance != null)
            {
                Debug.Log("Enemy impact sound triggered!");
                SoundManager.Instance.PlayClothImpact();
            }
        }

        // Remove projectile after impact.
        Destroy(gameObject);
    }
}
